using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public InventoryItem ItemGameObject => itemGameObject;

    [SerializeField]
    protected string displayName;
    [SerializeField]
    protected Sprite icon;
    [SerializeField]
    protected InventoryItem itemGameObject;
    public abstract void Activate(CombatController user);
   
}
