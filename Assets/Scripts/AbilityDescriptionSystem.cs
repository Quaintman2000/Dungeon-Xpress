using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDescriptionSystem : MonoBehaviour
{
    public static AbilityDescriptionSystem current;
    public AbilityDescriptions abilityDescriptions;

    private void Awake()
    {
        current = this;

    }
    public static void SuperShow(string title, string range, string ap, string description = "")
    {
        current.abilityDescriptions.SetText(title, range, ap, description);
        current.abilityDescriptions.gameObject.SetActive(true);
    }
    public static void SuperHide()
    {
        current.abilityDescriptions.gameObject.SetActive(false);

    }
}
