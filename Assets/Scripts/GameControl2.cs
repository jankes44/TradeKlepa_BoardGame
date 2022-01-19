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

    #region Singleton

    private static GameControl2 _instance;

    public static GameControl2 instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
             DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

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
    public PlayerStats currentPlayer;
    public GameObject freelook;
    public EventControl eventControl;
    public Equipment[] ItemList;
    public GameObject cameraObj;

    public Light mainLight;
    public Material day;
    public Material night;

    public BattleSystem battleSystem;

    public int playerCount;

    void Start()
    {
        Vector3 dicePos = new Vector3(dPosX, dPosY, dPosZ);
        Vector3 startPos = new Vector3(startPosX, startPosY, startPosZ);
        
        //RenderSettings.skybox = material this is how you change a skybox

        PV = GetComponent<PhotonView>();
        MyPlayer = PhotonNetwork.Instantiate("PhotonPrefabs/Player", startPos, Quaternion.identity);
        PhotonNetwork.AutomaticallySyncScene = false; //TODO, SYNC SCENE MANUALLY -------------------------------
        

        Debug.Log("Game started with " + PhotonNetwork.PlayerList.Length + " players");
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].TagObject);
        }
        playerCount = PhotonNetwork.PlayerList.Length;
        freelook = GameObject.FindGameObjectWithTag("cmfreelook");
        freelook.GetComponent<CinemachineFreeLook>().Follow = MyPlayer.GetComponent<PlayerStats>().follow;
        freelook.GetComponent<CinemachineFreeLook>().LookAt = MyPlayer.GetComponent<PlayerStats>().lookat;
        DayAndNight("day");
    }

    public void ChangeTurn()
    {
        currentPlayer = playersList[turnIndex].GetComponent<PlayerStats>();

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
        currentPlayer.RollTheDice();
    }

    public void StopDice()
    {
        currentPlayer.StopDice();
    }

    public void ToggleSkipTurnBtn(bool toggle)
    {
        skipTurnBtn.gameObject.SetActive(toggle);
    } 

    public void ToggleRollDiceBtn(bool toggle)
    {
        rollDiceBtn.gameObject.SetActive(toggle);
    }

    public void ToggleKostka(bool toggle)
    {
        imageRenderer.gameObject.SetActive(toggle);
    }

    public void DayAndNight(string dayornight)
    {
        Color dayColor = new Color(1f, 1f, 1f, 1f);
        Color nightColor = new Color(0f, 0f, 0f, 1f);

        RenderSettings.skybox = dayornight == "day" ? day : night;
        RenderSettings.ambientLight = dayornight == "day" ? dayColor : nightColor;
        RenderSettings.ambientIntensity = 0f;

        mainLight.intensity = dayornight == "day" ? 1f : 0.1f;
    }

    void Update()
    {
        playerCount = PhotonNetwork.PlayerList.Length;

    }
}
