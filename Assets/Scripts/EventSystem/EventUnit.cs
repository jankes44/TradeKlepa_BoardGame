using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EventUnit : MonoBehaviour
{
    public bool hasEvent = false;
    public string eventType;
    GameObject eventObject;
    GameControl2 gameControl;
    BoxCollider boxCollider;
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
        if (hasEvent && eventObject != null)
        {
            eventObject.transform.LookAt(gameControl.MyPlayer.transform);
        }
    }

    public void AddEvent(string type, GameObject eventGO)
    {
        hasEvent = true;
        eventType = type;

        Vector3 pos = transform.position;
        eventObject = Instantiate(eventGO, pos, Quaternion.identity);
    }
    public void RemoveEvent()
    {
        hasEvent = false;
        eventType = "";
        if (eventObject != null)
        {
            Destroy(eventObject);
            eventObject = null;
        }
    }

}
