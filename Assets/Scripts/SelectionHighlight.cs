using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHighlight : MonoBehaviour
{
    [SerializeField]
    private Color outlineColor;

    public void Select()
    {
        transform.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor", outlineColor);
    }

    public void DeSelect()
    {
        transform.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor", Color.black);
    }
}
