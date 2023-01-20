using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
   public Action<int> OnItemButtonPressed;
    public Action OnAbilityButtonPressed;

    public static UIManager Instance;
    //References for Bars
    [SerializeField] private Image healthBar;

    [SerializeField] private PlayerController playerCtrl;
    [SerializeField] private CombatController combatCtrl;
    [SerializeField] private GameObject skillBar;
     public GameObject pausePanel;

    [SerializeField] Sprite defualtButtonIcon;

    [Header("Abilities to assign")]
    //the abilities that will be used everytime a button is clicked
    [SerializeField] private AbilityData[] abilities = new AbilityData[4];
    [SerializeField] InventoryButton[] inventoryButtons = new InventoryButton[3];

    [SerializeField] TextMeshProUGUI actionPointsText;
   

    //assign the bars to player accordingly
    void Start()
    {
        combatCtrl.OnHealthChange += OnHealthChange;

        abilities[0] = combatCtrl.CharacterData.Abilities[0];
        abilities[1] = combatCtrl.CharacterData.Abilities[1];
        abilities[2] = combatCtrl.CharacterData.Abilities[2];
        abilities[3] = combatCtrl.CharacterData.Abilities[3];

        pausePanel.SetActive(false);
        // Subscribe to the player controller events.
        playerCtrl.OnCombatStartedAction += ToggleSkillBar;
        playerCtrl.OnPauseAction += PauseGame;
        // If the player controller has an inventory manager...
        if(playerCtrl.TryGetComponent<InventoryManager>(out InventoryManager inventoryManager))
        {
            // Subscribe to the inventory manager events.
            inventoryManager.OnItemPickUpSucess += SetInventoryIcon;
            inventoryManager.OnItemRemoved += RemoveInventoryIcon;
        }
        actionPointsText.text = "Action Points " + combatCtrl.actionPoints.ToString();
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
            OnAbilityButtonPressed?.Invoke();
            combatCtrl.abilityIndex = index + 1;
            combatCtrl.selectedAbilityData = abilities[index];
        }
    }
    /// <summary>
    /// Sets the icons for the inventory buttons when we pick up an item.
    /// </summary>
    /// <param name="itemData">The item data for the item.</param>
    void SetInventoryIcon(ItemData itemData)
    {
        // For each of our inventory slots...
        for(int i = 0; i < inventoryButtons.Length; i++)
        {
            // If this inventory slot does not have an item...
            if(inventoryButtons[i].hasItem == false)
            {
                // Set the button image to be our icon and set the has item to true.
                inventoryButtons[i].button.image.sprite = itemData.Icon;
                inventoryButtons[i].hasItem = true;
                break;
            }
        }
    }

    void RemoveInventoryIcon(ItemData itemData)
    {
        // Go through the list of buttons.
        for(int i = 0; i < inventoryButtons.Length; i++)
        {
            // If this button has the same icon as the item being removed...
            if(inventoryButtons[i].button.image.sprite == itemData.Icon)
            {
                // Set the icon back to defualt and set hast item to false.
                inventoryButtons[i].button.image.sprite = defualtButtonIcon;
                inventoryButtons[i].hasItem = false;
                break;
            }
        }

        // Sort the icons out so they are nice and neat.
        for (int i = 0; i < inventoryButtons.Length-1; i++)
        {
            // If the inventory button upfront has no icon but the next one does, switch them around.
            if(inventoryButtons[i].hasItem == false && inventoryButtons[i+1].hasItem == true)
            {
                inventoryButtons[i].button.image.sprite = inventoryButtons[i + 1].button.image.sprite;
                inventoryButtons[i].hasItem = true;

                inventoryButtons[i + 1].button.image.sprite = defualtButtonIcon;
                inventoryButtons[i + 1].hasItem = false;
            }
        }
    }
    public void UseItem(int index)
    {
        // If it is our turn.
        if (combatCtrl.IsTurn)
        {
            // Send out the event for an invent button press.
            OnItemButtonPressed?.Invoke(index);
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
    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        // Activate the pause panel.
        pausePanel.SetActive(true);
        // Set the timescale to 0.
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
    [System.Serializable]
    private struct InventoryButton
    {
        public Button button;
        public bool hasItem;
    }

    public void UpdateActionPoints(float actionPoints)
    {
        actionPointsText.text = "Action Points " + actionPoints.ToString();
    }

}
