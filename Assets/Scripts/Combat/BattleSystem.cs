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

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	PlayerStats eventPlayer;

	bool actionInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		// GameControl2.instance.ToggleKostka(false);

		//player unit
		GameObject playerGO = Instantiate(playerPrefab);
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
		GameObject enemyGO = Instantiate(enemyPrefab);
		enemyUnit = enemyGO.GetComponent<Unit>();

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
		playerUnit.SlashAnimPlayer(true, typeOfAttack);

		yield return new WaitForSeconds(1f);

		int baseDamage = playerUnit.damage;
		int damage = baseDamage;

		float chanceMultiplier = playerUnit.agilityMultiplier;
		float strengthMultiplier = playerUnit.strengthMultiplier;
		string attackDialog = "";

		float chanceCalculated = 1f+chance/100f;

		switch (typeOfAttack)
        {
			case "strong":
				// chance = Random.Range(1, range);

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
				// chance = Random.Range(1, range);
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
				// chance = Random.Range(1, range);
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
		
		yield return new WaitForSeconds(2f);
		
		if (damage > 0) enemyUnit.TakeDmgAnim(false);
		
		playerUnit.SlashAnimPlayer(false, typeOfAttack);

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
		Debug.Log(typeOfAttackStr);
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

		if (damage > 0) playerUnit.TakeDmgAnim(false);

		enemyUnit.SlashAnimPlayer(false, typeOfAttackStr);

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
			dialogueText.text = "Wygrałeś bitkę!";
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
