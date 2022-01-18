using UnityEngine;

/* An Item that can be equipped to increase armor/damage. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item {

	public EquipmentSlot equipSlot;		// What slot to equip it in
	public int armorModifier;
	public int damageModifier;
	public SkinnedMeshRenderer prefab;
	public GameObject weaponPrefab;

	public Vector3 PickPosition;
	public Vector3 PickRotation;
	public Vector3 PickScale;

	public Vector3 PositionCombat;
	public Vector3 RotationCombat;
	public Vector3 ScaleCombat;

	// Called when pressed in the inventory
	public override void Use ()
	{
		//EquipmentManager.instance.Equip(this);  // Equip
		GameControl2.instance.MyPlayer.GetComponent<EquipmentManager>().Equip(this);	// Equip
		RemoveFromInventory();	// Remove from inventory
	}

}

public enum EquipmentSlot { Null, Head, Chest, Legs, Weapon, Shield, Feet}
