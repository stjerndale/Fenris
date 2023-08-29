using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<PlantQuest> currentQuests;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text[] goals;

    private void Start()
    {
        currentQuests = new List<PlantQuest>();
        GameEvents.current.onQuestGoalUpdated += FindAndUpdateQuest;
    }

    public void AddNewQuest(QuestSO questSO)
    {
        if(IsQuestCurrentlyAssigned(questSO))
        {
            Debug.Log("Quest is already assigned and will not be assigned again.");
            return;
        }
        PlantQuest newQuest = new PlantQuest(questSO);
        currentQuests.Add(newQuest);
        UpdateQuestDescription(questSO);
        UpdateQuestGoals(newQuest);
    }

    private void UpdateQuestGoals(PlantQuest quest)
    {
        int goalIndex = 0;
        foreach(GoalSO goalSO in quest.questInfo.Goals)
        {
            goals[goalIndex].text = goalSO.plant.name + " (" + quest.GetProgress(goalIndex) + "/" + goalSO.goalAmount + ")";
            goalIndex++;
        }
    }

    private void FindAndUpdateQuest(String questID)
    {
        foreach (PlantQuest quest in currentQuests)
        {
            if(quest.questInfo.QuestID == questID)
            {
                UpdateQuestGoals(quest);
                return;
            }
        }
    }

    private void UpdateQuestDescription(QuestSO questSO)
    {
        description.text = questSO.description;
    }

    public bool IsQuestCurrentlyAssigned(QuestSO questSO)
    {
        foreach(PlantQuest quest in currentQuests)
        {
            if(quest.questInfo.QuestID == questSO.QuestID)
            {
                return true;
            }
        }
        return false;
    }

    void OnDisable()
    {
        GameEvents.current.onQuestGoalUpdated -= FindAndUpdateQuest;
    }
}
