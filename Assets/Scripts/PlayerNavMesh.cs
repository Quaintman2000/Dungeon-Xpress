using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    [SerializeField] Transform movePosition;
    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            MoveToClickPoint();
        }
    }

    public void MoveToClickPoint()
    {
        //Create a plane object (a mathematical representation of all the points in 2D)
        /*Plane groundPlane;

        //Set that plane so it is the X,Z plane the plaer is standing on
        groundPlane = new Plane(Vector3.up, transform.position);

        //Cast a ray from our camera toward the plane, through our mouse cursor
        float distance;*/
        RaycastHit hit;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //groundPlane.Raycast(cameraRay, out distance);

        //Find where that ray hits the plane
        //Vector3 raycastPoint = cameraRay.GetPoint(distance);
        
        if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity))
        {
            navMeshAgent.destination = hit.transform.position;
        }

        //Tell the NavMesh to go to the raycast point
        //navMeshAgent.destination = raycastPoint;
    }
}
