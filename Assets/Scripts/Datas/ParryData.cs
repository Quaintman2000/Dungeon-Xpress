using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability Data/Parry")]
public class ParryData : AbilityData
{
    [SerializeField]
    ParryStatusEffectData effectData;

    public override async Task Activate(CombatController combatController)
    {

        combatController.AddStatusEffect(effectData.GetStatusEffect(combatController));
    }
}
