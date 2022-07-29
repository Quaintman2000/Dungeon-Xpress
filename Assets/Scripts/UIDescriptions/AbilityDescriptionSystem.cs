using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDescriptionSystem : MonoBehaviour
{
    public static AbilityDescriptionSystem current;
    public AbilityDescriptions abilityDescriptions;
    public CrazyDescription crazyDescription;
    [SerializeField] private GameObject hideObject;

    private void Start()
    {
        SuperHide();
    }
    private void Awake()
    {
        current = this;

    }
    public static void SuperDescription(string description = "")
    {
        current.crazyDescription.SetText(description);
        current.crazyDescription.gameObject.SetActive(true);

    }
    public static void SuperShow(string title, string range, string ap = "")//, string description = "") 
    {

        current.abilityDescriptions.SetText(title, range, ap);//, description);
        current.abilityDescriptions.gameObject.SetActive(true);
        current.hideObject.gameObject.SetActive(true);
    }
    public static void SuperHide()
    {
        current.abilityDescriptions.gameObject.SetActive(false);
        current.crazyDescription.gameObject.SetActive(false);
        current.hideObject.gameObject.SetActive(false);
    }
}
