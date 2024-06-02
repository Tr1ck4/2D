using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;

    public float runSpeed = 5f;

    private Vector2 movement;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // This method is called when the character collides with another collider
        Debug.Log("Collided with " + collision.gameObject.name);
        
        // You can check the tag of the collided object
        if (collision.gameObject.tag == "Obstacle")
        {
            // Handle collision with an obstacle
            Debug.Log("Hit an obstacle!");
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // This method is called every frame while the character stays in contact with another collider
        Debug.Log("Staying in collision with " + collision.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // This method is called when the character stops colliding with another collider
        Debug.Log("Stopped colliding with " + collision.gameObject.name);
    }

    IEnumerator Chop()
    {
        animator.SetBool("isChop", true);
        yield return new WaitForSeconds(0.0f);
        animator.SetBool("isChop", false);
    }

    void MoveCharacter()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        // Check for horizontal movement
        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
        }

        // Check for vertical movement
        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1f;
        }

        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(moveHorizontal) + Mathf.Abs(moveVertical));
        animator.SetFloat("X", moveHorizontal);
        animator.SetFloat("Y", moveVertical);

        // Normalize the movement vector to ensure consistent speed in all directions
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        // Set the rigidbody velocity to the movement vector multiplied by the run speed
        rb.velocity = movement * runSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();
        
        if (Input.GetKey(KeyCode.E) && animator.GetFloat("Speed") == 0f)
        {
            StartCoroutine(Chop());
        }
    }
}
