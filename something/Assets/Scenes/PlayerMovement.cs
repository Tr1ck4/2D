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
        animator = GetComponent<Animator>();
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
        animator.Play("chopping");
        yield return new WaitForSeconds(0.4f);
        animator.Play("idle_right");
        animator.SetBool("isChop", false);
    }

    void MoveCharacter()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Set animator parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetBool("isWalking", movement.sqrMagnitude > 0f);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();
        
        if (Input.GetKeyDown(KeyCode.E) && animator.GetFloat("Horizontal") == 0f && animator.GetFloat("Vertical") == 0f)
        {
            StartCoroutine(Chop());
        }
    }

    void FixedUpdate()
    {
        // Move the character
        if(! animator.GetBool("isChop"))
        {
            rb.MovePosition(rb.position + movement.normalized * runSpeed * Time.fixedDeltaTime);
        }
    }
}
