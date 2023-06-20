using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<Quest> currentQuests;

    private void Start()
    {
        currentQuests = new List<Quest>();
    }

    public void AddNewQuest(Quest newQuest)
    {
        currentQuests.Add(newQuest);
        newQuest.ActivateQuest();
    }
}
