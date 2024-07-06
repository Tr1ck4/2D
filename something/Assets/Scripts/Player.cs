using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    static int DEFAULT_INVENTORY_SLOTS = 44;
    public Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory(DEFAULT_INVENTORY_SLOTS);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 debugPosition = new Vector3(transform.position.x, transform.position.y, 0);
            Debug.Log("Character's real position: " + debugPosition);

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
            Debug.Log("Plowed position: " + position);
            if (GameManager.Instance.tileManager.IsInteractable(position))
            {
                Debug.Log("on farmland");
                GameManager.Instance.tileManager.SetInteracted(position);
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
