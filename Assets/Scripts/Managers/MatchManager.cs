using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MatchManager : NetworkBehaviour
{
    //Reference to the BattleManager prefab
    [SerializeField] BattleManager battleManagerPrefab;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] PlayerCharacter playerCharacterPrefab;

    public NetworkVariable<float> GameSetUpProgress = new NetworkVariable<float>(0);

    //Instance of the MatchManager
    public static MatchManager Instance;

    public List<CharacterSpawner> playerSpawners = new List<CharacterSpawner>();

    //Ensure there is only one instance of the Match Manager
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsHost)
            StartCoroutine(SetupGameRoutine());
    }

    IEnumerator SetupGameRoutine()
    {
        if(mapGenerator != null)
        {
            mapGenerator.Generate();

            while(mapGenerator.progress.Value < 100)
            {
                GameSetUpProgress.Value = mapGenerator.progress.Value;
                yield return null;
            }
        }
        else
        {
            GameSetUpProgress.Value = 50;
        }
        yield return null;
        while (NetworkManager.Singleton.ConnectedClientsList.Count != LobbyManager.instance.GetLobby().Players.Count)
        {
            yield return null;
        }

        for(int i =0; i < LobbyManager.instance.GetLobby().Players.Count; i++)
        {
            LoadInPlayerServerRpc(i);
        }
        GameSetUpProgress.Value = 100;
    }


    [ServerRpc]
    public void LoadInPlayerServerRpc(int playerNum)
    {
        Debug.Log("Start RPC called!");

        //NetworkManager.SpawnManager.


       PlayerCharacter player = Instantiate<PlayerCharacter>(playerCharacterPrefab);
       player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId: NetworkManager.Singleton.ConnectedClientsList[playerNum].ClientId, destroyWithScene: true);
        //player.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
    }

    /// <summary>
    /// Called when combat begins
    /// </summary>
    /// <param name="initiator">The combatant who causes combat to begin</param>
    /// <param name="other">Victim of the initiator</param>
    public void StartCombat(Controller initiator, Controller other)
    {
        //Create an instance of the BattleManager so combat can begin
        BattleManager newBattleManager = Instantiate<BattleManager>(battleManagerPrefab);

        //Set the initiator to the InCombat state and add them to the list of Combatants
        initiator.StartChangeState(PlayerState.InCombat);
        newBattleManager.Combatants.Add(initiator.combatController);

        //Set the other combatant to the InCombat state and add them to the list of Combatants
        other.StartChangeState(PlayerState.InCombat);
        newBattleManager.Combatants.Add(other.combatController);
    }
}
