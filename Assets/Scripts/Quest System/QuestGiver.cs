using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] List<QuestSO> Quests;
    [SerializeField] QuestManager QuestManager;

    public void GiveQuest(string questID)
    {
        foreach (QuestSO quest in Quests)
        {
            if(quest.QuestID == questID)
            {
                QuestManager.AddNewQuest(quest);
                return;
            }
        }
        Debug.LogError("Tried to assign a quest that this NPC does not have.");
    }
}
