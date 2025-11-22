using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCode : MonoBehaviour
{

    public float upForce = 200f;
    private bool isDead = false;
    private Rigidbody2D rb;
    private Animator anim;

    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = Vector2.up*0.5f;
                rb.AddForce(new Vector2(0, upForce));
                anim.SetTrigger("Flap");
            }
            if (Camera.main != null)
            {
                float height = Camera.main.orthographicSize;

                transform.position = new Vector2(
                    transform.position.x,
                    Mathf.Clamp(transform.position.y, -height + 0.5f, height - 0.5f)
                );
            }
            }
        }
    void OnCollisionEnter2D()
    {
        isDead = true;
        anim.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        GameController.instance.BirdDied();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BirdCode>() != null)
        {
            GameController.instance.BirdScored();
        }
    }
}




