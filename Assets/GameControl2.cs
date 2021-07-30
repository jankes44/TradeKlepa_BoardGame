using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameControl2 : MonoBehaviourPun
{
    public float startPosX;
    public float startPosY;
    public float startPosZ;
    public float dPosX;
    public float dPosY;
    public float dPosZ;

    public int whosTurn;
    public int playerCount;

    public GameObject playerPrefab;

    void Start()
    {
        Vector3 startPos = new Vector3(startPosX, startPosY, startPosZ);
        Vector3 dicePos = new Vector3(dPosX, dPosY, dPosZ);

        PhotonNetwork.Instantiate("PhotonPrefabs/Player", startPos, Quaternion.Euler(90, 0, 0));

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("PhotonPrefabs/Dice", dicePos, Quaternion.identity);
        }
        Debug.Log("Game started with " + PhotonNetwork.PlayerList.Length + " players");
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].TagObject);
        }

        playerCount = PhotonNetwork.PlayerList.Length;
        whosTurn = 0;

        Debug.Log("First to go: " + PhotonNetwork.PlayerList[whosTurn].NickName);
    }

    public void ChangeTurn()
    {
        PlayerStats[] instances = FindObjectsOfType<PlayerStats>();

        for (int i = 0; i < instances.Length; i++)
        {
            instances[i].SyncTurn(whosTurn);
        }

        Debug.Log(playerCount - 1);
        Debug.Log(whosTurn);
        Debug.Log("Next in turn: " + PhotonNetwork.PlayerList[whosTurn].NickName);
    }

    void Update()
    {

    }
}
