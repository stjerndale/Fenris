using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;

//[ExecuteAlways]
public class Flower : MonoBehaviour
{
    public enum State
    {
        Seed,
        Leaf,
        Bloom,
        Growing
    }

    protected FlowerRender[] renders;
    protected BoxLogic box;
    protected Renderer shadow;

    [SerializeField]
    protected State state = new State();
    private State previousState = new State();

    [SerializeField]
    public FlowerInformation flowerInfo;
    protected FlowerInformation.Requirements[] requirements;

    [SerializeField]
    private Animator animator;

    //[SerializeField]
    //private float seedScale = 1;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(BatchSprites());
    }

    protected void UpdateMaterial()
    {
        State visibleState = state;
        if (state == State.Growing)
        {
            visibleState = previousState;
            foreach (FlowerRender render in renders)
            {
                render.SetState(visibleState);
            }
        }
        else
        {
            StartCoroutine(PopAndGrow(visibleState));
        }

        /*foreach (FlowerRender render in renders)
        {
            render.SetState(visibleState);
        }*/
    }

    public void SetParentBox()
    {
        box = transform.parent.GetComponent<BoxLogic>();
    }

    public State GetState()
    {
        return state;
    }

    public void SetState(State newState)
    {
        previousState = state;
        state = newState;

        UpdateMaterial();

        if (state == State.Bloom)
        {
            StartCoroutine(SpreadLoop());
        }
    }

    public State GetPreviousState()
    {
        return previousState;
    }

    public bool isGrowing()
    {
        return state == State.Growing;
    }

    public State ProgressState(State status)
    {
        if (status == State.Seed)
        {
            return State.Leaf;
        }
        else
        {
            return State.Bloom;
        }
    }

    protected void ProgressGrowth()
    {
        if (state == State.Seed || state == State.Leaf)
        {
            SetState(State.Growing);
        }
        else if (isGrowing())
        {
            if(previousState == State.Seed)
            {
                SetState(State.Leaf);
            }
            else if (previousState == State.Leaf)
            {
                SetState(State.Bloom);
            }
        }

        box.UpdateStats();
    }

    virtual public IEnumerator Grow()
    {
        if (state == State.Bloom || !GrowthRequirementsFulfilled(box) || state == State.Growing)
        {
            yield break;
        }

        ProgressGrowth();
        yield return new WaitForSeconds(flowerInfo.growthSpeed * GameInformation.SecondsPerTick);

        if(this != null)
        {
            ProgressGrowth();
        }

        box.GoDry();
    }

    public void SetFlowerInfo(FlowerInformation info)
    {
        flowerInfo = info;
    }

    public virtual void SetupFlower(FlowerInformation info)
    {
        renders = new FlowerRender[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            renders[i] = transform.GetChild(i).GetComponent<FlowerRender>(); // plant
            renders[i].SetupFlowerPart(info);
        }
        flowerInfo = info;
        requirements = flowerInfo.growthRequirements;
        SetState(State.Seed);
        SetParentBox();
        box.UpdateStats();
    }

    protected virtual IEnumerator SpreadLoop()
    {
        while (state == State.Bloom)
        {
            yield return new WaitForSeconds(flowerInfo.spreadRate);
            float rnd = Random.Range(0f, 1f);
            if(rnd < flowerInfo.spreadChance)
            {
                Transform spreadBox = box.GetRandomEligibleNeighbour(this);
                if(spreadBox != null)
                {
                    spreadBox.GetComponent<BoxLogic>().PlantFlower(flowerInfo);
                }
            }
        }

    }

    public bool GrowthRequirementsFulfilled(BoxLogic boxToCheck)
    {
        if (requirements.Contains(FlowerInformation.Requirements.NearWater))
        {
            return boxToCheck.IsWaterAdjecent();
        }
        
        return true;
    }

    virtual public void DestroyPlant()
    {
        StopAllCoroutines();
        Destroy(transform.gameObject);
    }





    // Attempt at improving performance by batching the sprite's sorting order based on their distance to the camera
    private IEnumerator BatchSprites()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            float dist = Mathf.Abs((Camera.main.transform.position - transform.position).magnitude);
            switch(dist)
            {
                case float n when n > 15:
                    GoBadQuality(1);
                    break;
                case float n when n > 12:
                    GoBadQuality(2);
                    break;
                case float n when n > 9:
                    GoBadQuality(3);
                    break;
                case float n when n > 6:
                    GoBadQuality(4);
                    break;
                case float n when n > 3:
                    GoBadQuality(5);
                    break;
                default: 
                    GoBadQuality(10); 
                    break;
            }
        }
    }

    private void GoBadQuality(int sorting)
    {
        foreach (FlowerRender flower in renders)
        {
            flower.GetComponent<SpriteRenderer>().sortingOrder = sorting;
            flower.GetComponent<SpriteRenderer>().receiveShadows = false;
            flower.GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void GoHighQuality(int sorting)
    {
        foreach (FlowerRender flower in renders)
        {
            flower.GetComponent<SpriteRenderer>().sortingOrder = sorting;
            flower.GetComponent<SpriteRenderer>().receiveShadows = true;
            flower.GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    public virtual bool NeedsWaterToGrow()
    {
        return true;
    }

    private IEnumerator PopAndGrow(State nextState)
    {
        foreach (FlowerRender render in renders)
        {
            render.SetState(nextState);
        }
        animator.Play("Plant_Pop");
        yield return new WaitForSeconds(0.75f);
        animator.Play("Default");
    }
}
