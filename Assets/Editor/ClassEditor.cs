using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;



[CustomEditor(typeof(ClassData))]
public class ClassEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
    }
}
