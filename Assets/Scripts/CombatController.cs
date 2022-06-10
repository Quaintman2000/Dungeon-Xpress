using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] int actionPoints;
    // Reference to the player Nav Mesh.
    [SerializeField] PlayerNavMesh playerNavMesh;
    // The close enough range to hit our target in melee.
    [SerializeField] float closeEnough = 0.1f;
    // Reference to the class data.
    public ClassData classData;
    public float Health;
    public bool IsTurn;
    // Reference to the player's combat state.
    public enum CombatState { Idle, Moving, Attacking };
    [SerializeField] public CombatState currentCombatState;

    public int abilityIndex = 0;
    [SerializeField] public AbilityData selectedAbilityData;

    [SerializeField] List<StatusEffect> statusEffects;

    [SerializeField] PlayerAnimationManager animationManager;

    public delegate void CombatantDeath( CombatController combatController);
    public event CombatantDeath OnCombatantDeath;

    // Start is called before the first frame update
    void Awake()
    {
        // Set the player's starting health to the max.
        Health = classData.MaxHealth;

        animationManager = GetComponent<PlayerAnimationManager>();
    }

  
    /// <summary>
    /// Uses the selected ability data and applies its functionality onto the target
    /// </summary>
    /// <param name="other">The combatant selected target.</param>
    public void UseAbility(CombatController other)
    {
        // The total length of the path from the player to the target.
        float distance = playerNavMesh.GetDistance(other.transform.position);
        // If we have any status effects...
        if (statusEffects.Count > 0)
        {
            // Apply the modifier to the total distance.
            distance /= statusEffects[0].Multiplier;
        }
        // Calculate the movement cost by how far along the path we have to travel.
        int movementCost = Mathf.RoundToInt(distance - selectedAbilityData.Range);
        // Clamp the value to 0 to prevent logic errors.
        movementCost = (int)Mathf.Clamp(movementCost, 0, Mathf.Infinity);


        Debug.Log("Movement Cost:" + movementCost);
        CheckCanUseAbility(other, movementCost);
        // Subtract ability cost from our action points.
        actionPoints -= selectedAbilityData.cost;
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
            animationManager.SetAbilityTrigger(abilityIndex);
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
            playerNavMesh.AttackMove(other.transform.position, selectedAbilityData.Range);

        }
        // While the player is still not within range...
        while (Vector3.Distance(transform.position, other.transform.position) > selectedAbilityData.Range)
        {
            // Keep moving.
            yield return null;
        }
        // Once we are in range of the target. Stop moving.
        playerNavMesh.StopAllCoroutines();
        Debug.Log("Hiya!");
        // Set them to attacking and deal damage to the other combatant.
        currentCombatState = CombatState.Attacking;
        other.TakeDamage(selectedAbilityData.Type != AbilityData.AbilityType.MeleeAttack ? selectedAbilityData.PhysDamage : classData.PhysicalDamage);

        // Set the combat state back to idle.
        currentCombatState = CombatState.Idle;
    }

    /// <summary>
    /// Reduces the player's health based on damage inputted.
    /// </summary>
    /// <param name="damage"> The amount of damage to be dealt.</param>
    public void TakeDamage(float damage)
    {
        Debug.Log("Ouch!");
        // Subtract health by damage.
        Health -= damage;
        //updates the health bar
        UIManager.Instance.AssignHealthBar();
        // If health is less than or equal to 0...
        if(Health <= 0)
        {
            // Commit die.
            Die();
        }
    }

    /// <summary>
    /// Applies the buff or debuff effects on self.
    /// </summary>
    /// <param name="abilityData"> The ability to apply.</param>
    private void DebuffOrBuff(AbilityData abilityData)
    {
        
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
        // If there's someone listening to this event...
        if (OnCombatantDeath != null)
            // Call the death event with this.
            OnCombatantDeath.Invoke(this);
    }
    // Starts the player's turn with their starting action points.
    public void StartTurn()
    {
        // Sets the isTurn to true.
        IsTurn = true;
        // Sets the action points to this class's starting action points.
        actionPoints = classData.StartingActionPoints;
    }
    // Checks if our turn is over.
    private void CheckEndTurn()
    {
        // If action poinst is less than or equal to 0.
        if (actionPoints <= 0)
        {
            // Set isTurn to false.
            IsTurn = false;
            // For each status effect in our list of status effects...
            for (int i = 0; i < statusEffects.Count; i++)
            {
                // Reduce the duration of that status effect by one.
                statusEffects[i].ReduceDuration();
                // If status effect's time has run out...
                if (statusEffects[i].EffectTime <= 0)
                    // Remove it from the list.
                    statusEffects.RemoveAt(i);
            }
            // Call the battlemanager to change turns.
            BattleManager.Instance.ChangeTurn();
        }
    }
    /// <summary>
    /// Moves to the raycast point and subtracts action points.
    /// </summary>
    /// <param name="raycastPoint"> The designated point.</param>
    public void MoveToPoint(Vector3 raycastPoint)
    {
        // Calculate length of the path.
        float distance = playerNavMesh.GetDistance(raycastPoint);
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
            playerNavMesh.SetMoveToMarker(raycastPoint);
        }
        // Check for end turn.
        CheckEndTurn();
    }
}


