using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MainMenuUIManager : MonoBehaviour
{
    //Variables 
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider masterSlider;

    // Graphics dropdown TM UI.
    [SerializeField] TMP_Dropdown graphicsDropdown;
    // Brightness slider UI.
    [SerializeField] Slider brightnessSlider;
    // Resolution dropdown TM UI.
    [SerializeField] TMP_Dropdown resolutionDropdown;

    //Gets the name of the paramter to change the volume 
    const string Mixer_Music = "MusicVolume";
    const string Mixer_SFX = "SFXVolume";
    const string Mixer_Master = "MasterVolume";

    // Brightness value.
    float brightness;
    // Graphics current index.
    int graphicsIndex;
    // Resolution current index;
    int resolutionIndex;

    Resolution[] resolutions;

    [SerializeField] SceneLoader sceneLoader;

    
    private void Awake()
    {
       
    }
    private void Start()
    {
        //will have the sliders listen in for the mixers functions and changed the volume 
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        brightness = PlayerPrefs.GetFloat("Brightness");
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
        sceneLoader.LoadScene("MapGenerationTestScene");
    }

   public void OnMainMenuButtonClicked()
    {
        sceneLoader.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
        Debug.Log("Left Application");
    }
}
