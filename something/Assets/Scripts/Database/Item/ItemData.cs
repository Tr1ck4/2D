using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int price;
    public Sprite itemSprite;
    public AnimationClip dropClip;
}
