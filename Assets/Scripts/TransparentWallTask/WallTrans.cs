using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WallTrans : MonoBehaviour
{
    [SerializeField]
    // Makes a private variable for transparent material
    private Material transparentMaterial;
    // List used to contain the orignal materials
    private List<Material> originalMaterials = new List<Material>();


    public void SetTransparent(Transform parentTransform)
    {
        // Counts the children of the parent object
        int childCount = parentTransform.childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            // Used to get the children/walls of the parent
            Transform childTransform = parentTransform.GetChild(i);
            // Gets the child/wall renderer
            Renderer childRenderer = childTransform.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                // Store the original material of the wall/child
                originalMaterials.Add(childRenderer.material);

                // Changes the material to transparent
                childRenderer.material = transparentMaterial;


            }
        }

    }

    public void SetToNormal(Transform parentTransform)
    {
        // Counts the children of the parent object
        int childCount = parentTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {

            Transform childTransform = parentTransform.GetChild(i);
            Renderer childRenderer = childTransform.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                // Restore the original material of the wall/child
                childRenderer.material = originalMaterials[i];
            }
        }


    }
}



