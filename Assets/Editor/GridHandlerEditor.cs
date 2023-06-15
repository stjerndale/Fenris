using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridHandler))]
public class GridHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridHandler gridHandler = (GridHandler)target;
        int[] stats = gridHandler.GetStats();

        EditorGUILayout.LabelField("Ground", gridHandler.GetStats()[0].ToString());
        EditorGUILayout.LabelField("Water", gridHandler.GetStats()[1].ToString());
        EditorGUILayout.LabelField("Seeds", gridHandler.GetStats()[2].ToString());
        EditorGUILayout.LabelField("Blooms", gridHandler.GetStats()[3].ToString());
        EditorGUILayout.LabelField("Unknown", gridHandler.GetStats()[4].ToString());

        Repaint();
    }
}
