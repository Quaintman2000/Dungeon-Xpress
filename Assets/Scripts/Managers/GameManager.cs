using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public bool gameStarting = false;

    [SerializeField] MatchManager matchManagerPrefab;
    float totalLoadProgress;


    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

       Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
    public void StartMatch(int numPlayers)
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadGame();

            StartCoroutine(LoadGameRoutine());

            SceneLoader.Instance.OnGameplaySceneLoaded += SpawnMatchManager;

            gameStarting = true;
        }
    }

    IEnumerator LoadGameRoutine()
    {
        totalLoadProgress = GetTotalLoadProgress();
        while (totalLoadProgress < 100)
        {
            yield return null;
            totalLoadProgress = GetTotalLoadProgress();
        }

        SceneLoader.Instance.UnloadLoadingScreen();
    }
    
    void SpawnMatchManager()
    {
        if (LobbyManager.instance.IsLobbyHost())
        {
            MatchManager matchManager = Instantiate<MatchManager>(matchManagerPrefab);
            matchManager.NetworkObject.Spawn();
        }
        SceneLoader.Instance.OnGameplaySceneLoaded -= SpawnMatchManager;
    }
   
    public float GetTotalLoadProgress()
    {
        float totalLoadProgress = 0;

        if(SceneLoader.Instance != null)
            totalLoadProgress += SceneLoader.Instance.totalSceneProgress / 2;

        if(MatchManager.Instance != null)
        {
            totalLoadProgress += MatchManager.Instance.GameSetUpProgress.Value/2;
        }

        return totalLoadProgress;
    }
}
