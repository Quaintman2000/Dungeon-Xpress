using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class ClassMakerWindow : OdinMenuEditorWindow
{
    // A reference to a instance of class data.
    CreateNewClassData createNewClassData;
   
    /// <summary>
    /// Allows us to have a button to open the editor window.
    /// </summary>
    [MenuItem("Maker Tools/Class Maker")]
    private static void OpenWindow()
    {
        GetWindow<ClassMakerWindow>().Show();
    }
    /// <summary>
    /// Cleans up the new class data instance for clean up.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Deletes the class data instance if it exists.
        if (createNewClassData != null)
            DestroyImmediate(createNewClassData.classData);
    }
    /// <summary>
    /// Overrides to ondraw editor function to allow us to add a delete button so can delete datas easier.
    /// </summary>
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
                ClassData asset = selected.SelectedValue as ClassData;
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

        createNewClassData = new CreateNewClassData();
        // Creates the create new button to create a new class data.
        tree.Add("Create New", createNewClassData);
        // Adds the all the class datas into the navigation tree.
        tree.AddAllAssetsAtPath("Class Data", "Assets/Scripts/Scriptable Objects/Classes", typeof(ClassData));


        return tree;
    }
    /// <summary>
    /// A class that allows us to make the class data.
    /// </summary>
    public class CreateNewClassData
    {
        public CreateNewClassData()
        {
            classData = ScriptableObject.CreateInstance<ClassData>();
            classData.ClassName = "New Class Data";
        }
        [InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
        public ClassData classData;

        [Button("Add New Class")]
        void CreateNewData()
        {
            AssetDatabase.CreateAsset(classData, "Assets/Scripts/Scriptable Objects/Classes/" + classData.ClassName +".asset");
            AssetDatabase.SaveAssets();

            classData = ScriptableObject.CreateInstance<ClassData>();
            classData.ClassName = "New Class Data";
        }
    }
}
