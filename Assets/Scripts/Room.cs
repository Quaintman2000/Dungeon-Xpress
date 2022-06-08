using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Room : MonoBehaviour
{

    public Door[] RoomDoors;

    public Vector2 roomPosition;

    

    /// <summary>
    /// Returns a reference to the door facing in the specified direction.
    /// </summary>
    /// <param name="direction">The direction of the door you want to find.</param>
    /// <returns>Returns the door if it exists in this room. If not, it'll return null.</returns>
    public Door GetDoor(Door.Direction direction)
    {
        foreach(Door door in RoomDoors)
        {
            if (door.faceDirection == direction)
                return door;
        }
        return null;
    }
    /// <summary>
    /// Rotates the room and reorientes the doors in the right directions.
    /// </summary>
    /// <param name="rotations">The number of 90 degree rotations clockwise (right). No more or less than 4 & -4! </param>
    public void RotateRoom(int rotations)
    {
        rotations = Mathf.Clamp(rotations, -4, 4);
        if(rotations == 1 || rotations == -3)
        {
            transform.Rotate(Vector3.up, 90);
            foreach(Door door in RoomDoors)
            {
                if(door.faceDirection == Door.Direction.North)
                {
                    door.faceDirection = Door.Direction.East;
                }
                else if(door.faceDirection == Door.Direction.East)
                {
                    door.faceDirection = Door.Direction.South;
                }
                else if(door.faceDirection == Door.Direction.West)
                {
                    door.faceDirection = Door.Direction.North;
                }
                else if(door.faceDirection == Door.Direction.South)
                {
                    door.faceDirection = Door.Direction.West;
                }
            }
        }
        else if(rotations == 2 || rotations == -2)
        {
            transform.Rotate(Vector3.up, 180);
            foreach (Door door in RoomDoors)
            {
                if (door.faceDirection == Door.Direction.North)
                {
                    door.faceDirection = Door.Direction.South;
                }
                else if (door.faceDirection == Door.Direction.East)
                {
                    door.faceDirection = Door.Direction.West;
                }
                else if (door.faceDirection == Door.Direction.West)
                {
                    door.faceDirection = Door.Direction.East;
                }
                else if (door.faceDirection == Door.Direction.South)
                {
                    door.faceDirection = Door.Direction.North;
                }
            }
        }
        else if( rotations == 3 || rotations == -1)
        {
            transform.Rotate(Vector3.up, 270);
            foreach (Door door in RoomDoors)
            {
                if (door.faceDirection == Door.Direction.North)
                {
                    door.faceDirection = Door.Direction.West;
                }
                else if (door.faceDirection == Door.Direction.East)
                {
                    door.faceDirection = Door.Direction.North;
                }
                else if (door.faceDirection == Door.Direction.West)
                {
                    door.faceDirection = Door.Direction.South;
                }
                else if (door.faceDirection == Door.Direction.South)
                {
                    door.faceDirection = Door.Direction.East;
                }
            }
        }
    }
}
[System.Serializable]
public class Door
{
    public enum Direction { North, South, East, West};
    public Direction faceDirection;
    public bool isConnectToAnotherRoom;
    public GameObject doorBlockOff;
}
