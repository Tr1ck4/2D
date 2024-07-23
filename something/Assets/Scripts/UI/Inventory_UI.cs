using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Player player;
    public GameObject inventorySlotPrefab; // Reference to the InventorySlot prefab
    public List<Slot_UI> slots = new List<Slot_UI>();
    public Slot_UI seedSlot;

    [SerializeField] private Canvas canvas;

    private Slot_UI draggedSlot; // slot being dragged
    private Image draggedIcon;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        FindPlayer();
        SetupSlots();
        Refresh(); // Ensure UI reflects initial state
    }

    private void Awake()
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.Log("canvas is null");
        }
        else
        {
            Debug.Log("found canvas: " +  canvas.name);
        }
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

        if (!string.IsNullOrEmpty(player.inventory.seedSlot.itemName))
        {
            seedSlot.SetItem(player.inventory.seedSlot);
        }
        else
        {
            seedSlot.SetEmpty(); // Optionally clear the UI if no item is present
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

    public void DragRemove()
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

        if (draggedSlot.slotID == seedSlot.slotID)
        {
            player.inventory.Remove(draggedSlot.slotID, player.inventory.seedSlot.count);
        }
        else if (draggedSlot.slotID < player.inventory.slots.Count)
        {
            player.inventory.Remove(draggedSlot.slotID, player.inventory.slots[draggedSlot.slotID].count);
        }
        Refresh();

        draggedSlot = null;
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

    public void SlotBeginDrag(Slot_UI slot)
    {
        draggedSlot = slot;
        draggedIcon = Instantiate(draggedSlot.itemIcon);
        draggedIcon.transform.SetParent(canvas.transform);
        draggedIcon.raycastTarget = false;
        draggedIcon.rectTransform.sizeDelta = new Vector2(50, 50);

        MoveToMousePosition(draggedIcon.gameObject);
        Debug.Log("Start Drag: " + draggedSlot.name);
    }

    public void SlotDrag() // Everytime mouse moves when draggin
    {
        MoveToMousePosition(draggedIcon.gameObject);

        Debug.Log("Dragging: " + draggedSlot.name);
    }

    public void SlotEndDrag() // Called when stop dragging
    {
        //Debug.Log("Done dragging: " + draggedSlot.name);
        Destroy(draggedIcon.gameObject);
        draggedIcon = null;
    }

    public void SlotDrop(Slot_UI slot) // Called by the second slot
    {
        //Debug.Log("Dropped: " + draggedSlot.name + " on " + slot.name);
        player.inventory.MoveSlot(draggedSlot.slotID, slot.slotID);
        Refresh();
    }

    public void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                Input.mousePosition,
                null,
                out position);

            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
        else
        {
            Debug.Log("MTMP: canvas is null");
        }

    }

    void SetupSlots()
    {
        int counter = 0;
        foreach (Slot_UI slot in slots)
        {
            slot.slotID = counter;
            counter++;
        }

        seedSlot.slotID = player.inventory.seedSlotID;
    }
}
