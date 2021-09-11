using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventControl : MonoBehaviour
{
    public EventUnit[] unitList;
    public Sprite[] eventList;
    public bool addEvent = false;
    public GameObject eventGO;
    public int lastEventIndex;
    public GameObject eventHolder;
    public Button eventBtn;

    public void OnValidate()
    {
        if (addEvent == true)
        {
            int rand = Random.Range(0, unitList.Length);
            int spriteRand = Random.Range(0, eventList.Length);
            lastEventIndex = rand;
            unitList[rand].AddEvent("Combat", eventGO, eventList[spriteRand]);
        }
        else if (addEvent == false)
        {
            unitList[lastEventIndex].RemoveEvent();
        }
    }

    public void RandomEvent(int rand)
    {
        int spriteRand = Random.Range(0, eventList.Length);
        unitList[rand].AddEvent("Combat", eventGO, eventList[spriteRand]);
    }

    public void EventEnter(Sprite eventSprite)
    {
        eventHolder.GetComponent<Image>().sprite = eventSprite;
        eventHolder.SetActive(true);
        string eventName = eventSprite.name;
        switch (eventName)
        {
            case "Bandyci":
                Debug.Log("transfer to bitka z bandytą");
                TransferToCombat();
                break;
            case "bagno":
                Debug.Log("bagnio debuff, skipujesz nast ture");
                break;
            case "akcja_waz":
                Debug.Log("przez nastepne 5 tur player losuje 1");
                break;
            case "troll":
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
    }

    public void Choice()
    {

    }
}
