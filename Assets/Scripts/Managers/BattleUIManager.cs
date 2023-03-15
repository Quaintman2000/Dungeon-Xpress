using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    //Reference to the current turn text object
    [SerializeField] Text turnText;

    [SerializeField]
    TextMeshProUGUI startEndText;

    [SerializeField]
    float displayTime;
    [SerializeField]
    string startTextMsg, endTextMsg;
    //Referecne to the turn order images parent object
    public Transform turnOrderImagesParent;

    //Prefab reference to the combatant images
    public GameObject combatantImagePrefab;

    //Reference to the current combatant's turn image
    public Image currentCombatantImage;

    //Update the current turn text
    public void SetTurnText()
    {
        turnText.text = "Turn: " + BattleManager.Instance.Combatants[BattleManager.Instance.CombatantsIndex % BattleManager.Instance.Combatants.Count].gameObject.name;
    }

    //Instantiate all of the needed images
    public void CreateTurnImages()
    {
        //For loop to iterate through the list of combatant images
        for (int i = 0; i < BattleManager.Instance.Combatants.Count; i++)
        {
            //Debug.Log("Begin Image For Loop.");
            //Instantiate the prefab template for the images and set the parent
            Image newImage = Instantiate(combatantImagePrefab.GetComponent<Image>(), turnOrderImagesParent);
            //Debug.Log("Image Instantiated and Parent Set.");

            //Set the image sprites to the correct image
            newImage.GetComponent<Image>().sprite = BattleManager.Instance.Combatants[i].CharacterData.CharacterImage; //.classData.CharacterImage
        }
    }

    //Update the current turn character images
    public void UpdateTurnImages()
    {
        //Set the current combatant image to the the last sibling to move it to the end of the list.
        //The images are in a horizontal layout group, and will auto adjust.
        turnOrderImagesParent.GetChild(0).SetAsLastSibling();
    }

    public void StartStartMatchRoutine()
    {
        StartCoroutine(DisplayBattleTextRoutine(displayTime, startTextMsg));
    }

    public Coroutine StartEndMatchRoutine()
    {
        return StartCoroutine(DisplayBattleTextRoutine(displayTime, endTextMsg));
    }

    IEnumerator DisplayBattleTextRoutine(float displayTime, string textToDisplay)
    {
        startEndText.gameObject.SetActive(true);
        startEndText.text = textToDisplay;
        Color originalColor = startEndText.color;
        float endTime = Time.time + displayTime;
        float alphaDegregationRate = originalColor.a / displayTime;

        Color newColor;
        float newAlpha;
        do
        {
            newAlpha = startEndText.color.a - alphaDegregationRate * Time.deltaTime;
            newAlpha = Mathf.Clamp(newAlpha, 0, 1);
            newColor = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            startEndText.color = newColor;
            yield return null;
        } while (Time.time < endTime);

        startEndText.gameObject.SetActive(false);
        startEndText.color = originalColor;
    }
}