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
        Load();
    }

    public int FindSlotIndexByItemName(string itemName)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName == itemName)
            {
                return i;
            }
        }
        return -1;
    }

    public void Add(Item item)
    {
        //Debug.Log(item.itemName);
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.data.itemName && slot.CanAddItem())
            {
                slot.AddItem(item);
                Save();
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                Save();
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
            Save();
        }
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(saveFilePath, json);
    }

    private void Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            Inventory loadedInventory = JsonUtility.FromJson<Inventory>(json);
            slots = loadedInventory.slots;
        }
    }
}
