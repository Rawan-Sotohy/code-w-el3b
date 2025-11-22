using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collumnpool : MonoBehaviour
{
    [Header("Pool Settings")]
    public int columnPoolSize = 5;
    public GameObject columnPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 3f;
    public float minSpacing = 6f; // Minimum horizontal distance between columns (increased for gaps)
    public float spawnAheadOfPlayer = 10f;

    [Header("Column Height Range")]
    public float minHeight = 3f;
    public float maxHeight = 6f;

    [Header("Column Y Position Range")]
    public float minY = -8f;
    public float maxY = -5f;

    [Header("Player Movement Threshold")]
    public float playerMoveThreshold = 0.01f;

    private GameObject[] columns;
    private Vector2 objectPoolPosition = new Vector2(-1000f, -1000f);
    private int currentColumn = 0;
    private Transform player;
    private scrolling scrollingScript;
    private float lastSpawnX = 0f; // Track last spawn X position

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        scrollingScript = FindObjectOfType<scrolling>();

        Debug.Log("Column Pool Started!");
        Debug.Log("Column Prefab: " + (columnPrefab != null ? "Found" : "NULL"));
        Debug.Log("Player Position: " + (player != null ? player.position.ToString() : "NULL"));

        columns = new GameObject[columnPoolSize];
        for (int i = 0; i < columnPoolSize; i++)
        {
            columns[i] = Instantiate(columnPrefab, objectPoolPosition, Quaternion.identity);
            columns[i].SetActive(false);

            // Set tag to "Obstacle"
            columns[i].tag = "Obstacle";

            // Ensure column has a collider (NOT trigger)
            Collider2D col = columns[i].GetComponent<Collider2D>();
            if (col == null)
            {
                BoxCollider2D boxCol = columns[i].AddComponent<BoxCollider2D>();
                boxCol.isTrigger = false;
            }
            else
            {
                col.isTrigger = false;
            }
        }

        InvokeRepeating("SpawnColumn", Random.Range(1f, 2f), spawnRate);
    }

    void Update()
    {
        // Scroll all active columns
        if (scrollingScript != null && scrollingScript.isScrolling)
        {
            float scrollAmount = scrollingScript.scrollSpeed * Time.deltaTime;
            foreach (GameObject col in columns)
            {
                if (col != null && col.activeInHierarchy)
                {
                    col.transform.Translate(Vector3.left * scrollAmount);

                    // Deactivate columns that moved far left
                    if (col.transform.position.x < player.position.x - 15f)
                    {
                        col.SetActive(false);
                    }
                }
            }
        }
    }

    void SpawnColumn()
    {
        if (GameControl.instance.gameOver || player == null) return;

        // Only spawn when scrolling is active
        if (scrollingScript == null || !scrollingScript.isScrolling)
            return;

        GameObject newColumn = columns[currentColumn];
        newColumn.SetActive(true);
        newColumn.tag = "Obstacle";

        
        newColumn.transform.localScale = new Vector3(1f, 1f, 1f);

        // Calculate spawn X - MUST be to the RIGHT of last column (not above/below)
        float spawnX = Mathf.Max(
            player.position.x + spawnAheadOfPlayer,
            lastSpawnX + minSpacing
        );

        // Random Y between minY and maxY
        float spawnY = Random.Range(minY, maxY);

        newColumn.transform.position = new Vector3(spawnX, spawnY, -1f);

        // Update last spawn X
        lastSpawnX = spawnX;

        Debug.Log("Column Spawned at index: " + currentColumn);
        Debug.Log("Column Position: " + newColumn.transform.position);
        Debug.Log("Column Scale: " + newColumn.transform.localScale);

        currentColumn++;
        if (currentColumn >= columnPoolSize)
            currentColumn = 0;
    }

    public Transform GetLastSpawnedColumn()
    {
        if (columns == null || columns.Length == 0) return null;

        int lastIndex = currentColumn - 1;
        if (lastIndex < 0) lastIndex = columnPoolSize - 1;

        if (columns[lastIndex] != null && columns[lastIndex].activeInHierarchy)
            return columns[lastIndex].transform;

        return null;
    }

    public float GetLastColumnX()
    {
        // Return the rightmost active column X position
        float maxX = player != null ? player.position.x : 0f;

        foreach (GameObject col in columns)
        {
            if (col != null && col.activeInHierarchy)
            {
                float colRightEdge = col.transform.position.x + (col.transform.localScale.x / 2f);
                if (colRightEdge > maxX)
                {
                    maxX = colRightEdge;
                }
            }
        }

        return maxX;
    }

    // Check if position is inside or ON TOP of any column
    public bool IsPositionInsideOrOnColumn(Vector2 position)
    {
        foreach (GameObject col in columns)
        {
            if (!col.activeInHierarchy) continue;

            Collider2D colCollider = col.GetComponent<Collider2D>();
            if (colCollider != null)
            {
                Bounds bounds = colCollider.bounds;

                // Check if horizontally aligned with column
                if (position.x > bounds.min.x && position.x < bounds.max.x)
                {
                    // Check if ON or INSIDE column (not just near it)
                    if (position.y >= bounds.min.y && position.y <= bounds.max.y + 0.5f)
                    {
                        return true; // Position is on/in column
                    }
                }
            }
        }
        return false;
    }

    public bool IsPositionInsideColumn(Vector2 position)
    {
        foreach (GameObject col in columns)
        {
            if (!col.activeInHierarchy) continue;

            Collider2D colCollider = col.GetComponent<Collider2D>();
            if (colCollider != null && colCollider.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    public GameObject[] GetAllColumns()
    {
        return columns;
    }
}