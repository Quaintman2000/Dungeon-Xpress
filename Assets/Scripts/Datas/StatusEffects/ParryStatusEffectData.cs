using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Status Effects/Parry Status")]
public class ParryStatusEffectData : StatusEffectData
{
    public SlowEffectData SlowEffect => slowEffect;
    [SerializeField]
    SlowEffectData slowEffect;
    public override StatusEffect GetStatusEffect(CombatController combatController)
    {
        return new ParryStatusEffect(this, combatController);
    }
}

public class ParryStatusEffect : StatusEffect, IDefensiveEffect
{

    // The parry statys effect data.
    ParryStatusEffectData parryEffectData;
    // Reference to the animation manager.
    PlayerAnimationManager animationManager;

    // Constructor
    public ParryStatusEffect(ParryStatusEffectData parryStatusEffectData, CombatController combatController)
    {
        combatant = combatController;
        parryEffectData = parryStatusEffectData;
        name = parryStatusEffectData.name;

        ApplyEffect(combatController);
    }

    public void ApplyEffect(CombatController combatController)
    {
        
        if(combatController.TryGetComponent<PlayerAnimationManager>(out animationManager))
        {
            animationManager.animator.SetBool("IsParryStance", true);
        }
        else
        {
            Debug.LogError("This character: " + combatController.gameObject.name + " does not contain a player animation Manager.");
            return;
        }
    }

    public void HandleEffectOver(CombatController defender, CombatController attacker)
    {
        if(animationManager != null)
        {
            animationManager.animator.SetTrigger("ActivateParry");
            animationManager.animator.SetBool("IsParryStance", false);
            parryEffectData.SlowEffect.GetStatusEffect(attacker);
        }
        else
        {
            Debug.LogError("This character: " + defender.gameObject.name + " does not contain a player animation Manager.");
            return;
        }
    }

  
}
