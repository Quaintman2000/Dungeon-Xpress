using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    float roomSpacing;
    [SerializeField]
    int maxNumRooms;
    [SerializeField]
    GameObject hallwayPrefab;
    [SerializeField]
    Room startRoom;
    [SerializeField]
    WeightedRoom[] roomPrefabs;
    [SerializeField]
    Room[] dungeonRooms;

    int roomsMade = 0;

    Coroutine mapGenCoroutine;

    private void Awake()
    {
        SortRoomsByWeight();
        dungeonRooms = new Room[maxNumRooms];
    }

    private void OnValidate()
    {
        dungeonRooms = new Room[maxNumRooms];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Starts the dungeon generating process.
    /// </summary>
    [ContextMenu("Generate Dungeon")]
    public void Generate()
    {
        // Spawn in the start room.
        Room start = Instantiate<Room>(startRoom, Vector3.zero, Quaternion.identity);
        dungeonRooms[0] = start;
        start.roomPosition = Vector3.zero;
        roomsMade = 1;
        // Start the map generating coroutine.
        if (mapGenCoroutine != null)
            StopCoroutine(mapGenCoroutine);
        mapGenCoroutine = StartCoroutine(GenerateDungeon());
    }

    
    /// <summary>
    /// Generates the dungeon.
    /// </summary>
    private IEnumerator GenerateDungeon()
    {
        // Determine the total weight for the weighted random.
        float totalWeight = 0;
        foreach (WeightedRoom weightedRoom in roomPrefabs)
        {
            totalWeight += weightedRoom.weight;
        }
        // For each room while we haven't made the max number of rooms....
        for (int i = 0; (i < maxNumRooms && roomsMade < maxNumRooms); i++)
        {
            Debug.Log("Making room: :" + i);
            // For each of the room's doors while we haven't made the max number of rooms...
            for (int j = 0; j < dungeonRooms[i].RoomDoors.Count && roomsMade < maxNumRooms; j++)
            {
                // Save the current door.
                Door currentDoor = dungeonRooms[i].RoomDoors[j];
                Debug.Log("Checking room " + i + ", Door " + j);
                // If this door at the j index is not connected to a room, check to see if it can spawn the room.
                if (currentDoor.isConnectToAnotherRoom == false)
                {
                    // Determine which way the door is facing to determine the "position" of the new room.
                    Vector2 newPosition = dungeonRooms[i].roomPosition;
                    Door.Direction currentDoorDirection = currentDoor.faceDirection;
                    // If the door is facing north...
                    if (currentDoorDirection == Door.Direction.North)
                    {
                        newPosition += Vector2.up;
                    }
                    // If the door is facing east...
                    else if (currentDoorDirection == Door.Direction.East)
                    {
                        newPosition += Vector2.right;
                    }
                    // If the door is facing west...
                    else if (currentDoorDirection == Door.Direction.West)
                    {
                        newPosition += Vector2.left;
                    }
                    // If the door is facing south...
                    else
                    {
                        newPosition += Vector2.down;
                    }


                    // Check to see if have any rooms at that position. 
                    Room roomToConnect = FindRoomAtLocation(newPosition);

                    Vector3 hallwayPosition = (new Vector3(((newPosition.x + dungeonRooms[i].roomPosition.x)/2), 0, ((newPosition.y + dungeonRooms[i].roomPosition.y)/2)) * roomSpacing * 2);
                    // If we do have a room at that position
                    if (roomToConnect != null)
                    {
                        Debug.Log("There is a room there.");
                        // If we are the north door...
                        if (currentDoorDirection == Door.Direction.North)
                        {
                            // If the room we want to connect has our corresponding door...
                            if (roomToConnect.GetDoor(Door.Direction.South) != null)
                            {
                                // Set up the hallway and connect the doors if we can.
                                Door doorToConnect = roomToConnect.GetDoor(Door.Direction.South);
                                if (doorToConnect.isConnectToAnotherRoom == false)
                                {
                                    Instantiate(hallwayPrefab, hallwayPosition, Quaternion.Euler(0, 0, 0));
                                    doorToConnect.isConnectToAnotherRoom = true;
                                    currentDoor.isConnectToAnotherRoom = true;
                                }
                            }
                        }
                        // If we are the east door...
                        else if (currentDoorDirection == Door.Direction.East)
                        {
                            // If the room we want to connect has our corresponding door...
                            if (roomToConnect.GetDoor(Door.Direction.West) != null)
                            {
                                // Set up the hallway and connect the doors if we can.
                                Door doorToConnect = roomToConnect.GetDoor(Door.Direction.East);
                                if (doorToConnect.isConnectToAnotherRoom == false)
                                {
                                    Instantiate(hallwayPrefab,hallwayPosition, Quaternion.Euler(0, 90, 0));
                                    doorToConnect.isConnectToAnotherRoom = true;
                                    currentDoor.isConnectToAnotherRoom = true;
                                }
                            }
                        }
                        // If we are the west door...
                        else if (currentDoorDirection == Door.Direction.West)
                        {
                            // If the room we want to connect has our corresponding door...
                            if (roomToConnect.GetDoor(Door.Direction.East) != null)
                            {
                                // Set up the hallway and connect the doors if we can.
                                Door doorToConnect = roomToConnect.GetDoor(Door.Direction.East);
                                if (doorToConnect.isConnectToAnotherRoom == false)
                                {
                                    Instantiate(hallwayPrefab, hallwayPosition, Quaternion.Euler(0, 90, 0));
                                    doorToConnect.isConnectToAnotherRoom = true;
                                    currentDoor.isConnectToAnotherRoom = true;
                                }
                            }
                        }
                        // If we are the south door...
                        else
                        {
                            // If the room we want to connect has our corresponding door...
                            if (roomToConnect.GetDoor(Door.Direction.North) != null)
                            {
                                // Set up the hallway and connect the doors if we can.
                                Door doorToConnect = roomToConnect.GetDoor(Door.Direction.North);
                                if (doorToConnect.isConnectToAnotherRoom == false)
                                {

                                    Instantiate(hallwayPrefab, hallwayPosition, Quaternion.Euler(0, 0, 0));
                                    doorToConnect.isConnectToAnotherRoom = true;
                                    currentDoor.isConnectToAnotherRoom = true;
                                }
                            }
                        }
                    }
                    // If we do not have a room at that position.
                    else
                    {
                        // If we're still under the max number of rooms...
                        if (roomsMade < maxNumRooms)
                        {
                            Debug.Log("There isn't a room there.");


                            // Determine the room to spawn via weighted random.
                            
                            Room roomToSpawn = WeightedRandomRoom(totalWeight);

                            // Spawn in the selected room at the new position and add it to the list.
                            Room newRoom = Instantiate(roomToSpawn, new Vector3(newPosition.x, 0, newPosition.y) * roomSpacing * 2, Quaternion.identity);
                            dungeonRooms[roomsMade] = newRoom;
                            newRoom.roomPosition = newPosition;

                            // Determine what doors the new room has.
                            bool hasNorth = false, hasSouth = false, hasWest = false, hasEast = false;
                            foreach (Door door in roomToSpawn.RoomDoors)
                            {
                                if (door.faceDirection == Door.Direction.North)
                                    hasNorth = true;
                                else if (door.faceDirection == Door.Direction.East)
                                    hasEast = true;
                                else if (door.faceDirection == Door.Direction.South)
                                    hasSouth = true;
                                else if (door.faceDirection == Door.Direction.West)
                                    hasWest = true;
                            }
                            // Rotate the new room so I connects with the old one.
                            // If our current door is the north one...
                            if (currentDoorDirection == Door.Direction.North)
                            {
                               // If it has all 4 doors...
                               if(hasNorth == true && hasSouth == true && hasEast == true && hasWest == true)
                                {
                                    // Rotate the room in a random way.
                                    newRoom.RotateRoom(Random.Range(-4, 4));
                                }
                               // Else if we have the north door...
                               else if(hasNorth == true)
                                {
                                    // If we have the east and west door as well...
                                    if(hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between 1 - 3 rotations.
                                        newRoom.RotateRoom(Random.Range(1, 3));
                                    }
                                    // If we have the east and south door as well...
                                    else if (hasEast == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between 0 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 2));
                                    }
                                    // If we have the south and west door as well...
                                    else if (hasWest == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between 0 - -2 rotations.
                                        newRoom.RotateRoom(Random.Range(0, -2));
                                    }
                                    // If we just have the east door as well...
                                    else if(hasEast == true)
                                    {
                                        // Rotate the room in a random way between 1 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(1, 2));
                                    }
                                    // If we just have the west door as well...
                                    else if(hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 - -2 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, -2));
                                    }
                                    // If we just have the north door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(2);
                                    }
                                }
                                // Else if we have the south door...
                                else if (hasSouth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, 1));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between 0 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 1));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between 0 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 1));
                                    }
                                    // If we just have the south door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(0);
                                    }
                                }
                               // Else if have the East door...
                               else if(hasEast == true)
                                {
                                    int[] possibleRotations = new int[2] { -1 , 1};
                                    // If we have the west door as well...
                                    if(hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 or 1 rotations.
                                        newRoom.RotateRoom(possibleRotations[Random.Range(0,possibleRotations.Length)]);
                                    }
                                    // If not, then it's just the east door, so...
                                    else
                                    {
                                        // Rotate the room to match the door to connect to.
                                        newRoom.RotateRoom(1);
                                    }
                                }
                               // Else if we have a West door...
                               else if(hasWest == true)
                                {
                                    // Rotate the room to match the door to connect to.
                                    newRoom.RotateRoom(-1);
                                }
                                // If we're at this point, we have no doors...
                                else
                                {
                                    Debug.LogError("Error: " + newRoom.name + " has no doors to connect to.");
                                    break;
                                }
                                Instantiate(hallwayPrefab, hallwayPosition, Quaternion.identity);
                                currentDoor.isConnectToAnotherRoom = true;
                                newRoom.GetDoor(Door.Direction.South).isConnectToAnotherRoom = true;
                            }
                            // If our current door is the East one...
                            else if (currentDoorDirection == Door.Direction.East)
                            {
                                // If it has all 4 doors...
                                if (hasNorth == true && hasSouth == true && hasEast == true && hasWest == true)
                                {
                                    // Rotate the room in a random way.
                                    newRoom.RotateRoom(Random.Range(-4, 4));
                                }
                                // Else if we have the north door...
                                else if (hasNorth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between -2 - 0 rotations.
                                        newRoom.RotateRoom(Random.Range(-2,0));
                                    }
                                    // If we have the east and south door as well...
                                    else if (hasEast == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between 0 - 3 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 3));
                                    }
                                    // If we have the south and west door as well...
                                    else if (hasWest == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between -1 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, 1));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between -2 - -1 rotations.
                                        newRoom.RotateRoom(Random.Range(-2 , -1));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 - 0 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, 0));
                                    }
                                    // If we just have the south door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(-1);
                                    }
                                }
                                // Else if we have the south door...
                                else if (hasSouth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between 0 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 2));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between 1 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(1, 2));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between 0 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(0,1));
                                    }
                                    // If we just have the south door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(1);
                                    }
                                }
                                // Else if have the East door...
                                else if (hasEast == true)
                                {
                                    int[] possibleRotations = new int[2] { 0, 2 };
                                    // If we have the west door as well...
                                    if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 or 1 rotations.
                                        newRoom.RotateRoom(possibleRotations[Random.Range(0, possibleRotations.Length)]);
                                    }
                                    // If not, then it's just the east door, so...
                                    else
                                    {
                                        // Rotate the room to match the door to connect to.
                                        newRoom.RotateRoom(2);
                                    }
                                }
                                // Else if we have a West door...
                                else if (hasWest == true)
                                {
                                    // Rotate the room to match the door to connect to.
                                    newRoom.RotateRoom(0);
                                }
                                // If we're at this point, we have no doors...
                                else
                                {
                                    Debug.LogError("Error: " + newRoom.name + " has no doors to connect to.");
                                    break;
                                }
                                Instantiate(hallwayPrefab, hallwayPosition, Quaternion.Euler(0, 90, 0));
                                currentDoor.isConnectToAnotherRoom = true;
                                newRoom.GetDoor(Door.Direction.West).isConnectToAnotherRoom = true;
                            }
                            // If our current door is the south one...
                            else if (currentDoorDirection == Door.Direction.South)
                            {
                                // If it has all 4 doors...
                                if (hasNorth == true && hasSouth == true && hasEast == true && hasWest == true)
                                {
                                    // Rotate the room in a random way.
                                    newRoom.RotateRoom(Random.Range(-4, 4));
                                }
                                // Else if we have the north door...
                                else if (hasNorth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, 1));
                                    }
                                    // If we have the east and south door as well...
                                    else if (hasEast == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between -2 - 0 rotations.
                                        newRoom.RotateRoom(Random.Range(-2,0));
                                    }
                                    // If we have the south and west door as well...
                                    else if (hasWest == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between 0 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 2));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between 0 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(0 , 1));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 - 0 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, -0));
                                    }
                                    // If we just have the north door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(0);
                                    }
                                }
                                // Else if we have the south door...
                                else if (hasSouth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between 1 - 3 rotations.
                                        newRoom.RotateRoom(Random.Range(1, 3));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between -2 - -1 rotations.
                                        newRoom.RotateRoom(Random.Range(-2, -1));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between 1 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(1,2));
                                    }
                                    // If we just have the south door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(2);
                                    }
                                }
                                // Else if have the East door...
                                else if (hasEast == true)
                                {
                                    int[] possibleRotations = new int[2] { -1, 1 };
                                    // If we have the west door as well...
                                    if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 or 1 rotations.
                                        newRoom.RotateRoom(possibleRotations[Random.Range(0, possibleRotations.Length)]);
                                    }
                                    // If not, then it's just the east door, so...
                                    else
                                    {
                                        // Rotate the room to match the door to connect to.
                                        newRoom.RotateRoom(-1);
                                    }
                                }
                                // Else if we have a West door...
                                else if (hasWest == true)
                                {
                                    // Rotate the room to match the door to connect to.
                                    newRoom.RotateRoom(1);
                                }
                                // If we're at this point, we have no doors...
                                else
                                {
                                    Debug.LogError("Error: " + newRoom.name + " has no doors to connect to.");
                                    break;
                                }
                                Instantiate(hallwayPrefab, hallwayPosition, Quaternion.identity);
                                currentDoor.isConnectToAnotherRoom = true;
                                newRoom.GetDoor(Door.Direction.North).isConnectToAnotherRoom = true;
                            }
                            // If our current door is the west one...
                            else if (currentDoorDirection == Door.Direction.West)
                            {
                                // If it has all 4 doors...
                                if (hasNorth == true && hasSouth == true && hasEast == true && hasWest == true)
                                {
                                    // Rotate the room in a random way.
                                    newRoom.RotateRoom(Random.Range(-4, 4));
                                }
                                // Else if we have the north door...
                                else if (hasNorth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between 0 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(0,2));
                                    }
                                    // If we have the east and south door as well...
                                    else if (hasEast == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between -1 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, 1));
                                    }
                                    // If we have the south and west door as well...
                                    else if (hasWest == true && hasSouth == true)
                                    {
                                        // Rotate the room in a random way between 1 - 3 rotations.
                                        newRoom.RotateRoom(Random.Range(1, 3));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between 0 - 1 rotations.
                                        newRoom.RotateRoom(Random.Range(0, 1));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between 1 - 2 rotations.
                                        newRoom.RotateRoom(Random.Range(1, 2));
                                    }
                                    // If we just have the north door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(1);
                                    }
                                }
                                // Else if we have the south door...
                                else if (hasSouth == true)
                                {
                                    // If we have the east and west door as well...
                                    if (hasEast == true && hasWest == true)
                                    {
                                        // Rotate the room in a random way between -2 - 0 rotations.
                                        newRoom.RotateRoom(Random.Range(-2, 0));
                                    }
                                    // If we just have the east door as well...
                                    else if (hasEast == true)
                                    {
                                        // Rotate the room in a random way between -1 - 0 rotations.
                                        newRoom.RotateRoom(Random.Range(-1, 0));
                                    }
                                    // If we just have the west door as well...
                                    else if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between 1 -2 rotations.
                                        newRoom.RotateRoom(Random.Range(1,2));
                                    }
                                    // If we just have the south door...
                                    else
                                    {
                                        // Rotate to connect to the room.
                                        newRoom.RotateRoom(-1);
                                    }
                                }
                                // Else if have the East door...
                                else if (hasEast == true)
                                {
                                    int[] possibleRotations = new int[2] { 0,2 };
                                    // If we have the west door as well...
                                    if (hasWest == true)
                                    {
                                        // Rotate the room in a random way between -1 or 1 rotations.
                                        newRoom.RotateRoom(possibleRotations[Random.Range(0, possibleRotations.Length)]);
                                    }
                                    // If not, then it's just the east door, so...
                                    else
                                    {
                                        // Rotate the room to match the door to connect to.
                                        newRoom.RotateRoom(0);
                                    }
                                }
                                // Else if we have a West door...
                                else if (hasWest == true)
                                {
                                    // Rotate the room to match the door to connect to.
                                    newRoom.RotateRoom(2);
                                }
                                // If we're at this point, we have no doors...
                                else
                                {
                                    Debug.LogError("Error: " + newRoom.name + " has no doors to connect to.");
                                    break;
                                }
                                Instantiate(hallwayPrefab, hallwayPosition, Quaternion.Euler(0, 90, 0));
                                currentDoor.isConnectToAnotherRoom = true;
                                newRoom.GetDoor(Door.Direction.East).isConnectToAnotherRoom = true;
                            }
                            roomsMade += 1;
                        }
                    }

                }

            }
            Debug.Log("Room " + i + " setup and made!");
            yield return new WaitForSeconds(1f);
        }

    }

    /// <summary>
    /// Returns a room via the random weight system. If all the weights are the same or equal to each other, than it'll be true random.
    /// </summary>
    /// <param name="randomWeight">The random weight</param>
    /// <returns>The room within the corressponding range.</returns>
    private Room WeightedRandomRoom(float totalWeight)
    {
        if (totalWeight == 0 || (totalWeight / roomPrefabs.Length) == 1)
            return roomPrefabs[Random.Range(0, roomPrefabs.Length)].room;

        float randomWeight = Random.Range(0, totalWeight);
        float sumWeight = 0;
        foreach(WeightedRoom weightedRoom in roomPrefabs)
        {
            sumWeight += weightedRoom.weight;
            if (sumWeight >= randomWeight)
            {
                return weightedRoom.room;
            }
           
        }
        return null;
    }

    /// <summary>
    /// Trys to find the room at the specified location within the grid.
    /// </summary>
    /// <param name="location">The location of the room to be found.</param>
    /// <returns>Returns the room if found. If not, it'll return null.</returns>
    private Room FindRoomAtLocation(Vector2 location)
    {
        int i = 0;
        // Search the rooms to until we have one at the specified location or hit null.
        while(dungeonRooms[i] != null && i < dungeonRooms.Length)
        {
            if (dungeonRooms[i].roomPosition == location)
            {
                return dungeonRooms[i];
            }
            else
                i++;
        }
        // If we don't have any rooms at that location, return null.
        return null;
    }

    [System.Serializable]
    private struct WeightedRoom
    {
        public Room room;
        [Range(0,Mathf.Infinity)]
        public float weight;
    }
    

    //Sort the rooms list by using the swap function
    void SortRoomsByWeight()
    {
        int i;
        for (i = 0; i < roomPrefabs.Length - 1; i++)
        {
            //If the current room's weight is greater than the next one in the list, swap them
            if (roomPrefabs[i].weight > roomPrefabs[i + 1].weight)
            {
                swap(i, (i + 1));
            }
        }

        /// <summary>
        /// Swap rooms in the list to organize them
        /// </summary>
        /// <param name="index1">First value to swap with</param>
        /// <param name="index2">Second value to swap with</param>
        void swap(int index1, int index2)
        {
            WeightedRoom temp = roomPrefabs[index1];
            roomPrefabs[index1] = roomPrefabs[index2];
            roomPrefabs[index2] = temp;
        }
    }
}


