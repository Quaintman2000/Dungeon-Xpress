using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFollow : MonoBehaviour
{
    public GameObject followPlayer;
    public Vector3 offset;

    public void Start()
    {
        offset = transform.position - followPlayer.transform.position;
    }
    public void LateUpdate()
    {
        transform.position = followPlayer.transform.position + offset;
    }
}
