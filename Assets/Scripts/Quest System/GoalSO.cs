using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GoalSO", order = 3)]
public class GoalSO : ScriptableObject
{
    [field: SerializeField] public FlowerInformation plant { get; set; }
    [field: SerializeField] public int goalAmount { get; set; }
    [field: SerializeField] public Flower.State goalFlowerState { get; set; }

    public bool IsGoalComplete(int progress)
    {
        if (progress >= goalAmount)
        {
            return true;
        }
        return false;
    }
}
