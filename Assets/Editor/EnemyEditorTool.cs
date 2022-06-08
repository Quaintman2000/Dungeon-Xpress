using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class EnemyEditorTool : OdinMenuEditorWindow
{
    // A reference to an instance of enemy data.
    CreateNewEnemyData createNewEnemyData;

    // Allows us to have a button to open the editor window.
    [MenuItem("Maker Tools/Enemy Maker")]
    private static void OpenWindow()
    {
        GetWindow<EnemyEditorTool>().Show();
    }

    /// Cleans up the new enemy data instance for clean up.
    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Deletes the enemy data instance if it exists.
        if (createNewEnemyData != null)
            DestroyImmediate(createNewEnemyData.enemyData);
    }

    // Overrides to ondraw editor function to allow us to add a delete button so can delete datas easier.
    protected override void OnBeginDrawEditors()
    {
        // Reference to the menu tree.
        OdinMenuTreeSelection selected = this.MenuTree.Selection;
        // Creates a horizontal tool bar.
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();
            // Create the toolbar button for the delete currnet.
            if(SirenixEditorGUI.ToolbarButton("Delete Current"))
            {
                // Finds the path of the select asset and deletes it from the assets.
                EnemyData asset = selected.SelectedValue as EnemyData;
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }
        }
        // Ends the toolbar.
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        // Creates the navigation tree window.
        var tree = new OdinMenuTree();

        createNewEnemyData = new CreateNewEnemyData();
        // Creates the create new button to create a new enemy data.
        tree.Add("Create New", createNewEnemyData);
        // Adds the all the enemy datas into the navigation tree.
        tree.AddAllAssetsAtPath("Enemy Data", "Assets/Scripts/Scriptable Objects/Enemies", typeof(EnemyData));

        return tree;
    }

    // A class that allows us to make the enemy data.
    public class CreateNewEnemyData
    {
        public CreateNewEnemyData()
        {
            enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.EnemyName = "New Enemy Data";
        }
        [InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
        public EnemyData enemyData;

        [Button("Add New Enemy")]
        void CreateNewData()
        {
            AssetDatabase.CreateAsset(enemyData, "Assets/Scripts/Scriptable Objects/Enemies/" + enemyData.EnemyName +".asset");
            AssetDatabase.SaveAssets();

            enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.EnemyName = "New Enemy Data";
        }
    }
}
