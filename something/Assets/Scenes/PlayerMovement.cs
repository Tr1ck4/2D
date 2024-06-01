using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    public CharacterController controller;
    public Animator animator;
    public Rigidbody2D rb;

    public float runSpeed = 5f;

    private Vector2 movement;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter(Collision collision)
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

    void OnCollisionStay(Collision collision)
    {
        // This method is called every frame while the character stays in contact with another collider
        Debug.Log("Staying in collision with " + collision.gameObject.name);
    }

    void OnCollisionExit(Collision collision)
    {
        // This method is called when the character stops colliding with another collider
        Debug.Log("Stopped colliding with " + collision.gameObject.name);
    }

    void MoveCharacter(){
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        animator.SetFloat("Speed", Mathf.Abs(moveHorizontal)+Mathf.Abs(moveVertical));
        animator.SetFloat("X", moveHorizontal);
        animator.SetFloat("Y", moveVertical);

        movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * runSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        MoveCharacter();
    }

}
