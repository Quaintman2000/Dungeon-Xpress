using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   
    public string header;
    public string content;
    public GameObject Panel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(gameObject.activeInHierarchy == true)
        {
            ToolTipSystem.Show(content, header);
        }
       // ToolTipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.Hide();
    }
}
