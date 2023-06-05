using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<SeedStack> seedStacks = new List<SeedStack>();

    [SerializeField]
    FlowerInformation startingFlower; 
    [SerializeField]
    int startingAmount;

    [SerializeField]
    public int selectedSeed;

    void Start()
    {
        StartCoroutine(AddWithDelay(new SeedStack(startingFlower, startingAmount)));
        GameEvents.current.onNewSelection += UpdateSelectedSeed;
    }

    IEnumerator AddWithDelay(SeedStack stack)
    {
        yield return null;
        Add(stack);
    }

    public void Add(SeedStack stack)
    {
        seedStacks.Add(stack);
        GameEvents.current.StacksChanged(seedStacks);
    }
    public void Remove(SeedStack stack)
    {
        seedStacks.Remove(stack);
        GameEvents.current.StacksChanged(seedStacks);
    }

    public void DecreaseSeedStack(int id)
    {
        if(id < seedStacks.Count)
        {
            seedStacks[id].DecreaseSeeds();
            RemoveStackIfDepleted(seedStacks[id]);
        }
    }

    public void IncreaseStackBy(FlowerInformation flowerInfo, int number)
    {
        bool success = false;
        foreach(SeedStack stack in seedStacks)
        {
            if(stack.flowerInformation.id == flowerInfo.id)
            {
                stack.IncreaseSeedsBy(number);
                success = true;
            }
        }
        // if there is no stack of this seed type yet, create a new one
        if(!success)
        {
            Add(new SeedStack(flowerInfo, number));
        }
    }

    public void UpdateSelectedSeed(int index)
    {
        selectedSeed = index;
    }

    public SeedStack GetSelectedStack()
    {
        if (seedStacks.Count > selectedSeed)
        {
            return seedStacks[selectedSeed];
        }
        else return null;
    }

    public int GetStackSize()
    {
        return seedStacks.Count;
    }

    public void RemoveStackIfDepleted(SeedStack stack)
    {
        if(stack.NrOfSeeds == 0)
        {
            Remove(stack);
        }
    }

    private void OnDisable()
    {
        GameEvents.current.onNewSelection -= UpdateSelectedSeed;

    }
}
