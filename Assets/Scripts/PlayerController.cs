using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerNavMesh))]
public class PlayerController : MonoBehaviour
{
    // AI pathing variable.
    PlayerNavMesh playerNav;
    // Start is called before the first frame update
    void Start()
    {
        // Grab our pathing component.
        playerNav = GetComponent<PlayerNavMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        // If we right click...
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            // Set the pathing to start.
            playerNav.SetMoveToMarker();
        }
    }
}
