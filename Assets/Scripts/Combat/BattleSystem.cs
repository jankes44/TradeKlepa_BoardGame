using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	public GameObject combatHUD;

	public GameObject playerSpawn;
	public GameObject enemySpawn;

	Vector3 AttackEnemyPos;
	Vector3 AttackPlayerPos;

	Vector3 EnemySpawnPos;
	Vector3 PlayerSpawnPos;

	Vector3 targetPos;
	
	bool playerMove = false;
	bool enemyMove = false;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	PlayerStats eventPlayer;

	public Chest chest;
	public Transform chestEndMarker;

	bool actionInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
		GameControl2.instance.battleSystem = this;
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	void Update() {
		// if (playerMove) {
		// 	float step = 2f * Time.deltaTime;
      	// 	playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, targetPos, step);
		// }
		// if (playerUnit.transform.position == targetPos) {
      	// 	playerMove = false;
       	// }
	}

	IEnumerator SetupBattle()
	{
		//player unit
		GameObject playerGO = Instantiate(playerPrefab, playerSpawn.transform.position, playerPrefab.transform.rotation);
		playerUnit = playerGO.GetComponent<Unit>();
		if (EventControl.instance != null) {
			eventPlayer = EventControl.instance.eventPlayer.GetComponent<PlayerStats>();
		}

		if (eventPlayer != null) {
			int eventActorNo = eventPlayer.actorNo;
			int myActorNo = GameControl2.instance.MyPlayer.GetComponent<PlayerStats>().actorNo;

			if (eventActorNo != myActorNo) {
				combatHUD.SetActive(false);
			}

			Debug.Log(eventPlayer.playerName);
			playerUnit.unitLevel = eventPlayer.level;
			playerUnit.unitName = eventPlayer.playerName; 
			playerUnit.maxHP = eventPlayer.maxHP;
			playerUnit.currentHP = eventPlayer.maxHP; 
			playerUnit.agility = eventPlayer.agility; 
			playerUnit.strength = eventPlayer.strength; 
			playerUnit.vitality = eventPlayer.vitality; 
			playerUnit.damage = eventPlayer.damage;
		}
		enemyPrefab = EventControl.instance.currentEnemy;

		//enemy unit
		GameObject enemyGO = Instantiate(enemyPrefab, enemySpawn.transform.position, enemyPrefab.transform.rotation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		AttackEnemyPos = new Vector3(enemyUnit.transform.position.x - 1.5f, playerUnit.transform.position.y, playerUnit.transform.position.z);
		EnemySpawnPos = new Vector3(enemySpawn.transform.position.x, enemyUnit.transform.position.y, enemyUnit.transform.position.z);

		AttackPlayerPos = new Vector3(playerUnit.transform.position.x + 1.5f, enemyUnit.transform.position.y, enemyUnit.transform.position.z);
		PlayerSpawnPos = new Vector3(playerSpawn.transform.position.x, playerUnit.transform.position.y, playerUnit.transform.position.z);

		dialogueText.text = "Dziki " + enemyUnit.unitName + " wkracza...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	public IEnumerator PlayerAttack(string typeOfAttack, int chance, int chanceEnemy, int typeOfAttackEnemy)
	{
		actionInProgress = true;
		combatHUD.SetActive(false);

		//player get closer to enemy
		targetPos = AttackEnemyPos;
		playerMove = true;

		playerUnit.WalkAnim(true);
		while(playerMove) 
        {
			float step = 2f * Time.deltaTime;
			playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, targetPos, step);
			if (playerUnit.transform.position == targetPos) {
				playerMove = false;
				
			}
			yield return null;
        }

		playerUnit.SlashAnimPlayer(true, typeOfAttack);
		playerUnit.WalkAnim(false);

		yield return new WaitForSeconds(1.75f);

		playerUnit.SlashAnimPlayer(false, typeOfAttack);

		int baseDamage = playerUnit.damage;
		int damage = baseDamage;

		float chanceMultiplier = playerUnit.agilityMultiplier;
		float strengthMultiplier = playerUnit.strengthMultiplier;
		string attackDialog = "";

		float chanceCalculated = 1f+chance/100f;

		switch (typeOfAttack)
        {
			case "strong":

				//multipliers
				damage = Mathf.RoundToInt(baseDamage * chanceCalculated * strengthMultiplier * 1.75f);

				if (chance < 75 / chanceMultiplier)
				{
					damage = 0;
					attackDialog = "Nie trafiłeś!";
				} else
                {
					attackDialog = "Trafiłeś za "+damage+"!";
				}

				break;
			case "medium":
				damage = Mathf.RoundToInt(baseDamage * chanceCalculated * strengthMultiplier * 1.4f);

				if (chance < 50 / chanceMultiplier)
				{
					damage = 0;
					attackDialog = "Nie trafiłeś!";
				} else
                {
					attackDialog = "Trafiłeś za "+damage+"!";
				}

				break;
			case "weak":
				damage = Mathf.RoundToInt(baseDamage * chanceCalculated * strengthMultiplier);

				if (chance < 15 / chanceMultiplier)
				{
					damage = 0;
					attackDialog = "Nie trafiłeś!";
				} else
                {
					attackDialog = "Trafiłeś za "+damage+"!";
				}

				break;
        }
		
		bool isDead = enemyUnit.TakeDamage(damage);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = attackDialog;
		
		if (damage > 0) enemyUnit.TakeDmgAnim(true);

		//player back to position
		targetPos = PlayerSpawnPos;
		playerMove = true;

		while(playerMove) 
        {
			float step = 2f * Time.deltaTime;
			playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, targetPos, step);
			if (playerUnit.transform.position == targetPos) {
				playerMove = false;
			}
			yield return null;
        }
		
		yield return new WaitForSeconds(2f);
		
		if (damage > 0) enemyUnit.TakeDmgAnim(false);

		if (isDead)
		{
			state = BattleState.WON;
			actionInProgress = false;
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.ENEMYTURN;
			actionInProgress = false;
			StartCoroutine(EnemyTurn(chanceEnemy, typeOfAttackEnemy));
		}
	}

	IEnumerator EnemyTurn(int chance, int typeOfAttack)
	{
		int baseDamage = enemyUnit.damage;
		int damage = baseDamage;
		// int chance;

		float agilityMultiplier = enemyUnit.agilityMultiplier;
		float strengthMultiplier = enemyUnit.strengthMultiplier;

		// int typeOfAttack = Random.Range(1, 4);
		string attackDialog = "";

		dialogueText.text = enemyUnit.unitName + " atakuje!";

		//enemy get closer to player
		targetPos = AttackPlayerPos;
		enemyMove = true;

		enemyUnit.WalkAnim(true);
		while(enemyMove) 
        {
			float step = 2f * Time.deltaTime;
			enemyUnit.transform.position = Vector3.MoveTowards(enemyUnit.transform.position, targetPos, step);
			if (enemyUnit.transform.position == targetPos) {
				enemyMove = false;
				
			}
			yield return null;
        }
		enemyUnit.WalkAnim(false);

		string typeOfAttackStr;
		switch (typeOfAttack)
		{
			case 1:
				typeOfAttackStr = "strong";
				break;
			case 2:
				typeOfAttackStr = "medium";
				break;
			case 3:
				typeOfAttackStr = "weak";
				break;
			default:
				typeOfAttackStr = "weak";
				break;
		}
		enemyUnit.SlashAnimPlayer(true, typeOfAttackStr);

		yield return new WaitForSeconds(1f);
	
		float chanceCalculated = 1f+chance/100f;

		switch (typeOfAttack)
		{
			case 1:
				// chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(baseDamage * chanceCalculated * strengthMultiplier * 1.75f);

				if (chance < 75 / agilityMultiplier)
				{
					damage = 0;
					attackDialog = enemyUnit.unitName + " nie trafił!";
				}
				else
				{
					attackDialog = enemyUnit.unitName + " trafił za " + damage + "!";
				}

				break;
			case 2:
				// chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(baseDamage * chanceCalculated * strengthMultiplier * 1.4f);

				if (chance < 50 / agilityMultiplier)
				{
					damage = 0;
					attackDialog = enemyUnit.unitName + " nie trafił!";
				}
				else
				{
					attackDialog = enemyUnit.unitName + " trafił za " + damage + "!";
				}

				break;
			case 3:
				// chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(baseDamage * chanceCalculated * strengthMultiplier);

				if (chance < 15 / agilityMultiplier)
				{
					damage = 0;
					attackDialog = enemyUnit.unitName + " nie trafił!";
				}
				else
				{
					attackDialog = enemyUnit.unitName + " trafił za " + damage + "!";
				}

				break;
		}

		Debug.Log(typeOfAttack + " " + chanceCalculated);

		bool isDead = playerUnit.TakeDamage(damage);
		if (damage > 0) playerUnit.TakeDmgAnim(true);

		dialogueText.text = attackDialog;

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);
		enemyUnit.SlashAnimPlayer(false, typeOfAttackStr);
		//enemy back to position
		targetPos = EnemySpawnPos;
		enemyMove = true;

		while(enemyMove) 
        {
			float step = 2f * Time.deltaTime;
			enemyUnit.transform.position = Vector3.MoveTowards(enemyUnit.transform.position, targetPos, step);
			if (enemyUnit.transform.position == targetPos) {
				enemyMove = false;
			}
			yield return null;
        }
		

		yield return new WaitForSeconds(1f);

		if (damage > 0) playerUnit.TakeDmgAnim(false);

		if (isDead)
		{
			state = BattleState.LOST;
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}

	}

	IEnumerator EndBattle() {
		if(state == BattleState.WON)
		{	
			Vector3 cameraPos = Camera.main.transform.position;
			dialogueText.text = "Wygrałeś bitkę!";
			while (true) {
				 //Interpolate Position
				Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, chestEndMarker.position, ref cameraPos, 0.2f);
				//Interpolate Rotation
				Camera.main.transform.rotation =  Quaternion.Slerp(Camera.main.transform.rotation, chestEndMarker.rotation, 3f *  Time.deltaTime);
				if (Camera.main.transform.rotation == chestEndMarker.rotation) break;
				yield return null;
			}
			chest.canOpen = true;
			// Camera.main.transform.position = 

			while (!chest.IsOpen()) {
				yield return null;
			}
			yield return new WaitForSeconds(9999f);
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "Sroga porażka.";
		}
		yield return new WaitForSeconds(3);
		EventControl.instance.TransferBack();
		GameControl2.instance.ToggleKostka(true);
		yield return null;
	}

	void PlayerTurn()
	{
		if (eventPlayer != null && eventPlayer.actorNo == GameControl2.instance.MyPlayer.GetComponent<PlayerStats>().actorNo) {
			//only event player ------------
			combatHUD.SetActive(true);
		}
		dialogueText.text = "Co robimy szefunciu?";
	}

	public IEnumerator PlayerHeal(int chanceEnemy, int typeOfAttackEnemy)
	{
		playerUnit.HealAnim(true);
		actionInProgress = true;
		combatHUD.SetActive(false);
		yield return new WaitForSeconds(1f);

		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(2f);
		dialogueText.text = "Czujesz się jak młody bóg!";
		playerUnit.HealAnim(false);

		state = BattleState.ENEMYTURN;
		actionInProgress = false;

		StartCoroutine(EnemyTurn(chanceEnemy, typeOfAttackEnemy));
	}

	// [PunRPC]
    // public void RPC_OnAttack(string attackType) {
	// 	print("Attack " + attackType);
    //     StartCoroutine(PlayerAttack(attackType));
    // }

	// [PunRPC]
    // public void RPC_Heal() {
	// 	print("Heal");
	// 	StartCoroutine(PlayerHeal());
    // }

	public void OnAttackStrongButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;
		
		// StartCoroutine(PlayerAttack("strong"));
		eventPlayer.OnAttack("strong");
	}

	public void OnAttackMediumButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		// StartCoroutine(PlayerAttack("medium"));
		eventPlayer.OnAttack("medium");
	}

	public void OnAttackWeakButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		// StartCoroutine(PlayerAttack("weak"));
		eventPlayer.OnAttack("weak");
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		// StartCoroutine(PlayerHeal());
		eventPlayer.OnHeal();
	}

}
