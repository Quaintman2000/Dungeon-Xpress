using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    // Sfx slider UI.
    [SerializeField] Slider sfxSlider;
    // Music slider UI.
    [SerializeField] Slider musicSlider;
    // Graphics dropdown TM UI.
    [SerializeField] TMP_Dropdown graphicsDropdown;
    // Brightness slider UI.
    [SerializeField] Slider brightnessSlider;
    // Resolution dropdown TM UI.
    [SerializeField] TMP_Dropdown resolutionDropdown;

    // Sound effects volume.
    float sfxVolume;
    // Music volume.
    float musicVolume;
    // Brightness value.
    float brightness;
    // Graphics current index.
    int graphicsIndex;
    // Resolution current index;
    int resolutionIndex;

    Resolution[] resolutions;

    [SerializeField] SceneLoader sceneLoader;
    

    private void Start()
    {
        // Upload saved settings data.
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        brightness = PlayerPrefs.GetFloat("Brightness");
        // Update the UI sliders to match.
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;
        brightnessSlider.value = brightness;
        // Update the graphics dropdown to the current setting.
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        // Set up the resolution dropbox.
        SetUpResolutionDropdown();

    }

    private void SetUpResolutionDropdown()
    {
        // Grab the available screen resolutions.
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        // Make a string list of options.
        List<string> options = new List<string>();
        // Make a current reslotion index variable.
        int currentResolutionIndex = 0;
        // For each resolution type...
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Add the option to the option list.
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            // If the option is our current resolution setting...
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                // Set the index to that.
                currentResolutionIndex = i;
            }
        }
        // Add the options to the dropdown and display the current set value.
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    // Adjust and save the SFX volume level.
    public void OnSFXSliderChanged()
    {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    // Adjust and save the Music volume level.
    public void OnMusicSliderChanged()
    {
        musicVolume = musicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    // Adjust and save the Brightness level.
    public void OnBrightnessSlider()
    {
        brightness = brightnessSlider.value;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }

    // Adjust and save the Graphics level.
    public void OnGraphicsDropdownValueChanged()
    {
        graphicsIndex = graphicsDropdown.value;
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
        PlayerPrefs.SetInt("GraphicsSetting", graphicsIndex);
    }

    // Adjust and save the resolution level.
    public void OnResolutionDropdownValueChanged()
    {
        resolutionIndex = resolutionDropdown.value;
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnMatchMakeButtonClicked()
    {
        // Have the sceneloader load the gameplay scene.
        sceneLoader.LoadScene("GameplayScene");
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
        Debug.Log("Left Application");
    }
}
