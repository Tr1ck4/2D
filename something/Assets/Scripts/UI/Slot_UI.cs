using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot_UI : MonoBehaviour
{
    public int slotID;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null && itemIcon != null && quantityText != null)
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = Color.white; // Ensure the icon is visible
            quantityText.text = slot.count.ToString();
        }
        else
        {
            Debug.LogWarning("Slot or UI components not properly initialized.");
            if (slot == null)
            {
                Debug.Log("slot is null");
            }
            if (itemIcon == null)
            {
                Debug.Log("itemIcon is null");
            }
            if (quantityText == null)
            {
                Debug.Log("quantityText is null");
            }
        }
    }

    public void SetEmpty()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.color = Color.clear; // Make the icon transparent
        }
        if (quantityText != null)
        {
            quantityText.text = "";
        }
    }
}
