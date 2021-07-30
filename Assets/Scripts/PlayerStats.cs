using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerStats : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject PlayerNameText;
    private PhotonView PV;
    private GameControl2 gameControl;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl2>();
    }

    void Update()
    {
        string nickName = GetComponent<PhotonView>().Owner.NickName;
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = nickName;
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

    [PunRPC]
    public void SyncTurnRPC(int whosTurn)
    {
        if (whosTurn == gameControl.playerCount-1)
        {
            whosTurn = 0;
        } else
        {
            whosTurn++;
        }
        gameControl.whosTurn = whosTurn;
        Debug.Log("Synchronised whosTurn " + whosTurn);
    }
}
