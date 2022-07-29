using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    //References for Bars
    [SerializeField] private Image healthBar;

    [SerializeField] private PlayerController playerCtrl;
    [SerializeField] private CombatController combatCtrl;
    [SerializeField] private GameObject skillBar;
     public GameObject pausePanel;
    [Header("Abilities to assign")]
    //the abilities that will be used everytime a button is clicked
    [SerializeField] private AbilityData[] abilities = new AbilityData[4];


    //assign the bars to player accordingly
    void Start()
    {
        combatCtrl.OnHealthChange += OnHealthChange;

        abilities[0] = combatCtrl.CharacterData.Abilities[0];
        abilities[1] = combatCtrl.CharacterData.Abilities[1];
        abilities[2] = combatCtrl.CharacterData.Abilities[2];
        abilities[3] = combatCtrl.CharacterData.Abilities[3];

        pausePanel.SetActive(false);
        
    }

    void Update()
    {
        //if player enters combat mode then show it
        if (playerCtrl.currentState == CharacterController.PlayerState.InCombat)
        {
            skillBar.SetActive(true);
            
        }
        //if player is not in combat mode then hide it
        else if (playerCtrl.currentState == CharacterController.PlayerState.FreeRoam)
        {
            skillBar.SetActive(false);
            

        }
    }

    // Toggles the skill bar off and on.
    public void ToggleSkillBar(bool setActive)
    {

    }

    /// <summary>
    /// Let's the combatcontroller know which ability was selected.
    /// </summary>
    /// <param name="index"></param>
    public void SelectAbility(int index)
    {
        if (combatCtrl)
        {
            combatCtrl.abilityIndex = index + 1;
            combatCtrl.selectedAbilityData = abilities[index];
        }
    }


    void Awake()
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

    //assign health bar of the player to its maximum value
    private void OnHealthChange(float health)
    {
        healthBar.fillAmount = health / combatCtrl.CharacterData.MaxHealth;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
}
