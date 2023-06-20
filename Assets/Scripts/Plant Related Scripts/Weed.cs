using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FlowerInformation;
using UnityEngine.UIElements;

public class Weed : Flower
{
    public override void SetupFlower(FlowerInformation info)
    {
        //render = GetComponent<MeshRenderer>();
        //shadow = transform.parent.GetChild(1).GetComponent<MeshRenderer>();
        /*renders = new FlowerRender[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            renders[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
        }
        flowerInfo = info;
        requirements = flowerInfo.growthRequirements;
        SetState(State.Seed);
        SetParentBox();
        box.UpdateStats();
        UpdateMaterial();*/

        base.SetupFlower(info);
        StartCoroutine(GrowWeed());
    }

    private IEnumerator GrowWeed()
    {
        while(state  != State.Bloom)
        {
            yield return StartCoroutine(Grow());
        }
    }

    public override IEnumerator Grow()
    {
        StartCoroutine( base.Grow());
        yield return null;
    }

    public override void DestroyPlant()
    {
        StopAllCoroutines();
        //Destroy(gameObject);
        Destroy(transform.gameObject);
    }

    public override bool NeedsWaterToGrow()
    {
        return false;
    }
}
