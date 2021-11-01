using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class SetUsername : MonoBehaviour
{
	public Button yourButton;
	public TMP_InputField usernameTxtField;
	public GameObject usernameDialog;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		string userNameInputText = usernameTxtField.text;
		PhotonNetwork.NickName = userNameInputText;
		gameObject.SetActive(false);
		usernameDialog.SetActive(false);
	}

	void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return)) { 
			TaskOnClick();
		}
    }
}
