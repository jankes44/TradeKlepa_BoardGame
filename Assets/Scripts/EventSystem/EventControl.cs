using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventControl : MonoBehaviour
{
    public EventUnit[] unitList;
    public bool addEvent = false;
    public GameObject eventGO;
    public int lastEventIndex;

    public void OnValidate()
    {
        if (addEvent == true)
        {
            int rand = Random.Range(0, unitList.Length+1);
            lastEventIndex = rand;
            unitList[rand].AddEvent("Combat", eventGO);
        }
        else if (addEvent == false)
        {
            unitList[lastEventIndex].RemoveEvent();
        }
    }

    public void RandomEvent(int rand)
    {
        unitList[rand].AddEvent("Combat", eventGO);
    }
}
