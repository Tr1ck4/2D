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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug character's position
            // Vector3 debugPosition = new Vector3(transform.position.x, transform.position.y, 0);
            // Debug.Log("Character's real position: " + debugPosition);

            Vector3 playerbotcen = GetPlayerBottomCenter();
            int plowX = (int)Math.Floor(playerbotcen.x);
            int plowY = (int)Math.Floor(playerbotcen.y);
            GetPlayerBottomCenter();
            Vector3Int position = 
                new Vector3Int(
                    plowX, 
                    plowY,
                    0
                );

            
            if (GameManager.Instance.tileManager.IsInteractable(position)) // Plow dirt
            {
                Debug.Log("Plowed " +  position);
                GameManager.Instance.tileManager.SetInteracted(position);
            }
            else if (GameManager.Instance.tileManager.IsPlowed(position)) // Plant seed
            {
                Debug.Log("Trying to plant " + position);
                GameManager.Instance.tileManager.PlantCrop(position, cropDatabase.crops[0]);
            }
        }
    }

    private Vector3 GetPlayerBottomCenter() // where the Player is standing
    {
        var bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
        Vector3 bottomCenter = new Vector3(bounds.center.x, bounds.min.y);
        return bottomCenter;
    }
}
