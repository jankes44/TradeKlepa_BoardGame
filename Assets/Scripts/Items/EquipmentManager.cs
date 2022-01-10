using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EquipmentManager : MonoBehaviour {

	public Equipment[] defaultWear;
	public Appearance[] appearance;

	public bool combatPlayer;

	[SerializeField]
	Equipment[] currentEquipment;
	[SerializeField]
	SkinnedMeshRenderer[] currentMeshes;
	SkinnedMeshRenderer[] appearanceMeshes;

	public EqSlot WeaponSlot;
	public EqSlot HeadSlot;
	public EqSlot ChestSlot;
	public EqSlot LegsSlot;
	public EqSlot ShieldSlot;
	public EqSlot FeetSlot;

	public SkinnedMeshRenderer targetMesh;

	// Callback for when an item is equipped
	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public event OnEquipmentChanged onEquipmentChanged;

	Inventory inventory;

	void Start ()
	{
		inventory = GameControl2.instance.MyPlayer.GetComponent<Inventory>();
		EqSlot[] EqSlots = Resources.FindObjectsOfTypeAll<EqSlot>();
		
		foreach (EqSlot item in EqSlots)
        {
            switch (item.equipSlot)
            {
                case EquipmentSlot.Null:
					break;
                case EquipmentSlot.Head:
					HeadSlot = item;
					break;
                case EquipmentSlot.Chest:
					ChestSlot = item;
					break;
                case EquipmentSlot.Legs:
					LegsSlot = item;
					break;
                case EquipmentSlot.Weapon:
					WeaponSlot = item;
					break;
                case EquipmentSlot.Shield:
					ShieldSlot = item;
					break;
                case EquipmentSlot.Feet:
					FeetSlot = item;
					break;
                default:
                    break;
            }
        }

		int numSlots = System.Enum.GetNames (typeof(EquipmentSlot)).Length;
		currentEquipment = new Equipment[numSlots];
		currentMeshes = new SkinnedMeshRenderer[numSlots];
		appearanceMeshes = new SkinnedMeshRenderer[2];

		if (combatPlayer) {
			EquipmentManager eventPlayer = EventControl.instance.eventPlayer.GetComponent<EquipmentManager>();
			defaultWear = eventPlayer.currentEquipment;
			appearance = eventPlayer.appearance;
		}
		
		EquipAllAppearance();
		EquipAllDefault();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.U)) {
			UnequipAll();
		}
	}


	public Equipment GetEquipment(EquipmentSlot slot) {
		return currentEquipment [(int)slot];
	}

	// Equip a new item
	public void Equip (Equipment newItem)
	{	
		if (!newItem) return;
		PlayerStats myPlayer = GameControl2.instance.MyPlayer.GetComponent<PlayerStats>();
		// Find out what slot the item fits in
		// and put it there.
		int slotIndex = (int)newItem.equipSlot;
		Equipment oldItem = Unequip(slotIndex);

		//if there was already item in the slot it will
		//get replaced in this function \/


		switch (newItem.equipSlot)
        {
            case EquipmentSlot.Null:
				Debug.Log("EquipmentSlot is null");
                break;
            case EquipmentSlot.Head:
				HeadSlot.Equip(newItem);
				UnequipAppearance();
				break;
            case EquipmentSlot.Chest:
				ChestSlot.Equip(newItem);
				break;
            case EquipmentSlot.Legs:
				LegsSlot.Equip(newItem);
                break;
            case EquipmentSlot.Weapon:
				WeaponSlot.Equip(newItem);
				break;
            case EquipmentSlot.Shield:
				ShieldSlot.Equip(newItem);
                break;
            case EquipmentSlot.Feet:
				FeetSlot.Equip(newItem);
                break;
            default:
                break;
        }

		// An item has been equipped so we trigger the callback
		if (onEquipmentChanged != null)
			onEquipmentChanged.Invoke(newItem, oldItem);

		currentEquipment [slotIndex] = newItem;
		Debug.Log(newItem.name + " equipped!");

		if (newItem.prefab) {
			AttachToMesh(newItem.prefab, slotIndex);
			myPlayer.EquipItem(newItem.name);
		}
		//equippedItems [itemIndex] = newMesh.gameObject;

	}

	public void EquipSync(Equipment newItem) {
		//attach mesh for other players to see
		int slotIndex = (int)newItem.equipSlot;
		if (newItem.prefab) {
			AttachToMesh(newItem.prefab, slotIndex);
		}
	}

	public void UnequipSync(int slotIndex) {
		//attach mesh for other players to see
		if (currentMeshes [slotIndex] != null) {
			Destroy(currentMeshes [slotIndex].gameObject);
		}
	}

	public Equipment Unequip(int slotIndex) {
		if (currentEquipment[slotIndex] != null)
		{
			Equipment oldItem = currentEquipment [slotIndex];
			inventory.Add(oldItem);

			PlayerStats myPlayer = GameControl2.instance.MyPlayer.GetComponent<PlayerStats>();

			currentEquipment [slotIndex] = null;
			if (currentMeshes [slotIndex] != null) {
				if (oldItem.equipSlot == EquipmentSlot.Head) EquipAllAppearance();
				myPlayer.UnequipItem(slotIndex);
				Destroy(currentMeshes [slotIndex].gameObject);
			}


			// Equipment has been removed so we trigger the callback
			if (onEquipmentChanged != null)
				onEquipmentChanged.Invoke(null, oldItem);

			return oldItem;
		}
		return null;
	
	}

	void UnequipAll() {
		for (int i = 0; i < currentEquipment.Length; i++) {
			Unequip (i);
		}
		EquipAllDefault();
	}

	void EquipAllDefault() {
		foreach (Equipment e in defaultWear) {
			Equip(e);
		}
	}
	void EquipAllAppearance()
	{
		foreach (Appearance e in appearance)
		{
			EquipAppearance(e);
		}
	}

	public void EquipAppearance(Appearance newItem)
    {
		if (newItem.prefab)
		{
			SkinnedMeshRenderer newMesh = Instantiate(newItem.prefab) as SkinnedMeshRenderer;
			if (newItem.name == "Beard") appearanceMeshes[0] = newMesh;
			if (newItem.name == "Hair") appearanceMeshes[1] = newMesh;
			
			newMesh.transform.parent = gameObject.transform;

			newMesh.bones = targetMesh.bones;
			newMesh.rootBone = targetMesh.rootBone;
		}
	}

	public void UnequipAppearance() {
		foreach (SkinnedMeshRenderer e in appearanceMeshes)
		{
			Destroy(e.gameObject);
		}
	}

	void AttachToMesh(SkinnedMeshRenderer mesh, int slotIndex) {

		if (currentMeshes [slotIndex] != null) {
			Destroy(currentMeshes [slotIndex].gameObject);
		}

		SkinnedMeshRenderer newMesh = Instantiate(mesh) as SkinnedMeshRenderer;
		newMesh.transform.parent = gameObject.transform;
		// newMesh.transform.parent = DontDestroy.Instance.gameObject.transform;

		newMesh.bones = targetMesh.bones;
		newMesh.rootBone = targetMesh.rootBone;
		currentMeshes [slotIndex] = newMesh;
	}

}
