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
