using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    public GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameObject GameControl = GameObject.Find("GameControl");
        GameControl controlScript = GameControl.GetComponent<GameControl>();

       // players = controlScript.players;

        DisplayPlayerList();

    }

    IEnumerator DisplayPlayerList()
    {

        yield return new WaitUntil(() => players.Length > 0 == true);
        Debug.Log(players.Length);
        foreach (GameObject player in players)
        {
            Debug.Log(player);
        }
    }
}
