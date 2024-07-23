using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDatabase", menuName = "ScriptableObjects/CropDatabase")]
public class CropDatabase : ScriptableObject
{
    public Crop[] crops;

    public Crop FindCropByName(string seedItemName)
    {
        if (seedItemName != "" && seedItemName != null)
        {
            foreach (Crop c in crops)
            {
                if (seedItemName.ToLower().Contains(c.cropName.ToLower()))
                {
                    Debug.Log("Found crop: " + c.cropName.ToLower());
                    return c;
                }
            }
        }

        Debug.Log("No seed");

        return null;
    }
}
