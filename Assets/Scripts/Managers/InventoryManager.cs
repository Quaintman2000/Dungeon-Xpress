using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    /// Should allow creatures to swap items or take items out of an inventory
    /// this is designed to allow transfership of items mainly
    //Instance of the InventoryManager
    public static InventoryManager Instance;

    //Keeps track of what inventory is being used by ui buttons
    public InventoryController playerInventory;

    //Item that is within range on the floor for the player to pick up
    public GameObject FloorItem;

    //used to display if an item is within pickup range
    [SerializeField] private GameObject IPickupNotification;
    //Contains the spots for the inventory ui sprites
    [SerializeField] private Image[] IInventorySpots; 

    // Ensures that only one instance of Inventory Manager exists
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        IInventorySpots = new Image[3];
    }
    //Inventory Spots self assign themselves to the list of inventoryspots
    public void AssignIInventorySpot(Image inventorySpot, int position)
    {
        IInventorySpots[position] = inventorySpot;
    }
    //Uses the players item then refreshes the ui
    public void UsePlayerItem(int position)
    {
        playerInventory.Use(position); 
        
        RefreshItemUI();
    }
    //Drops the players item on the ground
    public void DropPlayerItem(int position)
    {
        playerInventory.DropItem(position);

        RefreshItemUI();
    }
    /// <summary>
    /// When a player comes near a item it lets them know and makes it possible to pick it up
    /// </summary>
    /// <param name="item"></param>
    public void NearItem(GameObject item)
    {
        IPickupNotification.SetActive(true);
        FloorItem = item;
    }    
    /// <summary>
    /// Checks if the item parameter is the same as the floor item
    /// </summary>
    /// <param name="item"></param>
    public void LeftItem(GameObject item)
    {
        if (item == FloorItem)
        {
            IPickupNotification.SetActive(false);

            FloorItem = null;
        }
    }
    //Player presses e then checks if they are able to pickup an item or not
    public void PickUpItem()
    {
        //If no floor item then stop
        if(FloorItem == null) return;

        int itemPosition = playerInventory.AddItem(FloorItem);

        //if the item was set to a position then it deletes it from the floor
        if (itemPosition >= 0)
        {
            RefreshItemUI();
            IPickupNotification.SetActive(false);

            Destroy(FloorItem);
        }
        else
        {
            Debug.Log("Couldnt pick up item");
        }
    }

    //Goes through all three spots and refreshes them
    void RefreshItemUI()
    {
        for(int i = 0; i < IInventorySpots.Length; i++)
        {
            if (playerInventory.GetItem(i) != null)
            {
                IInventorySpots[i].sprite = playerInventory.GetItem(i).Icon;
            } else
            {
                IInventorySpots[i].sprite = null;
            }
        }
    }
}
