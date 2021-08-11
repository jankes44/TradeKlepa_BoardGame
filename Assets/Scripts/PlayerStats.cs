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
    }

    void Update()
    {
        string nickName = GetComponent<PhotonView>().Owner.NickName;
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = nickName;
        playerName = nickName;
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
