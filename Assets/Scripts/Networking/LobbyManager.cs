using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_ICON = "PlayerIcon";
    public const string KEY_IS_READY = "IsReady";

    public System.Action OnJoinedLobby, OnJoinedLobbyUpdate, OnLeaveLobby;

    public enum PlayerIcon
    {
        Knight,
        Wizard,
        Rogue,
        Paladin
    }

    public enum ReadyStatus
    {
        IsReady,
        NotReady
    }

    [SerializeField] float heartBeatTimerMax = 15;
    [SerializeField] float lobbyUpdateTimerMax = 1.1f;
    float heartBeatTimer = 0;
    float lobbyUpdateTimer = 0;
    Lobby joinedLobby;

    string playerName;

    private void Awake()
    {
        if (instance)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    // Start is called before the first frame update
    async void Start()
    {
        // Generate the random name for testing.
        playerName = "Quaintman#" + Random.Range(10, 99);
        // Turn on unity services.
        await UnityServices.InitializeAsync();
        // Write a log to show us we signed in.
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        // Sign in anonymously for testing.
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }
    /// <summary>
    /// Pings the lobby after a set seconds to keep the lobby active.
    /// </summary>
    async void HandleLobbyHeartbeat()
    {
        if(IsLobbyHost())
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer < 0)
            {
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    /// <summary>
    /// Ping the lobby for updates incase player joins or their info has been updated.
    /// </summary>
    async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                lobbyUpdateTimer = lobbyUpdateTimerMax;

              Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke();
            }
        }
    }


    public async Task<Lobby> CreateLobby()
    {
        try
        {
            // Test lobby parameters
            string lobbyName = "MyLobby";
            int maxPlayers = 4;
            // Create lobby options so we can have our player in.
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = MakePlayer()
            };
            // Make the lobby with inputted parameters.
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            // Set it as our joined lobby.
            joinedLobby = lobby;

            Debug.Log("Created Lobby");
            OnJoinedLobby?.Invoke();
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }
    /// <summary>
    /// Makes a player with data dictionary for Name, Icon, and ready status.
    /// </summary>
    /// <returns>A player object</returns>
    private Player MakePlayer()
    {
        return new Player()
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                        {KEY_PLAYER_ICON, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerIcon.Knight.ToString() ) },
                        {KEY_IS_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,ReadyStatus.NotReady.ToString())}
                    }
        };
    }
    /// <summary>
    /// Prints the # of lobbies found.
    /// </summary>
    public async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    /// <summary>
    /// Returns the player by ID
    /// </summary>
    /// <param name="id">ID of the player to be found.</param>
    /// <returns>Returns a player if they're in current lobby.</returns>
    public Player GetPlayer(string id)
    {
        try
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == id)
                    return player;
            }
        }
        catch
        {
            Debug.LogError("Cant find player: " + id);
        }
        return null;
    }
    // Returns our player ID
    public string GetPlayerID()
    {
        return AuthenticationService.Instance.PlayerId;
    }
    // Joins the first lobby in the list.
    public async void JoinLobby()
    {
        try
        {
            // Options containing our player.
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = MakePlayer()
            };
            // Gets the list of lobbies          
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            // Join the 1st lobby for testing.
            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id, joinLobbyByIdOptions);

            Debug.Log("Joined Lobby: " + queryResponse.Results[0].Id);
            OnJoinedLobby?.Invoke();
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    /// <summary>
    /// Joins lobby by lobby code.
    /// </summary>
    /// <param name="lobbyCode">The code for the lobby to join</param>
    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            // Options with our player.
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = MakePlayer()
            };
            // Join the coded lobby.
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            Debug.Log("Joined Lobby: " + lobbyCode);
            OnJoinedLobby?.Invoke();
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    /// <summary>
    /// Quick joins an open lobby.
    /// </summary>
    public async void QuickJoinLobby()
    {
        try
        {
            // Options with our player.
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions()
            {
                Player = MakePlayer()
            };
            // Quick joining a lobby.
            joinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);
            OnJoinedLobby?.Invoke();
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Prints all the players names and ids
    /// </summary>
    /// <param name="lobby">The lobby to find the players</param>
    public void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby: " + lobby.Name);

        foreach(Player player in lobby.Players)
        {
            Debug.Log("Player ID:" + player.Id + " Player Name: " + player.Data[KEY_PLAYER_NAME].Value);
        }
    }
    /// <summary>
    /// Makes our player leave the lobby.
    /// </summary>
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            OnLeaveLobby?.Invoke();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Returns our current lobby.
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
    /// <summary>
    /// Returns if we are in a lobby and if we are its host.
    /// </summary>
    /// <returns>Is host?</returns>
    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    /// <summary>
    /// Returns if everyone in the lobby is ready or not.
    /// </summary>
    /// <returns>Is everyone ready?</returns>
    public bool IsLobbyReady()
    {
        bool isAllReady = GetLobby().Players.TrueForAll(playerIsReady);

        bool playerIsReady(Player player)
        {
            return player.Data[KEY_IS_READY].Value == ReadyStatus.IsReady.ToString();
        }

        return isAllReady;
    }

    private void OnApplicationQuit()
    {
        if (joinedLobby != null)
        {
            LeaveLobby();
        }
    }

    #region UPDATE PLAYER FUNCTIONS
    /// <summary>
    /// Updates our player's name.
    /// </summary>
    /// <param name="newName"></param>
    public async void UpdatePlayerName(string newName)
    {
        // If it's the same name, don't bother.
        if (newName == playerName) return;

        try
        {
            playerName = newName;
            // Generate a update player options to input the new name.
            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions();

            updatePlayerOptions.Data = new Dictionary<string, PlayerDataObject>()
            {
                {KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newName)}
            };
            
            string playerID = GetPlayerID();
            // Update the player info.
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerID, updatePlayerOptions);
            joinedLobby = lobby;
            OnJoinedLobbyUpdate?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    /// <summary>
    /// Changes the player icon.
    /// </summary>
    /// <param name="newIcon"></param>
    public async void UpdatePlayerIcon(PlayerIcon newIcon)
    {
        try
        {
            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions();
            // Generate a update player options to input the new icon.
            updatePlayerOptions.Data = new Dictionary<string, PlayerDataObject>()
            {
                {KEY_PLAYER_ICON, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newIcon.ToString())}
            };

            string playerID = AuthenticationService.Instance.PlayerId;
            // Update the player info.
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerID, updatePlayerOptions);
            joinedLobby = lobby;
            OnJoinedLobbyUpdate?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    /// <summary>
    /// Changes the players ready status.
    /// </summary>
    /// <param name="newReadyStatus"></param>
    public async void UpdatePlayerReadyStatus(ReadyStatus newReadyStatus)
    {
        try
        {
            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions();
            // Generate a update player options to input the new ready status.
            updatePlayerOptions.Data = new Dictionary<string, PlayerDataObject>()
            {
                {KEY_IS_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newReadyStatus.ToString())}
            };

            string playerID = AuthenticationService.Instance.PlayerId;
            // Update the player info.
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerID, updatePlayerOptions);
            joinedLobby = lobby;
            OnJoinedLobbyUpdate?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    #endregion
}
