using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventControl : MonoBehaviour
{
    public EventUnit[] unitList;
    public EventObject[] eventList;
    public bool addEvent = false;
    public GameObject eventGO;
    public int lastEventIndex;
    public GameObject eventHolder;
    public GameObject eventHolderButton;
    public GameObject eventCloseButton;
    GameControl2 GC;

    private void Start()
    {
        GC = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl2>();
    }

    public void OnValidate()
    {
        if (addEvent == true)
        {
            int rand = Random.Range(0, unitList.Length);
            int randEvent = Random.Range(0, eventList.Length);
            lastEventIndex = rand;
            string uid = System.Guid.NewGuid().ToString();
            EventObject randEventObj = eventList[randEvent];
            unitList[rand].AddEvent(uid, randEventObj, eventGO, randEvent);
        }
        else if (addEvent == false)
        {
            unitList[lastEventIndex].RemoveEvent();
        }
    }

    public void AddEvent(string uid, int randPlace, int randEvent)
    {
        EventObject randEventObj = eventList[randEvent];
        unitList[randPlace].AddEvent(uid, randEventObj, eventGO, randEvent);
    }

    public void EventEnter(string eventID, EventObject eventObj)
    {
        eventHolder.GetComponent<EventDisplay>().eventObject = eventObj;
        eventHolder.GetComponent<EventDisplay>().UpdateDisplay();
        eventHolder.SetActive(true);
        foreach (EventUnit item in unitList)
        {
            if (item.eventID == eventID)
            {
                item.RemoveEvent();
            }
        }

        if (GC.MyPlayer.GetComponent<PlayerStats>().actorNo == GC.currentPlayer.actorNo)
        {
            //only for the current player
            Debug.Log($"Only current player {GC.currentPlayer.actorNo} {GC.MyPlayer.GetComponent<PlayerStats>().actorNo}");
            eventHolderButton.SetActive(true);
        } else
        {
            //only for other players to close the event card
            eventCloseButton.SetActive(true);
        }
        
        string eventName = eventObj.name;
        switch (eventName)
        {
            case "bandyciprawidlowi":
                Debug.Log("transfer to bitka z trzema rzesimieszkami");
                TransferToCombat();
                break;
            case "bagno":
                GC.currentPlayer.skipTurnDebuff = true;
                break;
            case "akcja waz":
                Debug.Log("przez nastepne 5 tur player losuje 1");
                GC.currentPlayer.rollDebuffCount = 5;
                break;
            case "Leroy Jenkins":
                Debug.Log("dopoki nie ukonczysz questa losujesz 1");
                break;
        }
    }

    public void TransferToCombat()
    {

    }

    public void EventConfirm()
    {
        eventHolder.SetActive(false);
        eventCloseButton.SetActive(false);
        eventHolderButton.SetActive(false);
    }

    public void EventImgClose()
    {
        eventHolder.SetActive(false);
        eventCloseButton.SetActive(false);
        eventHolderButton.SetActive(false);
    }

    public void Choice()
    {

    }
}
