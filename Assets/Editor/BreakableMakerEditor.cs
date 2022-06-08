using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class BreakableMakerEditor : OdinMenuEditorWindow
{
    // A reference to a instance of the breakable data.
    CreateNewBreakable createNewBreakable;

    /// Allows us to have a button to open the editor window.
    [MenuItem("Maker Tools/Breakable Maker")]
    private static void OpenWindow()
    {
        GetWindow<BreakableMakerEditor>().Show();
    }
    
    /// Cleans up the new breakable data instance for clean up.
    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Deletes the breakable data instance if it exists.
        if (createNewBreakable != null)
            DestroyImmediate(createNewBreakable.breakableData);
    }

    /// Overrides to ondraw editor function to allow us to add a delete button so can delete datas easier.
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
                BreakableData asset = selected.SelectedValue as BreakableData;
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
        // Ends the toolbar.
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        // Creates the navigation tree window.
        var tree = new OdinMenuTree();

        createNewBreakable = new CreateNewBreakable();
        // Creates the create new button to create a new Breakable data.
        tree.Add("Create New", createNewBreakable);
        // Adds the all the Breakable datas into the navigation tree.
        tree.AddAllAssetsAtPath("Breakable Data", "Assets/Scripts/Scriptable Objects/Breakables", typeof(BreakableData));


        return tree;
    }

    //Creates a new asset in the files after the entered name and with its set properties
    public class CreateNewBreakable
    {
        public CreateNewBreakable()
        {
            breakableData = ScriptableObject.CreateInstance<BreakableData>();
            breakableData.BreakableName = "New Breakable Data";
        }
        [InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
        public BreakableData breakableData;

        [Button("Add New Breakable")]
        void CreateNewData()
        {
            AssetDatabase.CreateAsset(breakableData, "Assets/Scripts/Scriptable Objects/Breakables/" + breakableData.BreakableName +".asset");
            AssetDatabase.SaveAssets();

            breakableData = ScriptableObject.CreateInstance<BreakableData>();
            breakableData.BreakableName = "New Breakable Data";
        }
    }
}
