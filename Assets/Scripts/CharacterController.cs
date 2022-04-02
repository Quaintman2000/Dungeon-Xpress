using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // AI pathing variable.
    protected PlayerNavMesh playerNav;

    // Reference to the player state.
    protected enum PlayerState { InCombat, FreeRoam };
    [SerializeField] 
    protected PlayerState currentState = PlayerState.FreeRoam;



    public bool IsTurn;
    [SerializeField] 
    protected CombatController combatController;
    // Start is called before the first frame update
    void Start()
    {
        // Grab our pathing component.
        playerNav = GetComponent<PlayerNavMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
