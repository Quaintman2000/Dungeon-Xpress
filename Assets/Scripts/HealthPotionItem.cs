using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthPotionData", menuName = "Items/Health potion data")]
public class HealthPotionItem : ItemData
{
    [SerializeField]
    float healAmount;
    public override void Activate(CombatController user)
    {
        user.Heal(healAmount);
    }

}
