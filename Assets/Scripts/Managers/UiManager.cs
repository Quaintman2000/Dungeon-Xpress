using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    //References for Bars
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    //assign the bars to player accordingly
    void Start ()
    {
        AssignHealthBar();
        AssignManaBar();
    }

    //assign health bar of the player to its maximum value
    public void AssignHealthBar()
    {
        healthBar.fillAmount = GameManager.Instance.playerData.Health / GameManager.Instance.playerData.classData.MaxHealth;
    }

    public void AssignManaBar()
    {
        //manaBar.fillAmount = GameManager.Instance.playerData.Mana?
    }

    void Update ()
    {
        //damage test
        if(Input.GetKeyDown("space"))
        {
            GameManager.Instance.playerData.TakeDamage(10f);
        }
    }
}
