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
    public bool isMoving = false;

    Animator animator;
    Transform gosciu;

    int isWalkingHash;
    public bool isLocal;
    public Transform follow;
    public Transform lookat;
    public bool YourTurnStarted = false;

    bool collided = false;

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

    //public void Move() //obsolete
    //{
    //    if (targetWaypointIndex > waypointIndex)
    //    {
    //        animator.SetBool(isWalkingHash, true);

    //        transform.position = Vector3.MoveTowards(transform.position, gameControl.waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);

    //        if (transform.position == gameControl.waypoints[waypointIndex].transform.position)
    //        {
    //            waypointIndex += 1;
    //        }
    //    } else
    //    {
    //        animator.SetBool(isWalkingHash, false);
    //        Debug.Log("allowed? False");
    //        moveAllowed = false;
    //    }
    //}



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
        gameControl.CurrentPlayerTxt.text = nextPlayerName+"'s turn";
        gameControl.freelook.GetComponent<CinemachineFreeLook>().Follow = gameControl.playersList[whosTurn].GetComponent<PlayerStats>().follow;
        gameControl.freelook.GetComponent<CinemachineFreeLook>().LookAt = gameControl.playersList[whosTurn].GetComponent<PlayerStats>().lookat;
        Debug.Log("Synchronised whosTurn " + nextPlayerName);
    }

    public void animationToggle(bool toggle)
    {
        animator.SetBool(isWalkingHash, toggle);
    }

    public void RollTheDice()
    {
        int rand = Random.Range(1, 7); //temporary before dice rolling is ready <-----TODO----->
        PV.RPC("RPC_RollTheDice", RpcTarget.AllBuffered, rand);
    }

    [PunRPC]
    public void RPC_RollTheDice(int rolled)
    {
        StartCoroutine(Roll(rolled));
        Debug.Log("RPC received, rolled: " + rolled);
    }

    IEnumerator Roll(int rolled)
    {
        int rand;

        for (int i = 0; i < 15; i++)
        {
            rand = Random.Range(1, 7);
            gameControl.imageRenderer.sprite = gameControl.kostkas[rand-1];
            yield return new WaitForSeconds(0.1f);
        }

        gameControl.imageRenderer.sprite = gameControl.kostkas[rolled-1];

        gameObject.GetComponent<GraphwayTest>().steps = rolled;
        Debug.Log(rolled);

        yield return null;
    }

    private IEnumerator YourTurn()
    {
        Debug.Log("Your turn started!");
        gameControl.ToggleSkipTurnBtn(true);
        gameControl.ToggleRollDiceBtn(true);
        yield return null;
    }

    private IEnumerator FirstTurn()
    {
        yield return new WaitForSeconds(2);
        SyncTurnMaster(0);
        yield return null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (collided) return;
        if (other.gameObject.GetComponent<EventUnit>() && other.gameObject.GetComponent<EventUnit>().hasEvent == true && !isMoving)
        {
            collided = true;
            Debug.Log("Event " + other.gameObject.GetComponent<EventUnit>().eventType + " start");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collided = false;
    }
}
