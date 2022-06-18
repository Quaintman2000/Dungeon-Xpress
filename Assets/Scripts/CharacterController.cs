using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    // Reference to the player state.
    public enum PlayerState { InCombat, FreeRoam };
    [SerializeField] 
    public PlayerState currentState = PlayerState.FreeRoam;

    public CombatController combatController;
    public InventoryController inventoryController;

    //get the current state of the player and then switch it to either combat or freeroam.
   public void ChangeState(PlayerState newState)
    {
        currentState = newState;
    }
}
