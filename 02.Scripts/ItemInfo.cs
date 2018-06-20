using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemInfo : NetworkBehaviour
{
    private GameObject target;

    PlayerItem item;

    [SerializeField]
    private string Name;

    [SerializeField]
    private int ItemCode;

    private GameObject player;

    public string GetName()
    {
        return Name;
    }

    public int GetItemCode()
    {
        return ItemCode;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerItem>().items.Add(gameObject.name);
            item = other.gameObject.GetComponent<PlayerItem>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerItem>().items.Remove(gameObject.name);
            item.CheckNearItem();
        }
    }
}
