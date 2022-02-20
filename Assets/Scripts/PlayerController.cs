using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerNavMesh))]
public class PlayerController : MonoBehaviour
{
    // AI pathing variable.
    PlayerNavMesh playerNav;

    //Reference to the CameraController
    [SerializeField] CameraController camControl;

    // Start is called before the first frame update
    void Start()
    {
        // Grab our pathing component.
        playerNav = GetComponent<PlayerNavMesh>();
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

        // If we right click...
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // Set the pathing to start.
            playerNav.SetMoveToMarker();
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
