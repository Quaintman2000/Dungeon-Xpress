using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : AnimationManager
{
    protected bool isParryStance = false;

    readonly int parryStanceIdleState = Animator.StringToHash("Parry Stance Idle State");
    readonly int parryState = Animator.StringToHash("Parry State");
    readonly int enterDoorState = Animator.StringToHash("Enter Door");
    readonly int castingState = Animator.StringToHash("Casting State");

    protected override void Start()
    {
        base.Start();
        if (TryGetComponent<CharacterController>(out CharacterController character))
        {
            character.OnCastingStateEnter += EnterCastingState;
           if(character is PlayerController player)
            {
                player.OnCastingStateCancelAction += ExitCastingState;
            }
        }
    }

    public void SetIsParryStance(bool shouldParryStace)
    {
        isParryStance = shouldParryStace;
    }

    void EnterCastingState()
    {
        StartCoroutine(ChangeAnimationState(castingState));
    }

    void ExitCastingState()
    {
        StartCoroutine(ChangeAnimationState(idleState));
    }

    protected override void ToggleWalkAnimation(bool isWalking)
    {
        // Change our animation state to walking if "isWalking" is true.
        // To idle if "isWalking" if false and if "isParryStance" is false, otherwise, go to parry stance idle. 
        StartCoroutine(ChangeAnimationState(isWalking ? movingState : (isParryStance ? parryStanceIdleState : idleState)));
    }

    protected override IEnumerator PlayAbilityAnimation(CombatController combatant)
    {
        // Get the state name from the ability index.
        var stateName = GetTriggerName(combatant.abilityIndex);
        // Change the animation state to that abilty state.
        yield return StartCoroutine(ChangeAnimationState(stateName));
        // Lock us in this state
        yield return StartCoroutine(LockState(animator.GetCurrentAnimatorStateInfo(0).length));
        // Set the animator to our respective idle state based if we're in the parry stance or not.
        yield return StartCoroutine(ChangeAnimationState(isParryStance ? parryStanceIdleState : idleState));
    }

    public IEnumerator PlayParryAnimation()
    {
        // Change the animation state to parry state.
        yield return StartCoroutine(ChangeAnimationState(parryState));
        // Lock us in this state
        yield return StartCoroutine(LockState(animator.GetCurrentAnimatorStateInfo(0).length));
        // Set the animator to our respective idle state based if we're in the parry stance or not.
        yield return StartCoroutine(ChangeAnimationState(idleState));
    }

    public IEnumerator PlayDoorAnimation()
    {
        // Change the animation state to parry state.
        yield return StartCoroutine(ChangeAnimationState(enterDoorState));
        // Lock us in this state for the duration of the animation.
        yield return StartCoroutine(LockState(animator.GetCurrentAnimatorClipInfo(0).Length));
        // Set the animator to the idle state based if we're in the parry stance or not.
        yield return StartCoroutine(ChangeAnimationState(isParryStance ? parryStanceIdleState : idleState));
    }

    //public void DoorEnter()
    //{
    //    AnimationStart();
    //    animator.SetTrigger("OpenDoor");
    //    AnimationEnd();
    //}
    //When certain animations start it should stop them in place
    //private void AnimationStart()
    //{
    //    PlayerController player = this.gameObject.GetComponent<PlayerController>();
    //    player.navMeshMovement.navMeshAgent.isStopped = true;
    //    player.isBusy = true;
    //}
    ////When the animation finishes it lets them move again
    //public void AnimationEnd()
    //{
    //    PlayerController player = this.gameObject.GetComponent<PlayerController>();
    //    player.navMeshMovement.navMeshAgent.isStopped = false;

    //    player.isBusy = false;
    //}
}
