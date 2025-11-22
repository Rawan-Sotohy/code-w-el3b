using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cp : MonoBehaviour
{
    [Header("Pool Settings")]
    public int coinPoolSize = 15;
    public GameObject coinPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 2f;
    public float minCoinSpacing = 4f; // Minimum HORIZONTAL distance between coins (increased)

    [Header("Column Reference")]
    public collumnpool columnPool;

    [Header("Player Reference")]
    public Transform player;

    [Header("Spawn Range")]
    public float minY = -2f;
    public float maxY = 3f;
    public float spawnAheadDistance = 10f;

    [Header("Column Safety Settings")]
    public float aboveColumnOffset = 3.5f; // Must be THIS far ABOVE column top
    public float sideColumnMinDistance = 3f; // Must be THIS far to SIDE of column horizontally
    public float minVerticalSpacing = 2f;


    private GameObject[] coins;
    private Vector2 poolPos = new Vector2(-1000f, -1000f);
    private int currentCoin = 0;
    private scrolling scrollingScript;
    private float lastCoinX = 0f; // Track last coin X to prevent stacking

    void Start()
    {
        scrollingScript = FindObjectOfType<scrolling>();

        coins = new GameObject[coinPoolSize];
        for (int i = 0; i < coinPoolSize; i++)
        {
            coins[i] = Instantiate(coinPrefab, poolPos, Quaternion.identity);
            coins[i].SetActive(false);

            // Add Collider
            if (coins[i].GetComponent<CircleCollider2D>() == null)
            {
                CircleCollider2D col = coins[i].AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.5f;
            }

            coins[i].tag = "Coin";

            // Disable rotation
            coins coinScript = coins[i].GetComponent<coins>();
            if (coinScript != null)
            {
                coinScript.rotationSpeed = 0f;
            }
        }

        InvokeRepeating("SpawnCoin", 3f, spawnRate);
    }

    void SpawnCoin()
    {
        if (GameControl.instance == null || GameControl.instance.gameOver) return;
        if (scrollingScript == null || !scrollingScript.isScrolling) return;
        if (player == null) return;

        Vector2 coinPos = Vector2.zero;
        bool validPosition = false;
        int maxAttempts = 50;
        int attempts = 0;

        while (!validPosition && attempts < maxAttempts)
        {
            attempts++;

            // Spawn to the RIGHT of last coin - NO VERTICAL STACKING!
            float spawnX = Mathf.Max(
                player.position.x + spawnAheadDistance,
                lastCoinX + minCoinSpacing
            );

            float spawnY = Random.Range(minY, maxY);
            coinPos = new Vector2(spawnX, spawnY);

            // Validate position
            validPosition = IsValidCoinPosition(coinPos);
        }

        if (validPosition)
        {
            GameObject coin = coins[currentCoin];
            coin.SetActive(true);
            coin.transform.position = new Vector3(coinPos.x, coinPos.y, -1f);
            coin.transform.localScale = new Vector3(1f, 1f, 1f);

            // Update last coin X
            lastCoinX = coinPos.x;

            currentCoin++;
            if (currentCoin >= coinPoolSize)
                currentCoin = 0;
        }
        else
        {
            // Could not find valid position - skip this spawn
        }
    }

    bool IsValidCoinPosition(Vector2 position)
    {
        if (columnPool == null) return true;

        GameObject[] allColumns = columnPool.GetAllColumns();

        foreach (GameObject col in allColumns)
        {
            if (col == null || !col.activeInHierarchy) continue;

            Bounds bounds = GetColumnBounds(col);

            // Check horizontal overlap with column
            float columnLeft = bounds.min.x;
            float columnRight = bounds.max.x;
            float columnTop = bounds.max.y;
            float columnBottom = bounds.min.y;

            // Is coin horizontally aligned with column?
            bool horizontallyAligned = position.x >= columnLeft && position.x <= columnRight;

            if (horizontallyAligned)
            {
                // FORBIDDEN: Coin ON or INSIDE column vertically
                if (position.y >= columnBottom && position.y <= columnTop)
                {
                    return false; // Coin is ON the column - REJECTED!
                }

                // Coin is in column's horizontal range
                // It MUST be well ABOVE the column (not just slightly above)
                if (position.y < columnTop + aboveColumnOffset)
                {
                    return false; // Too close to column top
                }
            }
            else
            {
                // Coin is NOT horizontally aligned with column
                // Check if it's too close to the sides
                float horizontalDistance = Mathf.Min(
                    Mathf.Abs(position.x - columnLeft),
                    Mathf.Abs(position.x - columnRight)
                );

                // If coin is beside column, make sure it's far enough
                if (horizontalDistance < sideColumnMinDistance)
                {
                    // Check if coin is at same vertical level as column
                    if (position.y >= columnBottom - 1f && position.y <= columnTop + 1f)
                    {
                        return false; // Too close to column side
                    }
                }
            }
        }

        // Check: No coins stacked vertically (must be horizontally separated)
        foreach (GameObject otherCoin in coins)
        {
            if (otherCoin == null || !otherCoin.activeInHierarchy) continue;

            Vector2 otherPos = otherCoin.transform.position;
            float horizontalDist = Mathf.Abs(position.x - otherPos.x);

            // Coins must be separated HORIZONTALLY
            if (horizontalDist < minCoinSpacing)
            {
                return false; // Too close horizontally
            }

            // Coins must be separated VERTICALLY
            float verticalDist = Mathf.Abs(position.y - otherPos.y);
            if (verticalDist < minVerticalSpacing)
            {
                return false; // Too close vertically
            }
        }

        return true;
    }

    Bounds GetColumnBounds(GameObject column)
    {
        Collider2D collider = column.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds;
        }

        Renderer renderer = column.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        return new Bounds(column.transform.position, new Vector3(2f, 2f, 0f));
    }

    void Update()
    {
        if (GameControl.instance == null || GameControl.instance.gameOver) return;

        if (scrollingScript != null && scrollingScript.isScrolling)
        {
            float scrollAmount = scrollingScript.scrollSpeed * Time.deltaTime;

            foreach (GameObject coin in coins)
            {
                if (coin != null && coin.activeInHierarchy)
                {
                    coin.transform.Translate(Vector3.left * scrollAmount, Space.World);

                    // Deactivate coins that moved far left
                    if (coin.transform.position.x < player.position.x - 10f)
                    {
                        coin.SetActive(false);
                    }
                }
            }
        }
    }

    public GameObject[] GetAllCoins()
    {
        return coins;
    }

    void OnDrawGizmos()
    {
        if (coins != null)
        {
            foreach (GameObject coin in coins)
            {
                if (coin != null && coin.activeInHierarchy)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(coin.transform.position, 0.5f);
                }
            }
        }

        if (player != null)
        {
            Gizmos.color = Color.green;
            Vector3 spawnPoint = new Vector3(player.position.x + spawnAheadDistance, 0, 0);
            Gizmos.DrawLine(new Vector3(spawnPoint.x, minY, 0), new Vector3(spawnPoint.x, maxY, 0));
        }
    }
}