using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EventUnit : MonoBehaviour
{
    public bool hasEvent = false;
    public string eventID;
    public string eventType;
    public Sprite eventIMG;
    public int spriteIndex;
    GameObject eventObject;
    GameControl2 gameControl;
    BoxCollider boxCollider;
    public bool playerOnField = false;
    Rigidbody rb;

    private void Start()
    {
        gameControl = FindObjectOfType<GameControl2>();
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        gameObject.layer = LayerMask.NameToLayer("hexes");
    }

    private void Update()
    {
        if (gameObject != null && hasEvent && eventObject != null)
        {
            eventObject.transform.LookAt(gameControl.MyPlayer.transform);
        }
    }

    public void AddEvent(string uid, string type, GameObject eventGO, Sprite sprite, int index)
    {
        if (!playerOnField) {
            hasEvent = true;
            eventID = uid;
            eventType = type;
            eventIMG = sprite;
            spriteIndex = index;

            Vector3 pos = transform.position;
            eventObject = Instantiate(eventGO, pos, Quaternion.identity);
        }
        
    }
    public void RemoveEvent()
    {
        hasEvent = false;
        eventType = "";
        eventID = null;
        eventIMG = null;
        spriteIndex = 0;

        if (eventObject != null)
        {
            Destroy(eventObject);
            eventObject = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Debug.Log(other.tag + " enter");
            playerOnField = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            Debug.Log(other.tag + " exit");
            playerOnField = false;
        }
    }
}
