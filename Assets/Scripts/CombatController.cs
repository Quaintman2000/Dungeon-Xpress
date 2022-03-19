using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] int actionPoints;
    // Reference to the player Nav Mesh.
    [SerializeField] PlayerNavMesh playerNavMesh;
    // The close enough range to hit our target in melee.
    [SerializeField] float closeEnough = 0.1f;
    // Reference to the class data.
    [SerializeField] ClassData classData;
    public float Health;
    // Reference to the player's combat state.
    enum CombatState { Idle, Moving, Attacking };
    [SerializeField] CombatState currentCombatState;

    [SerializeField] AbilityData selectedAbilityData;

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
        if (selectedAbilityData.cost <= actionPoints)
        {
            if (selectedAbilityData.BuffType == AbilityData.BuffOrDebuff.Buff || selectedAbilityData.BuffType == AbilityData.BuffOrDebuff.Buff)
            {
                //if(Vector3.Distance(transform.position, other.transform.position) <= selectedAbilityData.Range)
                //{
                //    other.DebuffOrBuff(selectedAbilityData);
                //}
            }

            if (selectedAbilityData.Type == AbilityData.AbilityType.MeleeAttack)
            {

                StartCoroutine(AttackMove(raycastPoint, other));
            }
            else if (selectedAbilityData.Type == AbilityData.AbilityType.Movement)
            {

            }
            else if (selectedAbilityData.Type == AbilityData.AbilityType.RangeAttack)
            {

            }
        }


    }

    public IEnumerator AttackMove(Vector3 raycastPoint, CombatController other)
    {
        Debug.Log("ATTACK!");

        // If the player is not within close enough range and the player isnt already moving...
        if (Vector3.Distance(transform.position, raycastPoint) > selectedAbilityData.Range && (currentCombatState == CombatState.Idle || currentCombatState == CombatState.Attacking))
        {
            // Set the player to moving and move the player.
            currentCombatState = CombatState.Moving;
            playerNavMesh.AttackMove(raycastPoint);

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
    }

    private void DebuffOrBuff(AbilityData abilityData)
    {
        
    }

    void RangeAttack(CombatController other)
    {

    }

    void Movement(CombatController other)
    {

    }

}


