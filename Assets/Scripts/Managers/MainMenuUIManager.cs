using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    // Sfx slider UI.
    [SerializeField] Slider sfxSlider;
    // ambient slider UI.
    [SerializeField] Slider ambientSlider;
    // Music slider UI.
    [SerializeField] Slider musicSlider;
    // character slider UI.
    [SerializeField] Slider characterSlider;
    // enemy slider UI.
    [SerializeField] Slider enemySlider;

    // Graphics dropdown TM UI.
    [SerializeField] TMP_Dropdown graphicsDropdown;
    // Brightness slider UI.
    [SerializeField] Slider brightnessSlider;
    // Resolution dropdown TM UI.
    [SerializeField] TMP_Dropdown resolutionDropdown;

    // Music volume.
    float musicVolume;
    // Ambient volume.
    float ambientVolume;
    // Sound effects volume.
    float sfxVolume;
    // Music volume.
    float characterVolume;
    // Music volume.
    float enemyVolume;

    // Brightness value.
    float brightness;
    // Graphics current index.
    int graphicsIndex;
    // Resolution current index;
    int resolutionIndex;

    Resolution[] resolutions;

    [SerializeField] SceneLoader sceneLoader;

    [SerializeField] AudioController mainMenuAudio;
    private void Awake()
    {
        mainMenuAudio = GetComponent<AudioController>();
    }
    private void Start()
    {
        // Upload saved settings data.
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        ambientVolume = PlayerPrefs.GetFloat("AmbientVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        characterVolume = PlayerPrefs.GetFloat("CharacterVolume");
        enemyVolume = PlayerPrefs.GetFloat("EnemyVolume");

        brightness = PlayerPrefs.GetFloat("Brightness");
        // Update the UI sliders to match.
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;
        brightnessSlider.value = brightness;
        // Update the graphics dropdown to the current setting.
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        // Set up the resolution dropbox.
        SetUpResolutionDropdown();
        mainMenuAudio.PlaySound(0);

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
    public void OnMusicSliderChanged()
    {
        musicVolume = musicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        if (AudioManager.instance != null)
            AudioManager.instance.AdjustVolume(SoundType.music, musicSlider.value);
    }
    // Adjust and save the Music volume level.
    public void OnAmbientSliderChanged()
    {
        ambientVolume = ambientSlider.value;
        PlayerPrefs.SetFloat("AmbientVolume", musicVolume);
        if (AudioManager.instance != null)
            AudioManager.instance.AdjustVolume(SoundType.ambient, ambientSlider.value);
    }
    public void OnSFXSliderChanged()
    {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        if (AudioManager.instance != null)
            AudioManager.instance.AdjustVolume(SoundType.soundFX, sfxSlider.value);
    }
    // Adjust and save the Music volume level.
    public void OnCharacterSliderChanged()
    {
        characterVolume = characterSlider.value;
        PlayerPrefs.SetFloat("CharacterVolume", characterVolume);
        if (AudioManager.instance != null)
            AudioManager.instance.AdjustVolume(SoundType.character, characterSlider.value);
    }
    public void OnEnemySliderChanged()
    {
        enemyVolume = enemySlider.value;
        PlayerPrefs.SetFloat("EnemyVolume", enemyVolume);

        if (AudioManager.instance != null)
            AudioManager.instance.AdjustVolume(SoundType.enemy, enemySlider.value);
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
