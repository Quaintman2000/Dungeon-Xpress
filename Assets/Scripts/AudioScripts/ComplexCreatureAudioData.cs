using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in bosses, players and creatures who have voicelines
/// </summary>
[CreateAssetMenu(fileName = "NewComplexPack", menuName = "Sounds/ComplexCreaturePack")]
public class ComplexCreatureAudioData : CreatureAudioData
{
    //Creatures different sounds/voicelines when making a move
    public List<SoundData> movementLines = new List<SoundData>();
    //Creatures different sounds/voicelines when choosing an ability
    public List<SoundData> abilityCastLines = new List<SoundData>();

    //Gets a random sound for movement
    public SoundData GetRandomMovementLine()
    {
        return movementLines[Random.Range(0, movementLines.Count)];
    }
    //When a UI button for abilities is grabbed it plays a sound
    public SoundData GetRandomAbilityCast()
    {
        return abilityCastLines[Random.Range(0, abilityCastLines.Count)];
    }
}
