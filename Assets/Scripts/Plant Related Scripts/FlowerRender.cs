using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerRender : MonoBehaviour
{
    [SerializeField]
    protected Sprite seed;

    [SerializeField]
    protected Sprite leaf;

    [SerializeField]
    protected Sprite bloom;

    [SerializeField]
    protected Texture mask;

    protected SpriteRenderer render;
    //protected Renderer shadow;

    [SerializeField]
    protected float offset = 0.2f;

    public virtual void SetupFlowerPart(FlowerInformation flowerInfo)
    {
        render = GetComponent<SpriteRenderer>();
        //shadow = transform.GetChild(0).GetComponent<Renderer>();
        seed = flowerInfo.seed;
        leaf = flowerInfo.leaf;
        bloom = flowerInfo.bloom;
        mask = flowerInfo.mask;
        Color colorOffset = new Color(Random.Range(-offset, offset), Random.Range(-offset, offset), Random.Range(-offset, offset));
        render.material.color = render.material.color + colorOffset;
        render.material.SetTexture("_ColorMask", mask);
    }

    public void SetState(Flower.State state)
    {
        switch (state)
        {
            case Flower.State.Leaf:
                render.sprite = leaf;
                //shadow.material.mainTexture = leaf;
                break;
            case Flower.State.Bloom:
                render.sprite = bloom;
                //shadow.material.mainTexture = bloom;
                break;
            default: //seed
                render.sprite = seed;
                //shadow.material.mainTexture = seed;
                break;
        }
    }
}
