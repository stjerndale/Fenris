using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class BoxColour : MonoBehaviour
{

    public enum Type
    {
        Ground,
        LowGround,
        Water
    }

    Renderer render;
    GridHandler gridHandler;

    [SerializeField]
    Material[] mats;

    [SerializeField]
    Type activeType = new Type();
    private Type previousType = new Type();

    [SerializeField]
    private bool dry;
    public bool passable;
    public bool changing;
    /*[SerializeField]
    private float dryTime = 10f;
    */

    [SerializeField]
    int[] coordinates = new int[2];

    public Transform plant;

    [SerializeField]
    private Transform[] neighbours;

    // Start is called before the first frame update
    void Start()
    {
        dry = true;
        passable = true;
        changing = false;
        render = GetComponent<MeshRenderer>();
        gridHandler = transform.parent.GetComponent<GridHandler>();

        UpdateMaterial();
        StartCoroutine(SetUpNeighbours());

        if (activeType == Type.Water)
        {
            Invoke("SpreadWater", 0.1f);
        }

        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Flower>() != null)
            {
                plant = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
            }
        }
    }

    public IEnumerator SetUpNeighbours()
    {
        yield return null;
        neighbours = gridHandler.GetNeighbours(transform);
    }

    private void UpdateMaterial()
    {
        if (activeType == Type.Ground || activeType == Type.LowGround)
        {
            render.material = mats[0];
            passable = true;
            if (! dry && activeType == Type.Ground)
            {
                Color newColor = render.material.color - new Color(0.1f, 0.1f, 0.1f, 0f);
                render.material.color = newColor;
            }
        }
        else if (activeType == Type.Water)
        {
            render.material = mats[1];
            passable = false;
        }
    }

    public Type GetActiveType()
    {
        return activeType;
    }

    public void SetActiveType(Type newType)
    {
        this.previousType = activeType;
        activeType = newType;
        UpdateBoxType();
    }

    public void RevertActiveType()
    {
        activeType = previousType;
        UpdateBoxType();
    }

    public void GoWet()
    {
        if (activeType == Type.Ground)
        {
            if (dry)
            {
                //Color newColor = render.material.color - new Color(0.1f, 0.1f, 0.1f, 0f);
                //render.material.color = newColor;
                dry = false;
                UpdateMaterial();
            }
            if(plant != null)
            {
                StartCoroutine(plant.GetComponent<Flower>().Grow());
            }
            //StartCoroutine(DryAfter(dryTime));
        }
    }

    // If nothing gets planted here before X seconds are up, the earth goes dry
    // Or if something is planted but cannot grow, also go dry
    /*private IEnumerator DryAfter(float seconds)
    {
        while (!dry)
        {
            yield return new WaitForSeconds(seconds);
            if (plant == null && activeType == Type.Ground)
            {
                GoDry();
            }
            else if (plant != null)
            {
                if (plant.GetComponent<Flower>().GetState() != Flower.State.Growing) // there is a plant but it's not growing
                {
                    GoDry();
                }
            }
        }
    }*/


    // Unless there is water nearby, go dry
    // If there is water, go dry then go wet again immediately
    public void GoDry()
    {
        if (!dry)
        {
            dry = true;

            if (IsWaterAdjecent())
            {
                GoWet();
            }
            else
            {
                UpdateMaterial();
            }
        }
    }

    public void SetDryness(bool dryness)
    {
        if (dryness)
        {
            GoDry();
        }
        else
        {
            GoWet();
        }
    }

    public bool GetDug(Transform playerTransform)
    {
        if (activeType == Type.Ground && isEmpty())
        {
            return BecomeLowGround();
        }
        else if (!isEmpty())
        {
            return DigOutFlower(playerTransform);
        }

        return false;
    }

    public bool BecomeWater()
    {
        SetActiveType(Type.Water);
        changing = false;
        SpreadWater();
        return true;
    }

    private bool BecomeLowGround()
    {
        if (IsDirectlyWaterAdjecent())
        {
            return BecomeWater();
        }
        SetActiveType(Type.LowGround);
        return true;
    }

    private void SpreadWater()
    {
        foreach (Transform n in neighbours)
        {
            if (n != null)
            {
                BoxColour box = n.GetComponent<BoxColour>();
                if (box.GetActiveType() == Type.LowGround && box.IsDirectlyWaterAdjecent() && !box.changing) // if a neighbour is low ground, spread the water there
                {
                    box.changing = true;
                    box.Invoke("BecomeWater", 0.3f);
                }
                n.GetComponent<BoxColour>().GoWet();
            }
        }
    }

    // Dig out a flower. If the flower has bloomed, recieve 3 seeds
    private bool DigOutFlower(Transform playerTransform)
    {
        int refill = 1;
        if(plant.GetComponent<Flower>().GetState() == Flower.State.Bloom)
        {
            refill = 3;
        }
        FlowerInformation info = plant.GetComponent<Flower>().flowerInfo;
        playerTransform.gameObject.GetComponent<PlayerInventory>().IncreaseStackBy(info, refill);
        DestroyLocalPlant();
        return true;
    }

    public void DestroyLocalPlant()
    {
        plant.GetComponent<Flower>().DestroyPlant();
        plant = null;
        UpdateStats();
    }

    public bool isDry()
    {
        return dry;
    }

    public bool isEmpty()
    {
        return plant == null;
    }

    // Adds a flower and if applicable, starts its growth
    public bool PlantFlower(FlowerInformation flowerInfo)
    {
        bool success = AddFlower(flowerInfo);

        if (!dry && success)
        {
            StartCoroutine(plant.GetComponent<Flower>().Grow());
        }

        return success;
    }

    // Simply add a flower on this box
    public bool AddFlower(FlowerInformation flowerInfo)
    {
        if (activeType == Type.Ground && flowerInfo != null && plant == null)
        {
            GameObject flowerPrefab = flowerInfo.prefab;
            GameObject flower = Instantiate(flowerPrefab);
            flower.transform.localPosition = new Vector3(transform.position.x, 0, transform.position.z);
            flower.transform.parent = transform;
            plant = flower.transform;

            Flower flowerScript = plant.GetComponent<Flower>();
            flowerScript.SetupFlower(flowerInfo);

            return true;
        }
        return false;
    }

    public void UpdateBoxType()
    {
        UpdateMaterial();

        UpdateStats();

        if(activeType != Type.Ground && previousType == Type.Ground)
        {
            transform.localPosition += Vector3.down * transform.localScale.y / 4f;
        }
        else if(previousType != Type.Ground && activeType == Type.Ground)
        {
            transform.localPosition += Vector3.up * transform.localScale.y / 4f;
        }

    }

    public void SetCoordinates(int x, int z)
    {
        coordinates[0] = x;
        coordinates[1] = z;
    }

    public int[] GetCoordinates()
    {
        return coordinates;
    }

    public Transform GetPlant() 
    { 
        if(plant == null)
        {
            return null;
        }
        return plant; 
    }

    public void UpdateStats()
    {
        gridHandler.UpdateStats();
    }

    public Transform GetRandomEligibleNeighbour(Flower flower)
    {
        List<Transform> neighs = new List<Transform>();
        int index = 0;

        foreach(Transform n in neighbours)
        {
            if(n != null)
            {
                neighs.Add(n);
            }
        }

        while (neighs.Count > 0)
        {
            index = Mathf.RoundToInt(Random.Range(0f, neighs.Count - 1));
            BoxColour box = neighs[index].GetComponent<BoxColour>();

            if (box.isEmpty() && flower.GrowthRequirementsFulfilled(box))
            {
                return neighs[index];
            }
            else
            {
                neighs.RemoveAt(index);
            }
        }
        return null;
    }

    // Returns true if any of the box's neighbours is a water tile
    public bool IsWaterAdjecent()
    {
        foreach (Transform n in neighbours)
        {
            if (n != null)
            {
                if(n.GetComponent<BoxColour>().GetActiveType() == Type.Water)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Returns true if any of the box's neighbours is a water tile
    public bool IsDirectlyWaterAdjecent()
    {
        for (int i = 0; i < neighbours.Count(); i++)
        {
            if (neighbours[i] != null && i % 2 == 0)
            {
                if (neighbours[i].GetComponent<BoxColour>().GetActiveType() == Type.Water)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
