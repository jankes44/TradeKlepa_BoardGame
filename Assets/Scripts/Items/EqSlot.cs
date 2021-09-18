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

	private void Start()
	{
		
	}

	// Add item to the slot
	public void Equip(Item newItem)
	{
		item = newItem;

		icon.sprite = item.slotIcon;
		icon.enabled = true;
	}

	// Use the item
	public void Unequip()
	{
		if (item != null)
        {
			Debug.Log("unequip " + item.name);
			item = null;
			int slotIndex = (int)equipSlot;
			GameControl2.instance.MyPlayer.GetComponent<EquipmentManager>().Unequip(slotIndex);
			icon.sprite = null;
			icon.enabled = false;
		}

	}

}
