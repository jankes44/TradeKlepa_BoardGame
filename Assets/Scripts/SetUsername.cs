using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SetUsername : MonoBehaviour
{
	public Button yourButton;
	public InputField usernameTxtField;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		string userNameInputText = usernameTxtField.text;
		PhotonNetwork.NickName = userNameInputText;
	}
}
