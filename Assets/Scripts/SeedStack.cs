using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedStack
{
    [SerializeField]
    public FlowerInformation flowerInformation;
    public int NrOfSeeds;

    public SeedStack(FlowerInformation flowerInformation, int NrOfSeeds)
    {
        this.flowerInformation = flowerInformation;
        this.NrOfSeeds = NrOfSeeds;
    }

    public void DecreaseSeeds()
    {
        NrOfSeeds--;
    }

    public void IncreaseSeedsBy(int number)
    {
        NrOfSeeds = NrOfSeeds + number;
    }
}
