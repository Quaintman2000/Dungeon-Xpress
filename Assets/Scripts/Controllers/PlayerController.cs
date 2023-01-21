using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : CharacterController
{
    //Reference to the CameraController
    [SerializeField] CameraController camControl;
    [SerializeField] UIManager UIManager;
    // Action events
    public Action AttemptInteractAction, OnRightClickDownAction, OnRightClickHeldDownAction, OnPauseAction;

    public Action<float, float> MoveCameraAction;
    public Action<float> RotateCameraAction;
    public Action<bool> OnCombatStartedAction;
    public Action<RaycastData> OnCombatRightClickAction;

    // TEMPORARY! Testing purposes only.
    public Action<CameraController.CameraStyle> SwitchCameraStyle;

    CharacterController selectedCharacter;
    const float rightClickHoldGap = 0.15f;
    float rightClickHoldTime;

    //Prevents player input during certain actions
    public bool isBusy;

    private void Awake()
    {
        if (UIManager)
        {
            UIManager.OnAbilityButtonPressed += EnterCastingState;
        }
    }
    protected override void Start()
    {
        base.Start();

        //currentStateRoutine = StartCoroutine(HandleFreeRoamState());
    }
    // Update is called once per frame
    void Update()
    {
      
    }

    void EnterCastingState()
    {
        ChangeState(PlayerState.Casting);
    }

    protected override IEnumerator HandleFreeRoamState()
    {
        while (currentState == PlayerState.FreeRoam)
        {
            // If left click.
            if (Input.GetMouseButtonDown(0))
            {
                // Send out a raycast and if we hit a character with a character controller, set that as our selected character. If not, set it to the player character.
                if (SendRaycast().collider.TryGetComponent<CharacterController>(out CharacterController characterController))
                    SelectCharacter(characterController);
                else
                    SelectCharacter(this);
            }

            // If right click down.
            if (Input.GetMouseButtonDown(1))
            {
                // Sets the start position for the rotate start position.
                OnRightClickDownAction?.Invoke();
                //camControl.rotateStartPosition = Input.mousePosition;
                // Set the right click hold time to zero.
                rightClickHoldTime = 0;
            }
            // If right click continued...
            if (Input.GetMouseButton(1))
            {
                // As we're holding right click, add onto the time hold.
                rightClickHoldTime += Time.deltaTime;
                // If we're holding it longer the hold gap time...
                if (rightClickHoldTime > rightClickHoldGap)
                {
                    // Rotate the camera
                    OnRightClickHeldDownAction?.Invoke();
                }
            }
            // If right click up and we didn't hold if for very long...
            if (Input.GetMouseButtonUp(1) && rightClickHoldTime < rightClickHoldGap)
            {
                // Hit info from the raycast.
                RaycastHit hit;
                // Makes the raycast from our mouseposition to the ground.
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Sends the raycast of to infinity until hits something.
                if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity))
                {

                    selectedCharacter = this;

                    if (hit.collider.TryGetComponent<CombatController>(out CombatController combatant))
                    {
                        if (combatant != this.combatController)
                        {
                            MatchManager.Instance.StartCombat(this, combatant.GetComponent<CharacterController>());
                            OnCombatStartedAction?.Invoke(true);
                        }
                    }
                    else
                    {
                        //Cast a ray from our camera toward the plane, through our mouse cursor
                        float distance;
                        // Grab the distance of the position we hit to get the point along the ray.
                        distance = hit.distance;

                        //Find where that ray hits the plane
                        Vector3 raycastPoint = cameraRay.GetPoint(distance);
                        // Set the pathing to start.
                        FreeMoveToPointAction?.Invoke(raycastPoint);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("works");
                // Rotate the camera to the right.
                RotateCameraAction?.Invoke(-1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("doesnt work");
                // Rotate the camera to the left.
                RotateCameraAction?.Invoke(1);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                //Attempts to pickup an item if there is one on the floor
                AttemptInteractAction?.Invoke();
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

            Debug.Log("Free Roam Coroutine works.");
            yield return null;
        }
    }
    protected override IEnumerator HandleInCombatState()
    {
        while(currentState == PlayerState.InCombat)
        {
            // If left click.
            if (Input.GetMouseButtonDown(0))
            {
                // Send out a raycast and if we hit a character with a character controller, set that as our selected character. If not, set it to the player character.
                if (SendRaycast().collider.TryGetComponent<CharacterController>(out CharacterController characterController))
                    SelectCharacter(characterController);
                else
                    SelectCharacter(this);
            }

            // If right click down.
            if (Input.GetMouseButtonDown(1))
            {
                // Sets the start position for the rotate start position.
                OnRightClickDownAction?.Invoke();
                //camControl.rotateStartPosition = Input.mousePosition;
                // Set the right click hold time to zero.
                rightClickHoldTime = 0;
            }
            // If right click continued...
            if (Input.GetMouseButton(1))
            {
                // As we're holding right click, add onto the time hold.
                rightClickHoldTime += Time.deltaTime;
                // If we're holding it longer the hold gap time...
                if (rightClickHoldTime > rightClickHoldGap)
                {
                    // Rotate the camera
                    OnRightClickHeldDownAction?.Invoke();
                }
            }
            // If right click up and we didn't hold if for very long...
            if (Input.GetMouseButtonUp(1) && rightClickHoldTime < rightClickHoldGap)
            {
                // Hit info from the raycast.
                RaycastHit hit;
                // Makes the raycast from our mouseposition to the ground.
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Sends the raycast of to infinity until hits something.

                RaycastData newRayCastData = new RaycastData(SendRaycast(), combatController);

                OnCombatRightClickAction?.Invoke(newRayCastData);

            }
            //Floats to keep track of the player's movement on the directional input
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            //If inputs a direction input...
            if (verticalInput != 0 || horizontalInput != 0)
            {
                //Move on the desired input
                MoveCameraAction?.Invoke(verticalInput, horizontalInput);

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("works");
                // Rotate the camera to the right.
                RotateCameraAction?.Invoke(-1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("doesnt work");
                // Rotate the camera to the left.
                RotateCameraAction?.Invoke(1);
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
            yield return null;
        }
    }

    protected override IEnumerator HandleCastingState()
    {
        // Keep track if we should cancel.
        bool shouldCancel = false;

        while (currentState == PlayerState.Casting && shouldCancel == false)
        {
            // If left click.
            if (Input.GetMouseButtonDown(0))
            {
                shouldCancel = true;
            }

            // If right click down.
            if (Input.GetMouseButtonDown(1))
            {
                // Sets the start position for the rotate start position.
                OnRightClickDownAction?.Invoke();
                //camControl.rotateStartPosition = Input.mousePosition;
                // Set the right click hold time to zero.
                rightClickHoldTime = 0;
            }
            // If right click continued...
            if (Input.GetMouseButton(1))
            {
                // As we're holding right click, add onto the time hold.
                rightClickHoldTime += Time.deltaTime;
                // If we're holding it longer the hold gap time...
                if (rightClickHoldTime > rightClickHoldGap)
                {
                    // Rotate the camera
                    OnRightClickHeldDownAction?.Invoke();
                }
            }
            // If right click up and we didn't hold if for very long...
            if (Input.GetMouseButtonUp(1) && rightClickHoldTime < rightClickHoldGap)
            {
                // Hit info from the raycast.
                RaycastHit hit;
                // Makes the raycast from our mouseposition to the ground.
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Sends the raycast of to infinity until hits something.

                RaycastData newRayCastData = new RaycastData(SendRaycast(), combatController);

                OnCombatRightClickAction?.Invoke(newRayCastData);

            }
            //Floats to keep track of the player's movement on the directional input
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            //If inputs a direction input...
            if (verticalInput != 0 || horizontalInput != 0)
            {
                //Move on the desired input
                MoveCameraAction?.Invoke(verticalInput, horizontalInput);

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("works");
                // Rotate the camera to the right.
                RotateCameraAction?.Invoke(-1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("doesnt work");
                // Rotate the camera to the left.
                RotateCameraAction?.Invoke(1);
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
            yield return null;
        }

        ReturnToPreviousState();
    }

    protected override IEnumerator HandleBusyState()
    {
        while (currentState == PlayerState.Busy)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("works");
                // Rotate the camera to the right.
                RotateCameraAction?.Invoke(-1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("doesnt work");
                // Rotate the camera to the left.
                RotateCameraAction?.Invoke(1);
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

            yield return null;
        }
    }
    // OLD METHOD. Kept for reference.
    // Keep track of all the different input options of the player
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
                    }
                    else
                    {
                        CombatMoveToPointAction?.Invoke(raycastPoint);
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
            Debug.Log("works");
            // Rotate the camera to the right.
            RotateCameraAction?.Invoke(-1);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("doesnt work");
            // Rotate the camera to the left.
            RotateCameraAction?.Invoke(1);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Attempts to pickup an item if there is one on the floor
            AttemptInteractAction?.Invoke();
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

    }
    void SelectCharacter(CharacterController character)
    {
        if (selectedCharacter != null)
            selectedCharacter.SelectionToggle(false);

        if (character != null)
            character.SelectionToggle(true);
        selectedCharacter = character;
    }

    private RaycastHit SendRaycast()
    {
        // Hit info from the raycast.
        RaycastHit hit;
        // Makes the raycast from our mouseposition to the ground.
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Sends the raycast of to infinity until hits something.
        Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

        return hit;
    }

}