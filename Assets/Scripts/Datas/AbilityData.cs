using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "Classes/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string AbilityName;
    public enum AbilityType { MeleeAttack, RangeAttack, Movement, Healing, ActionPoints , Neither };
    public AbilityType Type = AbilityType.MeleeAttack;
    public enum BuffOrDebuff { Buff, Debuff, None}
    public BuffOrDebuff BuffType = BuffOrDebuff.Buff;
    public enum TargetType { Self, Others, Ground, SelfAndOthers }
    public TargetType TargetStyle = TargetType.SelfAndOthers;
    public Sprite Icon;
    public float PhysDamage;
    public float MagicDamage;
    public float Range;
    public int Cooldown;
    public int cost;

    public float MovementModifier;

    public Projectile Projectile;

    public SoundData AbilitySound;

    public SoundClip AbilityClip;
}


