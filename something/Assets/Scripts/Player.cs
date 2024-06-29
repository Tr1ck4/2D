using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static int DEFAULT_INVENTORY_SLOTS = 44;
    public Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory(DEFAULT_INVENTORY_SLOTS);
    }
}
