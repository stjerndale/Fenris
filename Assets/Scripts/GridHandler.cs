using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GridHandler : MonoBehaviour
{

    private int[] stats = new int[5];
    private int cols;
    private int rows;


    // Start is called before the first frame update
    void Start()
    {
        cols = transform.GetComponent<GridGenerator>().cols;
        rows = transform.GetComponent<GridGenerator>().rows;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SaveMap(SaveToJson saver)
    {
        Transform[] children = new Transform[transform.childCount];
        BoxLogic.Type[] types = new BoxLogic.Type[children.Length];
        bool[] dryness = new bool[children.Length];
        Flower[] plants = new Flower[children.Length];

        BoxLogic box;
        for (int i = 0; transform.childCount > i; i++)
        {
            box = transform.GetChild(i).GetComponent<BoxLogic>();
            types[i] = box.GetActiveType();
            dryness[i] = box.isDry();
            if(box.GetPlant()  != null)
            {
                plants[i] = box.GetPlant().GetComponent<Flower>();
            }
        }
        saver = new SaveToJson();
        saver.SaveMapToJson(types, dryness, plants);
        saver.SaveGameInformationToJson();
    }

    public void UpdateStats()
    {
        stats[0] = 0;
        stats[1] = 0;
        stats[2] = 0;
        stats[3] = 0;
        stats[4] = 0;
        BoxLogic.Type type;
        foreach (Transform box in transform) {
            type = box.GetComponent<BoxLogic>().GetActiveType();
            if(type == BoxLogic.Type.Ground || type == BoxLogic.Type.LowGround)
            {
                if (box.GetComponent<BoxLogic>().GetPlant() == null)
                {
                    stats[0]++;
                }
                else
                {
                    Flower.State state = box.GetComponent<BoxLogic>().GetPlant().GetComponent<Flower>().GetState();
                    if (state.Equals(Flower.State.Growing))
                    {
                        state = box.GetComponent<BoxLogic>().GetPlant().GetComponent<Flower>().GetPreviousState();
                    }
                    if (state.Equals(Flower.State.Seed))
                    {
                        stats[2]++;
                    }
                    if (state.Equals(Flower.State.Leaf))
                    {
                        stats[3]++;
                    }
                    if (state.Equals(Flower.State.Bloom))
                    {
                        stats[4]++;
                    }
                }
            }
            else if (type == BoxLogic.Type.Water)
            {
                stats[1]++;
            }
        }
    }

    public int[] GetStats()
    {
        return stats;
    }

    public Transform[] GetNeighbours(Transform box)
    {
        BoxLogic boxC = box.GetComponent<BoxLogic>();
        int[] coordinates = boxC.GetCoordinates();

        int index = coordinates[1] + coordinates[0] * rows;

        Transform[] neighbours = new Transform[8];

        neighbours[0] = GetNeighbour(box, index + rows); //north
        neighbours[1] = GetNeighbour(box, index + rows + 1); //north east
        neighbours[2] = GetNeighbour(box, index + 1); // east
        neighbours[3] = GetNeighbour(box, index + 1 - rows); // south east
        neighbours[4] = GetNeighbour(box, index - rows); //south
        neighbours[5] = GetNeighbour(box, index - rows - 1); //south west
        neighbours[6] = GetNeighbour(box, index - 1); // west
        neighbours[7] = GetNeighbour(box, index - 1 + rows); // north west


        return neighbours;
    }

    private Transform GetNeighbour(Transform box, int index)
    {
        if(index >= 0 && index < cols * rows - 1)
        {
            if(Mathf.Abs((box.position - transform.GetChild(index).position).magnitude) < 5)
            return transform.GetChild(index);
        }
        return null;
    }

}
