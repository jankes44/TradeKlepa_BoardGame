using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Linq;
using Cinemachine;

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
    public bool isLocal;
    Camera camera;
    public GameObject freelook;
    public Transform follow;
    public Transform lookat;
    public bool YourTurnStarted = false;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl2>();
        actorNo = PV.Owner.ActorNumber;
        gameControl.playersList = GameObject.FindGameObjectsWithTag("Player").OrderBy(go => go.GetComponent<PlayerStats>().actorNo).ToArray();
        Vector3 startPos = new Vector3(gameControl.startPosX, gameControl.startPosY, gameControl.startPosZ);
        playerName = GetComponent<PhotonView>().Owner.NickName;
        isLocal = PV.IsMine || !PhotonNetwork.IsConnected;
        gosciu = transform.Find("gosciuu");
        animator = gosciu.GetComponent<Animator>();
        Debug.Log(animator);
        isWalkingHash = Animator.StringToHash("isWalking");

        if (PhotonNetwork.IsMasterClient && isLocal)
        {
            Debug.Log("Only master");
            StartCoroutine("FirstTurn");
        }

        if (!isLocal)
        {
        } else
        {
            freelook = GameObject.FindGameObjectWithTag("cmfreelook");
            freelook.GetComponent<CinemachineFreeLook>().Follow = follow;
            freelook.GetComponent<CinemachineFreeLook>().LookAt = lookat;
        }
    }

    void Update()
    {
        string nickName = GetComponent<PhotonView>().Owner.NickName;
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = nickName;
        playerName = nickName;
        if (moveAllowed && PV.IsMine && !YourTurnStarted)
        {
            YourTurnStarted = true;
            Debug.Log("if !yourturnstarted");
            StartCoroutine("YourTurn");
        }
    }
    
    public void Move() //obsolete
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



    public void SyncTurnMaster(int current)
    {
        if (PV.IsMine)
        {
            if (current == gameControl.playerCount - 1)
            {
                current = 0;
            }
            else
            {
                current++;
            }
            string nextPlayerName = gameControl.playersList[current].GetComponent<PlayerStats>().playerName;
            Debug.Log(nextPlayerName + " " + current + " " + gameControl.playerCount);
            SyncTurn(current, nextPlayerName);
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
        gameControl.playersList[whosTurn].GetComponent<PlayerStats>().moveAllowed = true;
        gameControl.CurrentPlayerTxt.text = nextPlayerName;
        Debug.Log("Synchronised whosTurn " + nextPlayerName);
    }

    public void animationToggle(bool toggle)
    {
        animator.SetBool(isWalkingHash, toggle);
    }

    private IEnumerator YourTurn()
    {
        Debug.Log("Your turn started!");
        gameControl.ToggleSkipTurnBtn(true);
        yield return null;
    }

    private IEnumerator FirstTurn()
    {
        yield return new WaitForSeconds(2);
        SyncTurnMaster(0);
        yield return null;
    }
}
