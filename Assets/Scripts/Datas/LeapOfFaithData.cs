using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Ability Data/Leap of Faith")]
public class LeapOfFaithData : AbilityData
{
    [SerializeField]
    float damage;
    [SerializeField]
    float maxHeight = 0.5f;
    [SerializeField]
    float minHeight = -0.5f;
    //Sperate from the range of the leap, used to adjust how far the damage is dealt by landing
    [SerializeField]
    float AOERadius = 1f;

    
    public override async Task Activate(CombatController combatcontroller)
    {
        combatcontroller.InstantMove(combatcontroller.MovementSpot);
        

        Debug.Log("Did leap of faith");

        // Initialize a list of combatants the will be hit.
        List<CombatController> hitCombatants = new List<CombatController>();
        // Create a overlapSphere around the player to detect those we hit with our spinning strike and store the colliders within it.
        var surroundingObjects = Physics.OverlapSphere(position: combatcontroller.transform.position + Vector3.up, radius: AOERadius);
        // For each object in our surrounding objects...
        foreach (var obj in surroundingObjects)
        {
            // Try to get their combatcontroller.
            // If they have one...
            if (obj.TryGetComponent<CombatController>(out CombatController combatant))
            {
                // If their position is within our min and max height and is not ourself...
                if (combatant.transform.position.y > minHeight && combatant.transform.position.y < maxHeight && combatant != combatcontroller)
                {
                    // Add the combatant to the list of hit combatants.
                    hitCombatants.Add(combatant);
                }
            }
        }

        //Damages every combatant in range of landing area.
        foreach(CombatController combatant in hitCombatants)
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
        /*
        //If the target is in height of the self and also in range then return true
        Vector3 selfPosition = self.transform.position;
        Vector3 targetPosition = target.transform.position;

        float distance = Vector3.Distance(selfPosition, targetPosition);
        if (targetPosition.y > minHeight && targetPosition.y < maxHeight && distance < range)
        {
            return true;
        }
        else
            return false;*/
    }
}
