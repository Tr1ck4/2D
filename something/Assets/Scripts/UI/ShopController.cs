using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    public GameObject shopPopup; // Reference to the shop popup panel
    public Button closeButton; // Reference to the close button
    public ItemDatabase itemDatabase; // Reference to the item database
    public GameObject itemPrefab; // Reference to the item prefab
    public Transform contentParent; // Reference to the content parent (scroll view content)

    private static ShopController instance;
    private PlayerMovement playerMovement;
    private Player player;

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
        playerMovement = FindObjectOfType<PlayerMovement>();
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        if (shopPopup == null)
        {
            Debug.LogError("Shop popup is not assigned!");
            return;
        }

        if (closeButton == null)
        {
            Debug.LogError("Close button is not assigned!");
            return;
        }

        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not assigned!");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is not assigned!");
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("Content parent is not assigned!");
            return;
        }

        shopPopup.SetActive(false);
        closeButton.onClick.AddListener(CloseShop);

        GenerateShop();
    }

    private void CloseShop()
    {
        shopPopup.SetActive(false);
    }

    private void GenerateShop()
    {
        foreach (ItemData item in itemDatabase.items)
        {
            GameObject newItem = Instantiate(itemPrefab, contentParent);
            TMP_Text[] textComponents = newItem.GetComponentsInChildren<TMP_Text>();
            Image itemImage = newItem.GetComponentInChildren<Image>();

            if (textComponents.Length < 2 || itemImage == null)
            {
                Debug.LogError("Item prefab is missing one of the required components.");
                continue;
            }

            

            textComponents[0].text = item.itemName;
            textComponents[1].text = item.price.ToString();
            itemImage.sprite = item.itemSprite;
            Debug.Log(itemImage.sprite);

            Button buyButton = newItem.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }
    }

    private void BuyItem(ItemData item)
    {
        if (playerMovement != null)
        {
            bool bought = playerMovement.BuyItem(item.price);
            if (bought)
            {
                player.inventory.Add(item);
                // Perform additional actions after successful purchase (e.g., add item to inventory)
                Debug.Log("Item purchased successfully!");
                // Implement further logic as needed
            }
            else
            {
                Debug.Log("Not enough money to buy this item.");
            }
        }
        else
        {
            Debug.LogError("PlayerMovement instance not found.");
        }
    }
    
}
