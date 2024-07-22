using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb;
    public float runSpeed = 5f;
    public int chopDamage = 25;
    private Vector2 movement;
    private TreeHealth targetTree;

    public GameObject shopPopup; 

    private static PlayerMovement instance;
    public int money = 0;

    public CropDatabase cropDatabase;

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

        LoadMoney(); // Load money when the game starts
    }

    public bool BuyItem(int price)
    {
        if (money >= price)
        {
            money -= price; // Deduct the price from the player's money
            Debug.Log("Item bought for " + price + " money. Remaining money: " + money);
            SaveMoney(); // Save money after purchase
            return true; // Purchase successful
        }
        else
        {
            Debug.Log("Not enough money to buy this item. Required: " + price + ", Current: " + money);
            return false; // Not enough money to purchase
        }
    }

    public void AddMoney(int price)
    {
        money += price;
        SaveMoney(); // Save money after adding
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        if (PlayerPrefs.HasKey("PlayerMoney"))
        {
            money = PlayerPrefs.GetInt("PlayerMoney");
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (shopPopup != null)
        {
            shopPopup.SetActive(false);
        }

        if (cropDatabase == null)
        {
            Debug.Log("Player.cropDatabase is null");
            return;
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
        Debug.Log("Going to scene: " + index);
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

    IEnumerator Water()
    {
        animator.SetBool("isWatering", true);
        animator.Play("watering");
        yield return new WaitForSeconds(0.4f);

        animator.Play("idle_right");
        animator.SetBool("isWatering", false);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug character's position
            // Vector3 debugPosition = new Vector3(transform.position.x, transform.position.y, 0);
            // Debug.Log("Character's real position: " + debugPosition);

            Vector3 playerbotcen = GetPlayerBottomCenter();
            int plowX = (int)Math.Floor(playerbotcen.x);
            int plowY = (int)Math.Floor(playerbotcen.y);
            GetPlayerBottomCenter();
            Vector3Int position =
                new Vector3Int(
                    plowX,
                    plowY,
                    0
                );


            if (GameManager.Instance.tileManager.IsPlowable(position)) // Plow dirt
            {
                GameManager.Instance.tileManager.SetPlowed(position);
            }
            else if (GameManager.Instance.tileManager.IsPlowed(position)) // Water dirt tile
            {
                StartCoroutine(Water());
                GameManager.Instance.tileManager.SetMoisturized(position);
            }
            else if (GameManager.Instance.tileManager.IsMoisturized(position) &&
                !GameManager.Instance.tileManager.HasCrop(position)) // Plant seed
            {
                GameManager.Instance.tileManager.PlantCrop(position, cropDatabase.crops[1]);
            }
            else if (GameManager.Instance.tileManager.IsHarvestable(position)) // Harvest
            {
                Debug.Log("Harvest pressed");
                GameManager.Instance.tileManager.Harvest(position);
            }
        }
    }

    void FixedUpdate()
    {
        if (!animator.GetBool("isChop"))
        {
            rb.MovePosition(rb.position + movement.normalized * runSpeed * Time.fixedDeltaTime);
        }
    }

    private Vector3 GetPlayerBottomCenter() // where the Player is standing
    {
        var bounds = spriteRenderer.bounds;
        Vector3 bottomCenter = new Vector3(bounds.center.x, bounds.min.y);
        return bottomCenter;
    }
}
