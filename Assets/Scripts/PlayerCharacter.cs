using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    PlayerNavMesh playerNavMesh;

    protected override void Awake()
    {
        base.Awake();

        playerNavMesh = GetComponent<PlayerNavMesh>();
    }
    public void MoveCommand(Vector3 moveToLocation)
    {
        if(!playerNavMesh.AttemptMove(moveToLocation))
        {
            // TODO, can't move there error log
            UIManager.Instance.DisplayErrorMessage("Can't go there.");
        }
    }

}
