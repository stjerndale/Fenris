
using UnityEngine;

[System.Serializable]
public class MapSaveData
{
    public BoxLogic.Type[] tileTypes;

    public bool[] tilesDryness;

    public PlantSaveData[] plants;

    public int TimeStamp;
}
