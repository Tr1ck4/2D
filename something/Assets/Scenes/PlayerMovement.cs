using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public float runSpeed = 5f;
    public int chopDamage = 25;
    private Vector2 movement;
    private TreeHealth targetTree;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            targetTree = other.GetComponentInParent<TreeHealth>(); // Get the Tree component from the parent
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            targetTree = null;
        }
    }

    IEnumerator Chop()
    {
        animator.SetBool("isChop", true);
        animator.Play("chopping");
        yield return new WaitForSeconds(0.4f);

        if (targetTree != null)
        {
            targetTree.Chop(chopDamage);
        }

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

    void Update()
    {
        MoveCharacter();

        if (Input.GetKeyDown(KeyCode.E) && animator.GetFloat("Horizontal") == 0f && animator.GetFloat("Vertical") == 0f && !animator.GetBool("isChop"))
        {
            StartCoroutine(Chop());
        }
    }

    void FixedUpdate()
    {
        if (!animator.GetBool("isChop"))
        {
            rb.MovePosition(rb.position + movement.normalized * runSpeed * Time.fixedDeltaTime);
        }
    }
}
