using UnityEngine;
using System.Collections;

public class ObstructionDetector : MonoBehaviour 
{
	//makes a transform for the player
	public Transform 	playerTrans;
	// refernces the WallTransScript and gives a shorthand name
	private WallTrans		m_Wall;
	
	void Start () 
	{
		StartCoroutine(DetectPlayerObstructions());
	}
	
	//creates a coroutine to detect player obstructions
	IEnumerator DetectPlayerObstructions()
	{
		//while this coroutine is running
		while(true)
		{
			//wait for .5 seconds
			yield return new WaitForSeconds(0.5f);
			// the direction of the player tranform postion and the camera postion is where the raycast will be directed
			Vector3 direction = (playerTrans.position - Camera.main.transform.position).normalized;
			RaycastHit rayCastHit;
			
			//** if raycast coming from the camera postion is hit by an obstruction
			if(Physics.Raycast(Camera.main.transform.position, direction, out rayCastHit, Mathf.Infinity))
			{
				//refernces the WallTransScript
				// if raycast hits a gameobject it will get the trans script
				WallTrans walltrans = rayCastHit.collider.gameObject.GetComponent<WallTrans>();

				//if the trans script is retrieved 
				if(walltrans)
				{
					//then it will set the gameobject transparent
					walltrans.SetTransparent();
					//**sets the m_wall and trans to be the same
					m_Wall = walltrans;
				}
				else 
				//else if matScript is retrieved
				{
					if (m_Wall)
					{
						//it will set the gameobject material to its normal shade
						m_Wall.SetToNormal();
						m_Wall = null;
					}
				}
			}
			
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