using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityDescriptionTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string title;
    public string range;
    public string ap;
    public string description;
    public void OnPointerEnter(PointerEventData eventData)
    {

        AbilityDescriptionSystem.SuperShow(title, range, ap);//, description);
        AbilityDescriptionSystem.SuperDescription(description);
      
        
     
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityDescriptionSystem.SuperHide();
    }
}
