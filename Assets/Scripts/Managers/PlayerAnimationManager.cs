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
        }
    }

    public void SetIsParryStance(bool shouldParryStace)
    {
        isParryStance = shouldParryStace;
    }

    void EnterCastingState()
    {
        ChangeAnimationState(castingState);
    }
    protected override void ToggleWalkAnimation(bool isWalking)
    {
        // Change our animation state to walking if "isWalking" is true.
        // To idle if "isWalking" if false and if "isParryStance" is false, otherwise, go to parry stance idle. 
        ChangeAnimationState(isWalking ? movingState : (isParryStance ? parryStanceIdleState : idleState));
    }

    protected override async void PlayAbilityAnimation(CombatController combatant)
    {
        // Get the state name from the ability index.
        var stateName = GetTriggerName(combatant.abilityIndex);
        // Change the animation state to that abilty state.
        await ChangeAnimationState(stateName);
        // Lock us in this state
        await LockState(animator.GetCurrentAnimatorStateInfo(0).length);
        // Set the animator to our respective idle state based if we're in the parry stance or not.
        await ChangeAnimationState(isParryStance ? parryStanceIdleState : idleState);
    }

    public async void PlayParryAnimation()
    {
        // Change the animation state to parry state.
        await ChangeAnimationState(parryState);
        // Lock us in this state
        await LockState(animator.GetCurrentAnimatorStateInfo(0).length);
        // Set the animator to our respective idle state based if we're in the parry stance or not.
        await ChangeAnimationState(idleState);
    }

    public async void PlayDoorAnimation()
    {
        // Change the animation state to parry state.
        await ChangeAnimationState(enterDoorState);
        // Lock us in this state for the duration of the animation.
        await LockState(animator.GetCurrentAnimatorClipInfo(0).Length);
        // Set the animator to the idle state based if we're in the parry stance or not.
        await ChangeAnimationState(isParryStance ? parryStanceIdleState : idleState);
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
