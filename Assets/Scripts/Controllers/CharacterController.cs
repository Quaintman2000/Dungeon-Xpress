using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // AI pathing variable.
    [SerializeField] public NavMeshMovement navMeshMovement;
    [SerializeField] GameObject selectionCircle;

    // Reference to the player state.
    public enum PlayerState { InCombat, FreeRoam, Dead };
    [SerializeField] 
    public PlayerState currentState = PlayerState.FreeRoam;

    public CombatController combatController;
    // Start is called before the first frame update
    void Awake()
    {
        // Grab our pathing component.
        navMeshMovement = gameObject.GetComponent<NavMeshMovement>();
    }
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
