using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumberPopUp : MonoBehaviour
{
    [SerializeField] float displayTime = 1.0f;
    [SerializeField] TextMeshProUGUI textUI;

    Coroutine displayRoutine;
    Color startingColor;

    private void Start()
    {
        if (textUI != null)
        {
            startingColor = textUI.color;
            textUI.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Starts the damage ui pop up routine.
    /// </summary>
    /// <param name="damageToDisplay"> The damage number to be displayed.</param>
    public void ActivatePopUp(float damageToDisplay)
    {
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        // Activate the text ui.
        textUI.gameObject.SetActive(true);

        displayRoutine = StartCoroutine(DisplayDamageNumberRoutine(damageToDisplay));
    }
    /// <summary>
    /// Displays the damage number and then gradually hides it while over the display time.
    /// </summary>
    /// <param name="displayValue"></param>
    /// <returns></returns>
    IEnumerator DisplayDamageNumberRoutine(float displayValue)
    {
        // Set the text for the Text UI.
        textUI.text = "-" + displayValue;
        // Set the text alpha to the original setting.
        textUI.color = startingColor;
        // Calculate the alpha depletion rate based on display time given.
        float alphaDepletionRate = startingColor.a / displayTime;
        // Run a do while timer loop for the length of the given display time while depleting the text alpha.
        float endTime = Time.time + displayTime;
        do
        {
            float newAlphaValue = textUI.color.a - alphaDepletionRate * Time.deltaTime;
            newAlphaValue = Mathf.Clamp(newAlphaValue, 0, 1);
            textUI.color = new Color(textUI.color.r, textUI.color.g, textUI.color.b, newAlphaValue);

            yield return null;
        } while (Time.time < endTime);
        // Deactivate the text once the timer is over.
        textUI.gameObject.SetActive(false);
    }
    
}
