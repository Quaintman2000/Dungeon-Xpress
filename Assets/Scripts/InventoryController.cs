using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    //Playercontroller should have a input that checks if the player is in combat and if they are then an
    ///ap check is done in combat controller with a bool return
    ///else if in playercontroller free state it should also then just use the item without ap cost

    [SerializeField]private ItemData[] itemInventory;
    // Start is called before the first frame update
    void Start()
    {
        //Holds three items

        itemInventory = new ItemData[3]; 
    }
    public void Use(int position, CombatController target)
    {
        //Uses current item on self [Combat controller use ability needs update to use this efficently]
        target.selectedAbilityData = itemInventory[position];
        target.UseAbility(target);


        itemInventory[position] = null;
    }
    public ItemData GetItem(int position)
    {
        return itemInventory[position];
    }
    //should also drop unused item [in later version] on floor
    public void AddToPosition(int position, ItemData item)
    {
        //ItemData temp = itemInventory[position];
        itemInventory[position] = item;

        //return temp;
    }
    //used when picking up item and returns spot that the item is in for the inventory or returns -1
    public int AddItem(GameObject item)
    {
        for(int i = 0; i < itemInventory.Length; i++)
        {
            if(itemInventory[i] == null)
            {
                itemInventory[i] = item.GetComponent<ItemController>().ItemStats;
                return i;
            }
        }
        Debug.Log("Inventory is Full");
        return -1;
    }
    //Drops the Item on the floor
    //would be easier with gameobject prefabs of items
    //For enemies they should drop all items on death with forloop using this
    public void DropItem(int position)
    {
        if(itemInventory[position] != null)
        {
            GameObject i = itemInventory[position].ItemGameObject;

            Instantiate(i, this.transform.position, Quaternion.identity);

            itemInventory[position] = null;
        }
    }
}
