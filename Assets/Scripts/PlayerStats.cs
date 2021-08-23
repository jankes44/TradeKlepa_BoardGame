using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class PlayerStats : MonoBehaviourPun
{
    public GameObject PlayerNameText;
    private PhotonView PV;
    private GameControl2 gameControl;
    private PunTurnManager turnManager;
    public GameObject[] waypoints;
    private float moveSpeed = 5f;
    private bool moveAllowed = false;

    public int currentWaypoint = 0;
    public int waypointIndex = 0;
    public int thrownAmount = 0;
    public int goalWaypointIndex = 0;
    public int myActorNumber;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        turnManager = gameObject.GetComponent<PunTurnManager>();
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl2>();
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(go => go.GetComponent<waypoint>().waypointID).ToArray();
        SyncMyActorNumber();
        turnManager.BeginTurn();
    }

   

    void Update()
    {
        string nickName = GetComponent<PhotonView>().Owner.NickName;
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = nickName;
        if (moveAllowed)
        {
            Move();
        }
        Debug.Log(turnManager.Turn);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
            info.Sender.TagObject = gameObject;

    }

    public void SyncTurn(int whosTurn)
    {
        if (PV.IsMine)
        {
            PV.RPC("SyncTurnRPC", RpcTarget.AllBuffered, whosTurn);
        }
    }

    public void SyncCurrentWp(int waypoint)
    {
        if (PV.IsMine)
        {
            PV.RPC("SyncCurrentWpRPC", RpcTarget.AllBuffered, waypoint);
        }
    }

    public void SyncMyActorNumber()
    {
        if (PV.IsMine)
        {
            PV.RPC("SyncMyActorNumberRPC", RpcTarget.AllBuffered, PV.OwnerActorNr);
        }
    }

    [PunRPC]
    public void SyncTurnRPC(int whosTurn)
    {
        gameControl.whosTurn = whosTurn;
        Debug.Log("Synchronised whosTurn " + whosTurn);
    }

    [PunRPC]
    public void SyncCurrentWpRPC(int waypoint)
    {
        waypointIndex = waypoint;    
        Debug.Log("Synchronised waypoint " + waypoint);
    }

    [PunRPC]
    public void SyncMyActorNumberRPC(int actorNumber)
    {
        myActorNumber = actorNumber;
        Debug.Log("Synchronised actor no " + actorNumber);
    }

    public void DiceThrown(int amount)
    {
        thrownAmount = amount;
        moveAllowed = true;
        goalWaypointIndex = waypointIndex + thrownAmount;
        Debug.Log(amount);
    }

    public void EndTurn()
    {
        Debug.Log("Skip");
    }

    public void Move()
    {
        Debug.Log(waypointIndex + " " + goalWaypointIndex);
        if (waypointIndex < goalWaypointIndex)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex+1].transform.position, moveSpeed * Time.deltaTime);

            if (transform.position == waypoints[waypointIndex+1].transform.position)
            {
                Debug.Log(transform.position + " " + waypoints[waypointIndex].transform.position);
                waypointIndex += 1;
                SyncCurrentWp(waypointIndex);
            }
        }

    }
}
