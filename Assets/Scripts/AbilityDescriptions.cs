using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


[ExecuteInEditMode()]
public class AbilityDescriptions : MonoBehaviour
{
    public TextMeshProUGUI titleField;
    public TextMeshProUGUI rangeField;
    public TextMeshProUGUI apField;
    public TextMeshProUGUI descriptionField;

    public LayoutElement mylayoutElement;

    public int textWrapLimit;

    public void SetText(string title, string range, string ap, string description = "")
    {
        if (string.IsNullOrEmpty(title))
        {
            titleField.gameObject.SetActive(false);
        }
        else
        {
            titleField.gameObject.gameObject.SetActive(true);
            titleField.text = title;
        }
        rangeField.text = range;
        apField.text = ap;
        descriptionField.text = description;
        
        if (Application.isEditor)
        {
            int titleLength = titleField.text.Length;
            int rangeLength = rangeField.text.Length;
            int apLength = apField.text.Length;
            int descriptionLength = descriptionField.text.Length;


            mylayoutElement.enabled = (titleLength > textWrapLimit || rangeLength > textWrapLimit || rangeLength > textWrapLimit
                || apLength > textWrapLimit || descriptionLength > textWrapLimit) ? true : false;
        }

    }

}
