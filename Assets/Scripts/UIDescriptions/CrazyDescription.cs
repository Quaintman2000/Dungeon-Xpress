using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


[ExecuteInEditMode()]
public class CrazyDescription : MonoBehaviour
{
    public TextMeshProUGUI descriptionField;
    public LayoutElement layoutElement;

    public int characterWrapLimit;
    public void SetText( string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            descriptionField.gameObject.SetActive(false);
        }
        else
        {
            descriptionField.gameObject.SetActive(true);
            descriptionField.text = header;
        }
      
        if (Application.isEditor)
        {
            int headerLength = descriptionField.text.Length;
           
            layoutElement.enabled = (headerLength > characterWrapLimit) ? true : false;
        }

    }
}
