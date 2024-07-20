using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    public GameObject shopPopup;
    public Button closeButton;
    public ItemDatabase itemDatabase;
    public GameObject itemPrefab; 
    public Transform contentParent; 

    public Button buyTabButton; 
    public Button sellTabButton;
    private bool isBuyingMode = true;

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

        if (buyTabButton == null || sellTabButton == null)
        {
            Debug.LogError("Tab buttons are not assigned!");
            return;
        }

        shopPopup.SetActive(false);
        closeButton.onClick.AddListener(CloseShop);
        buyTabButton.onClick.AddListener(() => SwitchTab(true));
        sellTabButton.onClick.AddListener(() => SwitchTab(false));

        GenerateShop();
    }

    private void SwitchTab(bool isBuying)
    {
        isBuyingMode = isBuying;
        GenerateShop();
    }
    private void CloseShop()
    {
        shopPopup.SetActive(false);
    }

    private void GenerateShop()
    {
        // Clear existing items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (isBuyingMode)
        {
            foreach (ItemData item in itemDatabase.items)
            {
                CreateShopItem(item, true);
            }
        }
        else
        {
            foreach (ItemData item in itemDatabase.items)
            {
                CreateShopItem(item, false);
            }
        }
    }

    private void CreateShopItem(ItemData item, bool isBuyMode)
    {
        GameObject newItem = Instantiate(itemPrefab, contentParent);
        TMP_Text[] textComponents = newItem.GetComponentsInChildren<TMP_Text>();
        Image itemImage = newItem.GetComponentInChildren<Image>();

        if (textComponents.Length < 2 || itemImage == null)
        {
            Debug.LogError("Item prefab is missing one of the required components.");
            return;
        }

        textComponents[0].text = item.itemName;
        textComponents[1].text = item.price.ToString();
        itemImage.sprite = item.itemSprite;

        Button actionButton = newItem.GetComponentInChildren<Button>();
        if (isBuyMode)
        {
            actionButton.onClick.AddListener(() => BuyItem(item));
            actionButton.GetComponentInChildren<TMP_Text>().text = "$" + item.price;
        }
        else
        {
            actionButton.onClick.AddListener(() => SellItem(item));
            actionButton.GetComponentInChildren<TMP_Text>().text = "$" + (item.price - 5);
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

    private void SellItem(ItemData item)
    {
        if (playerMovement != null)
        {
            int slotIndex = player.inventory.FindSlotIndexByItemName(item.itemName);
            if (slotIndex != -1)
            {
                player.inventory.Remove(slotIndex);
                playerMovement.AddMoney(item.price - 5);
                Debug.Log("Item sold successfully!");
            }
            else
            {
                Debug.Log("Item not found in inventory.");
            }
        }
        else
        {
            Debug.LogError("PlayerMovement instance not found.");
        }
    }
}
