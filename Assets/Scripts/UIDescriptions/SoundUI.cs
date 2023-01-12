using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundUI : MonoBehaviour
{
    //Variables 
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider masterSlider;

    //Gets the name of the paramter to change the volume 
    const string Mixer_Music = "MusicVolume";
    const string Mixer_SFX = "SFXVolume";
    const string Mixer_Master = "MasterVolume";

    private void Awake()
    {
        //will have the sliders listen in for the mixers functions and changed the volume 
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    //changes the volume of the mixer 
    private void SetMusicVolume(float value)
    {
        masterMixer.SetFloat(Mixer_Music, Mathf.Log10(value) * 20);
    }
    private void SetSFXVolume(float value)
    {
        masterMixer.SetFloat(Mixer_SFX, Mathf.Log10(value) * 20);
    }
    private void SetMasterVolume(float value)
    {
        masterMixer.SetFloat(Mixer_Master, Mathf.Log10(value) * 20);
    }
}
