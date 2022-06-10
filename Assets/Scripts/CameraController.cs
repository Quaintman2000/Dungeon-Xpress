using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class CameraController : MonoBehaviour
{
    //Get reference to the camera
    [SerializeField] Camera camera;

    //Reference to the player's transform
    [SerializeField] Transform player;

    //Movement, rotation, and zoom speed
    [Header("Camera Movement Properties:")]
    [SerializeField, Range(0, 50)] float cameraSpeed = 0.0f;
    [SerializeField, Range(0, 90)] float turnSpeed = 0.0f; 

    [Header("Zoom Properties:")]
    [SerializeField, Range(0, 50)] float zoomSpeed = 0.0f;

    //Maximum and minimum zoom distance
    [SerializeField, Range(-10, 0)] float maxZoom = 0.0f;
    [SerializeField, Range(0, 10)] float minZoom = 0.0f;

    //Bool to check if the Follow Player coroutine is running
    [HideInInspector] public bool IsFollowing = false;

    //Keeps track of our current zoom value
    float zoomDistance = 0.0f;


    //Move on the x and z axis based on input from the player controller
    public void MoveCamera(float verticalInput, float horizontalInput)
    {
        transform.position += transform.forward * verticalInput * cameraSpeed * Time.deltaTime;
        transform.position += transform.right * horizontalInput * cameraSpeed * Time.deltaTime;
    }

    //Rotate on either the y or x axis based on input from the player controller
    public void RotateCamera(float pivotDirection, float pitchDirection)
    {
        transform.Rotate((Vector3.up * pivotDirection * turnSpeed * Time.deltaTime), Space.Self);
        camera.transform.Rotate((Vector3.right * pitchDirection * turnSpeed * Time.deltaTime), Space.Self);
    }

    //Zoom in and out function
    public void Zoom(float direction)
    {
        //Set zoom distance
        zoomDistance += zoomSpeed * direction * Time.deltaTime;

        //Clamp between the min and max
        zoomDistance = Mathf.Clamp(zoomDistance, maxZoom, minZoom);

        //As long as we aren't at the max or min...
        if(zoomDistance != maxZoom && zoomDistance != minZoom)
        {
            //Zoom in or out
            camera.transform.position += camera.transform.forward * zoomSpeed * direction * Time.deltaTime;
        }
    }

    //Reset camera transform to match the player's transform
    public IEnumerator FollowPlayer()
    {
        //While IsFollowing is true...
        while(IsFollowing)
        {
            //Set the camera's transform equal to the player's transform
            transform.position = player.transform.position;

            yield return null;
        }
    }
}
