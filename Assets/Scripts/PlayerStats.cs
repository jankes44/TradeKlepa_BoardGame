using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStats : MonoBehaviour
{
    public GameObject PlayerNameText;

    void Start()
    {
        
    }

    void Update()
    {
        string nickName = GetComponent<PhotonView>().Owner.NickName;
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = nickName;
    }

    void SetUsername()
    {
        Debug.Log("Set username");
    }
}
