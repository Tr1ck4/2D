using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [SerializeField] private Tilemap dirtMap; // Serialized to access from inspector
    [SerializeField] private Tilemap cropMap;
    [SerializeField] private Tile hiddenInteractableTile;
    [SerializeField] private Tile plowedTile;
    [SerializeField] private Tile moisturizedTile;

    private Dictionary<Vector3Int, PlantedCrop> plantedCrops = new Dictionary<Vector3Int, PlantedCrop>();

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

    public bool IsPlowable(Vector3Int position)
    {
        TileBase tile = dirtMap.GetTile(position);
        return tile != null && tile.name == "Interactable";
    }

    public bool IsPlowed(Vector3Int position)
    {
        TileBase tile = dirtMap.GetTile(position);
        return tile != null && tile.name == "PlowedDirt";
    }

    public bool IsMoisturized(Vector3Int position)
    {
        TileBase tile = dirtMap.GetTile(position);
        return tile != null && tile.name == "MoisturizedDirt";
    }

    public bool HasCrop(Vector3Int position)
    {
        return plantedCrops.ContainsKey(position);
    }

    public bool IsHarvestable(Vector3Int position)
    {
        if (plantedCrops.TryGetValue(position, out var plantedCrop))
        {
            return plantedCrop.IsHarvestable();
        }
        return false;
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
        if (plantedCrops.TryGetValue(position, out var plantedCrop))
        {
            if (plantedCrop.IsHarvestable())
            {
                SpawnHarvestedItem(position, plantedCrop.crop);
                plantedCrops.Remove(position);
                SetNormal(position);
            }
        }
    }

    public void SpawnHarvestedItem(Vector3Int position, Crop crop)
    {
        if (crop.harvestDropPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(position.x, position.y, position.z);
            GameObject harvestedItem = Instantiate(crop.harvestDropPrefab, spawnPosition, Quaternion.identity);

            Animator itemAnimator = harvestedItem.GetComponent<Animator>();
            Item itemComponent = harvestedItem.GetComponent<Item>();

            if (itemAnimator != null)
            {
                ModifyAnimationClip(itemComponent.data.dropClip, harvestedItem.transform.position);
                // itemAnimator.Play(itemComponent.data.dropClip.name);
            }
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
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

        Debug.Log("Animation clip modified.");
    }

    private void UpdateTileAppearance(Vector3Int position, PlantedCrop plantedCrop)
    {
        Sprite cropSprite = plantedCrop.GetCurrentSprite();
        Tile cropTile = ScriptableObject.CreateInstance<Tile>();
        cropTile.sprite = cropSprite;
        cropMap.SetTile(position, cropTile);
    }

    public void UpdateGrowth(float deltaTime)
    {
        foreach (var position in plantedCrops.Keys)
        {
            plantedCrops[position].UpdateGrowth(deltaTime);
            UpdateTileAppearance(position, plantedCrops[position]);
        }
    }

    // Save and Load functionality
    [System.Serializable]
    public class TileState
    {
        public Vector3Int position;
        public bool isPlowed;
        public bool isMoisturized;
    }

    [System.Serializable]
    public class PlantedCropState
    {
        public Vector3Int position;
        public string cropName;
        public int currentStage;
        public float growthProgress;
    }

    [System.Serializable]
    public class GameState
    {
        public List<TileState> tileStates;
        public List<PlantedCropState> cropStates;
    }

    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "gameSave.json");
    }

    public void SaveGame()
    {
        GameState state = new GameState();
        state.tileStates = new List<TileState>();
        state.cropStates = new List<PlantedCropState>();

        foreach (var position in dirtMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = dirtMap.GetTile(position);
            if (tile != null && (tile == plowedTile || tile == moisturizedTile))
            {
                TileState tileState = new TileState
                {
                    position = position,
                    isPlowed = tile == plowedTile,
                    isMoisturized = tile == moisturizedTile
                };
                state.tileStates.Add(tileState);
            }
        }

        foreach (var kvp in plantedCrops)
        {
            PlantedCropState cropState = new PlantedCropState
            {
                position = kvp.Key,
                cropName = kvp.Value.crop.cropName,
                currentStage = kvp.Value.currentStage,
                growthProgress = kvp.Value.timeToNextStage
            };
            state.cropStates.Add(cropState);
        }

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(GetSaveFilePath(), json);
        Debug.Log("Game saved to " + GetSaveFilePath());
    }

    public void LoadGame()
    {
        string filePath = GetSaveFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameState state = JsonUtility.FromJson<GameState>(json);

            foreach (var tileState in state.tileStates)
            {
                if (tileState.isPlowed)
                {
                    SetPlowed(tileState.position);
                }
                else if (tileState.isMoisturized)
                {
                    SetMoisturized(tileState.position);
                }
            }

            foreach (var cropState in state.cropStates)
            {
                Crop crop = FindCropByName(cropState.cropName);
                if (crop != null)
                {
                    PlantedCrop plantedCrop = new PlantedCrop(crop)
                    {
                        currentStage = cropState.currentStage,
                        timeToNextStage = cropState.growthProgress
                    };
                    plantedCrops[cropState.position] = plantedCrop;
                    UpdateTileAppearance(cropState.position, plantedCrop);
                }
            }

            Debug.Log("Game loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + filePath);
        }
    }

    private Crop FindCropByName(string cropName)
    {
        foreach (var crop in GameManager.Instance.cropDatabase.crops)
        {
            if (crop.cropName == cropName)
            {
                return crop;
            }
        }
        return null;
    }
}
