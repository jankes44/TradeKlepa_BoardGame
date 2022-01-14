using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public string unitName;
	public int unitLevel;

	public int damage;

	public float agility = 10f;
	public float strength = 10f;
	public float vitality = 10f;
	public float agilityMultiplier;
	public float strengthMultiplier;
	public float vitalityMultiplier;
	public Animator unitAnimator;

	public int maxHP;
	public int currentHP;

	void OnValidate() {
		updateStats();
	}

    void Start()
    {
		updateStats();
		unitAnimator = gameObject.GetComponent<Animator>();
	}

	void updateStats() {
		agilityMultiplier = agility / 100f + 1f;
		strengthMultiplier = strength / 50f + 1f;
		vitalityMultiplier = vitality / 100f + 1f;
	}

	public void Anim(bool set)
    {
		Debug.Log(set);
		int hash = Animator.StringToHash("walk");
		unitAnimator.SetBool(hash, set);
	}

	public void HealAnim(bool set)
    {
		int hash = Animator.StringToHash("heal");
		unitAnimator.SetBool(hash, set);
    }

	public void SlashAnimPlayer(bool set, string type)
	{
		int hash = Animator.StringToHash("poke");
		switch (type)
		{
			case "strong":
				hash = Animator.StringToHash("smash");
				break;
			case "medium":
				hash = Animator.StringToHash("slash");
				break;
			case "weak":
				hash = Animator.StringToHash("poke");
				break;
		}
		unitAnimator.SetBool(hash, set);
	}


	public void SlashAnim(bool set)
	{
		int hash = Animator.StringToHash("slash");
		unitAnimator.SetBool(hash, set);
	}

	public void PunchAnim(bool set)
	{
		int hash = Animator.StringToHash("punch");
		unitAnimator.SetBool(hash, set);
	}

	public void TakeDmgAnim(bool set)
    {
		int hash = Animator.StringToHash("takehit");
		unitAnimator.SetBool(hash, set);
    }

	public void BlockAnim(bool set)
    {
		int hash = Animator.StringToHash("block");
		unitAnimator.SetBool(hash, set);
    }

	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			//die animation
			return true;
		else
        {
			return false;
		}
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}
