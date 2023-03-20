using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Ability Data/Leap of Faith")]
public class LeapOfFaithData : AbilityData
{
    [Header("Leap Properties")]
    [SerializeField]
    float damage;
    [SerializeField]
    float maxHeight = 0.5f;
    [SerializeField]
    float minHeight = -0.5f;
    //Sperate from the range of the leap, used to adjust how far the damage is dealt by landing
    [SerializeField]
    float AOERadius = 1f;
    [Header("Leap Animation Length")]
    [SerializeField] //How long the player jumps for
    float leapTime = 0.93f;
    //The player uses the combat controller to move
    public override async Task Activate(CombatController jumper, RaycastData raycastData)
    {
        Debug.Log("started leap of faith");

        if(jumper.TryGetComponent<NavMeshMovement>(out NavMeshMovement movement) && raycastData!=null)
        {
            movement.StartLeap(raycastData.ImpactPoint, leapTime);
            await Task.Delay((int)(leapTime * 1000f)); //Current delay for current animation land on Sword Attack 1

            Land(jumper.transform); //Player lands and does damage
        }
    }
    //The stuff that happens when the player has landed
    public void Land(Transform player)
    {
        // Initialize a list of combatants the will be hit.
        List<CombatController> hitCombatants = new List<CombatController>();
        // Create a overlapSphere around the player to detect those we hit with our spinning strike and store the colliders within it.
        var surroundingObjects = Physics.OverlapSphere(position: player.position + Vector3.up, radius: AOERadius);
        // For each object in our surrounding objects...
        foreach (var obj in surroundingObjects)
        {
            // Try to get their combatcontroller.
            // If they have one...
            if (obj.TryGetComponent<CombatController>(out CombatController combatant))
            {
                // If their position is within our min and max height and is not ourself...
                if (combatant.transform.position.y > minHeight && combatant.transform.position.y < maxHeight && combatant.transform != player)
                {
                    // Add the combatant to the list of hit combatants.
                    hitCombatants.Add(combatant);
                }
            }
        }

        //Damages every combatant in range of landing area.
        foreach (CombatController combatant in hitCombatants)
        {
            combatant.TakeDamage(damage);
        }
    }
    public override bool IsValidTarget(CombatController self, CombatController target)
    {
        //Avoids calculations if target is self
        if (target.transform.position.y > minHeight && target.transform.position.y < maxHeight && target != self && Vector3.Distance(self.transform.position, target.transform.position) < range)
        {
            return true;
        }
        else
            return false;
       
    }
    
}
