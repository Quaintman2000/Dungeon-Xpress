using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    //Reference to the BattleManager prefab
    [SerializeField] BattleManager battleManagerPrefab;
    [SerializeField] MapGenerator mapGenerator;

    //Instance of the MatchManager
    public static MatchManager Instance;

    //Ensure there is only one instance of the Match Manager
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    private void Start()
    {
        if(mapGenerator != null)
            mapGenerator.Generate();
    }

    /// <summary>
    /// Called when combat begins
    /// </summary>
    /// <param name="initiator">The combatant who causes combat to begin</param>
    /// <param name="other">Victim of the initiator</param>
    public void StartCombat(CharacterController initiator, CharacterController other)
    {
        //Create an instance of the BattleManager so combat can begin
        BattleManager newBattleManager = Instantiate<BattleManager>(battleManagerPrefab);

        //Set the initiator to the InCombat state and add them to the list of Combatants
        initiator.StartChangeState(CharacterController.PlayerState.InCombat);
        newBattleManager.Combatants.Add(initiator.combatController);

        //Set the other combatant to the InCombat state and add them to the list of Combatants
        other.StartChangeState(CharacterController.PlayerState.InCombat);
        newBattleManager.Combatants.Add(other.combatController);
    }
}
