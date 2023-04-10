using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Linq;
public class MainMenuUIManager : MonoBehaviour
{
    //Variables 
    [SerializeField] Button QuickJoinButton;
    [SerializeField] Button HostGameButton;

    [SerializeField] JoinedLobbyPanelUI lobbyPanel;
    [SerializeField] GameObject mainMenuPanel;

    [SerializeField] AudioMixer masterMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider masterSlider;
    [SerializeField] Toggle fullScreenToggle;

    // Graphics dropdown TM UI.
    [SerializeField] TMP_Dropdown graphicsDropdown;
    // Brightness slider UI.
    [SerializeField] Slider brightnessSlider;
    // Resolution dropdown TM UI.
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] private bool isFullScreen;
    

    //Gets the name of the paramter to change the volume 
    const string Mixer_Music = "MusicVolume";
    const string Mixer_SFX = "SFXVolume";
    const string Mixer_Master = "MasterVolume";

    // Brightness value.
    float brightness;
    // Graphics current index.
    int graphicsIndex;
    
    private List<Resolution> filteredResolutions;
    // Resolution current index;
    int currentResolutionIndex = 0;
    float currentRefreshRate;
    Resolution[] resolutions;

    [SerializeField] SceneLoader sceneLoader;

    private void Awake()
    {
        resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.GetInt("FullScreenSetting") == 1)
        {
            fullScreenToggle.isOn = true;
            SetFullscreen(true);
        }
        else
        {
            fullScreenToggle.isOn = false;
            SetFullscreen(false);
        }

        HostGameButton.onClick.AddListener(HostGameClicked);
        QuickJoinButton.onClick.AddListener(QuickJoinedClicked);

        LobbyManager.instance.OnLeaveLobby += HandleLobbyLeave;
        
    }
    private void Start()
    {
        PlayerPrefs.GetFloat("MixerMusic");
        //will have the sliders listen in for the mixers functions and changed the volume 
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        PlayerPrefs.GetFloat("MixerSFX");
        sfxSlider.onValueChanged.AddListener(SetSFXVolume); 
        PlayerPrefs.GetFloat("MasterMusic");
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        brightness = PlayerPrefs.GetFloat("Brightness");
        brightnessSlider.value = brightness;
        PlayerPrefs.GetInt("GraphicsSetting");
        // Update the graphics dropdown to the current setting.
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        PlayerPrefs.GetInt("ResolutionsSetting");
        // Set up the resolution dropbox.
        SetUpResolutionDropdown();
    }

    public void QuickJoinedClicked()
    {
        HandleQuickJoinClicked();
    }

    public void HostGameClicked()
    {
        HandleHostGameClicked();
    }

    void HandleLobbyLeave()
    {
        lobbyPanel.gameObject.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void HandleQuickJoinClicked()
    {
        try
        {
            LobbyManager.instance.QuickJoinLobby();

            lobbyPanel.gameObject.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
        catch
        {
            Debug.LogError("Failed to quick join");
            HandleLobbyLeave();


        }
    }
    async void HandleHostGameClicked()
    {
        try
        {
            await LobbyManager.instance.CreateLobby();

            lobbyPanel.gameObject.SetActive(true);
            mainMenuPanel.SetActive(false);

        }
        catch
        {
            lobbyPanel.gameObject.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    private void SetUpResolutionDropdown()
    {
        // Grab the available screen resolutions.
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        // For each resolution type...
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }
        // Make a string list of options.
        List<string> options = new List<string>();

        for(int i = 0; i < filteredResolutions.Count; i++)
        {
         // Add the option to the option list.
         string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + " Hz ";
            options.Add(resolutionOption);
            
            // If the option is our current resolution setting...
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
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
        PlayerPrefs.SetFloat("MixerMusic", value);
    }
    private void SetSFXVolume(float value)
    {
        masterMixer.SetFloat(Mixer_SFX, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MixerSFX", value);
    }
    private void SetMasterVolume(float value)
    {
        masterMixer.SetFloat(Mixer_Master, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MixerMaster", value);
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
    public void OnResolutionDropdownValueChanged(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionsSetting", currentResolutionIndex);
      
       
    }

    // Use a boolean to set the screen to fullscreen
    public void SetFullscreen(bool shouldFullScreen)
    {
        isFullScreen = shouldFullScreen;
        Screen.fullScreen = shouldFullScreen;
        PlayerPrefs.SetInt("FullScreenSetting", shouldFullScreen ? 1 : 0);
       

    }

    public void OnMatchMakeButtonClicked()
    {
        // Have the sceneloader load the gameplay scene.
        
    }

   public void OnMainMenuButtonClicked()
    {
        //sceneLoader.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
        Debug.Log("Left Application");
    }
    private void OnDestroy()
    {
        LobbyManager.instance.OnLeaveLobby -= HandleLobbyLeave;
    }
}
