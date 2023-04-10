using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

public class JoinedLobbyPanelUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyCodeText;
    [SerializeField] Button leaveButton, readyButton, StartButton;
    [SerializeField] Transform playerDisplayTransform;
    [SerializeField] LobbyPlayerUI playerUIPrefab;

    List<LobbyPlayerUI> lobbyPlayerUIs = new List<LobbyPlayerUI>();


    private void Awake()
    {
        LobbyManager.instance.OnJoinedLobby += LoadLobby;
        LobbyManager.instance.OnJoinedLobbyUpdate += LoadLobby;

        readyButton.onClick.AddListener(HandleReadyClicked);
        leaveButton.onClick.AddListener(HandleLeaveClicked);
        StartButton.onClick.AddListener(HandleStartClicked);
    }
    public void LoadLobby()
    {
        Clear();

        try
        {
            Lobby createdLobby = LobbyManager.instance.GetLobby();

            lobbyCodeText.text = createdLobby.LobbyCode;

            foreach (Player player in createdLobby.Players)
            {
                LobbyPlayerUI newPlayerUI = Instantiate<LobbyPlayerUI>(playerUIPrefab, playerDisplayTransform);

                newPlayerUI.Intialize(
                    newPlayerID: player.Id,
                    newPlayerName: player.Data[LobbyManager.KEY_PLAYER_NAME].Value, 
                    newPlayerIconName: player.Data[LobbyManager.KEY_PLAYER_ICON].Value,
                    newReadyStatus: player.Data[LobbyManager.KEY_IS_READY].Value);

                lobbyPlayerUIs.Add(newPlayerUI);
            }
        }
        catch 
        {
            Debug.LogError("Failed to load lobby");
        }
    }

    void HandleReadyClicked()
    {
        string myId = LobbyManager.instance.GetPlayerID();
        Player myPlayer = LobbyManager.instance.GetPlayer(myId);

        if (myPlayer.Data[LobbyManager.KEY_IS_READY].Value == LobbyManager.ReadyStatus.IsReady.ToString())
        {
            LobbyManager.instance.UpdatePlayerReadyStatus(LobbyManager.ReadyStatus.NotReady);
        }
        else
        {
            LobbyManager.instance.UpdatePlayerReadyStatus(LobbyManager.ReadyStatus.IsReady);
        }
    }

    void HandleStartClicked()
    {
        Debug.Log("Start clicked");
        LobbyManager.instance.StartMatch();
    }

    void HandleLeaveClicked()
    {
       LobbyManager.instance.LeaveLobby();
    }

    LobbyPlayerUI GetOurPlayerUI()
    {
        try
        {
            for(int i = 0; i < lobbyPlayerUIs.Count ; i++)
            {
                if(lobbyPlayerUIs[i].GetOwnerID() == LobbyManager.instance.GetPlayerID())
                {
                   return lobbyPlayerUIs[i];
                }
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public void Clear()
    {
        if (lobbyPlayerUIs.Count < 1)
            return;

        foreach(LobbyPlayerUI playerUI in lobbyPlayerUIs)
        {
            Destroy(playerUI.gameObject);
        }

        lobbyPlayerUIs.Clear();
    }
    private void OnDestroy()
    {
        LobbyManager.instance.OnJoinedLobby -= LoadLobby;
        LobbyManager.instance.OnJoinedLobbyUpdate -= LoadLobby;
    }
}
