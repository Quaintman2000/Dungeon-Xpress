using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefensiveEffect 
{
    public void ApplyEffect(CombatController combatController);

    public void HandleEffectOver(CombatController defender, CombatController attacker);
}
