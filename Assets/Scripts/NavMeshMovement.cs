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

    public Action<bool> WalkingAction;

    public Vector3 WarpPosition { set; private get; }

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if(TryGetComponent<CharacterController>(out CharacterController characterController))
        {
            // Stop the character from moving if we die.
            characterController.OnDeadStateEnter += Stop;
            if(!(characterController is PlayerController))
            {
                characterController.FreeMoveToPointAction = Move;
            }
        }
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
        WalkingAction?.Invoke(true);
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
        WalkingAction.Invoke(true);
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
        WalkingAction?.Invoke(false);
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
        WalkingAction?.Invoke(false);
        Stop();
    }
    public virtual void Stop()
    {
       
        isMoving = false;
        navMeshAgent.ResetPath();
    }
    public void Teleport(Vector3 newPosition)
    {
        Debug.Log("Teleported");
        navMeshAgent.Warp(newPosition);
    }

    public void Warp()
    {
        if(WarpPosition != null)
        {
            navMeshAgent.Warp(WarpPosition);
            WarpPosition = transform.position;
        }
    }
    //Used to start the leap
    public void StartLeap(Vector3 stop, float time)
    {
        Vector3 lookDirection = new Vector3(stop.x, this.transform.position.y, stop.z);
        this.transform.LookAt(lookDirection);//Looks where they are going to jump to
        StartCoroutine(Leap(stop, time));
    }
    /// <summary>
    /// The dynamic leap used for the player to leap from one position to another using a
    /// trajectory curve
    /// Uses three points with the mid point being the 3rd point to calc the curve for
    /// https://youtu.be/RF04Fi9OCPc
    /// First Half of the Video helps understand how the calculation is done
    /// </summary>
    /// <param name="stop">The stop point of the leap</param>
    /// <param name="time">The time to leap</param>
    /// <returns></returns>
    IEnumerator Leap( Vector3 stop, float time)
    {
        float counter = 0;
        Vector3 start = transform.position;


        // The mid point between the start and stop
        Vector3 midpoint = start + ((start - stop) / 2f);
        //The taller of both points and adds 10f to make the point slightly higher for the curve
        float highestY = Mathf.Max(start.y, stop.y) + 10f;

        //The height of the curve in which they jump to get to which is also in the middle of both
        Vector3 highestSpot = new Vector3(midpoint.x, highestY, midpoint.z);

        Vector3 p0;
        Vector3 p1;
        float percent;
        //Calculates where it is on the curve
        while (counter <= time)
        {
            counter += Time.deltaTime;//Runs each frame
            percent = counter / time;//How far in the animation from 0-1

            p0 = Vector3.Lerp(start, stop, percent);
            p1 = Vector3.Lerp(highestSpot, stop, percent);

            transform.position = Vector3.Lerp(p0, p1, percent);
            yield return null;
        }

        InstantMove(stop);
    }
    /// <summary>
    /// Used to instantly move the player to the raycast point
    /// </summary>
    public void InstantMove(Vector3 raycastPoint)
    {
        Teleport(raycastPoint);
    }
}
