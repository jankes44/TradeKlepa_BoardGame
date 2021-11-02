using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public GameObject NicknameDialogObj;

    private void OnEnable() {
        if (menuName == "title") {
            NicknameDialogObj.SetActive(true);
        }    
    }

    public void Open()
    {
        gameObject.SetActive(true);
        open = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        open = false;
    }
}
