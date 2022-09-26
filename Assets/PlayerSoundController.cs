using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    //this gets the clips from the sounddata
    [SerializeField] private SoundClipData audioData;

    protected virtual void Awake()
    {

        // If we have a combat controller...
        if (TryGetComponent<CombatController>(out CombatController combatController))
        {
            combatController.OnDeathAction += DeathSound;
            combatController.OnHurtAction += HurtSound;
        }

    }
    public void PlayAbilitySound(SoundClip Ability)
    {
        AudioManagerTwo.instance.PlaySound(Ability, transform.position);
    }
    public void PlayAbility()
    {
        AudioManagerTwo.instance.PlaySound(audioData.Ability, transform.position);
    }
    public virtual void WalkSound()
    {
        AudioManagerTwo.instance.PlaySound(audioData.Walking, transform.position);
    }
    public virtual void AttackSound()
    {
        AudioManagerTwo.instance.PlaySound(audioData.Attack, transform.position);
    }
    public virtual void HurtSound()
    {
        AudioManagerTwo.instance.PlaySound(audioData.Hurt, transform.position);
    }
    public virtual void DeathSound()
    {
        AudioManagerTwo.instance.PlaySound(audioData.Death, transform.position);
    }
}
