using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "NewSlashData", menuName = "Ability Data/Slash Data")]
public class SlashData : AbilityData
{
    [SerializeField] float damage;
    public virtual async Task Activate(CombatController combatController)
    {

        combatController.currentTarget.TakeDamage(damage);
    }
}
