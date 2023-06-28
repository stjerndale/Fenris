using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<PlantQuest> currentQuests;

    private void Start()
    {
        currentQuests = new List<PlantQuest>();
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
}
