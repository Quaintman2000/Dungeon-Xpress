using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] bool IsPlayerSpawner;
    [SerializeField] CombatController charaterPrefab;
    [SerializeField] CharacterData characterData;

    private void Start()
    {
        //if (IsPlayerSpawner)
            //MatchManager.Instance.playerSpawners.Add(this);

    }

    public void SpawnCharacter()
    {
        CombatController newCombatant = Instantiate<CombatController>(charaterPrefab, this.transform.position, this.transform.rotation);
        newCombatant.CharacterData = characterData;
    }

    public void SpawnPlayer(CombatController playerPrefab, CharacterData data)
    {
        charaterPrefab = playerPrefab;
        characterData = data;

        SpawnCharacter();
    }

    private void OnDrawGizmos()
    {
        // Set gizmo color to red.
        Gizmos.color = Color.red;
        // Draw a gizmo rectangle above the spawn point.
        Vector3 _cubeCenter = transform.position + (transform.up * 1);
        Vector3 _cubeSize = new Vector3(1, 2, 1);
        Gizmos.DrawCube(_cubeCenter, _cubeSize);
        // Draw a ray from the center to point foward.
        Gizmos.DrawRay(_cubeCenter, transform.forward);
    }

}
