using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    [Header("UI")]
    public GameObject gameOverText;
    public Text scoreText;

    [Header("Game State")]
    public bool gameOver = false;
    public float scrollSpeed = -1.5f;

    [Header("Score")]
    public int Score = 0;       // Now public so coins can modify it

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        instance = this;
        UpdateScoreUI();
    }

    void Update()
    {
        // Restart game on click if dead
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Called when the cat passes through a column
    public void CatScored()
    {
        if (gameOver)
            return;

        Score++;
        UpdateScoreUI();
    }

    // Called when the cat collects a coin
    public void AddCoinScore(int amount)
    {
        if (gameOver)
            return;

        Score += amount;
        UpdateScoreUI();
    }

    // Updates score text
    public void UpdateScoreUI()
    {
        scoreText.text = "Score: " + Score.ToString();
        /*if (scoreText != null)
            scoreText.text = "Score: " + Score;
        */
    }

    
    // Called when the cat hits an obstacle
    public void CatDied()
    {
        gameOverText.SetActive(true);
        gameOver = true;
    }
}
