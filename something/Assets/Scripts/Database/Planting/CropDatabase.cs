using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDatabase", menuName = "ScriptableObjects/CropDatabase")]
public class CropDatabase : ScriptableObject
{
    public Crop[] crops;
}
