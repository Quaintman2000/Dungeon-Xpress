using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private int id;
    //Spot that the player should exit out of from
    public Transform ExitPoint;
    public enum DoorType
    {
        Normal, //Teleports player but doesn't do anything special
        Boss, //Handles Special animations or procedures
    }
    [SerializeField] private DoorType doorType;
    /// Should handle teleporting the player from the door to a seperate spot
    /// 
    /// Create a prefab that has two doors like this that have their teleportation location to the others location
    /// 
    /// Should also let the player who touched it perform an animation to enter before teleporting
    /// Also should stop the player from doing any type of action until they finish the door event
    /// Should link up to a system for e that any actions in the area are commenced first before the door is opened

    private void Start()
    {
        id = GameManager.Instance.AddDoor(this);
    }
    //Sets the players position to the exit point
    void Teleport(PlayerController player) 
    {
        player.gameObject.transform.position = ExitPoint.position;
        player.gameObject.transform.rotation = ExitPoint.rotation;
    }
    //When the player presses E to enter
    public void OnDoorEnter(PlayerController player)
    {
        switch(doorType)
        {
            //Handles extra steps when the player opens the door
            case DoorType.Boss:
                PlayerAnimationManager playerAnimator = player.GetComponent<PlayerAnimationManager>();
                if(playerAnimator != null)
                {
                    playerAnimator.DoorEnter();
                }
                goto case DoorType.Normal; 
            case DoorType.Normal:
                Teleport(player);
                break;
        }
    }
    public int GetID()
    {
        return id;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            GameManager.Instance.CurrentDoor = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            GameManager.Instance.CurrentDoor = null;
        }
    }
}
