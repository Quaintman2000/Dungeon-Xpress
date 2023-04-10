using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool gameStarting = false;
   
    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

       Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
    public void StartMatch()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadGame();

        StartCoroutine(LoadGameRoutine());

        gameStarting = true;
    }

    IEnumerator LoadGameRoutine()
    {
        while (GetTotalLoadProgress() < 100)
            yield return null;

        SceneLoader.Instance.UnloadLoadingScreen();
    }
   
    public float GetTotalLoadProgress()
    {
        float totalLoadProgress = 0;

        if(SceneLoader.Instance != null)
            totalLoadProgress += SceneLoader.Instance.totalSceneProgress / 2;

        if(MatchManager.Instance != null)
        {
            totalLoadProgress += MatchManager.Instance.GameSetUpProgress/2;
        }

        return totalLoadProgress;
    }
}
