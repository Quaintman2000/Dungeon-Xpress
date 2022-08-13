using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] int actionPoints;
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

    public delegate void CombatantEvent(CombatController combatController);
     
    public event CombatantEvent OnCombatantDeath, OnStartTurn;

    public delegate void HealthChange(float health);
    public event HealthChange OnHealthChange;

    [SerializeField] PlayerAnimationManager animationManager;

    CreatureAudioController audioControl;
    InventoryManager inventoryManager;

    List<StatusEffect> statusEffects = new List<StatusEffect>();

    // Start is called before the first frame update
    void Awake()
    {
        navMesh = this.GetComponent<NavMeshMovement>();

        // Set the player's starting health to the max.
        Health = CharacterData.MaxHealth;
        animationManager = GetComponent<PlayerAnimationManager>();
        audioControl = GetComponent<CreatureAudioController>();

        if (TryGetComponent<InventoryManager>(out inventoryManager))
        {
            inventoryManager.OnUseItem += ApplyEffect;
        }
    }


    /// <summary>
    /// Uses the selected ability data and applies its functionality onto the target
    /// </summary>
    /// <param name="other">The combatant selected target.</param>
    public void UseAbility(CombatController other)
    {
        // The total length of the path from the player to the target.
        float distance = navMesh.GetDistance(other.transform.position);
        // If we have any status effects...
        //if (statusEffects.Count > 0)
        //{
        //    // Apply the modifier to the total distance.
        //    distance /= statusEffects[0].Multiplier;
        //}
        // Calculate the movement cost by how far along the path we have to travel.
        int movementCost = Mathf.RoundToInt(distance - selectedAbilityData.Range);
        // Clamp the value to 0 to prevent logic errors.
        movementCost = (int)Mathf.Clamp(movementCost, 0, Mathf.Infinity);


        Debug.Log("Movement Cost:" + movementCost);
        if (movementCost < actionPoints)
            CheckCanUseAbility(other, movementCost);

        // Check if we ran out of action points.
        CheckEndTurn();

    }

    /// <summary>
    /// Checks to see if we can use the ability.
    /// </summary>
    /// <param name="other">The target.</param>
    /// <param name="movementCost"> The cost to move for the ability. </param>
    private void CheckCanUseAbility(CombatController other, int movementCost)
    {
        // If we have enough action points for the ability...
        if (selectedAbilityData.cost <= actionPoints)
        {
            // If the ability has a buff or debuff effect...
            if (selectedAbilityData.BuffType == AbilityData.BuffOrDebuff.Buff || selectedAbilityData.BuffType == AbilityData.BuffOrDebuff.Buff)
            {
                // If the we can target ourselves or ourselves & others while also selecting us to use the ability...
                if ((selectedAbilityData.TargetStyle == AbilityData.TargetType.Self ||
                     selectedAbilityData.TargetStyle == AbilityData.TargetType.SelfAndOthers) &&
                     other == this)
                {
                    // Apply the buff or debuff onto self.
                    DebuffOrBuff(selectedAbilityData);
                    // Heal(15);
                    //other.Heal(selectedAbilityData.Type != AbilityData.AbilityType.Healing ? selectedAbilityData.PhysDamage : CharacterData.PhysicalDamage);
                }
                // Else, if we can target only others or ourselves and others while targeting someone else...
                else if ((selectedAbilityData.TargetStyle == AbilityData.TargetType.Others || selectedAbilityData.TargetStyle == AbilityData.TargetType.SelfAndOthers) && Vector3.Distance(transform.position, other.transform.position) <= selectedAbilityData.Range && other != this)
                {
                    // Apply the buff or debuff on them.
                    other.DebuffOrBuff(selectedAbilityData);
                }
            }
            // If we can target only others or ourselves and others while targeting someone else...
            if ((selectedAbilityData.TargetStyle == AbilityData.TargetType.Others || selectedAbilityData.TargetStyle == AbilityData.TargetType.SelfAndOthers) && other != this)
            {
                // If the ability is a melee type of ability...
                if (selectedAbilityData.Type == AbilityData.AbilityType.MeleeAttack)
                {
                    // If we have enough action points to cover the cost of the ability and to move to the target...
                    if (selectedAbilityData.cost + movementCost <= actionPoints)
                    {
                        // Subtract the movement cost from action points.
                        actionPoints -= movementCost;
                        // Start the attack move couroutine.
                        StartCoroutine(AttackMove(other));

                    }
                }
                // Else, if the ability was a movement type.
                else if (selectedAbilityData.Type == AbilityData.AbilityType.Movement)
                {
                    // TODO: for movement abilities like teleport.
                }
                // Else, if the ability is a range attack.
                else if (selectedAbilityData.Type == AbilityData.AbilityType.RangeAttack)
                {
                    // Save raycast hit info.
                    RaycastHit hit;
                    // Save the direction to the target.
                    Vector3 targetDirection = other.transform.position - transform.position;
                    // If our raycast hits anything...
                    if (Physics.Raycast(transform.position, targetDirection, out hit))
                    {
                        // If the collider we hit is the target's...
                        if (hit.collider == other.GetComponent<Collider>())
                        {
                            Debug.DrawRay(transform.position, other.transform.position, Color.green);
                            Debug.Log("Fireball!");
                            // Do range attack on target.
                            RangeAttack(other);
                            if (animationManager != null)
                                animationManager.SetAbilityTrigger(abilityIndex);
                        }
                    }
                    // Else, if we hit nothing.
                    else
                    {
                        // Do nothing.
                        Debug.Log("miss fire");
                        Debug.DrawRay(transform.position, targetDirection, Color.red);
                    }
                    Debug.Log("sizzle.");
                }
            }
            // Subtract ability cost from our action points.
            actionPoints -= selectedAbilityData.cost;

        }
    }

    /// <summary>
    /// Moves the player into range of the target and deals damage to the target.
    /// </summary>
    /// <param name="other">The target.</param>
    /// <returns></returns>
    public IEnumerator AttackMove(CombatController other)
    {
        Debug.Log("ATTACK!");

        // If the player is not within close enough range and the player isnt already moving...
        if (Vector3.Distance(transform.position, other.transform.position) > selectedAbilityData.Range && (currentCombatState == CombatState.Idle || currentCombatState == CombatState.Attacking))
        {
            // Set the player to moving and move the player.
            currentCombatState = CombatState.Moving;
            navMesh.AttackMove(other.transform.position, selectedAbilityData.Range);

        }
        // While the player is still not within range...
        while (Vector3.Distance(transform.position, other.transform.position) > selectedAbilityData.Range)
        {
            // Keep moving.
            yield return null;
        }
        // Once we are in range of the target. Stop moving.
        navMesh.StopAllCoroutines();
        Debug.Log("Hiya!");
        // Set them to attacking and deal damage to the other combatant.
        currentCombatState = CombatState.Attacking;
        other.TakeDamage(selectedAbilityData.PhysDamage);
        Debug.Log("Player took damage");
        if (animationManager != null)
            animationManager.SetAbilityTrigger(abilityIndex);

        AudioManager.instance.PlaySound(selectedAbilityData.AbilitySound, transform.position);

        // Set the combat state back to idle.
        currentCombatState = CombatState.Idle;
    }

    /// <summary>
    /// Reduces the player's health based on damage inputted.
    /// </summary>
    /// <param name="damage"> The amount of damage to be dealt.</param>
    public void TakeDamage(float damage)
    {
        audioControl.HurtSound();
        // Subtract health by damage.
        Health -= damage;
        if (OnHealthChange != null)
        {
            Debug.Log("Took Damage");
            //updates the health bar
            OnHealthChange.Invoke(Health);
        }
        // If health is less than or equal to 0...
        if (Health <= 0)
        {
            // Commit die.
            Die();
        }
    }

    /// <summary>
    /// Applies the buff or debuff effects on self.
    /// </summary>
    /// <param name="abilityData"> The ability to apply.</param>
    public void DebuffOrBuff(AbilityData abilityData)
    {
        Health += abilityData.PhysDamage;
        if (Health <= 99)
        {
            if (OnHealthChange != null)
            {
                //updates the health bar
                OnHealthChange.Invoke(Health);
            }
        }
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
    /// <summary>
    /// Shoots the projectile towards the target.
    /// </summary>
    /// <param name="other">The target</param>
    void RangeAttack(CombatController other)
    {
        // Need the direction from the player's position to the target position.
        Vector3 targetDirection = other.transform.position - transform.position;
        // Rotate the player to look at the target.
        transform.rotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        // Spawns in the projectile at the firepoint position and direction towards the target.
        Projectile newProjectile = Instantiate<Projectile>(selectedAbilityData.Projectile, firePoint.position, Quaternion.LookRotation(targetDirection));
        // Set the projectile's target to our target.
        newProjectile.Target = other;
        // Set it's damage to be the ability's damage.
        newProjectile.Damage = selectedAbilityData.MagicDamage;
    }
    /// <summary>
    /// Handles the movement ability like teleports and other special types of movement.
    /// </summary>
    /// <param name="other"> the Target</param>
    void Movement(CombatController other)
    {

    }

    [ContextMenu("Die")]
    // Handles the death procedure.
    void Die()
    {
        Debug.Log("Dead!");
        //Plays the death sound
        audioControl.DeathSound();
        // If there's someone listening to this event...
        if (OnCombatantDeath != null)
            // Call the death event with this.
            OnCombatantDeath.Invoke(this);
    }
    // Starts the player's turn with their starting action points.
    public void StartTurn()
    {
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
    /// <summary>
    /// Moves to the raycast point and subtracts action points.
    /// </summary>
    /// <param name="raycastPoint"> The designated point.</param>
    public void MoveToPoint(Vector3 raycastPoint)
    {
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
        // Check for end turn.
        CheckEndTurn();
    }

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
}


