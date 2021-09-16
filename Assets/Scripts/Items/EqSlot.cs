using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* Sits on all InventorySlots. */

public class EqSlot : MonoBehaviour
{

	public Image icon;
	public EquipmentSlot equipSlot;

	Item item;  // Current item in the slot

	//ONLY WEAPON SLOT IS DONE, NEED TO MAP ALL THE OTHER ONES!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	private void Start()
	{
		
	}

	// Add item to the slot
	public void Equip(Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
	}

	// Use the item
	public void Unequip()
	{
		item = null;
		int slotIndex = (int)equipSlot;
		EquipmentManager.instance.Unequip(slotIndex);
		icon.sprite = null;
		icon.enabled = false;
	}

}
