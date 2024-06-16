using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;  // Assign your tree prefab in the Inspector
    public int numberOfTrees = 10; // Number of trees to spawn
    public Vector2 areaSize = new Vector2(50, 50); // Size of the area in which to spawn trees

    void Start()
    {
        SpawnTrees();
    }

    void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector2 randomPosition = GetRandomPosition();
            Instantiate(treePrefab, randomPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float y = Random.Range(-areaSize.y / 2, areaSize.y / 2);
        return new Vector2(x, y) + (Vector2)transform.position;
    }
}


