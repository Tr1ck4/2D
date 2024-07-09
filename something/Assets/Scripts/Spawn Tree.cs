using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;  // Assign your tree prefab in the Inspector
    public int numberOfTrees = 10; // Number of trees to spawn

    private static TreeSpawner instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TreeSpawner instance created and set to not destroy on load.");
            SpawnTrees();
        }
        else
        {
            Debug.Log("Duplicate TreeSpawner instance detected. Destroying the new instance.");
            Destroy(gameObject);
        }
    }

    void SpawnTrees()
    {
        if (treePrefab == null)
        {
            Debug.LogWarning("Tree Prefab is not assigned.");
            return;
        }

        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector2 randomPosition = GetRandomPosition();
            Debug.Log(randomPosition);
            Vector3 spawnPosition = new Vector3(randomPosition.x, randomPosition.y, 0);
            Instantiate(treePrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(-14.0f, 30.0f);
        float y = Random.Range(-25.0f, 11.0f);
        return new Vector2(x, y);
    }
}
