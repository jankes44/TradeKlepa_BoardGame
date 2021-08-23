using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

public class GameControl2 : GameControl, IOnEventCallback, IPunTurnManagerCallbacks
{
    public float startPosX;
    public float startPosY;
    public float startPosZ;
    public float dPosX;
    public float dPosY;
    public float dPosZ;

    public int previousTurn;
    public int whosTurn;
    public int playerCount;
    public GameObject[] players;
    public PhotonView photonV;

    private Launcher networkManager;
    private PlayerStats localPlayer;
    private PlayerStats activePlayer;

    public void OnTurnBegins(int turn)
    {

    }
    public void OnTurnCompleted(int turn)
    {

    }
    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object obj)
    {

    }
    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object obj)
    {

    }
    public void OnTurnTimeEnds(int turn)
    {

    }

    public void SetNetworkManager(Launcher networkManager)
    {
        this.networkManager = networkManager;
    }

    public void OnEvent(EventData photonEvent)
    {
        
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    void Start()
    {
        Vector3 startPos = new Vector3(startPosX, startPosY, startPosZ);
        Vector3 dicePos = new Vector3(dPosX, dPosY, dPosZ);
        photonV = GetComponent<PhotonView>();
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

        SetActivePlayer(players[0].GetComponent<PlayerStats>());
        //whosTurn = PhotonNetwork.PlayerList[0].ActorNumber;
        
        //Debug.Log("First to go: " + players[FindPlayerByActorNo(whosTurn)].GetComponent<PlayerStats>().PlayerNameText.GetComponent<TextMesh>().text);
    }


    public int ChangeTurn()
    {
        int nextPlayerIndex = FindPlayerByActorNo(whosTurn);
        if (nextPlayerIndex == players.Length - 1)
        {
            nextPlayerIndex = 0;
        }
        else nextPlayerIndex++;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerStats>().SyncTurn(nextPlayerIndex);
        }

        Debug.Log(players.Length);
        Debug.Log(whosTurn);
        Debug.Log("Next in turn: " + PhotonNetwork.PlayerList[nextPlayerIndex].NickName);
        return whosTurn;
    }

    public void GetPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public int DiceTrigger(int number)
    {
        int pIndex = FindPlayerByActorNo(whosTurn);

        Debug.Log(players.Length);

        Debug.Log(whosTurn + " " + players[pIndex].GetComponent<PlayerStats>().PlayerNameText.GetComponent<TextMesh>().text);
        players[pIndex].GetComponent<PlayerStats>().DiceThrown(number);

        return ChangeTurn();
    }

    public int FindPlayerByActorNo(int actorNo)
    {
        int result = 0;

        for (int i = 0; i < players.Length; i++)
        {
            PlayerStats ps = players[i].GetComponent<PlayerStats>();
            if (ps.myActorNumber == actorNo)
            {
                result = i;
                break;
            }
        }
        return result;

    }

    public void SetLocalPlayer(PlayerStats player)
    {
        localPlayer = player;
    }

    public void SetActivePlayer(PlayerStats player)
    {
        activePlayer = player;
    }

    public bool IsLocalPlayersTurn()
    {
        return localPlayer == activePlayer;
    }

    public bool CanPerformMove()
    {
        if (!IsLocalPlayersTurn())
            return false;
        return true;
    }


    void Update()
    {

    }
}
