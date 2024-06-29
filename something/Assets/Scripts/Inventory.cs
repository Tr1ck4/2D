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
        public CollectableType type;
        public int count; // Number of items in slot
        public int maxCount; // Slot's capacity

        public Sprite icon;

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

        public void AddItem(Collectable item)
        {
            this.type = item.type;
            this.icon = item.icon;
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

    public void Add(Collectable item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == item.type && slot.CanAddItem())
            {
                slot.AddItem(item);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE)
            {
                slot.AddItem(item);
                return;
            }
        }
    }
}
