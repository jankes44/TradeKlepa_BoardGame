using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameControl2 : MonoBehaviour
{
    public float startPosX;
    public float startPosY;
    public float startPosZ;

    public GameObject playerPrefab;

    void Start()
    {
        Vector3 startPos = new Vector3(startPosX, startPosY, startPosZ);
        PhotonNetwork.Instantiate("PhotonPrefabs/"+playerPrefab.name, startPos, Quaternion.Euler(90, 0, 0));
    }

    void Update()
    {
        
    }
}
