using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterController : MonoBehaviour
{
    
    [SerializeField] GameObject selectionCircle;

    public Action<Vector3> CombatMoveToPointAction, FreeMoveToPointAction;
    public Action<CombatController> UseAbilityAction;
    // Reference to the player state.
    public enum PlayerState { InCombat, FreeRoam, Dead };
    [SerializeField] 
    public PlayerState currentState = PlayerState.FreeRoam;

    public CombatController combatController;
   
    //get the current state of the player and then switch it to either combat or freeroam.
   public void ChangeState(PlayerState newState)
    {
        currentState = newState;
    }

    public void SelectionToggle(bool isSelected)
    {
        selectionCircle.SetActive(isSelected);
    }

}
