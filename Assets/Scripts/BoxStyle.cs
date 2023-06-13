using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxStyle : MonoBehaviour
{
    [SerializeField] private BoxLogic boxLogic;
    [SerializeField] private Texture2D maskMid;
    [SerializeField] private Texture2D maskNW;
    [SerializeField] private Texture2D maskNE;
    [SerializeField] private Texture2D maskSW;
    [SerializeField] private Texture2D maskSE;

    public void UpdateTileStyle()
    {
        Transform[] neighbours = boxLogic.GetNeighbours();

        //neighbours[0].GetComponent<Renderer>().material.SetTexture("_Mask", maskSE);
        Renderer r = transform.GetComponent<Renderer>();

        /*
         * 0 - E
         * 1 - NE
         * 2 - N
         * 3 - NW
         * 4 - W
         * 5 - SW
         * 6 - S
         * 7 - SE
         */

        if (neighbours[0]?.GetComponent<BoxLogic>().GetActiveType() != BoxLogic.Type.Ground && neighbours[2]?.GetComponent<BoxLogic>().GetActiveType() != BoxLogic.Type.Ground)
        {
            r.material.SetTexture("_Mask", maskNE);
        }
    }
}
