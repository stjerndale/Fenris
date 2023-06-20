using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestGoal
{
    protected  Quest quest;
    public bool complete;

    [SerializeField] public int goalAmount;
    protected int progress;

    public virtual void Initialize(Quest parentQuest)
    {
        complete = false;
        quest = parentQuest;
    }

    public virtual void UpdateGoalComplete()
    {
        complete = false;
        return;
    }

    public virtual void ActivateGoal()
    {
        return;
    }
}
