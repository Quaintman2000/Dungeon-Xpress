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
        CharacterController characterController;
        if (TryGetComponent<CharacterController>(out characterController))
        {

            // Subscribe to the combat related events.
            characterController.CombatMoveToPointAction += MoveToPoint;
            characterController.UseAbilityAction += CheckCanUseAbility;
        }

        OnAbilityUsedEndAction += CheckEndTurn;
    }




    async void CheckCanUseAbility(CombatController other)
    {
        currentTarget = other;
        // We need to check to see if we can use this ability.
        // If the target is the activator and the target style is NOT self or Self and others, don't hit activator.
        if (currentTarget == this && (selectedAbilityData.TargetStyle != AbilityData.TargetType.Self && selectedAbilityData.TargetStyle != AbilityData.TargetType.SelfAndOthers))
            return;

        //When an enemy is selected with a ground ability it should try and sample the ground around to find a spot to land on.
        if (selectedAbilityData.TargetStyle == AbilityData.TargetType.Ground)
            //Player will land in front of where the enemy is facing 
            MovementSpot = other.transform.position + (other.transform.forward * 0.5f);


        Debug.Log("Distance between the player and target: " + Vector3.Distance(a: currentTarget.transform.position, b: transform.position) + " Units.");
        // If we have enough AP to use the ability and if we are in range...
        if (actionPoints >= selectedAbilityData.Cost && Vector3.Distance(a: currentTarget.transform.position, b: transform.position) <= selectedAbilityData.Range)
        {
            // Subtract the action points by the cost.
            actionPoints -= selectedAbilityData.Cost;
            
            // Call the event for those who need to know that we just used an ability.
            OnAbilityUsedStartAction?.Invoke();
            OnAbilityUsedAction?.Invoke(this);
            UIManager.Instance.UpdateActionPoints(actionPoints );
            // Grab the ability animation length and convert it to milleseconds.
            var animationLength = Mathf.RoundToInt(selectedAbilityData.AnimationClip.length * 1000);

            // Delay this function by the animation length.
            await Task.Delay(animationLength);

            // Tell those who are listening for the ability to end that the ability is over.
            OnAbilityUsedEndAction?.Invoke();
        }
    }

    // This is called during the ability animation to signal the ability functionality as they can start at different times.
    void Activate()
    {
        selectedAbilityData.Activate(this);
        selectedAbilityData = null;
    }


    #region Damage Healing and Death
    /// <summary>
    /// Moves the player into range of the target and deals damage to the target.
    /// </summary>
    /// <param name="other">The target.</param>
    /// <returns></returns>
    //public IEnumerator AttackMove(CombatController other)
    //{
    //    Debug.Log("ATTACK!");

    //    // If the player is not within close enough range and the player isnt already moving...
    //    if (Vector3.Distance(transform.position, other.transform.position) > selectedAbilityData.Range && (currentCombatState == CombatState.Idle || currentCombatState == CombatState.Attacking))
    //    {
    //        // Set the player to moving and move the player.
    //        currentCombatState = CombatState.Moving;
    //        navMesh.AttackMove(other.transform.position, selectedAbilityData.Range);

    //    }
    //    // While the player is still not within range...
    //    while (Vector3.Distance(transform.position, other.transform.position) > selectedAbilityData.Range)
    //    {
    //        // Keep moving.
    //        yield return null;
    //    }
    //    // Once we are in range of the target. Stop moving.
    //    navMesh.StopAllCoroutines();
    //    Debug.Log("Hiya!");
    //    // Set them to attacking and deal damage to the other combatant.
    //    currentCombatState = CombatState.Attacking;
    //    //other.TakeDamage(selectedAbilityData.PhysDamage);
    //    Debug.Log("Player took damage");
    //    if (animationManager != null)
    //        animationManager.SetAbilityTrigger(abilityIndex);

    //    AudioManager.instance.PlaySound(selectedAbilityData.AbilitySound, transform.position);

    //    // Set the combat state back to idle.
    //    currentCombatState = CombatState.Idle;
    //}

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

        // Subtract health by damage.
        Health -= damage;

        //updates the health bar
        OnHealthChange?.Invoke(Health);
        OnHurtAction?.Invoke();
        // If health is less than or equal to 0...
        if (Health <= 0)
        {
            // Commit die.
            Die();
        }
    }
    [ContextMenu("TakeDamage")]
    public void DebugTakeDamage()
    {
        OnHurtAction?.Invoke();
    }

    ///// <summary>
    ///// Applies the buff or debuff effects on self.
    ///// </summary>
    ///// <param name="abilityData"> The ability to apply.</param>
    //public void DebuffOrBuff(AbilityData abilityData)
    //{
    //    //Health += abilityData.PhysDamage;
    //    if (Health <= 99)
    //    {
    //        if (OnHealthChange != null)
    //        {
    //            //updates the health bar
    //            OnHealthChange.Invoke(Health);
    //        }
    //    }
    //}
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
    ///// <summary>
    ///// Shoots the projectile towards the target.
    ///// </summary>
    ///// <param name="other">The target</param>
    //void RangeAttack(CombatController other)
    //{
    //    // Need the direction from the player's position to the target position.
    //    Vector3 targetDirection = other.transform.position - transform.position;
    //    // Rotate the player to look at the target.
    //    transform.rotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
    //    // Spawns in the projectile at the firepoint position and direction towards the target.
    //    Projectile newProjectile = Instantiate<Projectile>(selectedAbilityData.Projectile, firePoint.position, Quaternion.LookRotation(targetDirection));
    //    // Set the projectile's target to our target.
    //    newProjectile.Target = other;
    //    // Set it's damage to be the ability's damage.
    //    newProjectile.Damage = selectedAbilityData.MagicDamage;
    //}
    /// <summary>
    /// Handles the movement ability like teleports and other special types of movement.
    /// </summary>
    /// <param name="other"> the Target</param>


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
    // Checks if our turn is over.
    private void CheckEndTurn()
    {
        // If action poinst is less than or equal to 0.
        if (actionPoints <= 0)
        {
            // Set isTurn to false.
            IsTurn = false;

            // Call the battlemanager to change turns.
            BattleManager.Instance.ChangeTurn();
        }
    }
    /// <summary>
    /// Debug function to the combatant's turn.
    /// </summary>
    [ContextMenu("End Turn")]
    private void EndTurn()
    {
        actionPoints = 0;
        CheckEndTurn();
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
        // Check for end turn.
        CheckEndTurn();
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


