using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    public CharacterController controller;
    public Animator animator;

    public float runSpeed = 0.1f;

    float horizontalMove = 0f;
    float verticalMove = 0f;
    private Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        // Create movement vector and normalize it
        movement = new Vector3(horizontalMove, verticalMove, 0).normalized;

        // Update position
        transform.position += movement * Time.deltaTime * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove)+Mathf.Abs(verticalMove));
        animator.SetFloat("X", horizontalMove);
        animator.SetFloat("Y", verticalMove);
    }

}
