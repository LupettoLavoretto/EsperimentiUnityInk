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

[Header("Choices UI")]
[SerializeField] private GameObject[] choices;
private TextMeshProUGUI[] choicesText;

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

    //prendi tutte le scelte testuali
    choicesText = new TextMeshProUGUI[choices.Length];
    int index = 0;
    foreach (GameObject choice in choices)
    {
        choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
        index ++;
    }

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
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
            if (currentStory.canContinue)
    {
        dialogueText.text = currentStory.Continue();
        // mostra le scelte, se esistono, per questa linea di dialogo
        DisplayChoices();
    }
    else
    {
        //Se non c'è testo, esci dal dialogo
        StartCoroutine(ExitDialogueMode());
    }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        //Un defensive check per vedere se abbiamo troppe scelte rispetto alle opzioni strutturate nella UI
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Ci sono più scelte di quante la UI possa supportare. Le scelte presenti sono:" + currentChoices.Count);
        }

    int index = 0;
    foreach (Choice choice in currentChoices)
    {
        choices[index].gameObject.SetActive(true);
        choicesText[index].text = choice.text;
        index ++;
    }

    //non ho capito benissimo cosa faccia
    for (int i = index; i < choices.Length; i++)
    {
      choices[i].gameObject.SetActive(false);  
    }


    }
}

