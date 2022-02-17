using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavMesh : MonoBehaviour
{
    // Marker gameobject to represent where we are moving to.
    [SerializeField] GameObject moveToMarker;
    [SerializeField] Text noGoText;
    [SerializeField] LineRenderer navPathLineRend;

    LineRenderer currentPathRenderer;
    // NavMeshAgent for pathfind.
    NavMeshAgent navMeshAgent;
    // Keeps track of the current spawnMarker.
    GameObject spawnedMarker;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // On right click...
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // If there is a spawn marker.
            if (spawnedMarker != null)
                Destroy(spawnedMarker);

            // Move to clicked position.
            MoveToClickPoint();
        }
    }

    public void MoveToClickPoint()
    {

        //Cast a ray from our camera toward the plane, through our mouse cursor
        float distance;
        // Hit info from the raycast.
        RaycastHit hit;
        // Makes the raycast from our mouseposition to the ground.
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Sends the raycast of to infinity until hits something.
        Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

        // Grab the distance of the position we hit to get the point along the ray.
        distance = hit.distance;

        //Find where that ray hits the plane
        Vector3 raycastPoint = cameraRay.GetPoint(distance);

        NavMeshHit navHit;
        if(!NavMesh.SamplePosition(raycastPoint, out navHit, 1f, NavMesh.AllAreas))
        {
            StartCoroutine(NoGoTextDisplay());
            return;
        }

        currentPathRenderer = Instantiate<LineRenderer>(navPathLineRend);
        currentPathRenderer.positionCount = navMeshAgent.path.corners.Length + 1;
        currentPathRenderer.SetPositions(navMeshAgent.path.corners);
        // Spawn the move to marker at the raycast point.
        spawnedMarker = Instantiate<GameObject>(moveToMarker, raycastPoint + Vector3.up, moveToMarker.transform.rotation);

        //Tell the NavMesh to go to the raycast point
        navMeshAgent.destination = raycastPoint;

        // Start moving courtine to make sure we delete the marker when we're done.
        StartCoroutine(Moving(raycastPoint));
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
        while(timer > 0)
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

        // While we're not at the at the move position...
        while (transform.position != movePosition)
        {
            // Set the position count of the renderer equal to the length of the cornors array.
            currentPathRenderer.positionCount = navMeshAgent.path.corners.Length;
            // Set the line position points to the corners.
            currentPathRenderer.SetPositions(navMeshAgent.path.corners);
          
            // Return null.
            yield return null;
        }
        // Destroy the marker once we've reached the position.
        Destroy(spawnedMarker);

        // Destroy the path once we've reached our position.
        Destroy(currentPathRenderer.gameObject);
    }
}
