using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DiceMP : MonoBehaviourPun, IPunOwnershipCallbacks
{
	static Rigidbody rb;
	private bool coroutineAllowed = true;
	public PhotonView photonVw;
	private GameControl2 gameControl;
	// Start is called before the first frame update
	void Start()
    {
		rb = GetComponent<Rigidbody>();
		photonVw = GetComponent<PhotonView>();
		gameControl = GameObject.Find("GameControl").GetComponent<GameControl2>();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public int FindPlayerByActorNo(int actorNo)
	{
		int result = 0;

		for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
			Player ps = PhotonNetwork.PlayerList[i];
			if (ps.ActorNumber == actorNo)
			{
				result = i;
				break;
			}
		}
		return result;

	}

	private void OnMouseDown()
	{
		if (photonVw.IsMine && coroutineAllowed)
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

		int number = Random.Range(1, 6);
		//int nextPlayerActorNo = gameControl.DiceTrigger(number);
		////PhotonNetwork.PhotonViews[gameControl.whosTurn].gameObject.GetComponent<PlayerStats>().DiceThrown(number);
		//int nextPlayerIndex = gameControl.FindPlayerByActorNo(nextPlayerActorNo);
		//for (int i = 0; i < gameControl.players.Length; i++)
		//{
		//	gameControl.players[i].GetComponent<PlayerStats>().SyncTurn(gameControl.players[nextPlayerIndex].GetComponent<PlayerStats>().myActorNumber);
		//}
		Player nextPlayer = PhotonNetwork.PlayerList[FindPlayerByActorNo(gameControl.whosTurn)];
		photonVw.TransferOwnership(nextPlayer);
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

