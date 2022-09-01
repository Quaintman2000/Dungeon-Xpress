using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Status Effects/Slow")]
public class SlowEffectData : StatusEffectData
{

    public override StatusEffect GetStatusEffect(CombatController combatController)
    {
        return new SlowEffect(this, combatController);
    }

}
public class SlowEffect : StatusEffect
{
    SlowEffectData effectData;

    public SlowEffect(SlowEffectData effectData, CombatController combatController)
    {
        combatant = combatController;
        combatant.OnStartTurn += Tick;
        this.effectData = effectData;
        currentDuration = effectData.MaxDuration;
        name = effectData.name;
    }

    protected override void Tick(CombatController combatController)
    {
        
        if(currentDuration <= 0)
        {
            EndEffect();
        }
    }
}