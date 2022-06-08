using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewEnemyData", menuName ="Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;

    [HorizontalGroup("Split")]
    [BoxGroup("Split/Enemy Model:")]
    [PreviewField(150)]
    public GameObject EnemyModel;
    
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

    public AbilityData[] Abilities = new AbilityData[4];
}
