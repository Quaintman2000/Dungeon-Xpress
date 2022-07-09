using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemData ItemStats;

    public void AssignItemStats(ItemData itemStats)
    {
        ItemStats = itemStats;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            InventoryManager.Instance.NearItem(this.gameObject);
        }
    }
    //Checks if the item is the floor item that they leave
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            InventoryManager.Instance.LeftItem(this.gameObject);
        }
    }
}

