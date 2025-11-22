using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrolling : MonoBehaviour
{
    public Transform ground;
    public Transform ground1;
    public float scrollSpeed = 5f;
    private float backgroundWidth = 20.48f;
    public bool isScrolling = false;
    private Vector3 grass1Start;
    private Vector3 grass2Start;

    void Start()
    {
        // Save original positions
        grass1Start = ground.position;
        grass2Start = ground1.position;
    }

    void Update()
    {
        if (isScrolling)
        {
            float scrollAmount = scrollSpeed * Time.deltaTime;

            // Scroll background only
            ground.Translate(Vector3.left * scrollAmount);
            ground1.Translate(Vector3.left * scrollAmount);

            // Loop background
            if (ground.position.x < -backgroundWidth)
            {
                ground.position = new Vector3(ground1.position.x + backgroundWidth, ground.position.y, ground.position.z);
            }
            if (ground1.position.x < -backgroundWidth)
            {
                ground1.position = new Vector3(ground.position.x + backgroundWidth, ground1.position.y, ground1.position.z);
            }
        }
    }

    public void ResetPositions()
    {
        ground.position = grass1Start;
        ground1.position = grass2Start;
    }
}