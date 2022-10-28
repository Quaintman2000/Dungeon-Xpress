using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleController : MonoBehaviour
{
    [SerializeField] Transform _magicCircleTransform;

    public void SetMagicCircleScale(Vector3 newScale)
    {
        if (_magicCircleTransform != null)
            _magicCircleTransform.localScale = newScale;
        else
            Debug.LogError("No magic circle to scale!");
    }
}
