using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewBreakable", menuName ="Breakables/BreakableData")]
public class BreakableData : ScriptableObject
{
    public string BreakableName;

    [HorizontalGroup("Split")]
    [BoxGroup("Split/Object Model:")]
    [PreviewField(150)]
    public GameObject BreakableObject;

}
