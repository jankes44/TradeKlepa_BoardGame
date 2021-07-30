using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DiceMP : MonoBehaviourPun, IPunOwnershipCallbacks
{
	static Rigidbody rb;
	private bool coroutineAllowed = true;
	private PhotonView photonView;
	private GameControl2 gameControl;
	// Start is called before the first frame update
	void Start()
    {
		rb = GetComponent<Rigidbody>();
		photonView = GetComponent<PhotonView>();
		gameControl = GameObject.Find("GameControl").GetComponent<GameControl2>();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	private void OnMouseDown()
	{
		if (photonView.IsMine && coroutineAllowed)
			StartCoroutine("RollTheDice");
		//GameObject GameController = GameObject.Find("GameControl");
		//GameControl controlScript = GameController.GetComponent<GameControl>();

		//players = controlScript.players;
	}

	private IEnumerator RollTheDice()
	{
		coroutineAllowed = false;

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
		yield return new WaitForSeconds(4);
		//coroutineAllowed = false;
		//int randomDiceSide = 0;
		//for (int i = 0; i <= 20; i++)
		//{
		//rend.sprite = diceSides[randomDiceSide];
		//yield return new WaitForSeconds(0.05f);
		//}
		gameControl.ChangeTurn();
		Player nextPlayer = PhotonNetwork.PlayerList[gameControl.whosTurn];
		photonView.TransferOwnership(nextPlayer);
		coroutineAllowed = true;
		yield return new WaitUntil(() => true);

		
	}

	public void OnTransferOwnership(object[] viewAndPlayer)
    {
		PhotonView view = viewAndPlayer[0] as PhotonView;
    }

	public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {

		//PLACE FOR CHECKS, ETC.

		base.photonView.TransferOwnership(requestingPlayer);
    }

	public void OnOwnershipTransfered (PhotonView targetView, Player previousOwner)
    {

    }
}

