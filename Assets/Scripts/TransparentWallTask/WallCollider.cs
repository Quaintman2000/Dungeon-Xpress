using UnityEngine;
using System.Collections;

public class WallCollider : MonoBehaviour
{
	//makes the reference to the Obstruction Detector script and gives a shorthand name
	public ObstructionDetector obstructionDetec;

	void OnTriggerEnter(Collider other)
	{
		//if the item under the player tag enters the collider
		if (other.tag == "Player")
		{
			//console writes *enter*
			Debug.Log("Enter");
			//then calls from the obstruction script to start the raycast
			obstructionDetec.StartRayCast();
		}
	}

	void OnTriggerExit(Collider other)
	{
		//if the player exits the collider 
		if (other.tag == "Player")
		{
			//console write exit 
			Debug.Log("Exit");
			//stops the raycast referneced in the obstruction script
			obstructionDetec.StopRayCast();
		}
	}
}