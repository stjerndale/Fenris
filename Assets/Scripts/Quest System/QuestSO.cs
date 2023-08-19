using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuestSO", order = 2)]
public class QuestSO : ScriptableObject
{
    [field: SerializeField] public List<GoalSO> Goals;
    [SerializeField] public string QuestID;
    [field: SerializeField] public string description { get; set; }

    [SerializeField] private string NPC;
}
