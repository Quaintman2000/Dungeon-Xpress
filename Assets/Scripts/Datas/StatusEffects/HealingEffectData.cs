using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="NewHealEffectData",menuName ="Status Effects/Heal Effect Data")]
public class HealingEffectData : StatusEffectData
{
    // The amount of health healed per tick.
    public float HealthPerTick => healthPerTick;
    [SerializeField]
    float healthPerTick = 0;
    // Returns the the status effect.
    public override StatusEffect GetStatusEffect(CombatController combatController)
    {
        return new HealingStatusEffect(this, combatController);
    }
}

public class HealingStatusEffect: StatusEffect
{
    // The healing effect data.
    HealingEffectData healingEffect;
    
    // Contstructor.
    public HealingStatusEffect(HealingEffectData effectData, CombatController combatController)
    {
        combatant = combatController;
        combatant.OnStartTurn += Tick;
        healingEffect = effectData;
        currentDuration = effectData.MaxDuration;
        name = effectData.name;
        combatant.Heal(healingEffect.HealthPerTick);
    }
    // A tick is called at the start of the turn to apply the status effect.
    protected override void Tick(CombatController combatController)
    {
        base.Tick(combatController);
        // Heal the player.
        combatController.Heal(healingEffect.HealthPerTick);
        // If the current duration is now equal to or less than 0,
        if(currentDuration <= 0)
        {
            // End the effect.
            EndEffect();
        }
    }
}
