using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantingQuest : Quest
{
    [SerializeField] private List<QuestGoalPlanting> plantingGoalList;

    protected override void Start()
    {
        goalList = new List<QuestGoal>();
        foreach (QuestGoal goal in plantingGoalList)
        {
            goalList.Add(goal);
        }
        base.Start();
    }

    public override void updateQuestComplete()
    {
        base.updateQuestComplete();
    }
}
