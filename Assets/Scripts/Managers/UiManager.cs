using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    //References for Bars
    [SerializeField] private Image healthBar;

    [SerializeField] private PlayerController playerCtrl;
    [SerializeField] private CombatController combatCtrl;
    [SerializeField] private GameObject skillBar;

    [Header("Abilities to assign")]
    //the abilities that will be used everytime a button is clicked
    [SerializeField] private AbilityData skill1_Ability;
    [SerializeField] private AbilityData skill2_Ability;
    [SerializeField] private AbilityData skill3_Ability;
    [SerializeField] private AbilityData skill4_Ability;

    //assign the bars to player accordingly
    void Start ()
    {
        AssignHealthBar();

        skill1_Ability = combatCtrl.CharacterData.Abilities[0];
        skill2_Ability = combatCtrl.CharacterData.Abilities[1];
        skill3_Ability = combatCtrl.CharacterData.Abilities[2];
        skill4_Ability = combatCtrl.CharacterData.Abilities[3];
    }

    void Update()
    {
        //if player enters combat mode then show it
        if(playerCtrl.currentState == CharacterController.PlayerState.InCombat)
        {
            skillBar.SetActive (true);
        }
        //if player is not in combat mode then hide it
        else if (playerCtrl.currentState == CharacterController.PlayerState.FreeRoam)
        {
            skillBar.SetActive (false);
            
        }
    }

    /// Skills button and what they do each
    public void Skill1()
    {
        if(combatCtrl)
        {
            combatCtrl.selectedAbilityData = skill1_Ability;
        }
    }
    public void Skill2()
    {
        if(combatCtrl)
        {
            combatCtrl.selectedAbilityData = skill2_Ability;
        }
    }
    public void Skill3()
    {
        if(combatCtrl)
        {
            combatCtrl.selectedAbilityData = skill3_Ability;
        }
    }
    public void Skill4()
    {
        if(combatCtrl)
        {
            combatCtrl.selectedAbilityData = skill4_Ability;
        }
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    //assign health bar of the player to its maximum value
    public void AssignHealthBar()
    {
        healthBar.fillAmount = GameManager.Instance.playerData.Health / GameManager.Instance.playerData.CharacterData.MaxHealth;
    }
}
