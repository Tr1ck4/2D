using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [SerializeField] private Tilemap dirtMap; // serialized to access from inspector
    [SerializeField] private Tilemap cropMap;

    private GameObject tilemapRoot;

    [SerializeField] private Tile hiddenInteractableTile;

    [SerializeField] private Tile plowedTile;
    [SerializeField] private Tile moisturizedTile;

    private Dictionary<Vector3Int, PlantedCrop> plantedCrops = new Dictionary<Vector3Int, PlantedCrop>();


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeTilemaps();
    }

    void Start()
    {
        InitializeTilemaps();
    }

    private void InitializeTilemaps()
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

    public bool IsPlowable(Vector3Int position) // actually IsPlowable
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
            if (tile.name == "PlowedDirt")
            {
                return true;
            }
        }
        return false;
    }

    public bool IsMoisturized(Vector3Int position)
    {
        TileBase tile = dirtMap.GetTile(position);
        if (tile != null)
        {
            if (tile.name == "MoisturizedDirt")
            {
                return true;
            }
        }
        return false;
    }

    public bool HasCrop(Vector3Int position) // Checks if a tile has crop planted
    {
        return plantedCrops.ContainsKey(position);
    }

    public bool IsHarvestable(Vector3Int position)
    {
        Debug.Log("IsHarvestable() called");
        if (plantedCrops.ContainsKey(position))
        {
            if (plantedCrops[position].IsHarvestable())
            {
                Debug.Log("Crop is READY");
                return true;
            }
            else
            {
                Debug.Log("Crop is not harvestable");
                return false;
            }
        }
        else
        {
            Debug.Log("No crop detected");
            return false;
        }
    }

    public void SetNormal(Vector3Int position)
    {
        dirtMap.SetTile(position, hiddenInteractableTile);
        cropMap.SetTile(position, hiddenInteractableTile);
    }

    public void SetPlowed(Vector3Int position)
    {
        dirtMap.SetTile(position, plowedTile);
    }

    public void SetMoisturized(Vector3Int position)
    {
        dirtMap.SetTile(position, moisturizedTile);
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

    public void Harvest(Vector3Int position)
    {

        if (plantedCrops.ContainsKey(position))
        {
            Debug.Log("Detected crop:" + plantedCrops[position].crop.name);
            SpawnHarvestedItem(position);

            plantedCrops.Remove(position);
            SetNormal(position);
        }
    }

    public void SpawnHarvestedItem(Vector3Int position) // Drop item "vegie" when harvesting
    {
        if (plantedCrops[position].crop.harvestDropPrefab != null)
        {
            GameObject vegie = Instantiate(plantedCrops[position].crop.harvestDropPrefab, position, Quaternion.identity);

            Animator vegAnimator = vegie.GetComponent<Animator>();
            Item vegItem = vegie.GetComponent<Item>();

            if (vegie != null && vegItem != null && vegItem.data != null && vegItem.data.dropClip != null)
            {
                ModifyAnimationClip(vegItem.data.dropClip, vegie.transform.position);
                vegAnimator.Play(vegItem.data.dropClip.name);
            }
        }
        else
        {
            Debug.LogWarning("harvestDropPrefab is not assigned.");
        }
    }

    private void ModifyAnimationClip(AnimationClip clip, Vector3 startPosition)
    {
        Keyframe[] posX = {
            new Keyframe(0, startPosition.x),
            new Keyframe(0.5f, startPosition.x + 0.5f),
            new Keyframe(1, startPosition.x + 1f)
        };
        Keyframe[] posY = {
            new Keyframe(0, startPosition.y),
            new Keyframe(0.5f, startPosition.y + 0.3f),
            new Keyframe(1, startPosition.y - 0.8f)
        };
        Keyframe[] posZ = {
            new Keyframe(0, startPosition.z),
            new Keyframe(0.5f, startPosition.z),
            new Keyframe(1, startPosition.z)
        };

        AnimationCurve curveX = new AnimationCurve(posX);
        AnimationCurve curveY = new AnimationCurve(posY);
        AnimationCurve curveZ = new AnimationCurve(posZ);

        clip.ClearCurves();
        clip.SetCurve("", typeof(UnityEngine.Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(UnityEngine.Transform), "localPosition.y", curveY);
        clip.SetCurve("", typeof(UnityEngine.Transform), "localPosition.z", curveZ);

        Debug.Log("Animation clip modified.");
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
