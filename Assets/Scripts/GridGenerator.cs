using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject cube; //Cube you want to make a copy of, this will appear in the editor
    public int cols;
    public int rows;

    public void Generate()
    {
        float cubeXLength = cube.transform.localScale.x;
        float cubeZLength = cube.transform.localScale.z;

        for (var x = 0; x < cols; x++)
        {
            for (var z = 0; z < rows; z++)
            {
                GameObject newCube = Instantiate(cube); //Creates an instance of the 'cube' object, think of this like a copy.
                newCube.transform.parent = transform;
                newCube.transform.localPosition = new Vector3(x * cubeXLength, 0, z * cubeZLength); //Places the cube on the x and z which is updated in the for loops
                newCube.GetComponent<BoxLogic>().SetCoordinates(x, z);
            }
        }
    }

    public void DestroyGrid()
    {
        for (int i = this.transform.childCount; i > 0; --i)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
    }


}
