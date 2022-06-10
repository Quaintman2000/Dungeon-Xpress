using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "Classes/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite Icon;
    public GameObject ItemGameObject;
    public string ItemName;

    public enum ItemType { Health, ActionPoints, Damage };
    public ItemType Type;

    //Action point cost to use
    public int Cost;
    //How much the Item grants to a user EX. health restored
    public float ItemValue;
    //How long an item should affect its user for (should have a corutine when out of combat to tick for each turn
    public float Duration;
}
