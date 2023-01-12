using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    // Item focus cone radius to help the player select and item from a pile.
    [SerializeField]
    float focusConeRadius = 45;
    // The pick up radius.
    [SerializeField]
    float interactionRadius = 1;
    [SerializeField]
    CapsuleCollider radiusTriggerCollider;

    // Keeps track of our items.
    IInteractable focusedInteractable;

    private void Awake()
    {
        if(TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.AttemptPickupAction += InteractWithInteractable;
        }

        // Sets the trigger radius.
        radiusTriggerCollider.radius = interactionRadius;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void InteractWithInteractable()
    {
        if (focusedInteractable == null)
            return;
        Debug.Log("interacting with... ");
        focusedInteractable.Interact(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        // If we're not focusing on another item...
        if (focusedInteractable == null)
        {
            // If they are within our focus.
            if (IsWithinFocus(other.transform.position))
            {
                // If the other is a inventory item, set it to be our focused item.
                other.TryGetComponent<IInteractable>(out focusedInteractable);
                // Set the UI to activate.
                focusedInteractable.SetFocused(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // If we're focusing on a item...
        if (focusedInteractable != null)
        {
            // If the other is the item we are focusing on...
            if (other.GetComponent<IInteractable>() == focusedInteractable)
            {
                // Set the UI to deactivate.
                focusedInteractable.SetFocused(false);
                // Clear the focus item.
                focusedInteractable = null;
            }
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

        return (angleToTarget < focusConeRadius);
    }
}
