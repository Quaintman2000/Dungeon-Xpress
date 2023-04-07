using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Character : NetworkBehaviour
{
    [SerializeField] GameObject selectionCircle;
    [SerializeField] protected Controller controller;
    [SerializeField] protected CombatController combatController;

    protected virtual void Awake()
    {
        combatController = GetComponent<CombatController>();
    }
    public void SelectionToggle(bool isSelected)
    {
        selectionCircle.SetActive(isSelected);
    }

    public void SetController(Controller newController)
    {
        controller = newController;
    }
    public Controller GetController()
    {
        return controller;
    }
}
