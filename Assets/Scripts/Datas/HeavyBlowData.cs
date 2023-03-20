using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Ability Data/Heavy Blow")]
public class HeavyBlowData : AbilityData
{
    [SerializeField, Tooltip("How much of current health you want to be converted to.\nExample: I have 30 health currently and with my 0.5 healthToDamageRatio, I will deal 15 damage to my target.")]
    float healthToDamageRatio = 0.01f;


    public override async Task Activate(CombatController activatorCombatController, RaycastData raycastData)
    {
        // Grab the current target.
        var target = activatorCombatController.currentTarget;
        
        // Calculate the damage to be dealt by health times the health To Damage Ratio.
        var damage = activatorCombatController.Health * healthToDamageRatio;
        // Tell the Target to take damage.
        target.TakeDamage(damage);

    }

    public override bool IsValidTarget(CombatController self, CombatController target)
    {
        return base.IsValidTarget(self, target);
    }
}
