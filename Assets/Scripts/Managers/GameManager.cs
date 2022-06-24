using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public CombatController playerData;

    public DoorTrigger CurrentDoor;
    private List<DoorTrigger> doors;
    private int TotalDoors;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    public void StartMatch()
    {

    }
    public int AddDoor(DoorTrigger door)
    {
        doors.Add(door);

        TotalDoors++;
        return TotalDoors;
    }
    public void OpenDoor(PlayerController player)
    {
        if (CurrentDoor != null)
        {
            CurrentDoor.OnDoorEnter(player);
        }
    }
}
