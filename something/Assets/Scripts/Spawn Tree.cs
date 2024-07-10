using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;  // Assign your tree prefab in the Inspector
    private int numberOfTrees = 35; // Number of trees to spawn

    private static TreeSpawner instance;
    private List<Vector3> treePositions = new List<Vector3>(); // To store the positions of spawned trees
    private List<GameObject> spawnedTrees = new List<GameObject>(); // To store the actual tree GameObjects

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TreeSpawner instance created and set to not destroy on load.");

            LoadTreeData();
            if (treePositions.Count == 0)
            {
                SpawnTrees();
            }
            else
            {
                SpawnTreesFromData();
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Duplicate TreeSpawner instance detected. Destroying the new instance.");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Indoor")
        {
            SetTreesActive(false);
        }
        else
        {
            SetTreesActive(true);
        }
    }

    void SetTreesActive(bool isActive)
    {
        foreach (GameObject tree in spawnedTrees)
        {
            if (tree != null)
            {
                tree.SetActive(isActive);
            }
        }
    }

    void SpawnTrees()
    {
        if (treePrefab == null)
        {
            Debug.LogWarning("Tree Prefab is not assigned.");
            return;
        }

        int spawnedTreesCount = 0;
        int attempts = 0; // To avoid infinite loops
        while (spawnedTreesCount < numberOfTrees && attempts < numberOfTrees * 10)
        {
            Vector2 randomPosition = GetRandomPosition();
            Vector3 spawnPosition = new Vector3(randomPosition.x, randomPosition.y, 0);

            // Check if the position collides with any "Static" tagged objects
            Collider2D hitCollider = Physics2D.OverlapCircle(spawnPosition, 0.5f);
            if (hitCollider != null && hitCollider.CompareTag("Static"))
            {
                attempts++;
                continue; // Skip this position
            }

            GameObject tree = Instantiate(treePrefab, spawnPosition, Quaternion.identity);
            DontDestroyOnLoad(tree); // Make the tree persistent across scenes
            spawnedTrees.Add(tree);
            treePositions.Add(spawnPosition);
            spawnedTreesCount++;
        }

        if (spawnedTreesCount < numberOfTrees)
        {
            Debug.LogWarning($"Could only spawn {spawnedTreesCount} trees out of {numberOfTrees} due to position constraints.");
        }

        SaveTreeData();
    }

    void SpawnTreesFromData()
    {
        foreach (Vector3 position in treePositions)
        {
            GameObject tree = Instantiate(treePrefab, position, Quaternion.identity);
            DontDestroyOnLoad(tree); // Make the tree persistent across scenes
            spawnedTrees.Add(tree);
        }
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(-14.0f, 30.0f);
        float y = Random.Range(-25.0f, 11.0f);
        return new Vector2(x, y);
    }

    void SaveTreeData()
{
    TreeData data = new TreeData();
    data.positions = new List<Vector3Serializable>();
    foreach (Vector3 position in treePositions)
    {
        data.positions.Add(new Vector3Serializable(position));
    }

    string json = JsonUtility.ToJson(data);
    File.WriteAllText(GetTreeDataPath(), json);
}


    void LoadTreeData()
{
    string path = GetTreeDataPath();
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        TreeData data = JsonUtility.FromJson<TreeData>(json);
        treePositions = new List<Vector3>();
        foreach (Vector3Serializable position in data.positions)
        {
            treePositions.Add(position.ToVector3());
        }
    }
}


    private string GetTreeDataPath()
{
    return Path.Combine(Application.persistentDataPath, "treeData.json");
}

}

[System.Serializable]
public class TreeData
{
    public List<Vector3Serializable> positions;
}

[System.Serializable]
public struct Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
