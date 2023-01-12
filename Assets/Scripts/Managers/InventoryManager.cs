using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // An event for when sucessfully pick up the item.
    public delegate void ItemAction(ItemData itemData);
    public event ItemAction OnItemPickUpSucess, OnUseItem, OnItemRemoved;
    // List of our items in our inventory.
    public List<ItemData> itemDatas { get; private set; }

    
    // Keeps track of our inventory size.
    [SerializeField]
    int inventorySize = 3;
   
    [SerializeField]
    float forwardDropForce = 100;

    
    // Reference to the player controller.
    PlayerController playerController;
    [SerializeField]
    UIManager uIManager;

    private void Awake()
    {
        // Initialize the list.
        itemDatas = new List<ItemData>();
      
        // Set us up with the key binding.
        //if (TryGetComponent<PlayerController>(out playerController))
        //{
        //    playerController.AttemptPickupAction += PickUpItem;
        //}
        if(uIManager != null)
        {
            uIManager.OnItemButtonPressed += UseItem;
        }
    }

    
    /// <summary>
    /// Attempts to pickup the item if we have enough space in our inventory and are focusing on an object.
    /// </summary>
    public bool PickUpItem(ItemData newItem)
    {
        // If there is still room in our inventory and if we're focusing on an item...
        if (itemDatas.Count < inventorySize)
        {
            // Add the item to our inventory.
            itemDatas.Add(newItem);
            // Invokes the event for a sucessfull pickup if anyone is listening.
            OnItemPickUpSucess?.Invoke(newItem);
            
            return true;
        }
        return false;
    }
    /// <summary>
    /// Drops the item at a index.
    /// </summary>
    /// <param name="index">The index of the item to drop.</param>
    public void DropItem(int index)
    {
        // If the index is within our items count...
        if (index < itemDatas.Count)
        {
            Rigidbody itemRigidbody;
            // Spawn in the inventory item of that item data.
            var droppedItem = Instantiate(itemDatas[index].ItemGameObject, (transform.forward + transform.up), Quaternion.identity);
            // If it has a rigidbody, apply forward force to it
            if (droppedItem.TryGetComponent<Rigidbody>(out itemRigidbody))
                itemRigidbody.AddForce(transform.forward.normalized * forwardDropForce);
            // Remove the item in our inventory.
            itemDatas.RemoveAt(index);
        }
    }
    /// <summary>
    /// Drops the item.
    /// </summary>
    /// <param name="item">The item to drop.</param>
    public void DropItem(ItemData item)
    {
        // If we have this item.
        if (itemDatas.Contains(item))
        {
            Rigidbody itemRigidbody;
            // Spawn in the inventory item of that item data.
            var droppedItem = Instantiate(item.ItemGameObject, (transform.forward + transform.up), Quaternion.identity);
            // If it has a rigidbody, apply forward force to it
            if (droppedItem.TryGetComponent<Rigidbody>(out itemRigidbody))
                itemRigidbody.AddForce(transform.forward.normalized * forwardDropForce);
            // Remove the item in our inventory.
            itemDatas.Remove(item);
        }
    }

    

    void UseItem(int index)
    {
        OnUseItem?.Invoke(itemDatas[index]);
        OnItemRemoved?.Invoke(itemDatas[index]);
        itemDatas.RemoveAt(index);
    }
}
