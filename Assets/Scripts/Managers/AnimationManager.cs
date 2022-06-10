using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AnimationManager : MonoBehaviour
{
    protected Animator animator;
    protected NavMeshAgent navMeshAgent;
    protected CombatController combatController;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        combatController = GetComponent<CombatController>();
    }
    protected abstract void UpdateAnimator();
}
