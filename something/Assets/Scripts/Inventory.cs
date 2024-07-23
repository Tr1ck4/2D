using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    static int DEFAULT_MAX_COUNT = 18;
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

        public bool IsEmpty
        {
            get
            {
                if (itemName == "" && count == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanAddItem(string itemName)
        {
            return this.itemName == itemName && count < maxCount;
        }

        public bool CanAddItem(string itemName, int numNewItems)
        {
            Debug.Log(this.itemName == itemName);
            Debug.Log(count + numNewItems);
            Debug.Log(maxCount);
            return (this.itemName == itemName) && (count + numNewItems <= maxCount);
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.itemSprite;
            this.count++;
        }

        public void AddItem(string itemName, Sprite icon, int numNewItems)
        {
            this.itemName = itemName;
            this.icon = icon;
            this.count+= numNewItems;
        }

        public void RemoveItem(int numRemovedItems)
        {
            if (count > 0)
            {
                count -= numRemovedItems;
                if (count <= 0)
                {
                    icon = null;
                    itemName = "";
                    count = 0;
                }
            }
        }
    }

    public List<Slot> slots = new List<Slot>();

    public int seedSlotID;
    public Slot seedSlot;

    public Inventory(int numSlots)
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "inventoryData.json");
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
        seedSlotID = numSlots;
        seedSlot = new Slot();
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
            if (slot.itemName == item.data.itemName && slot.CanAddItem(item.data.itemName))
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
            slots[slotIndex].RemoveItem(1);
            Save();
        }
        else if (slotIndex == seedSlotID)
        {
            seedSlot.RemoveItem(1);
            Save();
        }
    }

    public void Remove(int slotIndex, int numRemovedItems)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            if (slots[slotIndex].count >= numRemovedItems)
            {
                slots[slotIndex].RemoveItem(numRemovedItems);
                Save();
            }
        }
        else if (slotIndex == seedSlotID)
        {
            if (seedSlot.count >= numRemovedItems)
            {
                seedSlot.RemoveItem(numRemovedItems);
                Save();
            }
        }
    }

    public void MoveSlot(int fromIndex, int toIndex)
    {
        Slot fromSlot;
        Slot toSlot;
        if (fromIndex == seedSlotID)
        {
            fromSlot = seedSlot;
        }
        else
        {
            fromSlot = slots[fromIndex];
        }

        if (toIndex == seedSlotID)
        {
            Debug.Log("moving into seedSlot");
            if (fromSlot.itemName.Contains("seed"))
            {
                toSlot = seedSlot;
            }
            else
            {
                Debug.Log("Seedslot only contains seeds!");
                return;
            }
        }
        else
        {
            toSlot = slots[toIndex];
        }

        if (toSlot.IsEmpty || toSlot.CanAddItem(fromSlot.itemName, fromSlot.count))
        {
            toSlot.AddItem(fromSlot.itemName, fromSlot.icon, fromSlot.count);
            fromSlot.RemoveItem(fromSlot.count);
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
