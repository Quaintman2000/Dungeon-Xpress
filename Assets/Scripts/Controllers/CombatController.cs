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
    CharacterController characterController;

    [SerializeField] DamageNumberPopUp damageUIPopUp;

    Coroutine abilityRoutine;
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
        // Try to grab the character controller if we have one.
        if (TryGetComponent<CharacterController>(out characterController))
        {
            // Subscribe to the combat related events.
            characterController.CombatMoveToPointAction += MoveToPoint;
        }

        if(TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.OnCombatRightClickAction += ValidateInput;
        }

        
    }

    public void ValidateInput(RaycastData raycastData)
    {
        if (IsTurn == false)
            return;
       // If this combatant is casting an ability...
       if(characterController.currentState == CharacterController.PlayerState.Casting)
        {
            if (actionPoints >= selectedAbilityData.Cost)
            {
                // Check if the type for the selected ability matches the hit result.
                // If the target require is self...
                if (selectedAbilityData.TargetStyle == AbilityData.TargetType.Self)
                {
                    if (raycastData.Result == HitResult.Self)
                    {
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
                        abilityRoutine = StartCoroutine(UseAbilityRoutine());
                    }
                    else if (raycastData.Result == HitResult.Other)
                    {
                        if (Vector3.Distance(transform.position, raycastData.ImpactPoint) <= selectedAbilityData.Range) 
                        {
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
            if(raycastData.Result == HitResult.Ground)
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
            else if(raycastData.Result == HitResult.Other)
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
        }
    }


    public void SpendActionPoints(int spentPoints)
    {
        actionPoints -= spentPoints;
        UIManager.Instance.UpdateActionPoints(actionPoints);
    }


    IEnumerator UseAbilityRoutine()
    {
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
        selectedAbilityData.Activate(this);
        selectedAbilityData = null;
    }


    #region Damage Healing and Death

    /// <summary>
    /// Reduces the player's health based on damage inputted.
    /// </summary>
    /// <param name="damage"> The amount of damage to be dealt.</param>
    public void TakeDamage(float damage)
    {
        // Subtract health by damage.
        Health -= damage;

        //updates the health bar
        OnHealthChange?.Invoke(Health);
        OnHurtAction?.Invoke();

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
    #region Movement
    /// <summary>
    /// Moves to the raycast point and subtracts action points. unless the current selected ability 
    /// is a movement ability then it should run that instead
    /// </summary>
    /// <param name="raycastPoint"> The designated point.</param>
    public void MoveToPoint(Vector3 raycastPoint)
    {
        //If the player is just moving or using a non movement ability then if passes
        if (selectedAbilityData == null || selectedAbilityData.Type != AbilityData.AbilityType.Movement)
        {
            UIManager.Instance.UpdateActionPoints(actionPoints);
            // Calculate length of the path.
            float distance = navMesh.GetDistance(raycastPoint);
            // Calculate the movement cost.
            int movementCost = Mathf.RoundToInt(distance);
            // Clamp it so we get no logic errors.
            movementCost = (int)Mathf.Clamp(movementCost, 0, Mathf.Infinity);
            // If we have enough action points to cover the cost...

            if (movementCost <= actionPoints)
            {
                // Subtract the points.
                actionPoints -= movementCost;
                // Move to the position.
                navMesh.AttackMove(raycastPoint, 1f);
            }
        } else //Used for Movement Abilites in which the player moves around the map
        {
          
            MovementSpot = raycastPoint;

            //Conditions for if statement
            bool hasActionPoints = actionPoints >= selectedAbilityData.Cost;
            bool isClose = Vector3.Distance(raycastPoint, transform.position) <= selectedAbilityData.Range;

            //if has enough action points and is within range
            if (hasActionPoints && isClose)
            {
                // Subtract the action points by the cost.
                actionPoints -= selectedAbilityData.Cost;
                // Call the event for those who need to know that we just used an ability.
                OnAbilityUsedStartAction?.Invoke();
                OnAbilityUsedAction?.Invoke(this);

                // Grab the ability animation length and convert it to milleseconds.
                var animationLength = Mathf.RoundToInt(selectedAbilityData.AnimationClip.length * 1000);

                /*Find a solution to incorporate await function*/
                /// Delay this function by the animation length.
                ///await Task.Delay(animationLength);

                // Tell those who are listening for the ability to end that the ability is over.
                OnAbilityUsedEndAction?.Invoke();
            }
        }
    }
    //Used to start the leap
    public void StartLeap(Transform player, Vector3 start, Vector3 stop, float time)
    {
        Vector3 lookDirection = new Vector3(stop.x, start.y, stop.z);
        player.transform.LookAt(lookDirection);//Looks where they are going to jump to
        StartCoroutine(Leap(player, start, stop, time));
    }
    /// <summary>
    /// The dynamic leap used for the player to leap from one position to another using a
    /// trajectory curve
    /// Uses three points with the mid point being the 3rd point to calc the curve for
    /// https://youtu.be/RF04Fi9OCPc
    /// First Half of the Video helps understand how the calculation is done
    /// </summary>
    /// <param name="player">what is being moved between the point</param>
    /// <param name="start">The start point of the leap</param>
    /// <param name="stop">The stop point of the leap</param>
    /// <param name="time">The time to leap</param>
    /// <returns></returns>
    IEnumerator Leap(Transform player, Vector3 start, Vector3 stop, float time)
    {
        float counter = 0;

        // The mid point between the start and stop
        Vector3 midpoint = start + ((start - stop) / 2f);
        //The taller of both points and adds 10f to make the point slightly higher for the curve
        float highestY = Mathf.Max(start.y, stop.y) + 10f;

        //The height of the curve in which they jump to get to which is also in the middle of both
        Vector3 highestSpot = new Vector3(midpoint.x, highestY, midpoint.z);

        //Calculates where it is on the curve
        while (counter <= time)
        {
            counter += Time.deltaTime;//Runs each frame
            float percent = counter / time;//How far in the animation from 0-1

            Vector3 p0 = Vector3.Lerp(start, stop, percent);
            Vector3 p1 = Vector3.Lerp(highestSpot, stop, percent);

            player.position = Vector3.Lerp(p0, p1, percent);
            yield return null;
        }
    }
    /// <summary>
    /// Used to instantly move the player to the raycast point
    /// </summary>
    public void InstantMove(Vector3 raycastPoint)
    {
        navMesh.Teleport(raycastPoint);
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


