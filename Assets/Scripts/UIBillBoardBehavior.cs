using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillBoardBehavior : MonoBehaviour
{
    private void FixedUpdate()
    {
        // Look at the main camera at all times.
        transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
}
