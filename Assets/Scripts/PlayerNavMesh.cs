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
    [SerializeField] Text noGoText;
    [SerializeField] LineRenderer navPathLineRend;

    //LineRender to draw with
    LineRenderer currentPathRenderer;
    // NavMeshAgent for pathfind.
    GameObject spawnedMarker;
    //Draws the line renderer path and sets the position of the marker to the destination
    private void DrawPath(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(target, path);

        //Sets how many positions there are in the path rendering and sets the positions of the vertices
        currentPathRenderer.positionCount = navMeshAgent.path.corners.Length;
        currentPathRenderer.SetPositions(navMeshAgent.path.corners);


        //Sets the spawned marker at the position of the target
        spawnedMarker.transform.position = target + moveToMarker.transform.position;
        spawnedMarker.transform.rotation = moveToMarker.transform.rotation;

        
        currentPathRenderer.gameObject.SetActive(true);
        spawnedMarker.gameObject.SetActive(true);
    }
    public void SetMoveToMarker(Vector3 raycastPoint)
    {
        // If there is a spawn marker.
        if (spawnedMarker != null)
            Destroy(spawnedMarker);
        if (currentPathRenderer != null)
            Destroy(currentPathRenderer.gameObject);
        currentPathRenderer = Instantiate(navPathLineRend);
        spawnedMarker = Instantiate(moveToMarker);
        // Move to clicked position.
        MoveToClickPoint(raycastPoint);
    }

    public void MoveToClickPoint(Vector3 raycastPoint)
    {
        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(raycastPoint, out navHit, 1f, NavMesh.AllAreas))
        {
            StartCoroutine(NoGoTextDisplay());
            return;
        }
        //Draws a path for the character
        DrawPath(navHit.position);
        
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


    IEnumerator NoGoTextDisplay()
    {
        // Set the timer for the text fade away.
        float timer = 1.5f;
        // Set the text to be active.
        noGoText.gameObject.SetActive(true);
        // Set the aplha to max.
        Color textColor = new Color(noGoText.color.r, noGoText.color.g, noGoText.color.b, 1f);
        noGoText.color = textColor;
        // Timer...
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // Reduce the alpha of the text color over time.
            textColor.a -= Time.deltaTime;
            noGoText.color = textColor;
            yield return null;
        }
        // Once the timer is over, deactivate the text.
        noGoText.gameObject.SetActive(false);
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
        Stop();
    }
    public override void Stop()
    {
        base.Stop();

        currentPathRenderer.gameObject.SetActive(false);
        spawnedMarker.gameObject.SetActive(false);
    }
}
