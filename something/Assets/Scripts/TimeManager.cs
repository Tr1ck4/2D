using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float dayLength = 1f; // Length of a day in seconds
    private float timeElapsed = 0f;
    public event System.Action OnDayPassed;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= dayLength)
        {
            timeElapsed = 0f;
            OnDayPassed?.Invoke();
        }
    }
}
