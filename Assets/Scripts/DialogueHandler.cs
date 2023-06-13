using DS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private DSDialogue dialogueSystem;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private string speakerName;


    private void Start()
    {
        //OpenDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ProgressDialogue();
        }
    }

    public void OpenDialogue()
    {
        canvas.gameObject.SetActive(true);
        nameText.text = speakerName;
        text.text = dialogueSystem.StartDialogue();
    }

    public void ProgressDialogue()
    {
        string nextText = dialogueSystem.NextDialogue();
        if (nextText == "")
        {
            EndDialogue();
        }
        else
        {
            text.text = nextText;
        }
    }

    private void EndDialogue()
    {
        canvas.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            OpenDialogue();
        }
    }
}
