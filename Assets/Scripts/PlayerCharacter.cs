using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCharacter : Character
{
    PlayerNavMesh playerNavMesh;

    protected override void Awake()
    {
        base.Awake();

        playerNavMesh = GetComponent<PlayerNavMesh>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            controller = PlayerController.instance.SetPlayerCharacter(this);
        }
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
