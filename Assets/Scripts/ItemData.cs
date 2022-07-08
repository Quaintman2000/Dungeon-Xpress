using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "Classes/ItemData")]
public class ItemData : AbilityData
{
    public GameObject ItemGameObject;

    //How much the Item is valued at for coin or possibly determining loot rarity
    public float ItemValue;
}
