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
    enum CombatState { Idle, Moving, Attacking };
    [SerializeField] CombatState currentCombatState;

    [SerializeField] AbilityData selectedAbilityData;

    CharacterController controller;

    [SerializeField] List<StatusEffect> statusEffects;

    // Start is called before the first frame update
    void Start()
    {
        // Set the player's starting health to the max.
        Health = classData.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UseAbility(Vector3 raycastPoint, CombatController other)
    {
        float distance = playerNavMesh.GetDistance(raycastPoint);
        
        int movementCost = Mathf.RoundToInt(distance - selectedAbilityData.Range);
        movementCost = (int)Mathf.Clamp(movementCost, 0, Mathf.Infinity);

        if (statusEffects.Count > 0)
        {
            distance *= statusEffects[0].Multiplier;
        }

        Debug.Log("Movement Cost:" + movementCost);

        if (selectedAbilityData.cost <= actionPoints)
        {
            
            if (selectedAbilityData.BuffType == AbilityData.BuffOrDebuff.Buff || selectedAbilityData.BuffType == AbilityData.BuffOrDebuff.Buff)
            {
                if ((selectedAbilityData.TargetStyle == AbilityData.TargetType.Self ||
                     selectedAbilityData.TargetStyle == AbilityData.TargetType.SelfAndOthers) &&
                     other == this)
                {
                    DebuffOrBuff(selectedAbilityData);
                }
                else if ((selectedAbilityData.TargetStyle == AbilityData.TargetType.Others || selectedAbilityData.TargetStyle == AbilityData.TargetType.SelfAndOthers) && Vector3.Distance(transform.position, other.transform.position) <= selectedAbilityData.Range && other != this)
                {
                    other.DebuffOrBuff(selectedAbilityData);
                }
            }

            if (selectedAbilityData.Type == AbilityData.AbilityType.MeleeAttack)
            {
                if (selectedAbilityData.cost + movementCost <= actionPoints)
                {
                    actionPoints -= selectedAbilityData.cost + movementCost;
                    StartCoroutine(AttackMove(raycastPoint, other));
                }
            }
            else if (selectedAbilityData.Type == AbilityData.AbilityType.Movement)
            {

            }
            else if (selectedAbilityData.Type == AbilityData.AbilityType.RangeAttack)
            {

                RaycastHit hit;
                Vector3 targetDirection = other.transform.position - transform.position;
                if(Physics.Raycast(transform.position, targetDirection,out hit))
                {
                    Debug.DrawRay(transform.position, other.transform.position, Color.green);
                    Debug.Log("Fireball!");
                    RangeAttack(other);
                }
                else
                {
                    Debug.Log("miss fire");
                    Debug.DrawRay(transform.position,targetDirection, Color.red);
                }
                Debug.Log("sizzle.");
            }
        }

        CheckEndTurn();

    }

    public IEnumerator AttackMove(Vector3 raycastPoint, CombatController other)
    {
        Debug.Log("ATTACK!");

        // If the player is not within close enough range and the player isnt already moving...
        if (Vector3.Distance(transform.position, raycastPoint) > selectedAbilityData.Range && (currentCombatState == CombatState.Idle || currentCombatState == CombatState.Attacking))
        {
            // Set the player to moving and move the player.
            currentCombatState = CombatState.Moving;
            playerNavMesh.AttackMove(raycastPoint, selectedAbilityData.Range);

        }
        // While the player is still not within range...
        while (Vector3.Distance(transform.position, raycastPoint) > selectedAbilityData.Range)
        {
            yield return null;
        }
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
        Health -= damage;

        if(Health <= 0)
        {
            Die();
        }
    }

    private void DebuffOrBuff(AbilityData abilityData)
    {
        
    }

    void RangeAttack(CombatController other)
    {

        Vector3 targetDirection = other.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        Projectile newProjectile = Instantiate<Projectile>(selectedAbilityData.Projectile, firePoint.position, Quaternion.LookRotation(targetDirection));
        newProjectile.Target = other;
        newProjectile.Damage = selectedAbilityData.MagicDamage;
    }

    void Movement(CombatController other)
    {

    }

    void Die()
    {
        Debug.Log("Dead!");
    }

    public void StartTurn()
    {
        IsTurn = true;

        actionPoints = classData.StartingActionPoints;
    }
    private void CheckEndTurn()
    {
        if (actionPoints <= 0)
        {
            IsTurn = false;

            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i].ReduceDuration();
                if (statusEffects[i].EffectTime <= 0)
                    statusEffects.RemoveAt(i);
            }

        }
    }
    public void MoveToPoint(Vector3 raycastPoint)
    {

        float distance = playerNavMesh.GetDistance(raycastPoint);

        int movementCost = Mathf.RoundToInt(distance - selectedAbilityData.Range);
        movementCost = (int)Mathf.Clamp(movementCost, 0, Mathf.Infinity);
        if (movementCost <= actionPoints)
        {
            actionPoints -= movementCost;

            playerNavMesh.SetMoveToMarker(raycastPoint);
        }

        CheckEndTurn();
    }
}


