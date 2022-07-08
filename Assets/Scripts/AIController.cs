using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : CharacterController
{
    [SerializeField] NavMeshMovement aiNav;
    //Reference the creature animator
    [SerializeField] Animator charAnimator;

    //Used to stop from running multiple actions
    bool performingAction;

    //Different movements for the ai to use when navigating the map
    public enum MovementType
    {
        Stationary,
        Patrol,
        Random
    }
    //How the enemy navigates the map
    public MovementType movementType;

    //Determines if it is going to point 1 or 2 for patrol
    [SerializeField] bool goToPoint1;
    [SerializeField] Vector3 PatrolPoint1, PatrolPoint2;

    //How far the ai will wander in a bubble radius
    [SerializeField] float idleRadius;

    private void Awake()
    {
        // Grab our pathing component.
        aiNav = gameObject.GetComponent<NavMeshMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        combatController = GetComponent<CombatController>();

        performingAction = false;
        //Impliment when the AI has a Animator
        //charAnimator = GetComponent<Animator>();

        //If there aren't any patrol points on the map then the ai makes its own patrol points


        Vector3 randomPoint = Random.insideUnitSphere * 15f;

        //Creates a random point on the map and sets it as the second movement point
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, 15f, 1);

        PatrolPoint1 = gameObject.transform.position;
        PatrolPoint2 = hit.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!aiNav.isMoving)
        {
            AIState();
        }
    }
    void AIState()
    {
        switch (currentState)
        {
            case PlayerState.FreeRoam:
                Idle();
                break;
            case PlayerState.InCombat:
                CombatCheck();
                break;
        }
    }
    //When the enemy isn't in combat they wander the area
    void Idle()
    {
        switch(movementType)
        {
            case MovementType.Stationary:
                break;
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Random:
            default:
                RandomMovement();
                break;

        }
    }
    void Patrol()
    {
        Vector3 point = goToPoint1 ? PatrolPoint1 : PatrolPoint2;
        aiNav.Move(point);
        goToPoint1 = !goToPoint1;
    }
    //Gets a random direction for the enemy to move
    void RandomMovement()
    {
        //Gets a random point inside of a unit sphere and multiplies it by the radius of the idle radius
        Vector3 randomPoint = Random.insideUnitSphere * idleRadius;

        //Makes the navmesh hit and then 
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, idleRadius, 1)) // Checks if it finds a point or not and stops if it doesn't
        {
            aiNav.Move(hit.position + transform.position);
        }
    }
    void CombatCheck()
    {
        //Does their move when it is their turn to fight
        if (currentState == PlayerState.InCombat && combatController.IsTurn)
        {
            if (BattleManager.Instance.Combatants.Count > 1)
            {
                //Should go after the second combatant
                if(aiNav.GetDistance(BattleManager.Instance.Combatants[0].transform.position) <= combatController.classData.StartingActionPoints)
                combatController.UseAbility(BattleManager.Instance.Combatants[0]);
            }
        }
    }
}
