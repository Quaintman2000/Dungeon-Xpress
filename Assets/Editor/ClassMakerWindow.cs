using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class ClassMakerWindow : OdinMenuEditorWindow
{
    CreateNewClassData createNewClassData;
   
    [MenuItem("Maker Tools/Class Maker")]
    private static void OpenWindow()
    {
        GetWindow<ClassMakerWindow>().Show();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (createNewClassData != null)
            DestroyImmediate(createNewClassData.classData);
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            //GUILayout.FlexibleSpace();

            if(SirenixEditorGUI.ToolbarButton("Delete Current"))
            {
                ClassData asset = selected.SelectedValue as ClassData;
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        createNewClassData = new CreateNewClassData();
        tree.Add("Create New", createNewClassData);
        tree.AddAllAssetsAtPath("Class Data", "Assets/Scripts/Scriptable Objects/Classes", typeof(ClassData));


        return tree;
    }

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
