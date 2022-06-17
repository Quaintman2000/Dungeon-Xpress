using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //Create an Instance of the BattleManager so that it will be a singleton
    public static BattleManager Instance;

    //Reference to the BattleUIManager
    [SerializeField] BattleUIManager battleUIManager;

    //Create a list to hold all of the combatants
    public List<CombatController> Combatants;
    public int CombatantsIndex;

    //Create a list of all the combatant's character images
    public List<Sprite> combatantsSprites;

    //Variable to keep track of the current turn
    [SerializeField] int currentTurn;

    private void Awake()
    {
        //Make sure there is only one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Sort the list of combatants
        SortByAttackSpeed();

        //Call the function to fill out the turn order image wheel
        battleUIManager.CreateTurnImages();

        //Tell the BattleManager to listen for each combatant's death event
        foreach(CombatController combatController in Combatants)
        {
            combatController.OnCombatantDeath += OnCombatantDeath;
        }

        //Set the turn text to the first combatant
        battleUIManager.SetTurnText();

        //Tell the first combatant to start their turn
        Combatants[0].StartTurn();
    }

    //When a combatant dies...
    private void OnCombatantDeath(CombatController combatController)
    {
        //remove them from the list
        Combatants.Remove(combatController);

        //If there is only one combatant left...
        if (Combatants.Count < 2)
        {
            //End combat by setting the remaining combatant to the free roam state and destroy the battle manager intstance
            Combatants[0].IsTurn = false;
            Combatants[0].GetComponent<CharacterController>().ChangeState(CharacterController.PlayerState.FreeRoam);
            Destroy(this.gameObject);
        }
        combatController.OnCombatantDeath -= OnCombatantDeath;
    }
    
    private void CheckForEndBattle()
    {
       
    }

    //Move through the list to determine the next combatant's turn
    public void ChangeTurn()
    {
        CombatantsIndex++;

        currentTurn = CombatantsIndex;

        //Update the turn text UI
        battleUIManager.SetTurnText();

        //Update the turn order image in the wheel
        battleUIManager.UpdateTurnImages();

        //Call the start turn function to set isTurn true and reset action points
        Combatants[CombatantsIndex].StartTurn();
    }

    /// <summary>
    /// Swap Combatants in the list to organize them
    /// </summary>
    /// <param name="index1">First value to swap with</param>
    /// <param name="index2">Second value to swap with</param>
    void swap(int index1, int index2)
    {
        CombatController temp = Combatants[index1];
        Combatants[index1] = Combatants[index2];
        Combatants[index2] = temp;
    }

    //Sort the Combatants list by using the swap function
    void SortByAttackSpeed()
    {
        int i;
        for (i = 0; i < Combatants.Count - 1; i++)
        {
            //If the current combatant's attack speed is less than the next one in the list, swap them
            if (Combatants[i].classData.AttackSpeed < Combatants[i + 1].classData.AttackSpeed)
            {
                swap(i, (i + 1));
            }
        }
    }
}
