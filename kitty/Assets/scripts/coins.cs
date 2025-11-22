using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class coins : MonoBehaviour
{
    public int scoreValue = 1;
    public float rotationSpeed = 90f; // Rotation speed in degrees per second
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add to Score
            GameControl.instance.Score += scoreValue;
            // Update UI
            GameControl.instance.UpdateScoreUI();
            // Hide coin
            gameObject.SetActive(false);
            Debug.Log("Coin Collected! Score: " + GameControl.instance.Score);
        }
    }
    void Update()
    {
    }
}
