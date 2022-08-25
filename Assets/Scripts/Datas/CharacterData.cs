using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterData : ScriptableObject
{
    public string CharacterName;

    [HorizontalGroup("Split")]
    [BoxGroup("Split/Character Model:")]
    [PreviewField(150)]
    public GameObject CharacterModel;

    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float MaxHealth;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float MovementRange;
   
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public float AttackSpeed;
    [BoxGroup("Split/ Stats:"), MinValue(0)]
    public int StartingActionPoints;

    public Sprite CharacterImage;

    public AbilityData[] Abilities = new AbilityData[4];
}
