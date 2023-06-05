using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(PlayerInventory))]
public class PlayerInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        PlayerInventory inventory = (PlayerInventory)target;

        if(inventory.GetStackSize() > inventory.selectedSeed)
        {
            EditorGUILayout.LabelField("Seeds", inventory.GetSelectedStack().NrOfSeeds.ToString());
        }

        Repaint();
    }
}
