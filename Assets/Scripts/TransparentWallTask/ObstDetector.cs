using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstDetector : MonoBehaviour
{
    // Makes a transform for the player
    private Transform playerTrans;
    // list to store wallparent obj
    private List<Transform> parentsList = new List<Transform>();

    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(TrackPlayerObstructions());
    }

    // Creates a coroutine to detect player obstructions
    IEnumerator TrackPlayerObstructions()
    {
        // While this coroutine is running
        while (true)
        {
            // Wait for .5 seconds
            yield return new WaitForSeconds(0.5f);

            // Get the direction from the camera to the player
            Vector3 direction = (playerTrans.position - Camera.main.transform.position).normalized;
            RaycastHit[] hits;

            // Detect all the objects hit by the raycast
            hits = Physics.RaycastAll(Camera.main.transform.position, direction, Mathf.Infinity);

            // Keep track of the closest wall hit
            float closestWallDistance = Mathf.Infinity;
            Transform closestWallTransform = null;

            // Loop through each hit object
            foreach (RaycastHit hit in hits)
            {
                // Check if the hit object has the Wall tag and is a child of a WallParent object with a WallTrans script
                if (hit.collider.CompareTag("Wall"))
                {
                    Transform wallParentTransform = hit.collider.transform.parent;
                    if (wallParentTransform != null && wallParentTransform.CompareTag("WallParent"))
                    {
                        WallTrans wallTrans = wallParentTransform.gameObject.GetComponent<WallTrans>();
                        if (wallTrans != null)
                        {
                            // Calculate the distance between the player and the hit point
                            float distance = Vector3.Distance(Camera.main.transform.position, hit.point);

                            // If this is the first wall hit by the raycast or it is closer than the other wall hit make it transparent (if the first wall hit gets turned transparent then the others in its group will be too.)
                            if (closestWallTransform == null || distance < closestWallDistance)
                            {
                                closestWallTransform = wallParentTransform;
                                closestWallDistance = distance;

                                // Turn the wall transparent
                                wallTrans.SetTransparent(wallParentTransform);
                                parentsList.Add(wallParentTransform);
                            }
                        }
                    }
                }
            }

            // Set all the other walls back to normal
            foreach (Transform parent in parentsList)
            {
                // If parent is not equal to closest wall then set it back to normal
                if (parent != closestWallTransform)
                {
                    WallTrans wallTrans = parent.gameObject.GetComponent<WallTrans>();
                    if (wallTrans != null)
                    {
                        wallTrans.SetToNormal(parent);
                    }
                }
            }

            // Remove all the parents that are not obstructing the player's view
            parentsList.RemoveAll(parent => parent != closestWallTransform);

            yield return null;
        }
    }
}
