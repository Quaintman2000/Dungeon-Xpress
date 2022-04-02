using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public List<CombatController> combatants;
    public int combatantsIndex;
    public int currentTurn;

    //Create an instance of the BattleManager so that it will be a singleton
    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Sort the list of combatants and have the first one start their turn
        SortByAttackSpeed();
        combatants[0].StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Move through the list to determine the next combatant's turn
    public void ChangeTurn()
    {
        combatantsIndex++;

        currentTurn = combatantsIndex;

        //Call the start turn function to set isTurn true and reset action points
        combatants[combatantsIndex].StartTurn();
    }

    //Swap combatants in the list to organize them
    void swap(int index1, int index2)
    {
        CombatController temp = combatants[index1];
        combatants[index1] = combatants[index2];
        combatants[index2] = temp;
    }

    //Sort the combatants list by using swap()
    void SortByAttackSpeed()
    {
        int i;
        for (i = 0; i < combatants.Count - 1; i++)
        {
            if (combatants[i].classData.AttackSpeed < combatants[i + 1].classData.AttackSpeed)
            {
                swap(i, (i + 1));
            }
        }
    }
}
