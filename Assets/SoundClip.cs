using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default Sound Data used by all sounds to be able to play in the AudioManager
/// </summary>
[CreateAssetMenu(fileName = "CreateSound", menuName = "SfxSounds/newSound")]
public class SoundClip : ScriptableObject
{
    public AudioClip sfxAudio;
    //If the sound loops or it destroyes after one loop
    public bool DoesLoop;
}
