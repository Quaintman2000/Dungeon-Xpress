using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MatchManager : NetworkBehaviour
{
    //Reference to the BattleManager prefab
    [SerializeField] BattleManager battleManagerPrefab;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] PlayerController localPlayerController;
    [SerializeField] PlayerCharacter playerCharacterPrefab;

    public float GameSetUpProgress { get; private set; }

    //Instance of the MatchManager
    public static MatchManager Instance;

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

    private void Start()
    {
        StartCoroutine(SetupGameRoutine());
    }

    IEnumerator SetupGameRoutine()
    {
        if(mapGenerator != null)
        {
            mapGenerator.Generate();

            while(mapGenerator.progress < 100)
            {
                GameSetUpProgress = mapGenerator.progress;
                yield return null;
            }
        }
        else
        {
            GameSetUpProgress = 100;
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Start Host!");
        LoadInPlayerClientRpc(0);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Start Client!");
        LoadInPlayerClientRpc(1);
    }

    [ClientRpc]
    public void LoadInPlayerClientRpc(int playerNum)
    {
        Debug.Log("Start RPC called!");
        //if (playerNum == 0)
        //    NetworkManager.Singleton.StartHost();
        //else
        //    NetworkManager.Singleton.StartClient();

        PlayerCharacter player = Instantiate<PlayerCharacter>(playerCharacterPrefab);
        player.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

        player.SetController(localPlayerController);
        localPlayerController.SetPlayerCharacter(player);
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
