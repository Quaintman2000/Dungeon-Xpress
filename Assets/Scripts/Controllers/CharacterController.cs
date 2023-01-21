using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CharacterController : MonoBehaviour
{
    
    [SerializeField] GameObject selectionCircle;

    public Action<Vector3> CombatMoveToPointAction, FreeMoveToPointAction;
    public Action<CombatController> UseAbilityAction;
    public Action OnFreeRoamStateEnter, OnCombatStateEnter, OnCastingStateEnter, OnBusyStateEnter, OnDeadStateEnter;
    public Action OnFreeRoamStateExit, OnCombatStateExit, OnCastingStateExit, OnBusyStateExit, OnDeadStateExit;
    // Reference to the player state.
    public enum PlayerState { InCombat, FreeRoam, Dead, Busy, Casting };
    [SerializeField] 
    public PlayerState currentState = PlayerState.FreeRoam;
    protected PlayerState previousState;

    public CombatController combatController;

    protected Coroutine currentStateRoutine;

    protected virtual void Start()
    {
        combatController.OnAbilityUsedStartAction += EnterBusyState;
        combatController.OnAbilityUsedEndAction += ReturnToPreviousState;
        currentStateRoutine = StartCoroutine(HandleFreeRoamState());
    }

    //get the current state of the player and then switch it to either combat or freeroam.
    public void ChangeState(PlayerState newState)
    {
        // If the new state is the current, just stop it because we don't need to do anything.
        if (currentState == newState)
            return;

        // Switch out the current state for the new and keep track of the old.
        if(currentState == PlayerState.InCombat || currentState == PlayerState.FreeRoam)
            previousState = currentState;
        currentState = newState;
        // Stop the current state routine.
        StopCoroutine(currentStateRoutine);
        // Start the new state routine.
        if (newState == PlayerState.FreeRoam)
        {
            OnFreeRoamStateEnter?.Invoke();
            currentStateRoutine = StartCoroutine(HandleFreeRoamState());
        }
        else if (newState == PlayerState.InCombat)
        {
            OnCombatStateEnter?.Invoke();
            currentStateRoutine = StartCoroutine(HandleInCombatState());
        }
        else if (newState == PlayerState.Busy)
        {
            OnBusyStateEnter?.Invoke();
            currentStateRoutine = StartCoroutine(HandleBusyState());
        }
        else if (newState == PlayerState.Casting)
        {
            OnCastingStateEnter?.Invoke();
            currentStateRoutine = StartCoroutine(HandleCastingState());
        }
        else
            Debug.LogError(this.gameObject.name + " has attempted to change into an invalid Player state. State name:" + newState.ToString());
    }

    public void ReturnToPreviousState()
    {
        ChangeState(previousState);
    }

    protected virtual void EnterBusyState()
    {
        ChangeState(PlayerState.Busy);
    }

    protected abstract IEnumerator HandleFreeRoamState();
    protected abstract IEnumerator HandleInCombatState();
    protected abstract IEnumerator HandleCastingState();
    protected abstract IEnumerator HandleBusyState();
    public void SelectionToggle(bool isSelected)
    {
        selectionCircle.SetActive(isSelected);
    }

}
