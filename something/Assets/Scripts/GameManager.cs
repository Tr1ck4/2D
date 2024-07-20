using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * This class is designed in Singleton Pattern (only need to manage the game once) --> from now refered as 'this object'.
     * This object manages: game states
     */

    public static GameManager Instance;

    public TileManager tileManager;
    public TimeManager timeManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // if already exists, destroy 1 instance
        }
        else
        {
            Instance = this; // else it is the first and only instance
        }

        DontDestroyOnLoad(this.gameObject);

        tileManager = GetComponent<TileManager>();
        timeManager = GetComponent<TimeManager>();
    }

    private void Start()
    {
        if (timeManager != null)
        {
            timeManager.OnDayPassed += OnDayPassed;
        }
    }

    private void OnDayPassed()
    {
        tileManager.UpdateGrowth(timeManager.dayLength);
    }
}
