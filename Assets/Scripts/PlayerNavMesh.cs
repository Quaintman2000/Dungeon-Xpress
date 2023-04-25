using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavMesh : NavMeshMovement
{
    public List<Vector3> corners;

    // Marker gameobject to represent where we are moving to.
    [SerializeField] GameObject moveToMarker;
    [SerializeField] LineRenderer navPathLineRend;

    //LineRender to draw with
    [SerializeField] LineRenderer currentPathRenderer;
    // NavMeshAgent for pathfind.
    [SerializeField] GameObject spawnedMarker;
    //Draws the line renderer path and sets the position of the marker to the destination

    protected override void Awake()
    {
        base.Awake();
        
    }
    private void DrawPath(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(target, path);

        if (currentPathRenderer == null)
            currentPathRenderer = Instantiate(navPathLineRend);
        if (spawnedMarker == null)
            spawnedMarker = Instantiate(moveToMarker);
        //Sets how many positions there are in the path rendering and sets the positions of the vertices
        currentPathRenderer.positionCount = navMeshAgent.path.corners.Length;
        currentPathRenderer.SetPositions(navMeshAgent.path.corners);


        //Sets the spawned marker at the position of the target
        spawnedMarker.transform.position = target + moveToMarker.transform.position;
        spawnedMarker.transform.rotation = moveToMarker.transform.rotation;

        
        currentPathRenderer.gameObject.SetActive(true);
        spawnedMarker.gameObject.SetActive(true);
    }
   
    public bool AttemptMove(Vector3 raycastPoint)
    {
        Debug.Log("This player is on a navmesh: " + navMeshAgent.isOnNavMesh);

        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(raycastPoint, out navHit, 1f, NavMesh.AllAreas))
        {
            return false;
        }
        //Draws a path for the character
        DrawPath(navHit.position);

        // Move to clicked position.
        MoveToClickPoint(raycastPoint);

        return true;
    }
    void MoveToClickPoint(Vector3 raycastPoint)
    {
        //Tell the NavMesh to go to the raycast point
        navMeshAgent.destination = raycastPoint;

        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
        }
        // Start moving courtine to make sure we delete the marker when we're done.
        movingCoroutine = StartCoroutine(Moving(raycastPoint));
    }

    //We make a new path because most times the navMeshAgent has no path yet
    public override float GetDistance(Vector3 raycastPoint)
    {
        float distance;

        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(raycastPoint, path);

        distance = 0;
        corners.Clear();
        for (int i = 1; i < path.corners.Length; i++)
        {
            corners.Add(path.corners[i]);
            float displacement = (Vector3.Distance(path.corners[i], path.corners[i - 1]));
            distance += displacement;
        }
        return distance;
    }

    public override void AttackMove(Vector3 position, float closeEnough)
    {
        WalkingAction?.Invoke(true);
        DrawPath(position);
        //Tell the NavMesh to go to the raycast point
        navMeshAgent.destination = position;

        if(movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
        }
        // Start moving coroutine to make sure we delete the marker when we're done.
        movingCoroutine = StartCoroutine(Moving(position, closeEnough));
    }

    IEnumerator Moving(Vector3 movePosition)
    {
        isMoving = true;

        // While we're not at the at the move position...
        while (GetDistance(movePosition) > 0.2f)
        {
            // Set the position count of the renderer equal to the length of the cornors array.
            currentPathRenderer.positionCount = navMeshAgent.path.corners.Length;
            // Set the line position points to the corners.
            currentPathRenderer.SetPositions(navMeshAgent.path.corners);

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
            // Set the position count of the renderer equal to the length of the cornors array.
            currentPathRenderer.positionCount = navMeshAgent.path.corners.Length;
            // Set the line position points to the corners.
            currentPathRenderer.SetPositions(navMeshAgent.path.corners);

            // Return null.
            yield return null;
        }
        WalkingAction?.Invoke(false);
        Stop();
    }
    public override void Stop()
    {
        base.Stop();

        currentPathRenderer.gameObject.SetActive(false);
        spawnedMarker.gameObject.SetActive(false);
    }
}
