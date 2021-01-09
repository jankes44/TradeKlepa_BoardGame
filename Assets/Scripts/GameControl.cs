using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public class Player : MonoBehaviour
    {
        public int PlayerId;
        public string PlayerName;
        public bool PlayersTurn = false;

        public Player(string _PlayerName, bool _PlayersTurn, int _PlayerId)
        {
            PlayerId = _PlayerId;
            PlayerName = _PlayerName;
            PlayersTurn = _PlayersTurn;
        }

        public string GetName()
        {
            return PlayerName;
        }
    }

    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText;

    private static GameObject player1, player2;

    public static GameObject[] playersGo;
    public Player[] players;
    public int numberOfPlayers;
    public static int whosTurn;
    public static int whosTurnIndex;
    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;
    public static List<GameObject> playersList = new List<GameObject>();
    public GameObject prefab;
    public static bool gameOver = false;
    public Transform[] Waypoints;
    bool ready = false;

// Use this for initialization
void Start()
    {

        whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");

        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        GameObject[] _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        Waypoints = new Transform[_waypoints.Length];
        for (int i = 0; i < Waypoints.Length; ++i)
        {
            Waypoints[i] = _waypoints[i].transform;
        }

        numberOfPlayers = 2;
        for (int i = 0; i < numberOfPlayers; i++)
        {
          //  Player players = new Player(playersGo[i].name, false, counter);
            Instantiate(prefab, new Vector3(-4, 0.72f, 4.257f), Quaternion.Euler(90,0,0));
        }

        //foreach (GameObject player in playersGo)
        //{
        //  Player players = new Player(player.name, false, counter);
        //Instantiate(prefab, new Vector3(-4, 0.72f, 4.257f), Quaternion.identity);
        //counter++;
        //}

        StartCoroutine("Initialize");

        //player1.GetComponent<FollowThePath>().moveAllowed = false;
        //player2.GetComponent<FollowThePath>().moveAllowed = false;

        whoWinsTextShadow.gameObject.SetActive(false);
        //player1MoveText.gameObject.SetActive(true);
        //player2MoveText.gameObject.SetActive(false);
    }

    private IEnumerator Initialize()
    {
        playersGo = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playersGo)
        {
            playersList.Add(player);
            int playerID = player.GetComponent<FollowThePath>().PlayerID;
            player.GetComponent<FollowThePath>().moveAllowed = false;
            Debug.Log(playerID);
        }

        yield return new WaitUntil(() => ready == true);

        whosTurn = playersList[0].GetComponent<FollowThePath>().PlayerID;

        Debug.Log("whosTurn " + whosTurn);
    }

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject player in playersList)
        {
            
            if (player.GetComponent<FollowThePath>().PlayerID != 0 && ready == false)
            {
                whosTurn = playersGo[0].GetComponent<FollowThePath>().PlayerID;
                whosTurnIndex = 0;
                ready = true;
            }
            if (player.GetComponent<FollowThePath>().waypointIndex >
           player.GetComponent<FollowThePath>().playerStartWaypoint + diceSideThrown)
            {

                player.GetComponent<FollowThePath>().moveAllowed = false;
                //player1MoveText.gameObject.SetActive(false);
                //player2MoveText.gameObject.SetActive(true);
                player.GetComponent<FollowThePath>().playerStartWaypoint = player.GetComponent<FollowThePath>().waypointIndex - 1;
            }

            if (player.GetComponent<FollowThePath>().waypointIndex ==
            player.GetComponent<FollowThePath>().waypoints.Length)
            {
                whoWinsTextShadow.gameObject.SetActive(true);
                //player1MoveText.gameObject.SetActive(false);
                //player2MoveText.gameObject.SetActive(false);
                whoWinsTextShadow.GetComponent<Text>().text = "Player Wins";
                gameOver = true;
            }
        }

        
    }

    public static void MovePlayer(int playerToMove)
    {
        Debug.Log("Player to move " + playerToMove);
        foreach (GameObject player in playersGo)
        {
            
            if (player.GetComponent<FollowThePath>().PlayerID == playerToMove)
            {
                
                player.GetComponent<FollowThePath>().moveAllowed = true;

                if (whosTurnIndex == playersGo.Length-1)
                {
                    whosTurn = playersList[0].GetComponent<FollowThePath>().PlayerID;
                    whosTurnIndex = 0;
                } else
                {
                    whosTurn = playersList[whosTurnIndex + 1].GetComponent<FollowThePath>().PlayerID;
                    Debug.Log(playersList[whosTurnIndex + 1].GetComponent<FollowThePath>().PlayerID);
                    whosTurnIndex += 1;
                }
                Debug.Log(whosTurnIndex + " " + playersGo.Length);
            }
        }

    }
}
