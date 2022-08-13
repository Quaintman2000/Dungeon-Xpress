using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItemData", menuName = "Item")]
public class ItemData : ScriptableObject
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
    [SerializeField]
    StatusEffectData statusEffectData;
    public void Activate(CombatController user)
    {
        user.AddStatusEffect(statusEffectData.GetStatusEffect(user));
    }
   
}
