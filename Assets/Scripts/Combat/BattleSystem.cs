using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	bool actionInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		PlayerStats player = EventControl.instance.eventPlayer;
		Debug.Log(player.playerName);
		GameControl2.instance.ToggleKostka(false);
		GameObject playerGO = Instantiate(playerPrefab);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "Dziki " + enemyUnit.unitName + " wkracza...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack(string typeOfAttack)
	{
		actionInProgress = true;
		combatHUD.SetActive(false);
		playerUnit.SlashAnimPlayer(true, typeOfAttack);

		yield return new WaitForSeconds(1f);

		int damage = playerUnit.damage;
		int range = 100;
		int chance;
		float chanceMultiplier = playerUnit.agilityMultiplier;
		float strengthMultiplier = playerUnit.strengthMultiplier;
		string attackDialog = "";

		switch (typeOfAttack)
        {
			case "strong":
				chance = Random.Range(1, range);

				//multipliers
				damage = Mathf.RoundToInt(damage * Random.Range(2.25f, 3f) * strengthMultiplier);

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
				chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(damage * Random.Range(1.25f, 2f) * strengthMultiplier);

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
				chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(damage * Random.Range(0.75f, 1f) * strengthMultiplier);

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

		yield return new WaitForSeconds(2f);

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
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		int damage = enemyUnit.damage;
		int range = 100;
		int chance;

		float agilityMultiplier = enemyUnit.agilityMultiplier;
		float strengthMultiplier = enemyUnit.strengthMultiplier;

		int typeOfAttack = Random.Range(1, 4);
		string attackDialog = "";

		dialogueText.text = enemyUnit.unitName + " atakuje!";
		enemyUnit.SlashAnim(true);
		yield return new WaitForSeconds(1f);

		switch (typeOfAttack)
		{
			case 1:
				chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(damage * Random.Range(2.25f,3f) * strengthMultiplier);

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
				chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(damage * Random.Range(1.25f, 2f) * strengthMultiplier);

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
				chance = Random.Range(1, range);
				damage = Mathf.RoundToInt(damage * Random.Range(0.75f, 1f) * strengthMultiplier);

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

		Debug.Log(typeOfAttack);

		bool isDead = playerUnit.TakeDamage(damage);
		if (damage > 0) playerUnit.TakeDmgAnim(true);

		dialogueText.text = attackDialog;
		enemyUnit.SlashAnim(false);

		playerHUD.SetHP(playerUnit.currentHP);

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

	// void EndBattle()
	// {
		
	// }

	void PlayerTurn()
	{
		combatHUD.SetActive(true);
		dialogueText.text = "Co robimy szefunciu?";
	}

	IEnumerator PlayerHeal()
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
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackStrongButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		StartCoroutine(PlayerAttack("strong"));
	}

	public void OnAttackMediumButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		StartCoroutine(PlayerAttack("medium"));
	}

	public void OnAttackWeakButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		StartCoroutine(PlayerAttack("weak"));
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		if (actionInProgress)
			return;

		StartCoroutine(PlayerHeal());
	}

}
