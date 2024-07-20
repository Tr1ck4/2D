using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public float runSpeed = 5f;
    public int chopDamage = 25;
    private Vector2 movement;
    private TreeHealth targetTree;

    public GameObject shopPopup; 

    private static PlayerMovement instance;
    public int money = 0;

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

    public bool BuyItem(int price)
    {
        if (money >= price)
        {
            money -= price; // Deduct the price from the player's money
            Debug.Log("Item bought for " + price + " money. Remaining money: " + money);
            return true; // Purchase successful
        }
        else
        {
            Debug.Log("Not enough money to buy this item. Required: " + price + ", Current: " + money);
            return false; // Not enough money to purchase
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (shopPopup != null)
        {
            shopPopup.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            targetTree = other.GetComponentInParent<TreeHealth>();
        }
        if (other.CompareTag("ShopArea"))
        {
            LoadArea(2, new Vector2(8.5f, -15f));
        }
        if (other.CompareTag("OtherArea"))
        {
            LoadArea(0, new Vector2(-13f, 7f));
        }
        if (other.CompareTag("ShopUp"))
        {
            OpenShop();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            targetTree = null;
        }
    }

    private void OpenShop()
    {
        if (shopPopup != null)
        {
            shopPopup.SetActive(true);
        }
    }

    public void LoadArea(int index, Vector2 position)
    {
        targetPosition = position;
        StartCoroutine(LoadSceneAndSetPosition(index));
    }

    private Vector2 targetPosition;

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
