using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateGridMenuItem : MonoBehaviour
{

    //public static GameObject cube; //Cube you want to make a copy of, this will appear in the editor

    public static int cols = 10;
    public static int rows = 10;

    [MenuItem ("MyMenu/Generate 10 x 10")]
    static void Generate()
    {
        if (EditorUtility.IsPersistent(Selection.activeObject)) {
            GameObject activeGO = (GameObject) Selection.activeObject;

            for (var x = 0; x < rows; x++)
            {
                for (var z = 0; z < cols; z++)
                {
                    GameObject newCube = Instantiate(activeGO); //Creates an instance of the 'cube' object, think of this like a copy.
                    newCube.transform.position = new Vector3(x, 0, z); //Places the cube on the x and z which is updated in the for loops
                }
            }
        }
    }

}
