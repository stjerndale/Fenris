using DS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private DSDialogue dialogueSystem;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private string speakerName;


    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceTexts;

    private bool dialogueOpened;


    private void Start()
    {
        //OpenDialogue();
        dialogueOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && dialogueOpened)
        {
            ProgressDialogue();
        }
    }

    public void OpenDialogue()
    {
        GameEvents.current.ImmobilizePlayer(true);

        canvas.gameObject.SetActive(true);
        ResetButtons();
        nameText.text = speakerName;
        text.text = dialogueSystem.StartDialogue();
        UpdateButtons();
        StartCoroutine(Cooldown());
    }

    public void ProgressDialogue(int choice = 0)
    {
        ResetButtons();

        string nextText = dialogueSystem.NextDialogue(choice);
        if (nextText == "")
        {
            EndDialogue();
        }
        else
        {
            text.text = nextText;
            UpdateButtons();
            StartCoroutine(Cooldown());
        }
    }

    private void UpdateButtons()
    {
        List<string> choices = dialogueSystem.Choices();
        if (choices.Count > 1)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];
            }
        }
    }

    private void ResetButtons()
    {
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void EndDialogue()
    {
        GameEvents.current.ImmobilizePlayer(false);
        canvas.gameObject.SetActive(false);
        dialogueOpened = false;
    }

    private void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.Space) && !dialogueOpened)
        {
            OpenDialogue();
        }
    }

    private IEnumerator Cooldown()
    {
        dialogueOpened = false;
        yield return new WaitForSeconds(0.3f);
        dialogueOpened = true;
    }
}
