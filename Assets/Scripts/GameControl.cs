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

    public static Object[] playersGo;
    public Player[] players;

    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;
    public GameObject prefab;
    public static bool gameOver = false;
    public Transform[] Waypoints;

// Use this for initialization
void Start()
    {

        whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");

        player1 = GameObject.Find("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player");

        playersGo = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        Waypoints = new Transform[_waypoints.Length];
        for (int i = 0; i < Waypoints.Length; ++i)
        {
            Waypoints[i] = _waypoints[i].transform;
        }

        for (int i = 0; i < 2; i++)
        {
          //  Player players = new Player(playersGo[i].name, false, counter);
            Instantiate(prefab, new Vector3(-4, 0.72f, 4.257f), this.transform.rotation);
        }

        //foreach (GameObject player in playersGo)
        //{
          //  Player players = new Player(player.name, false, counter);
            //Instantiate(prefab, new Vector3(-4, 0.72f, 4.257f), Quaternion.identity);
            //counter++;
        //}

     

        player1.GetComponent<FollowThePath>().moveAllowed = false;
        player2.GetComponent<FollowThePath>().moveAllowed = false;

        whoWinsTextShadow.gameObject.SetActive(false);
        //player1MoveText.gameObject.SetActive(true);
        //player2MoveText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player1.GetComponent<FollowThePath>().waypointIndex >
            player1StartWaypoint + diceSideThrown)
        {
            player1.GetComponent<FollowThePath>().moveAllowed = false;
            //player1MoveText.gameObject.SetActive(false);
            //player2MoveText.gameObject.SetActive(true);
            player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
        }

        if (player2.GetComponent<FollowThePath>().waypointIndex >
            player2StartWaypoint + diceSideThrown)
        {
            player2.GetComponent<FollowThePath>().moveAllowed = false;
            //player2MoveText.gameObject.SetActive(false);
            //player1MoveText.gameObject.SetActive(true);
            player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
        }

        if (player1.GetComponent<FollowThePath>().waypointIndex ==
            player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            //player1MoveText.gameObject.SetActive(false);
            //player2MoveText.gameObject.SetActive(false);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gameOver = true;
        }

        if (player2.GetComponent<FollowThePath>().waypointIndex ==
            player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            //player1MoveText.gameObject.SetActive(false);
            //player2MoveText.gameObject.SetActive(false);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
            gameOver = true;
        }
    }

    public static void MovePlayer(int playerToMove)
    {
        switch (playerToMove)
        {
            case 1:
                player1.GetComponent<FollowThePath>().moveAllowed = true;
                break;

            case 2:
                player2.GetComponent<FollowThePath>().moveAllowed = true;
                break;
        }
    }
}
