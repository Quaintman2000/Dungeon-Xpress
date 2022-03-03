using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ClassEditorWindow : EditorWindow
{
    public static void Open(ClassData dataObject)
    {
        ClassEditorWindow window = GetWindow<ClassEditorWindow>("Class Editor");
    }
}
