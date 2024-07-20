using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    static int DEFAULT_INVENTORY_SLOTS = 44;
    public Inventory inventory;

    public CropDatabase cropDatabase; 

    private void Awake()
    {
        inventory = new Inventory(DEFAULT_INVENTORY_SLOTS);
    }

    private void Start()
    {
        if (cropDatabase == null)
        {
            Debug.Log("Player.cropDatabase is null");
            return;
        }
    }
}
