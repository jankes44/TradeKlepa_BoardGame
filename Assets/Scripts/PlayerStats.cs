using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Linq;

public class PlayerStats : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject PlayerNameText;
    private PhotonView PV;
    private GameControl2 gameControl;
    public GameObject PlayerAvatar;
    public string playerName;
    public int actorNo;
    public int waypointIndex = 0;
    public int targetWaypointIndex = 0;
    public float moveSpeed = 2.1f;
    public bool moveAllowed = false;
    Animator animator;
    Transform gosciu;
    int isWalkingHash;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl2>();
        Vector3 startPos = new Vector3(gameControl.startPosX, gameControl.startPosY, gameControl.startPosZ);
        actorNo = PV.Owner.ActorNumber;
        playerName = GetComponent<PhotonView>().Owner.NickName;
        gameControl.playersList = GameObject.FindGameObjectsWithTag("Player").OrderBy(go => go.GetComponent<PlayerStats>().actorNo).ToArray();
        if (PhotonNetwork.IsMasterClient)
        {
            SyncTurnMaster(-1);

        }
        gosciu = transform.Find("gosciuu");
        animator = gosciu.GetComponent<Animator>();
        Debug.Log(animator);
        isWalkingHash = Animator.StringToHash("isWalking");

    }

    void Update()
    {
        string nickName = GetComponent<PhotonView>().Owner.NickName;
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = nickName;
        playerName = nickName;
        if (moveAllowed)
            Move();
    }
    
    public void Move()
    {
        if (targetWaypointIndex > waypointIndex)
        {
            animator.SetBool(isWalkingHash, true);

            transform.position = Vector3.MoveTowards(transform.position, gameControl.waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);
            
            if (transform.position == gameControl.waypoints[waypointIndex].transform.position)
            {
                waypointIndex += 1;
            }
        } else
        {
            animator.SetBool(isWalkingHash, false);
            Debug.Log("allowed? False");
            moveAllowed = false;
        }
    }

    public void SyncTurnMaster(int next)
    {
        if (PV.IsMine)
        {
            if (next == gameControl.playerCount - 1)
            {
                next = 0;
            }
            else
            {
                next++;
            }
            string nextPlayerName = gameControl.playersList[next].GetComponent<PlayerStats>().playerName;
            SyncTurn(next, nextPlayerName);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    public void SyncTurn(int whosTurn, string nextPlayerName)
    {
        PV.RPC("SyncTurnRPC", RpcTarget.AllBuffered, whosTurn, nextPlayerName);
    }

    [PunRPC]
    public void SyncTurnRPC(int whosTurn, string nextPlayerName)
    {        
        gameControl.turn = nextPlayerName;
        gameControl.turnIndex = whosTurn;
        Debug.Log("Synchronised whosTurn " + nextPlayerName);
    }
}
