using UnityEngine;
using System.Collections;

public class ObstructionDetector : MonoBehaviour
{
    //makes float for max gizmo raycast distance
    private float m_MaxDistance;

    //makes a transform for the player
    public Transform playerTrans;

    //refernces the WallTransScript and gives a shorthand name
    private WallTrans m_Wall;

    //makes an array for the Game object's children that the Walltrans script is attached to. 
    WallTrans[] childWallTrans;

    //creates recognizable string name for the tag "Wall"
    private readonly string WallTag = "Wall";

    //creates recognizable string name for the tag "WallParent"
    private readonly string WallparentTag = "WallParent";



    void Start()
    {
        StartCoroutine(DetectPlayerObstructions());

        //sets gizmo raycast max distance
        m_MaxDistance = 50.0f;

    }


    //creates a coroutine to detect player obstructions
    IEnumerator DetectPlayerObstructions()
    {
        //while this coroutine is running
        while (true)
        {
            //wait for .5 seconds
            yield return new WaitForSeconds(0.5f);
            // the direction of the player tranform postion and the camera postion is where the raycast will be directed
            Vector3 direction = (playerTrans.position - Camera.main.transform.position).normalized;
            RaycastHit raycastHit;


            //if raycast coming from the camera postion is hit by an obstruction
            if (Physics.Raycast(Camera.main.transform.position, direction, out raycastHit, Mathf.Infinity))

            {
                //checks if the item hit has the Walltrans script
                WallTrans walltrans = raycastHit.collider.gameObject.GetComponent<WallTrans>();
                //checks if the item hit is tagged as a "Wall"
                if (raycastHit.collider.transform.CompareTag("Wall"))
                {
                    //searches for the "Wallparent" tag
                    Transform wallParent = raycastHit.collider.transform.parent;
                    //while the tag of the hit item is not a wallparent it will continue to search for one
                    while (!wallParent.CompareTag(WallparentTag))
                    {
                        wallParent = wallParent.parent;
                    }
                    // if the wallParent tag is found
                    if (wallParent.CompareTag(WallparentTag))
                    {
                        //walltrans gets a shorthand name
                        m_Wall = walltrans;


                        if (childWallTrans != null)
                        {
                            //if the items in the childWallTrans Array do not have a wallparent then set the items in the array to normal/non-transparent
                            if (childWallTrans != wallParent.GetComponentsInChildren<WallTrans>() && childWallTrans.Length > 0)
                            {
                                //each item/wall that is a child in the Array will be set to normal
                                foreach (WallTrans child in childWallTrans)
                                {

                                    child.SetToNormal();
                                }
                            }
                        }
                        //if the items in the childWallTrans Array have a wallparent and are hit by the raycast then set them to transparent
                        childWallTrans = wallParent.GetComponentsInChildren<WallTrans>();

                        //each items/wall that is a child in the Array will be set to transparent.
                        foreach (WallTrans child in childWallTrans)
                        {
                            child.SetTransparent();
                        }
                    }




                }
                else
                {
                    //else if the Array has childlren and the raycast has stopped hitting them then all childlren will be set back to normal.
                    if (childWallTrans != null)
                    {
                        if (childWallTrans.Length > 0)
                        {
                            foreach (WallTrans child in childWallTrans)
                            {
                                child.SetToNormal();
                            }
                        }
                    }

                }

            }

        }
    }

    void OnDrawGizmos()
    {
        //draws a red visual raycast from the camera to the player
        Gizmos.color = Color.red;
        Vector3 direction = (playerTrans.position - Camera.main.transform.position).normalized;
        RaycastHit raycastHit;

        //Checks if there has been a hit yet
        if (Physics.Raycast(Camera.main.transform.position, direction, out raycastHit, Mathf.Infinity))
        {
            //Draws a Ray forward from the camera toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * raycastHit.distance);

        }
        //If there hasn't been a hit yet, draws the ray at the maximum distance
        else
        {
            //Draw a Ray forward from the camera toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistance);

        }
    }

    //creates the start raycast coroutine
    public void StartRayCast()
    {
        StopCoroutine("DetectPlayerObstructions");
        StartCoroutine(DetectPlayerObstructions());
    }

    //creates the stopraycast coroutine function
    public void StopRayCast()
    {
        StopCoroutine("DetectPlayerObstructions");
    }


}