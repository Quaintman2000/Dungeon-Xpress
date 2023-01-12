using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LoadingBar : MonoBehaviour
{
    [Tooltip("Set the sceneloader object here.")]
    [SerializeField] SceneLoader sceneLoader;
    [Tooltip("Set the slider that represents the loading bar here.")]
    [SerializeField] Slider loadingBarSlider;

    private void Update()
    {
        // Update the slider of the progress until it's done loading.
        if (!sceneLoader.operation.isDone)
        {
            // Adjust the progress values from 0 - 0.9 to 0 - 1.
            float progress = Mathf.Clamp01(sceneLoader.operation.progress / 0.9f);
            // Update the slider value to match the loading progress value.
            loadingBarSlider.value = progress;
        }
    }
}
