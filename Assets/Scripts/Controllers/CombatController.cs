using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class CombatController : MonoBehaviour
{

    [SerializeField] Transform firePoint;
    [SerializeField] public int actionPoints;
    // Reference to the player Nav Mesh.
    [SerializeField] NavMeshMovement navMesh;
    // The close enough range to hit our target in melee.
    [SerializeField] float closeEnough = 0.1f;
    // Reference to the class data.
    public CharacterData CharacterData;
    public float Health;
    public bool IsTurn;
    // Reference to the player's combat state.
    public enum CombatState { Idle, Moving, Attacking };
    [SerializeField] public CombatState currentCombatState;

    public int abilityIndex;
    [SerializeField] public AbilityData selectedAbilityData;

    public Action OnDeathAction, OnHurtAction, OnAbilityUsedStartAction, OnAbilityUsedEndAction;
    public Action<CombatController> OnCombatantDeath, OnStartTurn, OnAbilityUsedAction;
    public Action<float> OnHealthChange;

    public CombatController currentTarget { get; private set; }

    List<StatusEffect> statusEffects = new List<StatusEffect>();

    public Vector3 MovementSpot;
    Controller characterController;

    [SerializeField] DamageNumberPopUp damageUIPopUp;

    Coroutine abilityRoutine;
    RaycastData inputData;

    // Start is called before the first frame update
    void Awake()
    {
        navMesh = this.GetComponent<NavMeshMovement>();

        // Set the player's starting health to the max.
        Health = CharacterData.MaxHealth;


        if (TryGetComponent<AnimationManager>(out AnimationManager animationManager))
        {
            animationManager.DeathAnimationOverAction += DestroyThis;
        }
        // Try to geth the inventory manager component if we have one.
        if (TryGetComponent<InventoryManager>(out InventoryManager inventoryManager))
        {
            inventoryManager.OnUseItem += ApplyEffect;
        }

        TryGetComponent<Controller>(out characterController);

        if(TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.OnCombatRightClickAction += ValidateInput;
        }

        
    }

    public void ValidateInput(RaycastData raycastData)
    {
        if (IsTurn == false)
            return;
        inputData = raycastData;
        // If this combatant is casting an ability...
        if (characterController.currentState == PlayerState.Casting)
        {
            if (actionPoints >= selectedAbilityData.Cost)
            {
                // Check if the type for the selected ability matches the hit result.
                // If the target require is self...
                if (selectedAbilityData.TargetStyle == AbilityData.TargetType.Self)
                {
                    if (raycastData.Result == HitResult.Self)
                    {
                        currentTarget = raycastData.HitCombatant;
                        abilityRoutine = StartCoroutine(UseAbilityRoutine());
                    }
                    else
                    {
                        return;
                    }
                }
                // If the target requirement is ground...
                else if (selectedAbilityData.TargetStyle == AbilityData.TargetType.Ground)
                {
                    if (raycastData.Result == HitResult.Ground)
                    {
                        if (Vector3.Distance(transform.position, raycastData.ImpactPoint) <= selectedAbilityData.Range) 
                        {
                            abilityRoutine = StartCoroutine(UseAbilityRoutine());
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                // If the target requirement is others...
                else if (selectedAbilityData.TargetStyle == AbilityData.TargetType.Others)
                {
                    if (raycastData.Result == HitResult.Other)
                    {
                        if (Vector3.Distance(transform.position, raycastData.ImpactPoint) <= selectedAbilityData.Range) 
                        {
                            currentTarget = raycastData.HitCombatant;
                            abilityRoutine = StartCoroutine(UseAbilityRoutine());
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                // If the target requirement is self and/or others...
                else if (selectedAbilityData.TargetStyle == AbilityData.TargetType.SelfAndOthers)
                {
                    if (raycastData.Result == HitResult.Self)
                    {
                        currentTarget = raycastData.HitCombatant;
                        abilityRoutine = StartCoroutine(UseAbilityRoutine());
                    }
                    else if (raycastData.Result == HitResult.Other)
                    {
                        if (Vector3.Distance(transform.position, raycastData.ImpactPoint) <= selectedAbilityData.Range) 
                        {
                            currentTarget = raycastData.HitCombatant;
                            abilityRoutine = StartCoroutine(UseAbilityRoutine());
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                // If there is not target requirement...
                else if (selectedAbilityData.TargetStyle == AbilityData.TargetType.Nothing)
                {
                    abilityRoutine = StartCoroutine(UseAbilityRoutine());
                }
                else
                {
                    return;
                }

                // Spend the action points for the ability.
                SpendActionPoints(selectedAbilityData.Cost);
            }
            else
            {
                Debug.LogError("Can't afford to use the ability!");
                return;
            }
        }
        // If this combatant is not casting an ability...
       else
        {
            if (raycastData.Result == HitResult.Ground)
            {
                // Movement
                float distance = navMesh.GetDistance(raycastData.ImpactPoint);
                // Calculate the movement cost.
                int movementCost = Mathf.RoundToInt(distance);
                // Clamp it so we get no logic errors.
                movementCost = (int)Mathf.Clamp(movementCost, 0, Mathf.Infinity);
                // If we have enough action points to cover the cost...

                if (movementCost <= actionPoints)
                {
                    // Subtract the points.
                    SpendActionPoints(movementCost);
                    // Move to the position.
                    navMesh.AttackMove(raycastData.ImpactPoint, 1f);
                }
            }
            else if (raycastData.Result == HitResult.Other)
            {
                selectedAbilityData = CharacterData.DefualtAttack;
                abilityIndex = 0;
                currentTarget = raycastData.HitCombatant;
                if (Vector3.Distance(transform.position, currentTarget.transform.position) < selectedAbilityData.Range)
                {
                    abilityRoutine = StartCoroutine(UseAbilityRoutine());
                }
                // Spend the action points for the ability.
                SpendActionPoints(selectedAbilityData.Cost);
                //throw new NotImplementedException();
            }
            else
                return;
        }
        
    }


    public void SpendActionPoints(int spentPoints)
    {
        actionPoints -= spentPoints;
        UIManager.Instance.UpdateActionPoints(actionPoints);
    }


    IEnumerator UseAbilityRoutine()
    {
        transform.LookAt(new Vector3(inputData.ImpactPoint.x, transform.position.y, inputData.ImpactPoint.z));

        // Call the event for those who need to know that we just used an ability.
        OnAbilityUsedStartAction?.Invoke();
        OnAbilityUsedAction?.Invoke(this);

        // Grab the ability animation length and convert it to milleseconds.
        var animationLength = Mathf.RoundToInt(selectedAbilityData.AnimationClip.length);
        // Set the time to end the busy state.
        var timeToEnd = Time.time + animationLength;
        // Do nothing until we reach that time.
        do
        {
            yield return null;
        } while (Time.time < timeToEnd);
        // Let everyone know we're done.
        OnAbilityUsedEndAction?.Invoke();
    }

    // This is called during the ability animation to signal the ability functionality as they can start at different times.
    void Activate()
    {
        selectedAbilityData.Activate(this, inputData);
        selectedAbilityData = null;
        inputData = null;
    }


    #region Damage Healing and Death

    /// <summary>
    /// Reduces the player's health based on damage inputted.
    /// </summary>
    /// <param name="damage"> The amount of damage to be dealt.</param>
    public void TakeDamage(float damage)
    {
        if (Health <= 0) return;
        // Subtract health by damage.
        Health -= damage;

        //updates the health bar
        OnHealthChange?.Invoke(Health);
        

        // Have the pop up occure if we have any.
        if(damageUIPopUp != null)
        {
            damageUIPopUp.ActivatePopUp(damage);
        }

        // If health is less than or equal to 0...
        if (Health <= 0)
        {
            // Commit die.
            Die();
        }
        else
        {
            OnHurtAction?.Invoke();
        }
    }
    public void TakeDamage(float damage, CombatController attacker)
    {
        // For each effect in our status effects...
        foreach (var effect in statusEffects)
        {
            // If effect is an IDefensiveEffect...
            if (effect is IDefensiveEffect defensiveEffect)
            {
                // Call it to handle it's effect over.
                defensiveEffect.HandleEffectOver(this, attacker);
                RemoveStatusEffect(effect);
                return;
            }
        }

        // Take the damage.
        TakeDamage(damage);
    }
    [ContextMenu("TakeDamage")]
    public void DebugTakeDamage()
    {
        OnHurtAction?.Invoke();
    }

   
    public void Heal(float hAmount)
    {
        Health += hAmount;
        Health = Mathf.Clamp(Health, 0, CharacterData.MaxHealth);

        if (OnHealthChange != null)
        {
            //updates the health bar
            OnHealthChange.Invoke(Health);
        }

    }
   

    [ContextMenu("Die")]
    // Handles the death procedure.
    void Die()
    {
        Debug.Log("Dead!");
        //Plays the death sound

        characterController.StartChangeState(PlayerState.Dead);

        OnDeathAction?.Invoke();
        // Call the death event with this.
        OnCombatantDeath?.Invoke(this);

    }
    #endregion
    void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    #region Turn
    // Starts the player's turn with their starting action points.
    public void StartTurn()
    {
        UIManager.Instance.UpdateActionPoints(actionPoints);
        OnStartTurn?.Invoke(this);
        // Sets the isTurn to true.
        IsTurn = true;
        // Sets the action points to this class's starting action points.
        actionPoints = CharacterData.StartingActionPoints;
        
    }
   
    /// <summary>
    /// Debug function to the combatant's turn.
    /// </summary>
    [ContextMenu("End Turn")]
    private void EndTurn()
    {
        actionPoints = 0;
        OnAbilityUsedEndAction?.Invoke();
    }
    #endregion
   
 
    #region Status Effects
    void ApplyEffect(ItemData item)
    {
        item.Activate(this);
    }

    /// <summary>
    /// Adds the status effect to a list of status effects.
    /// </summary>
    /// <param name="statusEffect"></param>
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        // Add the status effect.
        statusEffects.Add(statusEffect);
        // Listen for when the effect ends.
        statusEffect.OnEffectEnd += RemoveStatusEffect;
    }
    /// <summary>
    /// Removes the status effect.
    /// </summary>
    /// <param name="effect"></param>
    void RemoveStatusEffect(StatusEffect effect)
    {
        // Remove the status effect from out list.
        statusEffects.Remove(effect);
        // Stop listening for when the effect ends.
        effect.OnEffectEnd -= RemoveStatusEffect;
    }
    #endregion
}


