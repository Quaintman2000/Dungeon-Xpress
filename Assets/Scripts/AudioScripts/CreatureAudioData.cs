using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCreaturePack", menuName = "Sounds/CreaturePack")]
public class CreatureAudioData : ScriptableObject
{
    //The Sound of the Creature Walking(footsteps)
    public SoundData Walking;
    //Creatures attack sound
    public SoundData Attack;
    //Creature makes this type of sound when hurt
    public SoundData Hurt;
    //Creature dies
    public SoundData Death;

    
}
