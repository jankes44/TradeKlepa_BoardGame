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
	public Animator playerAnimator;

	public int maxHP;
	public int currentHP;

    public void Start()
    {
		agilityMultiplier = agility / 100f + 1f;
		strengthMultiplier = strength / 50f + 1f;
		vitalityMultiplier = vitality / 100f + 1f;
		playerAnimator = gameObject.GetComponent<Animator>();
		Debug.Log(strengthMultiplier);
	}

	public void Anim(bool set)
    {
		Debug.Log(set);
		int hash = Animator.StringToHash("walk");
		playerAnimator.SetBool(hash, set);
	}

	public void HealAnim(bool set)
    {
		int hash = Animator.StringToHash("heal");
		playerAnimator.SetBool(hash, set);
    }

	public void SlashAnim(bool set)
	{
		int hash = Animator.StringToHash("slash");
		playerAnimator.SetBool(hash, set);
	}

	public void PunchAnim(bool set)
	{
		int hash = Animator.StringToHash("punch");
		playerAnimator.SetBool(hash, set);
	}

	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}
