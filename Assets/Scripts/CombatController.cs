using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
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

    public IEnumerator Attack()
    {
        Debug.Log("ATTACK!");
        //Cast a ray from our camera toward the plane, through our mouse cursor
        float distance;
        // Hit info from the raycast.
        RaycastHit hit;
        // Makes the raycast from our mouseposition to the ground.
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Sends the raycast of to infinity until hits something.
        Physics.Raycast(cameraRay, out hit, Mathf.Infinity);

        // Grab the distance of the position we hit to get the point along the ray.
        distance = hit.distance;

        //Find where that ray hits the plane
        Vector3 raycastPoint = cameraRay.GetPoint(distance);
        // If we hit a combatant...
        if (hit.collider.GetComponent<CombatController>())
        {
            Debug.Log("Target Locked!");
            // Set the combatant as other.
            CombatController other = hit.collider.GetComponent<CombatController>();
            // If the combatant isnt us...
            if (other != this)
            {
                // If the player is not within close enough range and the player isnt already moving...
                if (Vector3.Distance(transform.position, raycastPoint) > closeEnough && (currentCombatState == CombatState.Idle || currentCombatState == CombatState.Attacking))
                {
                    // Set the player to moving and move the player.
                    currentCombatState = CombatState.Moving;
                    playerNavMesh.AttackMove(raycastPoint);
                    
                }
                // While the player is still not within range...
                while (Vector3.Distance(transform.position, raycastPoint) > closeEnough)
                {
                    yield return null;
                }
                Debug.Log("Hiya!");
                // Set them to attacking and deal damage to the other combatant.
                currentCombatState = CombatState.Attacking;
                other.TakeDamage(classData.PhysicalDamage);
            }
        }
        // If we didn't hit a combatant.
        else
        {
            Debug.Log("Not a valid target");
        }
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
}
