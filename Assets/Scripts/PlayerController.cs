using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerNavMesh))]
public class PlayerController : CharacterController
{
    //Reference to the CameraController
    [SerializeField] CameraController camControl;
    [SerializeField] UIManager uIManager;
    [SerializeField] CharacterController selectedCharacter;

    private void Awake()
    {
        combatController = GetComponent<CombatController>();
        
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Call the inputs every frame
        GetInputs();

    }

    //Keep track of all the different input options of the player
    private void GetInputs()
    {
        //Floats to keep track of the player's movement on the directional input
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        //if(Input.GetKeyDown(KeyCode.Mouse1))
        //{
        //    //Cast a ray from our camera toward the plane, through our mouse cursor
        //    float distance;
        //    // Hit info from the raycast.
        //    RaycastHit hit;
        //    // Makes the raycast from our mouseposition to the ground.
        //    Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    // Sends the raycast of to infinity until hits something.
        //    Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

        //    // Grab the distance of the position we hit to get the point along the ray.
        //    distance = hit.distance;

        //    //Find where that ray hits the plane
        //    Vector3 raycastPoint = cameraRay.GetPoint(distance);

        //    // If we right click...
        //    if (currentState == PlayerState.FreeRoam )
        //    {
        //        if (!hit.collider.GetComponent<CombatController>())
        //        {
        //            // Set the pathing to start.
        //            playerNav.SetMoveToMarker(raycastPoint);
        //        }
        //        else
        //        {
        //            MatchManager.Instance.StartCombat(this, hit.collider.GetComponent<CharacterController>());
        //            uIManager.ToggleSkillBar(true);
        //        }
        //    }

        //    if (currentState == PlayerState.InCombat && combatController.IsTurn == true)
        //    {
        //        // If we hit a combatant...
        //        if (hit.collider.GetComponent<CombatController>())
        //        {
        //            Debug.Log("Target Locked!");
        //            // Set the combatant as other.
        //            CombatController other = hit.collider.GetComponent<CombatController>();
        //            // If the combatant isnt us...
                    
        //                combatController.UseAbility(other);
        //        }
        //        else
        //        {
        //            combatController.MoveToPoint(raycastPoint);
        //        }
        //    }
        //}

        if(Input.GetMouseButtonDown(0))
        {
            // Hit info from the raycast.
            RaycastHit hit;
            // Makes the raycast from our mouseposition to the ground.
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Sends the raycast of to infinity until hits something.
            Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

            // If we hit a character with a character controller, set that as our selected character. If not, set it to null.
            if (hit.collider.GetComponent<CharacterController>())
                SelectCharacter(hit.collider.GetComponent<CharacterController>());
            else
                SelectCharacter(null);
        }

        if(Input.GetMouseButtonDown(1))
        {
            // Hit info from the raycast.
            RaycastHit hit;
            // Makes the raycast from our mouseposition to the ground.
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Sends the raycast of to infinity until hits something.
            Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

            camControl.rotateStartPosition = Input.mousePosition;

            if(selectedCharacter == this)
            {
                //Cast a ray from our camera toward the plane, through our mouse cursor
                float distance;
                // Grab the distance of the position we hit to get the point along the ray.
                distance = hit.distance;

                //Find where that ray hits the plane
                Vector3 raycastPoint = cameraRay.GetPoint(distance);

                // If we right click...
                if (currentState == PlayerState.FreeRoam)
                {
                    if (!hit.collider.GetComponent<CombatController>())
                    {
                        // Set the pathing to start.
                        playerNav.SetMoveToMarker(raycastPoint);
                    }
                    else
                    {

                        if (hit.collider.GetComponent<CombatController>() != this.combatController)
                        {
                            MatchManager.Instance.StartCombat(this, hit.collider.GetComponent<CharacterController>());
                            uIManager.ToggleSkillBar(true);
                        }
                    }
                }
            }
        }
        if(Input.GetMouseButton(1))
        {
            // Rotate the camera
            camControl.RotateCamera(Input.mousePosition);
        }

        if(camControl.cameraStyle == CameraController.CameraStyle.Freeform || camControl.cameraStyle == CameraController.CameraStyle.RoomLocked)
        {
           
            //If inputs a direction input...
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                //Move on the desired input
                camControl.MoveCamera(verticalInput, horizontalInput);
            }

        }
            if (Input.GetKey(KeyCode.E))
            {
                camControl.RotateCamera(-1, 0);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                camControl.RotateCamera(1, 0);
            }
       
        //When scrolling the mouse wheel...
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            //Zoom in or out based on input
            camControl.Zoom(Input.mouseScrollDelta.y);
        }

        //If pressed 1...
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Set camera style to player focused.
            camControl.SwitchCameraStyle(CameraController.CameraStyle.PlayerFocused);
        }
        // If pressed 2...
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Set camera style to free form.
            camControl.SwitchCameraStyle(CameraController.CameraStyle.Freeform);
        }
        // If pressed 3...
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Set camera style to room locked.
            camControl.SwitchCameraStyle(CameraController.CameraStyle.RoomLocked);
        }
    }

    void SelectCharacter(CharacterController character)
    {
        if (selectedCharacter != null)
            selectedCharacter.SelectionToggle(false);

        if(character != null)
            character.SelectionToggle(true);
        selectedCharacter = character;
    }
}
