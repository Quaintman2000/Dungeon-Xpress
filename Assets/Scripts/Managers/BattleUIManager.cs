using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public Text turnText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Update the current turn text
        turnText.text = "Turn: " + BattleManager.instance.combatants[BattleManager.instance.combatantsIndex].gameObject.name;
    }
}
