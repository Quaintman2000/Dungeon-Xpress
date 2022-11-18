using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleController : MonoBehaviour
{
    [SerializeField] Transform _magicCircleTransform;

    public void ActivateMagicCircle(Vector3 newScale)
    {
        _magicCircleTransform.gameObject.SetActive(true);

        if (_magicCircleTransform != null)
            _magicCircleTransform.localScale = newScale;
        else
            Debug.LogError("No magic circle to scale!");
    }

    public void HideMagicCircle()
    {
        _magicCircleTransform.gameObject.SetActive(false);
    }
}
