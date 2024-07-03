using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public float runSpeed = 5f;
    public int chopDamage = 25;
    private Vector2 movement;
    private TreeHealth targetTree;

    private Vector2 targetPosition;
    private static PlayerMovement instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            targetTree = other.GetComponentInParent<TreeHealth>(); // Get the Tree component from the parent
        }
        if (other.CompareTag("ShopArea"))
        {
            LoadArea(2, new Vector2(8.5f,-15f));
        }
        if (other.CompareTag("OtherArea"))
        {
            LoadArea(0, new Vector2(-13f,7f));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            targetTree = null;
        }
    }

    public void LoadArea(int index, Vector2 position)
    {
        
        targetPosition = position;
        StartCoroutine(LoadSceneAndSetPosition(index));
    }

    private IEnumerator LoadSceneAndSetPosition(int index)
    {
        yield return SceneManager.LoadSceneAsync(index);
        yield return new WaitForEndOfFrame();
        rb.position = targetPosition;
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
