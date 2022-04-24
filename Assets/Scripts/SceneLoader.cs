using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "SceneLoader", menuName = "Scene Loader")]
public class SceneLoader : ScriptableObject
{
    // Stores our loading scene.
    [SerializeField] string loadingSceneName;
    public AsyncOperation operation { get; private set; }

    /// <summary>
    /// Begins the loading process to specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load to.
    /// WARNING: The scene MUST be in build settings to work.</param>
    public void LoadScene(string sceneName)
    {
        // Load specified scene.
        LoadAsync(sceneName);
    }

    /// <summary>
    /// Asynchronously loads the scene in the background and loads up the loading screen. 
    /// </summary>
    /// <param name="sceneName">Name of the scene to load to.</param>
    void LoadAsync(string sceneName)
    {
        // Load the loading screen.
        SceneManager.LoadScene(loadingSceneName);

        // Create the async operation variable.
        operation = SceneManager.LoadSceneAsync(sceneName);
    }
}