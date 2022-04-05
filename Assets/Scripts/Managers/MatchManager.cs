using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] BattleManager battleManagerPrefab;

    public static MatchManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void StartCombat(CharacterController initiator, CharacterController other)
    {
        BattleManager newBattleManager = Instantiate<BattleManager>(battleManagerPrefab);
        initiator.ChangeState(CharacterController.PlayerState.InCombat);
        newBattleManager.combatants.Add(initiator.combatController);
        other.ChangeState(CharacterController.PlayerState.InCombat);
        newBattleManager.combatants.Add(other.combatController);
    }
}
