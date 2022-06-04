using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavMesh : MonoBehaviour
{
    public List<Vector3> corners;
    // Marker gameobject to represent where we are moving to.
    [SerializeField] GameObject moveToMarker;
    [SerializeField] Text noGoText;
    [SerializeField] LineRenderer navPathLineRend;

    //LineRender to draw with
    LineRenderer currentPathRenderer;
    // NavMeshAgent for pathfind.
    public NavMeshAgent navMeshAgent;
    // Keeps track of the current spawnMarker.
    GameObject spawnedMarker;



    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetMoveToMarker(Vector3 raycastPoint)
    {
        // If there is a spawn marker.
        if (spawnedMarker != null)
            Destroy(spawnedMarker);
        if (currentPathRenderer != null)
            Destroy(currentPathRenderer.gameObject);
        // Move to clicked position.
        MoveToClickPoint(raycastPoint);
    }

    private void MoveToClickPoint(Vector3 raycastPoint)
    {

        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(raycastPoint, out navHit, 1f, NavMesh.AllAreas))
        {
            StartCoroutine(NoGoTextDisplay());
            return;
        }

        currentPathRenderer = Instantiate<LineRenderer>(navPathLineRend);
        currentPathRenderer.positionCount = navMeshAgent.path.corners.Length + 1;
        currentPathRenderer.SetPositions(navMeshAgent.path.corners);
        // Spawn the move to marker at the raycast point.
        spawnedMarker = Instantiate<GameObject>(moveToMarker, raycastPoint + moveToMarker.transform.position, moveToMarker.transform.rotation);

        //Tell the NavMesh to go to the raycast point
        navMeshAgent.destination = raycastPoint;






        // Start moving courtine to make sure we delete the marker when we're done.
        StartCoroutine(Moving(raycastPoint));
    }

    public float GetDistance(Vector3 raycastPoint)
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

    public void AttackMove(Vector3 position, float closeEnough)
    {
        // If there is a spawn marker.
        if (spawnedMarker != null)
            Destroy(spawnedMarker);
        if (currentPathRenderer != null)
            Destroy(currentPathRenderer.gameObject);

        currentPathRenderer = Instantiate<LineRenderer>(navPathLineRend);
        currentPathRenderer.positionCount = navMeshAgent.path.corners.Length + 1;
        currentPathRenderer.SetPositions(navMeshAgent.path.corners);
        // Spawn the move to marker at the raycast point.
        spawnedMarker = Instantiate<GameObject>(moveToMarker, position + moveToMarker.transform.position, moveToMarker.transform.rotation);

        //Tell the NavMesh to go to the raycast point
        navMeshAgent.destination = position;

        // Start moving courtine to make sure we delete the marker when we're done.
        StartCoroutine(Moving(position, closeEnough));
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
        Debug.Log("GottaGo");
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
        // Destroy the marker once we've reached the position.
        if (spawnedMarker)
            Destroy(spawnedMarker);

        // Destroy the path once we've reached our position.
        if (currentPathRenderer)
            Destroy(currentPathRenderer.gameObject);
    }

    IEnumerator Moving(Vector3 movePosition, float closeEnough)
    {
        Debug.Log("Moving");
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
        // Destroy the marker once we've reached the position.
        if (spawnedMarker)
            Destroy(spawnedMarker);

        // Destroy the path once we've reached our position.
        if (currentPathRenderer)
            Destroy(currentPathRenderer.gameObject);
    }
}
