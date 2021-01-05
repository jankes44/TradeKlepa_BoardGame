﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour {

	static Rigidbody rb;
	public static Vector3 diceVelocity;
	private int whosTurn = 1;
	private bool coroutineAllowed = true;
	private GameObject[] players;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	private void OnMouseDown()
	{
		if (!GameControl.gameOver && coroutineAllowed)
			StartCoroutine("RollTheDice");
		GameObject GameController = GameObject.Find("GameControl");
		GameControl controlScript = GameController.GetComponent<GameControl>();

		players = controlScript.players;

	}

	private IEnumerator RollTheDice()
	{
		coroutineAllowed = false;

		Debug.Log("Whosturn? " + whosTurn);

		float forwardForce = Random.Range(350, 500);

		Debug.Log(forwardForce);
		DiceNumberTextScript.diceNumber = 0;
		float dirX = Random.Range(0, 1000);
		float dirY = Random.Range(0, 1000);
		float dirZ = Random.Range(0, 1000);
		transform.rotation = Quaternion.identity;
		transform.position = new Vector3(2.5f, 2f, -5);

		rb.AddForce(transform.up * 10);
		rb.AddForce(transform.forward * forwardForce);
		rb.AddForce(transform.right * -150);
		rb.AddTorque(dirX + 500, dirY, dirZ);

		//coroutineAllowed = false;
		//int randomDiceSide = 0;
		//for (int i = 0; i <= 20; i++)
		//{
		//rend.sprite = diceSides[randomDiceSide];
		//yield return new WaitForSeconds(0.05f);
		//}
		yield return new WaitUntil(() => DiceNumberTextScript.diceNumber != 0);

		GameControl.diceSideThrown = DiceNumberTextScript.diceNumber;
		GameControl.MovePlayer(whosTurn);

		if (whosTurn == 1)
			{
				GameControl.MovePlayer(1);
			}
			else if (whosTurn == -1)
			{
				GameControl.MovePlayer(2);
			}
		
		if ()
		
		whosTurn *= -1;
		coroutineAllowed = true;
		DiceNumberTextScript.diceNumber = 0;
	}

    // Update is called once per frame

    private void Update()
    {
		diceVelocity = rb.velocity;
	}

}
