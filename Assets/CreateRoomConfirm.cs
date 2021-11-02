using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomConfirm : MonoBehaviour
{
    void Update() {
         if (Input.GetKeyUp(KeyCode.Return)) { 
			Launcher.Instance.CreateRoom();
		}
    }
}
