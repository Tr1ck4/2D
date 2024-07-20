using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crop")]
public class Crop : ScriptableObject
{
    public string cropName;
    public Sprite[] growthStages; // Array of sprites for each growth stage
    public float timeToNextStage; // Time to progress to the next stage
}
