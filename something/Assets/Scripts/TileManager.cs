using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap dirtMap; // serialized to access from inspector
    [SerializeField] private Tilemap cropMap;

    [SerializeField] private Tile hiddenInteractableTile;

    [SerializeField] private Tile interactedTile;

    private Dictionary<Vector3Int, PlantedCrop> plantedCrops = new Dictionary<Vector3Int, PlantedCrop>();


    // Start is called before the first frame update
    void Start()
    {
        foreach (var position in dirtMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = dirtMap.GetTile(position);
            if (tile != null && tile.name == "Interactable_Visible")
            {
                dirtMap.SetTile(position, hiddenInteractableTile);
                cropMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    public bool IsInteractable(Vector3Int position) // actually IsPlowable
    {
        TileBase tile = dirtMap.GetTile(position);
        if (tile != null)
        {
            if (tile.name == "Interactable")
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPlowed(Vector3Int position)
    {
        TileBase tile = dirtMap.GetTile(position);
        if (tile != null)
        {
            if (tile.name == "HoedDirt")
            {
                return true;
            }
        }
        return false;
    }

    public void SetInteracted(Vector3Int position)
    {
        Color colorFilter = new Color(255, 255, 255, 255);
        dirtMap.SetColor(position, colorFilter);
        dirtMap.SetTile(position, interactedTile);
    }

    public void PlantCrop(Vector3Int position, Crop crop)
    {
        if (!plantedCrops.ContainsKey(position))
        {
            PlantedCrop plantedCrop = new PlantedCrop(crop);
            plantedCrops[position] = plantedCrop;
            UpdateTileAppearance(position, plantedCrop);
        }
    }

    public void UpdateGrowth(float deltaTime)
    {
        foreach (var position in plantedCrops.Keys)
        {
            plantedCrops[position].UpdateGrowth(deltaTime);
            UpdateTileAppearance(position, plantedCrops[position]);
        }
    }

    private void UpdateTileAppearance(Vector3Int position, PlantedCrop plantedCrop)
    {
        Sprite cropSprite = plantedCrop.GetCurrentSprite();
        Tile cropTile = ScriptableObject.CreateInstance<Tile>();
        cropTile.sprite = cropSprite;
        cropMap.SetTile(position, cropTile);
    }
}
