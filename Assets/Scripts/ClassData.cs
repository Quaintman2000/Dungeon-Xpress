using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewClassData", menuName ="Classes/ClassData")]
public class ClassData : ScriptableObject
{
    public string ClassName;
    public float MaxHealth;
    public float MovementRange;
    public float PhysicalDamage;
    public float MagicDamage;
    public float AttackSpeed;
    [PreviewField(75)]
    public GameObject CharacterModel;

    public string[] Abilities = new string[4];



}
