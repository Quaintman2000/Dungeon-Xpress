using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData : ScriptableObject
{
    // The name of the status effect.
    public string Name => name;
    // Flavor text.
    public string FlavorText => flavorText;
    // Description text.
    public string Description => description;
    // The max duration of the effect.
    public int MaxDuration => maxDuration;

    [SerializeField]
    protected new string name;
    [SerializeField]
    protected string flavorText;
    [SerializeField]
    protected string description;
    [SerializeField]
    protected int maxDuration;

    public virtual StatusEffect GetStatusEffect(CombatController combatController)
    {
        return null;
    }
}
[System.Serializable]
public abstract class StatusEffect
{
    // The event for when the effect ends.
    public delegate void EffectEvent(StatusEffect effect);
    public event EffectEvent OnEffectEnd;
    // Reference the combatant
    protected CombatController combatant;
    // Name of the status effect.
    public string name { get; protected set; }
    // Current duration time.
    protected int currentDuration;
    protected virtual void Tick(CombatController combatController)
    {
        // Subtract the current duration by 1.
        currentDuration -= 1;
    }

    // Ends the effect.
    protected virtual void EndEffect()
    {
        combatant.OnStartTurn -= Tick;
    }
}

