using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    public GameObject PlayerNameText;

    void Start()
    {
        
    }

    void Update()
    {
        PlayerNameText.transform.LookAt(Camera.main.transform.position);
        PlayerNameText.transform.Rotate(0, 180, 0);
        PlayerNameText.GetComponent<TextMesh>().text = gameObject.name;
        if (gameObject.name == "Player2")
            PlayerNameText.GetComponent<TextMesh>().color = Color.blue;
        else PlayerNameText.GetComponent<TextMesh>().color = Color.green;
    }
}
