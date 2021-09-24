using UnityEngine;

/* The base item class. All items should derive from this. */

[CreateAssetMenu(fileName = "New Event", menuName = "EventSystem/Event")]
public class EventObject : ScriptableObject {

	public new string name;
	public string description;
	public string type;

	public Sprite artwork;

	public void Print ()
	{
		Debug.Log(name + ": " + description);
	}


	// Called when the item is pressed in the inventory
	public virtual void Use ()
	{
		// Use the item
		// Something may happen
	}

	// Call this method to remove the item from inventory
	public void RemoveFromInventory ()
	{
		// GameControl2.instance.MyPlayer.GetComponent<Inventory>().Remove(this);
		//Inventory.instance.Remove(this);
	}

}
