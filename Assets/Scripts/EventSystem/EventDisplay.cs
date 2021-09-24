using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventDisplay : MonoBehaviour {

	public EventObject eventObject;

	public Text nameText;
	public TMP_Text descriptionText;

	public Image artworkImage;

	// Use this for initialization
	void Start () {
		// nameText.text = card.name;
		// descriptionText.text = card.description;

		// artworkImage.sprite = card.artwork;

		// manaText.text = card.manaCost.ToString();
		// attackText.text = card.attack.ToString();
		// healthText.text = card.health.ToString();
        nameText.text = eventObject.name;
        descriptionText.text = eventObject.description;
        artworkImage.sprite = eventObject.artwork;

	}

	public void UpdateDisplay() {
		nameText.text = eventObject.name;
        descriptionText.text = eventObject.description;
        artworkImage.sprite = eventObject.artwork;
	}
	
}