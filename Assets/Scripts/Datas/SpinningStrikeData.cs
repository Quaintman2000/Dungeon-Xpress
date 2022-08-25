using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Ability Data/Spinning Strike")]
public class SpinningStrikeData : AbilityData
{
    [SerializeField]
    float damage;
    [SerializeField]
    float maxHeight = 0;
    [SerializeField]
    float minHeight = 0;
    public override async Task Activate(CombatController activatorCombatController)
    {
        // Initialize a list of combatants the will be hit.
        List<CombatController> hitCombatants = new List<CombatController>();

        // Create a overlapSphere around the player to detect those we hit with our spinning strike and store the colliders within it.
        var surroundingObjects = Physics.OverlapSphere(position: activatorCombatController.transform.position + Vector3.up, radius: range);
        // For each object in our surrounding objects...
        foreach(var obj in surroundingObjects)
        {
            // Try to get their combatcontroller.
            // If they have one...
            if(obj.TryGetComponent<CombatController>(out CombatController combatant))
            {
                // If their position is within our min and max height and is not ourself...
                if(combatant.transform.position.y > minHeight && combatant.transform.position.y < maxHeight && combatant != activatorCombatController)
                {
                    // Add the combatant to the list of hit combatants.
                    hitCombatants.Add(combatant);
                }
            }
        }

        // Now tell all of them to take damage.
        // For each combatant at index "i", in our list...
        for (int i = 0; i < hitCombatants.Count; i++)
        {
            // Take the set damage.
            hitCombatants[i].TakeDamage(damage);
        }
    }
}
