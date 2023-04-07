using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAssetHolder : MonoBehaviour
{
    public static LobbyAssetHolder Instance;

    [Header("Sprites:")]
    [SerializeField] Sprite readyImageSprite;
    [SerializeField] Sprite notReadyImageSprite;
    [SerializeField] Sprite knightIconSprite;
    [SerializeField] Sprite rogueIconSprite;
    [SerializeField] Sprite wizardIconSprite;
    [SerializeField] Sprite paladinIconSprite;

    private void Awake()
    {
        if (Instance)
            Destroy(Instance.gameObject);
        else
            Instance = this;
    }

    public Sprite GetReadyStatusImage(string readyStatus)
    {
        if(readyStatus == LobbyManager.ReadyStatus.IsReady.ToString())
        {
            return readyImageSprite;
        }
        else if(readyStatus == LobbyManager.ReadyStatus.NotReady.ToString())
        {
            return notReadyImageSprite;
        }
        else
        {
            return null;
        }
    }
    public Sprite GetPlayerIconSprite(string iconName)
    {
        if(iconName == LobbyManager.PlayerIcon.Knight.ToString())
        {
            return knightIconSprite;
        }
        else if (iconName == LobbyManager.PlayerIcon.Wizard.ToString())
        {
            return wizardIconSprite;
        }
        else if (iconName == LobbyManager.PlayerIcon.Paladin.ToString())
        {
            return paladinIconSprite;
        }
        else if(iconName == LobbyManager.PlayerIcon.Rogue.ToString())
        {
            return rogueIconSprite;
        }
        else
        {
            return null;
        }

    }
}
