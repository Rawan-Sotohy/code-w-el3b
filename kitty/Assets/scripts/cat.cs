using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class cat : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 1f; // Adjust this value in Inspector if needed
    public float maxY = 4f; // Maximum Y position the cat can reach
    [Header("Scrolling Reference")]
    public scrolling scrollingScript;
    private bool isDead = false;
    private Rigidbody2D rb;
    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Find scrolling script if not assigned
        if (scrollingScript == null)
        {
            scrollingScript = FindObjectOfType<scrolling>();
        }
    }
    void Update()
    {
        if (!isDead)
        {
            HandleInput();
            UpdateAnimationState();
            ClampPosition();
        }
    }
    void HandleInput()
    {
        // -------- Control Scrolling with Right Arrow --------
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (scrollingScript != null)
            {
                scrollingScript.isScrolling = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (scrollingScript != null)
            {
                scrollingScript.isScrolling = false;
            }
        }
        // -------- Jump --------
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Use velocity directly instead of AddForce for more responsive jump
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("isjumping", true);
        }
    }
    void UpdateAnimationState()
    {
        // Determine animation state based on velocity and input
        if (rb.velocity.y > 0.1f)
        {
            // Jumping up
            anim.SetInteger("state", 2); // JumpAnimation
        }
        else if (rb.velocity.y < -0.1f)
        {
            // Falling down
            anim.SetInteger("state", 3); // FallAnimation
            anim.SetBool("isjumping", false);
        }
        else
        {
            // On ground or neutral velocity
            anim.SetBool("isjumping", false);
            if (scrollingScript != null && scrollingScript.isScrolling)
            {
                // Running
                anim.SetInteger("state", 1); // RunningAnimation
            }
            else
            {
                // Idle
                anim.SetInteger("state", 0); // IdleAnimation
            }
        }
    }
    void ClampPosition()
    {
        // Prevent cat from going above maxY
        if (transform.position.y > maxY)
        {
            transform.position = new Vector2(transform.position.x, maxY);
            // Stop upward velocity when hitting max height
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isDead = true;
            anim.SetTrigger("die");
            anim.SetInteger("state", 4); // DeadAnimation
            rb.velocity = Vector2.zero;
            // Stop scrolling when dead
            if (scrollingScript != null)
            {
                scrollingScript.isScrolling = false;
            }
            // Notify GameControl
            if (GameControl.instance != null)
            {
                GameControl.instance.CatDied();
            }
        }
    }
}