using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
   // public Transform SpawnFog;
    public GameObject fogOfWarPlane;
    public Transform playerOne;
    public LayerMask fogOfWarLayer;
    public float exposedRadius = 5f;
    private float exposedRadiusSqr { get { return exposedRadius * exposedRadius; } }
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] color;

    public void Start()
    {
        //fogOfWarPlane = GameObject.Find("Fog(Clone)").GetComponent<GameObject>();
        InitializeFog();
       

    }
    public void Update()
    {
        
        UpdateFog();    
    }

    public void UpdateFog()
    {
        Ray r = new Ray(transform.position, playerOne.position - transform.position);
     
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 1000, fogOfWarLayer, QueryTriggerInteraction.Collide))
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                //vertices[i] = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                // Vector3 v = vertices[i];
                float dist = Vector3.SqrMagnitude(v - hit.point);
                if (dist < exposedRadiusSqr)
                {
                    float alpha = Mathf.Min(color[i].a, dist / exposedRadiusSqr);
                    color[i].a = alpha;

                }
                if (dist > exposedRadiusSqr)
                {
                    //color[i] = Color.black;
                }
            }
            UpdateColor();
        }
    }
    void InitializeFog()
    {
        mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        color = new Color[vertices.Length];
        for(int i=0; i < color.Length; i++)
        {
            color[i] = Color.black;
        }
        UpdateColor();
    }
    void UpdateColor()
    {
        mesh.colors = color;
    }
}
