using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    protected List<QuestGoal> goalList;

    protected bool questComplete;
    protected bool questActive;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        questComplete = false;
        questActive = false;
        foreach (QuestGoal goal in goalList)
        {
            goal.Initialize(this);
        }
    }

    public virtual void updateQuestComplete()
    {
        foreach (QuestGoal goal in goalList)
        {
            if (goal.complete) continue;
            else return;
        }
        questComplete = true;
        Debug.Log("Quest Complete!");
    }

    public void ActivateQuest()
    {
        questActive = true;
        foreach (QuestGoal goal in goalList)
        {
            goal.ActivateGoal();
        }
    }
}
