using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlantQuest
{
    [SerializeField] public QuestSO questInfo;
    private List<GoalSO> goals;

    [SerializeField] private int[] progress;

    private bool questActive;
    private bool questComplete;

    public PlantQuest(QuestSO questSO)
    {
        questInfo = questSO;
        Init();
    }

    void Init()
    {
        goals = questInfo.Goals;
        progress = new int[goals.Count];
        ActivateGoals();
    }

    public virtual void UpdateQuestComplete()
    {
        for(int i  = 0; i < progress.Length; i++)
        {
            if (goals[i].IsGoalComplete(progress[i])) continue;
            else return;
        }
        questComplete = true;
        DeactivateGoals();
        Debug.Log("Quest Complete!");
    }

    public void ActivateQuest()
    {
        questActive = true;
        foreach (GoalSO goal in goals)
        {
            //goal.ActivateGoal();
        }
    }

    // Updates goals of type Plant X flowers
    public void UpdatePlantingGoals(FlowerInformation flowerInfo)
    {
        for(int i = 0; i < progress.Length;i++)
        {
            if (goals[i].goalFlowerState == Flower.State.Seed && flowerInfo.id == goals[i].plant.id)
            {
                progress[i]++;
                UpdateGoalComplete(i);
            }
        }
    }

    // Updates goals of type Help X flowers bloom
    public void UpdateBloomingGoals(FlowerInformation flowerInfo)
    {
        for (int i = 0; i < progress.Length; i++)
        {
            if (goals[i].goalFlowerState == Flower.State.Bloom && flowerInfo.id == goals[i].plant.id)
            {
                progress[i]++;
                UpdateGoalComplete(i);
            }
        }
    }

    private void UpdateGoalComplete(int index)
    {
        if(goals[index].IsGoalComplete(progress[index]))
        {
            UpdateQuestComplete();
        }
    }

    public void ActivateGoals()
    {
        GameEvents.current.onFlowerPlanted += UpdatePlantingGoals;
        GameEvents.current.onFlowerBloomed += UpdateBloomingGoals;
    }

    private void DeactivateGoals()
    {
        GameEvents.current.onFlowerPlanted -= UpdatePlantingGoals;
        GameEvents.current.onFlowerBloomed -= UpdateBloomingGoals;
    }

    void OnDisable()
    {
        DeactivateGoals();
    }
}
