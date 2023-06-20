using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedRender : FlowerRender
{
    public override void SetupFlowerPart(FlowerInformation flowerInfo)
    {
        render = GetComponent<SpriteRenderer>();
        //shadow = transform.GetChild(0).GetComponent<Renderer>();
        int i = Mathf.FloorToInt(Random.Range(0.0f, 3.0f));
        switch (i)
        {
            case 1: // no bloom
                seed = null;
                leaf = flowerInfo.seed;
                bloom = flowerInfo.leaf;
                break;
            case 2: // no leaf or bloom
                seed = null;
                leaf = null;
                bloom = flowerInfo.seed;
                break;
            default: //normal
                seed = flowerInfo.seed;
                leaf = flowerInfo.leaf;
                bloom = flowerInfo.bloom;
                break;
        }
    }
}
