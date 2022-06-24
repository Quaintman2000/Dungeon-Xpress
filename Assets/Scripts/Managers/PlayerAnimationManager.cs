using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : AnimationManager
{
    ClassData classData;

    protected override void Start()
    {
        base.Start();
        classData = (ClassData)combatController.CharacterData;
        animator.runtimeAnimatorController = classData.ClassAnimatorOverride;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimator();   
    }

    protected override void UpdateAnimator()
    {
        animator.SetBool("IsMoving", navMeshAgent.velocity.magnitude > 0.1f);
    }

    public void SetAbilityTrigger(int index)
    {
        index = (int)Mathf.Clamp(index, 1, 4);
        switch(index)
        {
            case 1:
                animator.SetTrigger("UseSkillOne");
                break;

            case 2:
                animator.SetTrigger("UseSkillTwo");
                break;

            case 3:
                animator.SetTrigger("UseSkillThree");
                break;

            case 4:
                animator.SetTrigger("UseSkillFour");
                break;
        }
    }
    public void DoorEnter()
    {
        //Player Opens a door 
    }
}
