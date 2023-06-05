
using UnityEngine;

[System.Serializable]
public class PlantSaveData
{
    public FlowerInformation plantInfo;
    public Flower.State state;

    public PlantSaveData( FlowerInformation info, Flower.State plantState)
    {
        plantInfo = info;
        state = plantState;
    }
}
