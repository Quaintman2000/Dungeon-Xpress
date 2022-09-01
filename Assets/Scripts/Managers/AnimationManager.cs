using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public abstract class AnimationManager : MonoBehaviour
{
    public Animator animator { get; protected set; }
    protected NavMeshAgent navMeshAgent;
    protected CombatController combatController;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        combatController = GetComponent<CombatController>();
        combatController.OnAbilityUsedAction += PlayAbilityAnimation;
    }
    protected abstract void UpdateAnimator();

    protected void PlayAbilityAnimation(CombatController combatant)
    {
        var stateName = GetTriggerName(combatant.abilityIndex);

        animator.SetTrigger(stateName);
    }

    protected string GetTriggerName(int index)
    {
        // Clamp the index.
        index = Mathf.Clamp(index, 1, 4);
        // Initialize the trigger name string.
        string stateName = "";
        // Get the trigger name based on the index given.
        switch(index)
        {
            case 1:
                stateName = "UseSkillOne";
                break;

            case 2:
                stateName = "UseSkillTwo";
                break;

            case 3:
                stateName = "UseSkillThree";
                break;

            case 4:
                stateName = "UseSkillFour";
                break;
        }
        // Return the state name.
        return stateName;
    }
}
