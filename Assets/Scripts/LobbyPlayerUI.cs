using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayerUI : MonoBehaviour
{
    [Header("Components:")]
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] Image playerIconImage, playerReadyImage;
    

    string ownerID;
    
    public void Intialize(string newPlayerID, string newPlayerName, string newPlayerIconName, string newReadyStatus)
    {
        ownerID = newPlayerID;
        SetNameText(newPlayerName);
        SetPlayerIconImage(newPlayerIconName);
        TogglePlayerReadyImage(newReadyStatus);
    }

    void SetNameText(string playerName)
    {
        playerNameText.text = playerName;
    }

    void SetPlayerIconImage(string newIconName)
    {
        playerIconImage.sprite = LobbyAssetHolder.Instance.GetPlayerIconSprite(newIconName);
    }

    void TogglePlayerReadyImage(string isReady)
    {
        playerReadyImage.sprite = LobbyAssetHolder.Instance.GetReadyStatusImage(isReady);
    }

    public string GetOwnerID()
    {
        return ownerID;
    }
}
