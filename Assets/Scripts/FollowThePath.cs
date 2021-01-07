using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{
    public Transform[] waypoints;

    [SerializeField]
    private float moveSpeed = 1f;

    [HideInInspector]
    public int waypointIndex = 0;

    public bool moveAllowed = false;
    public string PlayerName;
    public int PlayerID;
    public int playerStartWaypoint = 0;

    // Use this for initialization
    private void Start()
    {
        GameObject GameCont = GameObject.Find("GameControl");
        GameControl gameContScript = GameCont.GetComponent<GameControl>();

        waypoints = gameContScript.Waypoints;
        PlayerName = this.gameObject.name;
        PlayerID = Random.Range(1,1000000);

        Debug.Log(PlayerName);
        Debug.Log(PlayerID);
        
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (moveAllowed)
            Move();
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Length - 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);

            if (transform.position == waypoints[waypointIndex].transform.position)
            {
                waypointIndex += 1;
            }
        }
    }
}