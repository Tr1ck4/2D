using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Player player;
    public GameObject inventorySlotPrefab; // Reference to the InventorySlot prefab
    public List<Slot_UI> slots = new List<Slot_UI>();

    private void Start()
    {
        inventoryPanel.SetActive(false);
        FindPlayer();
        Refresh(); // Ensure UI reflects initial state
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            Refresh();
        }
    }

    void Refresh()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null. Trying to find player again.");
            FindPlayer();
            if (player == null)
            {
                Debug.LogError("Failed to find player.");
                return;
            }
        }

        if (player.inventory == null)
        {
            Debug.LogError("Player's inventory is null.");
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < player.inventory.slots.Count)
            {
                if (!string.IsNullOrEmpty(player.inventory.slots[i].itemName))
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else
                {
                    slots[i].SetEmpty(); // Optionally clear the UI if no item is present
                }
            }
            else
            {
                slots[i].SetEmpty(); // Clear UI for unused slots
            }
        }
    }

    public void Remove(int slotIndex)
    {

        if (player == null)
        {
            Debug.LogError("Player reference is null. Trying to find player again.");
            FindPlayer();
            if (player == null)
            {
                Debug.LogError("Failed to find player.");
                return;
            }
        }

        if (player.inventory == null)
        {
            Debug.LogError("Player's inventory is null.");
            return;
        }

        player.inventory.Remove(slotIndex);
        Refresh();
    }

    private void FindPlayer()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene. Creating a new player object.");
            GameObject playerGO = new GameObject("Player");
            player = playerGO.AddComponent<Player>();
            DontDestroyOnLoad(playerGO);
        }
        else
        {
            Debug.Log("Player found in scene.");
        }
    }
}
