using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Creature controller is used by creatures, enemies, etc. To be dragged into an enemies gameobject and is called upon by their aiController and combat controller.
/// </summary>
public class CreatureAudioController : AudioController
{
    [SerializeField] private CreatureAudioData audioData;

    protected virtual void Awake()
    {
        // If we have a navMeshmovement component...
        if(TryGetComponent<NavMeshMovement>(out NavMeshMovement navMeshMovement))
        {
            // Subscribe its action event(s).
           // navMeshMovement.WalkingAction += WalkSound;
        }
    }

    /// <summary>
    /// Plays the abilties sound when the ability is cast
    /// </summary>
    /// <param name="i"></param>
    /// <param name="targetPostion"></param>
    public void PlayAbilitySound(SoundData Ability)
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
