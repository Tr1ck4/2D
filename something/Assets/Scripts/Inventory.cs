using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    static int DEFAULT_MAX_COUNT = 44;
    private string saveFilePath;

    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count; // Number of items in slot
        public int maxCount; // Slot's capacity
        public Sprite icon;

        public Slot()
        {
            itemName = "";
            count = 0;
            maxCount = DEFAULT_MAX_COUNT;
        }

        public bool CanAddItem()
        {
            return count < maxCount;
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.itemSprite;
            this.count++;
        }

        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;
                if (count == 0)
                {
                    icon = null;
                    itemName = "" ;

                }
            }
        }
    }

    public List<Slot> slots = new List<Slot>();

    public Inventory(int numSlots)
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "inventoryData.json");
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
        Load(); // Load inventory from file if it exists
    }

    public void Add(Item item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.data.itemName && slot.CanAddItem())
            {
                slot.AddItem(item);
                Save(); // Save after adding item
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                Save(); // Save after adding item
                return;
            }
        }
    }

    public void Add(ItemData itemData)
    {
        Item item = new GameObject().AddComponent<Item>();
        item.data = itemData;
        Add(item);
    }

    public void Remove(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            slots[slotIndex].RemoveItem();
            Save(); // Save after removing item
        }
    }

    private void Save()
    {
        // Serialize the inventory to JSON
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(saveFilePath, json);
    }

    private void Load()
    {
        // Deserialize the inventory from JSON
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            Inventory loadedInventory = JsonUtility.FromJson<Inventory>(json);
            // Copy data from loadedInventory to this instance
            slots = loadedInventory.slots;
        }
    }
}
