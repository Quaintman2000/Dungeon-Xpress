using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public abstract class AnimationManager : MonoBehaviour
{
    [SerializeField]
    float animationTransitionTime;
    public Animator animator { get; protected set; }
    protected NavMeshAgent navMeshAgent;
    protected CombatController combatController;


    protected readonly int idleState = Animator.StringToHash("Idle State");
    protected readonly int skillOneState = Animator.StringToHash("Skill 1 State");
    protected readonly int skillTwoState = Animator.StringToHash("Skill 2 State");
    protected readonly int skillThreeState = Animator.StringToHash("Skill 3 State");
    protected readonly int skillFourState = Animator.StringToHash("Skill 4 State");
    protected readonly int movingState = Animator.StringToHash("Moving State");
    protected readonly int hitState = Animator.StringToHash("Hit State");
    protected readonly int dieState = Animator.StringToHash("Die State");

    protected int currentState;
    protected int previousState;
    protected bool isLocked = false;

    public System.Action<bool> AnimationLockAction;
    public System.Action DeathAnimationOverAction;
   
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Set our starting state to the idle state.
        ChangeAnimationState(idleState);

        // Try to get the combat controller component.
        if(TryGetComponent<CombatController>(out combatController))
        {
            // Subscribe to it's combat event(s).
            combatController.OnAbilityUsedAction += PlayAbilityAnimation;
            combatController.OnHurtAction += PlayHitAnimation;
            combatController.OnDeathAction += PlayDeathAnimation;
           
            // Override the current animations with the new ones.
            animator.runtimeAnimatorController = combatController.CharacterData.ClassAnimatorOverride;
        }

        // Try to get the navmeshMovement component.
        if(TryGetComponent<NavMeshMovement>(out NavMeshMovement movement))
        {
            // Subscribe the it's walking action event.
            movement.WalkingAction += ToggleWalkAnimation;
        }

    }
    

    /// <summary>
    /// Sets us to the moving state depending if we're walking or not.
    /// </summary>
    /// <param name="isWalking"> Are we walking? </param>
    protected virtual void ToggleWalkAnimation(bool isWalking)
    {
        // Change our animation state to walking if "isWalking" is true.
        // To idle if "isWalking" if false. 
        ChangeAnimationState(isWalking ? movingState : idleState);
    }
    
    /// <summary>
    /// Plays the ability animation with the combatant's ability index.
    /// </summary>
    /// <param name="combatant">The combatant using the ability.</param>
    protected virtual async void PlayAbilityAnimation(CombatController combatant)
    {
        // Get the state name from the ability index.
        var stateName = GetTriggerName(combatant.abilityIndex);
        // Change the animation state to that abilty state.
        await ChangeAnimationState(stateName);
        // Lock us in this state
        await LockState(animator.GetCurrentAnimatorStateInfo(0).length);

        await ChangeAnimationState(idleState);
    }

    /// <summary>
    /// Plays the hit animation and sets us back to the previous state.
    /// </summary>
    protected virtual async void PlayHitAnimation()
    {
        // Change the state to the hit animation.
        await ChangeAnimationState(hitState);
        // Set our lock state for the hit duration.
        await LockState(animator.GetCurrentAnimatorStateInfo(0).length);
        // Change our animation back to our previous state.
        await ChangeAnimationState(previousState);
    }
    /// <summary>
    /// Plays our death animation.
    /// </summary>
    protected virtual async void PlayDeathAnimation()
    {
        // Change the state to the hit animation.
        await ChangeAnimationState(dieState);
        // Set our lock state for the hit duration.
        await LockState(animator.GetCurrentAnimatorStateInfo(0).length);

        DeathAnimationOverAction?.Invoke();
    }

    /// <summary>
    /// Gets the trigger hascode based on ability index inputted.
    /// </summary>
    /// <param name="index">The index of the ability used.</param>
    /// <returns></returns>
    protected int GetTriggerName(int index)
    {
        // Clamp the index.
        index = Mathf.Clamp(index, 1, 4);
      
        // Get the trigger name based on the index given.
        switch(index)
        {
            case 1:
                return skillOneState;

            case 2:
                return skillTwoState;

            case 3:
                return skillThreeState;

            case 4:
                return skillFourState;
        }
        // If something went wrong, defualt to skill on state.
        return skillOneState;
    }


    protected async Task ChangeAnimationState(int stateHashCode)
    {
        // Prevent us from repeating the same state unintentionally.
        if (currentState == stateHashCode) return;
        // Swap to our new state and keep track of our previous state.
        previousState = currentState;
        currentState = stateHashCode;
        // Crossfade our state into the new state.
        animator.CrossFade(stateHashCode, animationTransitionTime, 0);

        var waitTime = Time.time + animationTransitionTime + .5f;

        while (waitTime > Time.time)
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// Calls the "AnimationLockAction" event at the start and end of the timer.
    /// </summary>
    /// <param name="lockTime"> The delay between calls.</param>
    /// <returns>Returns a task to await to.</returns>
    protected async Task LockState(float lockTime)
    {
        Debug.Log("LockTime: " + lockTime);

        AnimationLockAction?.Invoke(true);

        var waitTime = Time.time + lockTime;

        while(waitTime > Time.time)
        {
            await Task.Yield();
        }

        AnimationLockAction?.Invoke(false);
    }

}
