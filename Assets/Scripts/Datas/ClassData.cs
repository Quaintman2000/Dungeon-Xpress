using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewClassData", menuName ="Classes/ClassData")]
public class ClassData : CharacterData
{
  

    private void OnValidate()
    {
        //// If we don't have an animation override controller, return.
        //if (animatorOverrideController == null)
        //    return;
        
        //// Grab the current clip overrides.
        //var currentClipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
        //animatorOverrideController.GetOverrides(currentClipOverrides);

        //// Set the new clip ovverrides.
        //var newClipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
        //int index = 0;
        //// For each clip pair in the current clip overrides...
        //foreach(var clipPair in currentClipOverrides)
        ////for(int i = 0; i< currentClipOverrides.Count && index < 4; i++)
        //{
        //    // If the key is a walk or idle, skip it.
        //    if (!clipPair.Key.name.Contains("Ability") || index >= Abilities.Length)
        //        continue;

        //    var newClip = Abilities[index].AnimationClip != null ? Abilities[index].AnimationClip : null;

        //    // Make a new pair with the old key and the new ability animation.
        //    var newClipPair = new KeyValuePair<AnimationClip, AnimationClip>(clipPair.Key, Abilities[index].AnimationClip);
        //    if(Abilities[index].AnimationClip != null)
        //        Debug.Log(newClipPair.Key.name + " , " + newClipPair.Value.name);
        //    // Add it to the new clip overrides.
        //    newClipOverrides.Add(newClipPair);
        //    // Increase the index for the abilites.
        //    index++;
        //}
        //// Apply the overrides.
        //animatorOverrideController.ApplyOverrides(newClipOverrides);
    }

}
