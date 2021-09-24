using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EventUnit : MonoBehaviour
{
    public bool hasEvent = false;
    public string eventName;
    public string eventID;
    public string eventType;
    public int eventIndex;
    GameObject eventGO;
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
        if (gameObject != null && hasEvent && eventGO != null)
        {
            eventGO.transform.LookAt(gameControl.MyPlayer.transform);
        }
    }

    public void AddEvent(string uid, EventObject eventObj, GameObject eventGOPassed, int index)
    {
        if (!playerOnField && !hasEvent) {
            hasEvent = true;
            eventID = uid;
            eventName = eventObj.name;
            eventType = eventObj.type;
            eventIndex = index;

            Vector3 pos = transform.position;
            eventGO = Instantiate(eventGOPassed, pos, Quaternion.identity);
        }
        
    }
    public void RemoveEvent()
    {
        hasEvent = false;
        eventName = "";
        eventType = "";
        eventID = null;
        eventIndex = 0;

        if (eventGO)
        {
            Destroy(eventGO);
            eventGO = null;
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
