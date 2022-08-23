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

    [SerializeField]
    CapsuleCollider pickupRadiusTriggerCollider;
    // Keeps track of our inventory size.
    [SerializeField]
    int inventorySize = 3;
    // Item focus cone radius to help the player select and item from a pile.
    [SerializeField]
    float itemFocusConeRadius = 45;
    // The pick up radius.
    [SerializeField]
    float itemPickupRadius = 1;
    [SerializeField]
    float forwardDropForce = 100;

    // Keeps track of our items.
    InventoryItem focusedItem;
    // Reference to the player controller.
    PlayerController playerController;
    [SerializeField]
    UIManager uIManager;

    private void Awake()
    {
        // Initialize the list.
        itemDatas = new List<ItemData>();
        // Sets the trigger radius.
        pickupRadiusTriggerCollider.radius = itemPickupRadius;
        // Set us up with the key binding.
        if (TryGetComponent<PlayerController>(out playerController))
        {
            playerController.AttemptPickupAction += PickUpItem;
        }
        if(uIManager != null)
        {
            uIManager.OnItemButtonPressed += UseItem;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If we're not focusing on another item...
        if (focusedItem == null)
        {
            // If they are within our focus.
            if (IsWithinFocus(other.transform.position))
            {
                // If the other is a inventory item, set it to be our focused item.
                other.TryGetComponent<InventoryItem>(out focusedItem);
                // Set the UI to activate.
                focusedItem.SetFocused(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // If we're focusing on a item...
        if (focusedItem != null)
        {
            // If the other is the item we are focusing on...
            if (other.GetComponent<InventoryItem>() == focusedItem)
            {
                // Set the UI to deactivate.
                focusedItem.SetFocused(true);
                // Clear the focus item.
                focusedItem = null;
            }
        }
    }
    /// <summary>
    /// Attempts to pickup the item if we have enough space in our inventory and are focusing on an object.
    /// </summary>
    private void PickUpItem()
    {
        // If there is still room in our inventory and if we're focusing on an item...
        if (itemDatas.Count < inventorySize && focusedItem != null)
        {
            // Add the item to our inventory.
            itemDatas.Add(focusedItem.Pickup());
            // Invokes the event for a sucessfull pickup if anyone is listening.
            OnItemPickUpSucess?.Invoke(focusedItem.CurrentItemData);
            // Clear the focused item.
            focusedItem = null;
        }
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

    /// <summary>
    /// Determines whether or not if the position is within our field of view.
    /// </summary>
    /// <param name="otherPosition">The other's position. </param>
    /// <returns>True if they are within the field of view, else false.</returns>
    bool IsWithinFocus(Vector3 otherPosition)
    {
        // Get the vector to the target.
        var vectorToTarget = otherPosition - transform.position;
        // Get the angle to the target.
        var angleToTarget = Vector3.Angle(vectorToTarget, transform.forward);

        return (angleToTarget < itemFocusConeRadius);
    }

    void UseItem(int index)
    {
        OnUseItem?.Invoke(itemDatas[index]);
        OnItemRemoved?.Invoke(itemDatas[index]);
        itemDatas.RemoveAt(index);
    }
}
