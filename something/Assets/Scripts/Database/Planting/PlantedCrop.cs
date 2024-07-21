using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlantedCrop
{
    public Crop crop;
    public int currentStage;
    public float timeToNextStage;

    public PlantedCrop(Crop crop)
    {
        this.crop = crop;
        currentStage = 0;
        timeToNextStage = crop.timeToNextStage;
    }

    public void UpdateGrowth(float deltaTime)
    {
        timeToNextStage -= deltaTime;
        if (timeToNextStage <= 0 && currentStage < crop.growthStages.Length - 1)
        {
            currentStage++;
        }
    }

    public Sprite GetCurrentSprite()
    {
        return crop.growthStages[currentStage];
    }

    public bool IsHarvestable()
    {
        return (currentStage >=  crop.growthStages.Length - 1);
    }
}

