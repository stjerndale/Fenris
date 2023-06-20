using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class QuestGoalPlanting : QuestGoal
{
    [SerializeField] public FlowerInformation FlowerInformation;

    public override void Initialize(Quest parentQuest)
    {
        base.Initialize(parentQuest);
        //GameEvents.current.onFlowerPlanted += updateGoal;
    }

    public override void UpdateGoalComplete()
    {
        if(progress >= goalAmount)
        {
            complete = true;
            quest.updateQuestComplete();
        }
    }

    public void UpdateGoal(FlowerInformation flowerInfo)
    {
        if(flowerInfo.id == FlowerInformation.id)
        {
            progress++;
            Debug.Log("progress: " + progress);
            UpdateGoalComplete();
        }
    }

    public override void ActivateGoal()
    {
        GameEvents.current.onFlowerPlanted += UpdateGoal;
    }

    void OnDisable()
    {
        GameEvents.current.onFlowerPlanted -= UpdateGoal;
    }
}
