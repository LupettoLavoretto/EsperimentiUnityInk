using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
[Header("Dialogue UI")]
[SerializeField] private GameObject dialoguePanel;
[SerializeField] private TextMeshProUGUI dialogueText;

private Story currentStory;

public bool dialogueIsPlaying {get; private set;}


private static DialogueManager instance;
private void Awake()
{
    if(instance != null)
    {
        Debug.LogWarning("Trovati più di un Dialogue Manager nella scena");
    }
    instance = this;
}

public static DialogueManager GetInstance()
{
    return instance;
}

private void Start()
{
    dialogueIsPlaying = false;
    dialoguePanel.SetActive(false);
}

private void Update()
{
    if (!dialogueIsPlaying)
    {
        return;
    }
    //Se la giocatrice clicca il tasto per avanzare, la storia avanza
    if (InputManager.GetInstance().GetSubmitPressed())
    {
        ContinueStory();
    }
}

public void EnterDialogueMode(TextAsset inkJSON)
{
    //Qui diciamo ad Unity che quando entriamo in modalità "dialogo" la storia deve venire dal JSON di ink, che il dialogo è attivo e il panel visibile (l'interfaccia dove compare il testo)
    currentStory = new Story(inkJSON.text);
    dialogueIsPlaying = true;
    dialoguePanel.SetActive(true);

    ContinueStory();

}
    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
            if (currentStory.canContinue)
    {
        dialogueText.text = currentStory.Continue();
    }
    else
    {
        //Se non c'è testo, esci dal dialogo
        ExitDialogueMode();
    }
    }
}

