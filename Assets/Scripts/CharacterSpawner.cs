using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] CombatController charaterPrefab;
    [SerializeField] CharacterData characterData;

    private void Awake()
    {
        SpawnCharacter();
    }

    void SpawnCharacter()
    {
        CombatController newCombatant = Instantiate<CombatController>(charaterPrefab, this.transform.position, this.transform.rotation);
        newCombatant.CharacterData = characterData;
    }

}
