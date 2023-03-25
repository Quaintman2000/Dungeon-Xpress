using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityDescriptionTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string title;
    private string range;
    private string ap;
    private string description;
    public void OnPointerEnter(PointerEventData eventData)
    {

        AbilityDescriptionSystem.SuperShow(title, range, ap);//, description);
        AbilityDescriptionSystem.SuperDescription(description);
      
        
     
    }
    public void SetUpDescription(string newTitle, string newRange, string newAp, string newDescription)
    {
        title = newTitle;
        range = newRange;
        ap = newAp;
        description = newDescription;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityDescriptionSystem.SuperHide();
    }
}
