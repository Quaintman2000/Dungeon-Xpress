using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Default Sound Data used by all sounds to be able to play in the AudioManager
/// </summary>
[CreateAssetMenu(fileName = "NewSound", menuName ="Sounds/DefaultSound")]
public class SoundData : ScriptableObject
{
    public AudioClip audio;
    [SerializeField] AudioMixer sfxMixer;
    public SoundType Type;
    //How loud the sound is
    //turn into slider
    [Range(0f, 1f)]
    public float Volume;
    //If the sound loops or it destroyes after one loop
    public bool DoesLoop;
}
