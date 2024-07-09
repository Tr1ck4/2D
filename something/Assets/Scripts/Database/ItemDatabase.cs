using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 2)]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] items;
}
