using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    static int DEFAULT_MAX_COUNT = 64;

    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
        public int count; // Number of items in slot
        public int maxCount; // Slot's capacity

        public Slot()
        {
            type = CollectableType.NONE;
            count = 0;
            maxCount = DEFAULT_MAX_COUNT;
        }

        public bool CanAddItem()
        {
            return count < maxCount;
        }

        public void AddItem()
        {
            this.count++;
        }

        public void AddNewItemType(CollectableType type)
        {
            this.type = type;
            this.count++;
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

    public void Add(CollectableType type)
    {
        // If there is a slot of same type, add to that slot
        foreach (Slot slot in slots)
        {
            if (slot.type == type && slot.CanAddItem())
            {
                slot.AddItem();
                return;
            }
        }
        // No slot of the same type, use the first found empty slot
        foreach (Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE)
            {
                slot.AddNewItemType(type);
                return;
            }
        }
    }
}
