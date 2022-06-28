using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class CameraController : MonoBehaviour
{
    //Get reference to the camera
    [SerializeField] Camera camera;

    //Reference to the player's transform
    [SerializeField] Transform playerTransform;

    //Movement, rotation, and zoom speed
    [Header("Camera Movement Properties:")]
    [SerializeField, Range(0, 50)] float cameraSpeed = 0.0f;
    [SerializeField, Range(0, 90)] float turnSpeed = 0.0f;

    [Header("Zoom Properties:")]
    [SerializeField, Range(0, 50)] float zoomSpeed = 0.0f;

    //Maximum and minimum zoom distance
    [SerializeField, Range(-10, 0)] float maxZoom = 0.0f;
    [SerializeField, Range(0, 10)] float minZoom = 0.0f;

    [SerializeField] float cameraFollowSpeed = 5;

    //Bool to check if the Follow Player coroutine is running
    bool isFollowing = false;

    //Keeps track of our current zoom value
    float zoomDistance = 0.0f;

    public enum CameraStyle { Freeform, PlayerFocused, RoomLocked };
    public CameraStyle cameraStyle = CameraStyle.Freeform;
    private CameraStyle previousCameraStyle;

    private Coroutine followPlayerCoroutine;

    Vector2 roomLockMinValue = new Vector2();
    Vector2 roomLockMaxValue = new Vector2();

    [HideInInspector] public Vector3 rotateStartPosition;
    Vector3 rotateCurrentPosition;

    private void Start()
    {
        if (cameraStyle == CameraStyle.PlayerFocused)
            ToggleCameraFollow(true);
        else if (cameraStyle == CameraStyle.RoomLocked)
            ToggleRoomLocked();
    }

    //Move on the x and z axis based on input from the player controller
    public void MoveCamera(float verticalInput, float horizontalInput)
    {
        if (isFollowing == true)
            //Set the camera to stop following the player
            ReturnToPreviousCameraStyle();

        Vector3 newPosition = transform.position + (transform.forward * verticalInput * cameraSpeed * Time.deltaTime) + (transform.right * horizontalInput * cameraSpeed * Time.deltaTime);
        Vector3 cameraPositon = camera.transform.position;
        // If the camera style is not room locked or not going to exceeding the room's parameters.
        if (cameraStyle != CameraStyle.RoomLocked || ((newPosition.x > roomLockMinValue.x && newPosition.x < roomLockMaxValue.x) && (newPosition.z > roomLockMinValue.y && newPosition.z < roomLockMaxValue.y)))
        {
            transform.position = newPosition;
        }
    }

    //Rotate on either the y or x axis based on input from the player controller
    public void RotateCamera(float pivotDirection, float pitchDirection)
    {
        transform.Rotate((Vector3.up * pivotDirection * turnSpeed * Time.deltaTime), Space.Self);
        camera.transform.Rotate((Vector3.right * pitchDirection * turnSpeed * Time.deltaTime), Space.Self);
    }

    public void RotateCamera(Vector3 mousePosition)
    {
        // Set the current position.
        rotateCurrentPosition = mousePosition;
        // Calculate the difference amongst frames.
        Vector3 difference = rotateStartPosition - rotateCurrentPosition;
        // Reset drag position for the next frame.
        rotateStartPosition = rotateCurrentPosition;
        // Set the new rotation.
        transform.rotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
    }

    //Zoom in and out function
    public void Zoom(float direction)
    {
        //Set zoom distance
        zoomDistance += zoomSpeed * direction * Time.deltaTime;

        //Clamp between the min and max
        zoomDistance = Mathf.Clamp(zoomDistance, maxZoom, minZoom);

        //As long as we aren't at the max or min...
        if (zoomDistance != maxZoom && zoomDistance != minZoom)
        {
            //Zoom in or out
            camera.transform.position += camera.transform.forward * zoomSpeed * direction * Time.deltaTime;
        }
    }
    /// <summary>
    /// Toggles the camera following player functionality.
    /// </summary>
    /// <param name="toggleValue"></param>
    public void ToggleCameraFollow(bool toggleValue)
    {

        // Set the is Following value to the toggle value.
        isFollowing = toggleValue;

        // If the Follow player coroutine was still running for whatever reason...
        if (followPlayerCoroutine != null)
        {
            // Stop it.
            StopCoroutine(followPlayerCoroutine);
        }
        // If we are following the player...
        if (isFollowing)
        {
            cameraStyle = CameraStyle.PlayerFocused;
            transform.position = playerTransform.transform.position;
            // Start following the player.
            followPlayerCoroutine = StartCoroutine(FollowPlayer());
            // Set the camera to look at the player.
            camera.transform.LookAt(playerTransform);
        }
    }
    public void ToggleRoomLocked()
    {

        // Stop the camera following.
        ToggleCameraFollow(false);

        Vector2 roomPosition;

        RaycastHit hit;
        Physics.Raycast(playerTransform.position, -transform.up, out hit, Mathf.Infinity);
        if (hit.collider != null)
        {
            if (!hit.collider.bounds.Contains(transform.position))
                transform.position = playerTransform.position;

            Vector3 roomSize = hit.collider.bounds.size;
            Debug.Log("Room Size: " + roomSize);

            // Get the upmost parent object so we can get the position of the room.
            Transform roomParent = hit.collider.transform.parent;
            while (roomParent.GetComponent<Room>() != null)
            {
                roomParent = roomParent.parent;
            }

            // Divide the room size by two to get the radius.
            roomSize /= 2;
            roomSize -= (Vector3.one * Mathf.Abs(camera.transform.localPosition.z));
            // Set the min and max vector 2 values that our camera can travel within.
            roomLockMinValue = new Vector2(roomParent.position.x - roomSize.x, roomParent.position.z - roomSize.z);
            Debug.Log("Room Min Size: " + roomLockMinValue);

            roomLockMaxValue = new Vector2(roomParent.position.x + roomSize.x, roomParent.position.z + roomSize.z);
            Debug.Log("Room Max Size: " + roomLockMaxValue);


        }
    }

    //Reset camera transform to match the player's transform
    IEnumerator FollowPlayer()
    {
        //While IsFollowing is true...
        while (isFollowing)
        {
            // Move the camera towards the player position to get that follow effect.
            transform.position += (playerTransform.transform.position - transform.position) * cameraFollowSpeed * Time.deltaTime;

            yield return null;
        }
    }

    void ReturnToPreviousCameraStyle()
    {
        SwitchCameraStyle(previousCameraStyle);
    }

    public void SwitchCameraStyle(CameraStyle newCameraStyle)
    {
        previousCameraStyle = cameraStyle;

        cameraStyle = newCameraStyle;

        if (cameraStyle == CameraStyle.PlayerFocused)
        {
            ToggleCameraFollow(true);
        }
        else if (cameraStyle == CameraStyle.RoomLocked)
        {
            ToggleRoomLocked();
        }
        else if (cameraStyle == CameraStyle.Freeform)
        {
            ToggleCameraFollow(false);
        }
    }
}
