using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* Sits on all InventorySlots. */

public class InventorySlot : MonoBehaviour {

	public Image icon;
	public Button removeButton;
	public EquipmentSlot equipSlot;
	public bool isEqHolder = false;

	Item item;  // Current item in the slot

    private void Start()
    {
        if (!isEqHolder)
        {
			equipSlot = EquipmentSlot.Null;
        }
    }

    // Add item to the slot
    public void AddItem (Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
		if (removeButton)
			removeButton.interactable = true;
		
	}

	// Clear the slot
	public void ClearSlot ()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		if (removeButton)
			removeButton.interactable = false;
	}

	// If the remove button is pressed, this function will be called.
	public void RemoveItemFromInventory ()
	{
		Inventory.instance.Remove(item);
	}

	// Use the item
	public void UseItem ()
	{
		if (item != null)
		{
			item.Use();
		}
	}

}
