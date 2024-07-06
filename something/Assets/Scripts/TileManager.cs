using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // serialized to access from inspector

    [SerializeField] private Tile hiddenInteractableTile;

    [SerializeField] private Tile interactedTile;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);
            if (tile != null && tile.name == "Interactable_Visible")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(position);
        if (tile != null)
        {
            Debug.Log("tile.name = " + tile.name);
            if (tile.name == "Interactable")
            {
                return true;
            }
        }
        Debug.Log("tile is null");
        return false;
    }

    public void SetInteracted(Vector3Int position)
    {
        Color colorFilter = new Color(255, 255, 255, 255);
        interactableMap.SetColor(position, colorFilter);
        interactableMap.SetTile(position, interactedTile);
    }
}
