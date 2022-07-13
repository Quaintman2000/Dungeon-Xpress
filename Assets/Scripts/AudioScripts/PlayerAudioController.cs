using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player adds additional sounds for voicelines when the player is doing a certain action and also can get a random line for certain events to introduce randomization.
/// </summary>
public class PlayerAudioController : CreatureAudioController
{
    [SerializeField] private ComplexCreatureAudioData playerAudio;
    //When the player starts a move during combat
    public void WalkLineSound()
    {
        AudioManager.instance.PlaySound(playerAudio.GetRandomMovementLine(), transform.position);
    }
    //Plays when the player choses an ability through the button in ui
    public void AbilityCastlineSound()
    {
        AudioManager.instance.PlaySound(playerAudio.GetRandomAbilityCast(), transform.position);
    }
    //Whenever the player walks they should play this sound and when they stop this sound should stop
    public override void WalkSound()
    {
        AudioManager.instance.PlaySound(playerAudio.Walking, transform.position);
    }
    //Whenever the player does a normal attack this should play
    public override void AttackSound()
    {
        AudioManager.instance.PlaySound(playerAudio.Attack, transform.position);
    }
    //When the player is hurt this sound should play
    public override void HurtSound()
    {
        AudioManager.instance.PlaySound(playerAudio.Hurt, transform.position);
    }
    public override void DeathSound()
    {
        AudioManager.instance.PlaySound(playerAudio.Death, transform.position);
    }
}
