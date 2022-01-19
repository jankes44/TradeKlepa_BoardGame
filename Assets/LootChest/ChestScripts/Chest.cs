using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour
{
    public List<Item> items;

    public Transform itemHolder;
    public bool canOpen = false;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // if (canOpen && Input.GetKeyDown("space"))
        // {
        //     animator.SetTrigger("open");
        //     // if (IsOpen())
        //     // {
        //     //     animator.SetTrigger("close");
        //     //     HideItem();
        //     // }
        //     // else
        //     // {
        //     //     animator.SetTrigger("open");
        //     // }
        // }
    }

    private void OnMouseDown() {
        if (canOpen)
        {
            int item = GetRandomIndex();
            Debug.Log(item);
            EventControl.instance.eventPlayer.GetComponent<PlayerStats>().OpenChest(item);
        }
    }

    public void OpenChest(int itemIndex) {
        Item item = items[itemIndex];
        Transform itemTransform = item.weaponPrefab.transform;
        animator.SetTrigger("open");
        ShowItem(itemTransform);
    }

    public bool IsOpen()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("ChestOpen");
    }

    void HideItem()
    {
        itemHolder.localScale = Vector3.zero;
        itemHolder.gameObject.SetActive(false);

        foreach (Transform child in itemHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowItem(Transform item)
    {
        Instantiate(item, itemHolder);
        itemHolder.gameObject.SetActive(true);
    }

    public Item GetRandom()
    {
        float totalWeight = 0;

        foreach (Item i in items)
        {
            totalWeight += i.rarity;
        }

        float value = Random.value * totalWeight;

        float sumWeight = 0;

        foreach (Item i in items)
        {
            sumWeight += i.rarity;

            if (sumWeight >= value)
            {
                return i;
            }
        }

        return default(Item);
    }

    public int GetRandomIndex() {
        float totalWeight = 0;
        int randItem = -1;

        foreach (Item i in items)
        {
            totalWeight += i.rarity;
        }

        float value = Random.value * totalWeight;

        float sumWeight = 0;

        foreach (Item i in items)
        {
            sumWeight += i.rarity;

            if (sumWeight >= value)
            {
                randItem = items.FindIndex(x => x.Equals(i));
            }
        }

        if (randItem == -1) {
            randItem = 0;
        }
        Debug.Log(randItem);
        return randItem;
    }
}
