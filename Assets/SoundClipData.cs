using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoolSoundPack", menuName = "CoolSounds/CreaturePack")]
public class SoundClipData : ScriptableObject
{
    //The Sound of the Creature Walking(footsteps)
    public SoundClip Walking;
    //Creatures attack sound
    public SoundClip Attack;
    //Creature makes this type of sound when hurt
    public SoundClip Hurt;
    //Creature dies
    public SoundClip Death;

    public SoundClip Ability;


}
