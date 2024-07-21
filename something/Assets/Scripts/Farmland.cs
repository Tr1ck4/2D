using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Farmland : MonoBehaviour
{
    private Farmland instance;

    [SerializeField] private Tilemap dirtMap;
    [SerializeField] private Tilemap cropMap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        else
        {
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
            SetTilemapsActive(false);
        }
        else
        {
            SetTilemapsActive(true);
        }
    }

    void SetTilemapsActive(bool isActive)
    {
        dirtMap.gameObject.SetActive(isActive);
        cropMap.gameObject.SetActive(isActive);
    }

}
