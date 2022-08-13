using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    // The item data for this object.
    public ItemData CurrentItemData => itemData;
    [SerializeField]
    ItemData itemData;
    // The world ui canvas to show the pickup button.
    [SerializeField]
    Canvas worldUICanvas;

    /// <summary>
    /// Destroys the item and returns the item data.
    /// </summary>
    /// <returns>The item data of this item.</returns>
    public ItemData Pickup()
    {
        Destroy(this.gameObject);
        return itemData;
    }
    /// <summary>
    /// Toggles our world UI to set active.
    /// </summary>
    /// <param name="isFocused">True for active, false for not active.</param>
    public void SetFocused(bool isFocused)
    {
        worldUICanvas.gameObject.SetActive(isFocused);
    }
}
