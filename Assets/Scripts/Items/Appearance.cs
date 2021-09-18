using UnityEngine;

/* An Item that can be equipped to increase armor/damage. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Appearance")]
public class Appearance : Item
{
	public SkinnedMeshRenderer prefab;

	// Called when pressed in the inventory
	public override void Use()
	{
		//EquipmentManager.instance.Equip(this);  // Equip
		GameControl2.instance.MyPlayer.GetComponent<EquipmentManager>().EquipAppearance(this);    // Equip
	}

}
