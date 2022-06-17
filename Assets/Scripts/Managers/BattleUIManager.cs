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

    //Create a list to hold all of combatants images
    public List<Image> combatantsImages;

    //Prefab reference to the combatant images
    public GameObject combatantImagePrefab;

    //Reference to the current combatant's turn image
    public Image currentCombatantImage;

    public ClassData classData;

    //Update the current turn text
    public void SetTurnText()
    {
        turnText.text = "Turn: " + BattleManager.Instance.Combatants[BattleManager.Instance.CombatantsIndex].gameObject.name;
    }

    //Instantiate all of the needed images
    public void CreateTurnImages()
    {
        //For loop to iterate through the list of combatant images
        for (int i = 0; i < combatantsImages.Count; i++)
        {
            //Instantiate the prefab template for the images
            Instantiate(combatantImagePrefab);

            //Set the image sprites to the correct image
            currentCombatantImage.sprite = classData.CharacterImage;

            //Set the native size of the image to make sure it is within the desired size
            currentCombatantImage.SetNativeSize();

            //Set images as children of turn order images object
            combatantImagePrefab.transform.SetParent(turnOrderImagesParent);
        }
    }

    //Update the current turn character images
    public void UpdateTurnImages()
    {
        //Set the current combatant image to the the last sibling to move it to the end of the list.
        //The images are in a horizontal layout group, and will auto adjust.
        currentCombatantImage.transform.SetAsLastSibling();
    }
}
