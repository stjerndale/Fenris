using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxLogic)), CanEditMultipleObjects]
public class BoxColourEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoxLogic box = (BoxLogic)target;

        if (GUILayout.Button("Become Water"))
        {
            box.BecomeWater();
        }
        if (GUILayout.Button("Become Ground"))
        {
            box.SetActiveType(BoxLogic.Type.Ground);
        }
    }

}
