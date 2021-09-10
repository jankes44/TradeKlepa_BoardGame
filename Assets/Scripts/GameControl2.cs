using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class GameControl2 : MonoBehaviourPun
{
    private PhotonView PV;
    public float startPosX;
    public float startPosY;
    public float startPosZ;
    public float dPosX;
    public float dPosY;
    public float dPosZ;
    public GameObject[] playersList;
    public string turn;
    public int turnIndex;
    public TMP_Text CurrentPlayerTxt;
    public Button skipTurnBtn;
    public Button rollDiceBtn;
    public Sprite newSprite;
    public Image imageRenderer;
    public Sprite[] kostkas;
    public GameObject MyPlayer;
    public GameObject freelook;
    public EventControl eventControl;

    public int playerCount;

    void Start()
    {
        Vector3 dicePos = new Vector3(dPosX, dPosY, dPosZ);
        Vector3 startPos = new Vector3(startPosX, startPosY, startPosZ);

        PV = GetComponent<PhotonView>();
        MyPlayer = PhotonNetwork.Instantiate("PhotonPrefabs/Player", startPos, Quaternion.identity);

        Debug.Log("Game started with " + PhotonNetwork.PlayerList.Length + " players");
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].TagObject);
        }
        playerCount = PhotonNetwork.PlayerList.Length;
        freelook = GameObject.FindGameObjectWithTag("cmfreelook");
        freelook.GetComponent<CinemachineFreeLook>().Follow = MyPlayer.GetComponent<PlayerStats>().follow;
        freelook.GetComponent<CinemachineFreeLook>().LookAt = MyPlayer.GetComponent<PlayerStats>().lookat;
    }

    public void ChangeTurn()
    {
        PlayerStats currentPlayer = playersList[turnIndex].GetComponent<PlayerStats>();

        //end turn here
        ToggleSkipTurnBtn(false);
        ToggleRollDiceBtn(false);

        currentPlayer.moveAllowed = false;
        currentPlayer.YourTurnStarted = false;

        //begin new players turn \/
        currentPlayer.SyncTurnMaster(turnIndex);

    }
    
    public void RollTheDice()
    {
        rollDiceBtn.gameObject.SetActive(false);
        PlayerStats currentPlayer = playersList[turnIndex].GetComponent<PlayerStats>();
        currentPlayer.RollTheDice();
    }

    public void ToggleSkipTurnBtn(bool toggle)
    {
        skipTurnBtn.gameObject.SetActive(toggle);
    } 

    public void ToggleRollDiceBtn(bool toggle)
    {
        rollDiceBtn.gameObject.SetActive(toggle);
    }

    void Update()
    {
        playerCount = PhotonNetwork.PlayerList.Length;

    }
}
