using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AbilityData : ScriptableObject
{
    public string AbilityName => name;
    [SerializeField]
    protected new string name;

    public string Description => description;
    [SerializeField, TextArea]
    protected string description;

    public AbilityType Type => abilityType;
    [SerializeField]
    protected AbilityType abilityType = AbilityType.MeleeAttack;

    public BuffOrDebuff BuffType => buffOrDebuff;
    [SerializeField]
    protected BuffOrDebuff buffOrDebuff = BuffOrDebuff.Buff;

    public TargetType TargetStyle => targetStyle;
    [SerializeField]
    protected TargetType targetStyle = TargetType.SelfAndOthers;

    public Sprite Icon => icon;
    [SerializeField]
    protected Sprite icon;
    
    public float Range =>range;
    [SerializeField]
    protected float range;
    
    public int Cost =>cost;
    [SerializeField]
    protected int cost;

    public AnimationClip AnimationClip => animationClip;
    [SerializeField]
    protected AnimationClip animationClip;

    public enum AbilityType { MeleeAttack, RangeAttack, Movement, Healing, ActionPoints , Neither };
    public enum BuffOrDebuff { Buff, Debuff, None}
    public enum TargetType { Self, Others, Ground, SelfAndOthers, Nothing }

    public virtual bool IsValidTarget(CombatController self, CombatController target)
    {
        if (target == self && (targetStyle != TargetType.Self && targetStyle != TargetType.SelfAndOthers))
            return false;
        else if (target != self && (targetStyle != TargetType.Others && targetStyle != TargetType.SelfAndOthers))
            return false;

        return true;
    }
    public virtual bool IsValidTarget(CombatController self, CombatController target, out CombatController combatController)
    {
        combatController = target;
        return true;
    }

    public virtual async Task Activate(CombatController combatController, RaycastData raycastData) {

        await Task.Yield();
    }
}


