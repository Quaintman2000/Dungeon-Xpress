using UnityEngine;
using System.Collections;

public class WallTrans : MonoBehaviour
{
	//makes material variable for the initial object color
	private Material m_initialMaterial;

	[SerializeField]
	//makes a private variable for the renderer
	private Renderer renderer;
	[SerializeField]
	//makes a private variable for the new transparent material
	private Material transparentMaterial;

	void Start()
	{
		//sets the renderer to the component's(wall's) Renderer
		renderer = GetComponent<Renderer>();


		// On start this sets the item's material to its inital color
		m_initialMaterial = GetComponent<Renderer>().material;
	}

	public void SetTransparent()
	{
		//this is incharge of swapping the items material to a new tranparent material
		GetComponent<Renderer>().material = transparentMaterial;
	}

	public void SetToNormal()
	{
		// this is in charge of swapping the items material back to the original.
		GetComponent<Renderer>().material= m_initialMaterial;
	}
}