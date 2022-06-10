using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : CharacterController
{
    //Reference the creature animator
    [SerializeField] Animator charAnimator;

    //Used to stop from running multiple actions
    bool performingAction;

    //How far the ai will wander in a bubble radius
    [SerializeField] float idleRadius;
    //How long the ai waits before making another action again
    [SerializeField] float idleWait;

    // Start is called before the first frame update
    void Start()
    {
        combatController = GetComponent<CombatController>();

        performingAction = false;
        //Impliment when the AI has a Animator
        //charAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!performingAction)
        {
            Idle();
            CombatCheck();
        }

        ///Problem in battlemanager setturn with combantantindex going over. Recommend % Combantants.Count

        ///Attack move never reaches actual damage because never fully in range
        ///They never reach range because they are trying to go exactly where the other enemy is which is impossible
        ///due to colliders preventing from going inside each other
        ///current fix is setting ability distance to 1.6 or higher
        ///Gave test ability and buff both a range of 1.6 to avoid softlock of not reaching other 

        ///Changed test ability to melee to test since no projectile currently throws exception
        ///will test ranged attacks after projectiles are fully implemented and made a full scriptableobject

        ///line:navMeshAgent.isStopped in PlayerNavMesh.Moving(Vector3 movePosition, float closeEnough) stops moving no matter what and doesnt move after

        ///Needed some kind of buffer to allow only one action for all characters etc movement,attack
        ///implemented QuickBreak for use in idle but recommend a check during combat that prevents any type of action until the current ability/action finishes happening
    }
    //When the enemy isn't in combat they wander the area
    void Idle()
    {
        if(currentState == PlayerState.FreeRoam && !performingAction)
        {
            RandomMovement();
        }
    }
    //Gets a random direction for the enemy to move
    void RandomMovement()
    {
        //Gets a random point inside of a unit sphere and multiplies it by the radius of the idle radius
        Vector3 randomPoint = Random.insideUnitSphere * idleRadius;

        //Makes the navmesh hit and then 
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, idleRadius, 1);


        playerNav.SetMoveToMarker(hit.position + transform.position);

        //stops the ai from moving in multiple directions each update
        StartCoroutine(QuickBreak());
    }
    void CombatCheck()
    {
        //Does their move when it is their turn to fight
        if (currentState == PlayerState.InCombat && combatController.IsTurn)
        {
            if (BattleManager.Instance.Combatants.Count > 1)
            {
                //Should go after the second combatant
                StartCoroutine(QuickBreak());
                combatController.UseAbility(BattleManager.Instance.Combatants[0]);
            }
        }
    }
    //Slows the Enemy from performing actions right after another
    IEnumerator QuickBreak()
    {
        performingAction = true;

        //waits a set time to allow ample time to finish this current action before beginning a new one
        yield return new WaitForSeconds(idleWait);
        performingAction = false;
    }
    
}
