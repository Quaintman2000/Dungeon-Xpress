using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerNavMesh))]
public class PlayerController : CharacterController
{
    //Reference to the CameraController
    [SerializeField] CameraController camControl;
    //Reference the player animator
    [SerializeField] Animator charAnimator;

    private void Awake()
    {
        combatController = GetComponent<CombatController>();
        charAnimator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {
        //set override animator controller to the class' one
       charAnimator.runtimeAnimatorController = combatController.classData.ClassAnimatorOverride;
      
    }

    // Update is called once per frame
    void Update()
    {
        //Call the inputs every frame
        GetInputs();

        //if player is moving then walk
        if(playerNav.navMeshAgent.remainingDistance > 0.1f)
        {
            charAnimator.SetInteger("Walking", 1);
        }

        //if player is not moving then idle
        else if(playerNav.navMeshAgent.remainingDistance <= 0)
        {
            charAnimator.SetInteger("Walking", 0);

            //if player attacks and its a melee attack then play swing animation
            if(combatController.currentCombatState == CombatController.CombatState.Attacking && combatController.selectedAbilityData.Type == AbilityData.AbilityType.MeleeAttack)
            {
                charAnimator.SetInteger("Melee", 1);
            }else
            {
                charAnimator.SetInteger("Melee", 0);
            }

            //if player attacks and its a ranged attack then play cast animation
            if(combatController.currentCombatState == CombatController.CombatState.Attacking && combatController.selectedAbilityData.Type == AbilityData.AbilityType.RangeAttack)
            {
                charAnimator.SetInteger("Ranged", 1);
            }else
            {
                charAnimator.SetInteger("Ranged", 0);
            }
        }
    }

    //Keep track of all the different input options of the player
    private void GetInputs()
    {
        //Floats to keep track of the player's movement on the directional input
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //Cast a ray from our camera toward the plane, through our mouse cursor
            float distance;
            // Hit info from the raycast.
            RaycastHit hit;
            // Makes the raycast from our mouseposition to the ground.
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Sends the raycast of to infinity until hits something.
            Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

            // Grab the distance of the position we hit to get the point along the ray.
            distance = hit.distance;

            //Find where that ray hits the plane
            Vector3 raycastPoint = cameraRay.GetPoint(distance);

            // If we right click...
            if (currentState == PlayerState.FreeRoam )
            {
                if (!hit.collider.GetComponent<CombatController>())
                {
                    // Set the pathing to start.
                    playerNav.SetMoveToMarker(raycastPoint);
                }
                else
                {
                    MatchManager.Instance.StartCombat(this, hit.collider.GetComponent<CharacterController>());
                }
            }

            if (currentState == PlayerState.InCombat && combatController.IsTurn == true)
            {
                // If we hit a combatant...
                if (hit.collider.GetComponent<CombatController>())
                {
                    Debug.Log("Target Locked!");
                    // Set the combatant as other.
                    CombatController other = hit.collider.GetComponent<CombatController>();
                    // If the combatant isnt us...
                    
                        combatController.UseAbility(other);
                }
                else
                {
                    combatController.MoveToPoint(raycastPoint);
                }
            }
        }


        //If inputs a direction input...
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            //Set the camera to stop following the player
            camControl.IsFollowing = false;

            //Move on the desired input
            camControl.MoveCamera(verticalInput, horizontalInput);
        }

        //If pressing Q...
        if (Input.GetKey(KeyCode.Q))
        {
            //Rotate on the y axis clockwise
            camControl.RotateCamera(1.0f, 0.0f);
        }

        //If pressing E...
        if (Input.GetKey(KeyCode.E))
        {
            //Rotate on the y axis counter-clockwise
            camControl.RotateCamera(-1.0f, 0.0f);
        }

        //If pressing R...
        if (Input.GetKey(KeyCode.R))
        {
            //Set IsFollowing to false
            camControl.IsFollowing = false;

            //Pitch forward
            camControl.RotateCamera(0.0f, -1.0f);
        }

        //If pressing F...
        if (Input.GetKey(KeyCode.F))
        {
            //Set IsFollowing to false
            camControl.IsFollowing = false;

            //Pitch backward
            camControl.RotateCamera(.0f, 1.0f);
        }

        //When scrolling the mouse wheel...
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            //Zoom in or out based on input
            camControl.Zoom(Input.mouseScrollDelta.y);
        }

        //If pressing C...
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Set IsFollowing to true
            camControl.IsFollowing = true;

            //Start the Follow Player coroutine
            StartCoroutine(camControl.FollowPlayer());

            //Look at the player's position
            camControl.transform.LookAt(transform.position);
        }
    }
}
