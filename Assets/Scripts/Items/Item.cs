using UnityEngine;

/* The base item class. All items should derive from this. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

	new public string name = "New Item";	// Name of the item
	public Sprite icon = null;              // Item icon
	public Sprite slotIcon = null;
	public bool showInInventory = true;
	public int rarity;

	public SkinnedMeshRenderer prefab;
	public GameObject weaponPrefab;

	// Called when the item is pressed in the inventory
	public virtual void Use ()
	{
		// Use the item
		// Something may happen
	}

	// Call this method to remove the item from inventory
	public void RemoveFromInventory ()
	{
		GameControl2.instance.MyPlayer.GetComponent<Inventory>().Remove(this);
		//Inventory.instance.Remove(this);
	}

}
