using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public System.Action OnLoadingSceneOpened, OnGameplaySceneLoaded;
    public float totalSceneProgress { get; private set; }

    List<AsyncOperation> asyncOperations = new List<AsyncOperation>();

    Scene currentLoadingScene;

    Coroutine loadCoroutine;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadGame()
    {
        asyncOperations.Clear();

        if (loadCoroutine != null)
            StopCoroutine(loadCoroutine);
        loadCoroutine = StartCoroutine(LoadGameRoutine());

      
    }
    IEnumerator LoadGameRoutine()
    {
       AsyncOperation loadingScreenOperation = SceneManager.LoadSceneAsync((int)SceneIndexes.LOADING_SCREEN);
        bool IsLoadingScreenLoaded()
        {
            return loadingScreenOperation.isDone;
        }
        currentLoadingScene = SceneManager.GetSceneByName("LoadingScene");
        
        while(!IsLoadingScreenLoaded())
        {
            yield return null;
        }

       // asyncOperations.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_MENU_SCREEN));

        asyncOperations.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAMEPLAY_SCENE));

        StartCoroutine(SetSceneLoadProgress());
    }

    IEnumerator SetSceneLoadProgress()
    {
        for (int i = 0; i < asyncOperations.Count; i++)
        {
            while (!asyncOperations[i].isDone)
            {
                totalSceneProgress = 0;

                foreach(AsyncOperation operation in asyncOperations)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / asyncOperations.Count) * 100;

                yield return null;
            }
        }
        totalSceneProgress = 100;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameplayScene"));

        OnGameplaySceneLoaded?.Invoke();
    }

    public void UnloadLoadingScreen()
    {
        //SceneManager.UnloadSceneAsync(currentLoadingScene, UnloadSceneOptions.None);
    }
}
public enum SceneIndexes
{
    MAIN_MENU_SCREEN,
    LOADING_SCREEN,
    GAMEPLAY_SCENE,
    MAPGENERATION_SCENE
}