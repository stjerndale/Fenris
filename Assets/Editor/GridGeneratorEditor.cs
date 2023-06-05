using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGenerator = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridGenerator.Generate();
        }

        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.red;

        if (GUILayout.Button("Destroy Grid", style))
        {
            gridGenerator.DestroyGrid();
        }
    }

}
