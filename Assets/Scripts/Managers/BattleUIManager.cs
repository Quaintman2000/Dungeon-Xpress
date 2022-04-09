using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    //Reference to the current turn text object
    [SerializeField] Text turnText;

    //Update the current turn text
    public void SetTurnText()
    {
        turnText.text = "Turn: " + BattleManager.Instance.Combatants[BattleManager.Instance.CombatantsIndex].gameObject.name;
    }
}
