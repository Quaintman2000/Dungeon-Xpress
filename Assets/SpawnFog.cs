using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFog : MonoBehaviour
{
    [SerializeField] private GameObject fog;
    [SerializeField] private Vector3 offSet;
    private void Awake()
    {
      Instantiate<GameObject>(fog, this.transform.position + offSet, this.transform.rotation);
    }
}
