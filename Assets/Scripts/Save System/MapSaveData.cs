
using UnityEngine;

[System.Serializable]
public class MapSaveData
{
    public BoxColour.Type[] tileTypes;

    public bool[] tilesDryness;

    public PlantSaveData[] plants;

    public int TimeStamp;
}
