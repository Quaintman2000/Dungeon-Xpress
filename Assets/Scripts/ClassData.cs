using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewClassData", menuName ="Classes/ClassData")]
public class ClassData : ScriptableObject
{
    public string ClassName;

    [HorizontalGroup("Split")]
    [BoxGroup("Split/Character Model:")]
    [PreviewField(150)]
    public GameObject CharacterModel;

    [BoxGroup("Split/ Stats:"),MinValue(0)]
    public float MaxHealth;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float MovementRange;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float PhysicalDamage;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float MagicDamage;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float AttackSpeed;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public int StartingActionPoints;

    public Sprite characterImage;

    public AbilityData[] Abilities = new AbilityData[4];

    public AnimatorOverrideController ClassAnimatorOverride;

}
