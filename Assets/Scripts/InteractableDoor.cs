using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour , IInteractable
{
    // The world UI Canvas object that will display the button to interact with it.
    [SerializeField]
    Canvas worldUICanvas;


    // The exit position for when teleport through.
    public Vector3 DoorExitPosition => doorExitPosition.position;
    [SerializeField]
    Transform doorExitPosition;

    // The door to teleport us to.
    [SerializeField]
    InteractableDoor doorPair;



    public void Interact(InteractionController controller)
    {
        // Try to get the player animation controller...
        if(controller.TryGetComponent<PlayerAnimationManager>(out PlayerAnimationManager playerAnimationManager))
        {
            // Tell that animation manager to play the door animation.
            playerAnimationManager.PlayDoorAnimation();

            // Try to get their nav mesh movement component.
            if(controller.TryGetComponent<NavMeshMovement>(out NavMeshMovement navMeshMovement))
            {
                // Set the warp destination.
                navMeshMovement.WarpPosition = doorPair.DoorExitPosition;
            }
        }
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
