using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioMixer gameMixer;
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider ambientSlider;


    const string Mixer_Music = "Music";
    const string Mixer_SFX = "SFX";

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    private void SetMusicVolume(float value)
    {
        gameMixer.SetFloat(Mixer_Music, Mathf.Log10(value) * 20);
    }
    private void SetSFXVolume(float value)
    {
        sfxMixer.SetFloat(Mixer_Music, Mathf.Log10(value) * 20);
    }
}
