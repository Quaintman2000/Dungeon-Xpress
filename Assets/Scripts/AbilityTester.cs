using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTester : MonoBehaviour
{
    [SerializeField]
    AbilityData abilityDataToTest;

    [SerializeField]
    CombatController player;

    [SerializeField]
    List<CombatController> enemies = new List<CombatController>();

  

    private void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(abilityDataToTest == null || player == null)
            return;

        Gizmos.color = Color.blue;
        
        Gizmos.DrawWireSphere(player.transform.position, abilityDataToTest.Range);

        Gizmos.color = abilityDataToTest.IsValidTarget(player, player) ? Color.green : Color.red;
        Gizmos.DrawCube(player.transform.position + Vector3.up, new Vector3(1, 1, 1));

        foreach (CombatController enemy in enemies)
        {
            Gizmos.color = abilityDataToTest.IsValidTarget(player,enemy) ? Color.green : Color.red;
            Gizmos.DrawCube(enemy.transform.position + Vector3.up, new Vector3(1, 1, 1));
        }
    }
}
