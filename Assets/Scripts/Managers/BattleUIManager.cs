using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleUIManager : MonoBehaviour
{
    //Reference to the current turn text object
    [SerializeField] Text turnText;

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
}