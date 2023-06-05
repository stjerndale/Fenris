using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxColour)), CanEditMultipleObjects]
public class BoxColourEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoxColour box = (BoxColour)target;

        if (GUILayout.Button("Become Water"))
        {
            box.BecomeWater();
        }
        if (GUILayout.Button("Become Ground"))
        {
            box.SetActiveType(BoxColour.Type.Ground);
        }
    }

}
