using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    static int DEFAULT_MAX_COUNT = 44;
    
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
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    public void Add(Item item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.data.itemName && slot.CanAddItem())
            {
                slot.AddItem(item);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                return;
            }
        }
    }

    public void Remove(int slotIndex)
    {
        slots[slotIndex].RemoveItem();
    }
}
