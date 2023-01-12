using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundController : MonoBehaviour
{
    //this gets the audio data from the soundclip script. 
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
        AudioManager.instance.PlaySound(Ability, transform.position);
    }
    public virtual void WalkSound()
    {
        AudioManager.instance.PlaySound(audioData.Walking, transform.position);
    }
    public virtual void AttackSound()
    {
        AudioManager.instance.PlaySound(audioData.Attack, transform.position);
    }
    public virtual void HurtSound()
    {
        AudioManager.instance.PlaySound(audioData.Hurt, transform.position);
    }
    public virtual void DeathSound()
    {
        AudioManager.instance.PlaySound(audioData.Death, transform.position);
    }
}
