using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour
{
    public bool isMoving;

    //Checks if there is only one of its kind
    protected Coroutine movingCoroutine;
    // NavMeshAgent for pathfind.
    public NavMeshAgent navMeshAgent;

    public Action WalkingAction;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    //We make a new path because most times the navMeshAgent has no path yet
    public virtual float GetDistance(Vector3 raycastPoint)
    {
        float distance;
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(raycastPoint, path);

        distance = 0;

        //Calculates each corners distance from the last and adds them all together
        for (int i = 1; i < path.corners.Length; i++)
        {
            float displacement = (Vector3.Distance(path.corners[i], path.corners[i - 1]));
            distance += displacement;
        }

        return distance;
    }
    public void Move(Vector3 movePosition)
    {
        //Tell the NavMesh to go to the move position
        navMeshAgent.destination = movePosition;
        WalkingAction?.Invoke();
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
        }
        // Start moving courtine to make sure we reach the position
        movingCoroutine = StartCoroutine(Moving(movePosition));
    }
    public virtual void AttackMove(Vector3 position, float closeEnough)
    {
        //Tell the NavMesh to go to the raycast point
        navMeshAgent.destination = position;
        WalkingAction.Invoke();
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
        }
        // Start moving courtine to make sure we delete the marker when we're done.
        movingCoroutine = StartCoroutine(Moving(position, closeEnough));
    }
    IEnumerator Moving(Vector3 movePosition)
    {
        isMoving = true;
        // While we're not at the at the move position...
        while (GetDistance(movePosition) > 0.2f)
        {
            // Return null.
            yield return null;
        }
        Stop();
    }

    IEnumerator Moving(Vector3 movePosition, float closeEnough)
    {
        isMoving = true;
        // While we're not at the at the move position...
        while (GetDistance(movePosition) > closeEnough)
        {
            // Return null.
            yield return null;
        }
        Stop();
    }
    public virtual void Stop()
    {
        isMoving = false;
    }
}
