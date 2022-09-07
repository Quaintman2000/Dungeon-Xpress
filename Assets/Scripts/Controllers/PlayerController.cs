using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : CharacterController
{
    
    //Reference to the CameraController
    [SerializeField] CameraController camControl;
    [SerializeField] PlayerAudioController audioControl;

  
    // Action events
    public Action AttemptPickupAction, OnRightClickDownAction, OnRightClickHeldDownAction, OnPauseAction;
    
    
    public Action<float, float> MoveCameraAction;
    public Action<float> RotateCameraAction;
    public Action<bool> OnCombatStartedAction;

    // TEMPORARY! Testing purposes only.
    public Action<CameraController.CameraStyle> SwitchCameraStyle;

    CharacterController selectedCharacter;
    const float rightClickHoldGap = 0.15f;
    float rightClickHoldTime;

    //Used to avoid a player from going through multiple doors in a frame
    bool canOpenDoor;

    //Prevents player input during certain actions
    public bool isBusy;
    private void Awake()
    {
        audioControl = GetComponent<PlayerAudioController>();
    }
    private void Start()
    {

        canOpenDoor = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isBusy)
        {
            //Call the inputs every frame
            GetInputs();
        }


    }
    //Keep track of all the different input options of the player
    private void GetInputs()
    {
        //Floats to keep track of the player's movement on the directional input
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");



        if (Input.GetMouseButtonDown(0))
        {
            // Hit info from the raycast.
            RaycastHit hit;
            // Makes the raycast from our mouseposition to the ground.
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Sends the raycast of to infinity until hits something.
            Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

            // If we hit a character with a character controller, set that as our selected character. If not, set it to null.
            CharacterController characterController;
            if (hit.collider.TryGetComponent<CharacterController>(out characterController))
                SelectCharacter(characterController);
            else
                SelectCharacter(this);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Sets the start position for the rotate start position.
            OnRightClickDownAction?.Invoke();
            //camControl.rotateStartPosition = Input.mousePosition;
            // Set the right click hold time to zero.
            rightClickHoldTime = 0;
        }
        if (Input.GetMouseButton(1))
        {
            // As we're holding right click, add onto the time hold.
            rightClickHoldTime += Time.deltaTime;
            // If we're holding it longer the hold gap time...
            if (rightClickHoldTime > rightClickHoldGap)
            {
                // Rotate the camera
                OnRightClickHeldDownAction?.Invoke();
                //camControl.RotateCamera(Input.mousePosition);

                audioControl.WalkSound();
            }
        }

        if (Input.GetMouseButtonUp(1) && rightClickHoldTime < rightClickHoldGap)
        {
            // Hit info from the raycast.
            RaycastHit hit;
            // Makes the raycast from our mouseposition to the ground.
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Sends the raycast of to infinity until hits something.
            Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

           

            if (selectedCharacter == this)
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
                        FreeMoveToPointAction?.Invoke(raycastPoint);
                        
                        audioControl.WalkLineSound();
                        audioControl.WalkSound();
                    }
                    else
                    {

                        if (hit.collider.GetComponent<CombatController>() != this.combatController)
                        {
                            MatchManager.Instance.StartCombat(this, hit.collider.GetComponent<CharacterController>());
                            OnCombatStartedAction?.Invoke(true);
                        }
                    }
                }
                else if (currentState == PlayerState.InCombat && combatController.IsTurn == true)
                {
                     
                    // If we hit a combatant...
                    if (hit.collider.TryGetComponent<CombatController>(out CombatController other))
                    {
                        // If the combatant isnt us...
                        UseAbilityAction?.Invoke(other);
                        //combatController.UseAbility(other);
                        //audioControl.AbilityCastlineSound();
                    }
                    else
                    {
                        CombatMoveToPointAction?.Invoke(raycastPoint);
                       // audioControl.WalkLineSound();
                        //audioControl.WalkSound();
                    }
                }
            }
        }

        //If inputs a direction input...
        if (verticalInput != 0 || horizontalInput != 0)
        {
            //Move on the desired input
            MoveCameraAction?.Invoke(verticalInput, horizontalInput);
            
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            // Rotate the camera to the right.
            RotateCameraAction?.Invoke(-1);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            // Rotate the camera to the left.
            RotateCameraAction?.Invoke(1);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Attempts to pickup an item if there is one on the floor
            AttemptPickupAction?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.R) && canOpenDoor)
        {
            //Checks if player is near door and enters if they do

            //GameManager.Instance.OpenDoor(this);
            //Starts cooldown
            StartCoroutine(DoorCooldown());
        }
        // If we press P.
        if (Input.GetKey(KeyCode.P))
        {
            // Invoke the pause action event.
            OnPauseAction?.Invoke();
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
            SwitchCameraStyle?.Invoke(CameraController.CameraStyle.PlayerFocused);
        }

        // If pressed 3...
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Set camera style to room locked.
            SwitchCameraStyle?.Invoke(CameraController.CameraStyle.RoomLocked);
        }
        //Used for items when held down drops the item in the slot instead of using them
        //bool shiftDown = Input.GetKey(KeyCode.LeftShift);

        ////If Pressing 5 at top of keyboard or numpad
        //if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        //{

        //    Debug.Log("Used Item");
        //}
        ////If Pressing 6 at top of keyboard or numpad
        //if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        //{

        //}
        ////If Pressing 6 at top of keyboard or numpad
        //if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        //{

        //}
    }
    void SelectCharacter(CharacterController character)
    {
        if (selectedCharacter != null)
            selectedCharacter.SelectionToggle(false);

        if (character != null)
            character.SelectionToggle(true);
        selectedCharacter = character;
    }
    public void Warped(Vector3 Offset)
    {
        //Sets camera to player position
        camControl.SetPosition(this.transform.position + Offset);

        //Anything else that should move with the player should also move here
    }
    //Used to prevent a player from going through multiples doors in a frame
    IEnumerator DoorCooldown()
    {
        canOpenDoor = false;
        yield return new WaitForSeconds(1f);
        canOpenDoor = true;
    }
}