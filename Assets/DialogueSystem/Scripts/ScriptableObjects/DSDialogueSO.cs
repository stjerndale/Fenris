using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class DSDialogueSO : ScriptableObject
    {
        [field: SerializeField]
        public string DialogueName { get; set; }

        [field: SerializeField]
        [field: TextArea()]
        public string Text { get; set; }

        [field: SerializeField]
        [field: TextArea()]
        public string QuestID { get; set; }

        [field: SerializeField]
        public List<DSDialogueChoiceData> Choices { get; set; }

        [field: SerializeField]
        public DSDialogueType DialogueType { get; set; }

        [field: SerializeField]
        public bool IsStartingDialogue { get; set; }

        public void Initialize(string dialogueName, string text, string questID, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            Text = text;
            QuestID = questID;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }

}
