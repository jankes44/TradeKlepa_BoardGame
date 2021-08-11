using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;

public class GameControl2 : MonoBehaviourPun
{
    public float startPosX;
    public float startPosY;
    public float startPosZ;
    public float dPosX;
    public float dPosY;
    public float dPosZ;
    public GameObject[] playersList;
    public string turn;
    public int turnIndex;
    public Transform[] waypoints;

    public int playerCount;

    void Start()
    {
        Vector3 dicePos = new Vector3(dPosX, dPosY, dPosZ);
        Vector3 startPos = new Vector3(startPosX, startPosY, startPosZ);

        PhotonNetwork.Instantiate("PhotonPrefabs/Player", startPos, Quaternion.Euler(90, 0, 0));

        GameObject[] _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        waypoints = new Transform[_waypoints.Length];
        for (int i = 0; i < waypoints.Length; ++i)
        {
            waypoints[i] = _waypoints[i].transform;
            Debug.Log(waypoints[i].name);
        }
        waypoints = waypoints.OrderBy(a => a.GetComponent<Waypoint>().id).ToArray();

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

    }

    void Update()
    {
        playerCount = PhotonNetwork.PlayerList.Length;

    }
}
