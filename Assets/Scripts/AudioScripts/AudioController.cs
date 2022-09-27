using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The default audio controller for any object. Used by items, envioronment, etc. Should be set up to be called for two sound types when the objects controller call upon the audioController.
/// </summary>
public class AudioController : MonoBehaviour
{
    [SerializeField] private SoundData[] sounds;

    public void PlaySoundHere(int position)
    {
        AudioManager.instance.PlaySound(sounds[position], this.transform.position);
    }
    public void PlaySound(int position)
    {
        AudioManager.instance.PlaySound(sounds[position]);
    }

    
}
