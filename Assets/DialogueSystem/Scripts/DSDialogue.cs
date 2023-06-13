using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS
{
    using ScriptableObjects;
    using System;

    public class DSDialogue : MonoBehaviour
    {
        /* Dialogue Scriptable Objects */
        [SerializeField] private DSDialogueContainerSO dialogueContainer;
        [SerializeField] private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;

        /* Filters */
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;

        /* Indexes */
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;

        DSDialogueSO currentDialogue;

        private void Awake()
        {
            currentDialogue = dialogue;
            //Debug.Log(dialogue.Text);
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                //NextDialogue();
            }
        }

        public string NextDialogue()
        {
            currentDialogue = currentDialogue.Choices[0].NextDialogue;

            if(currentDialogue == null )
            {
                currentDialogue = dialogue;
                return "";
            }

            //Debug.Log(currentDialogue.Text);

            return currentDialogue.Text;
        }

        internal string StartDialogue()
        {
            return currentDialogue.Text;
        }
    }

}