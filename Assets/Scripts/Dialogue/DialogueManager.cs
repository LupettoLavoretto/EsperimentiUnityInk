using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
[Header("Dialogue UI")]
[SerializeField] private GameObject dialoguePanel;
[SerializeField] private TextMeshProUGUI dialogueText;
[SerializeField] private TextMeshProUGUI displayNameText;

[Header("Choices UI")]
[SerializeField] private GameObject[] choices;
private TextMeshProUGUI[] choicesText;

private Story currentStory;

public bool dialogueIsPlaying {get; private set;}

private const string SPEAKER_TAG = "speaker";
private const string PORTRAIT_TAG = "portrait";

private const string LAYOUT_TAG = "layout";


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
        HandleTags(currentStory.currentTags);

    }
    else
    {
        //Se non c'è testo, esci dal dialogo
        StartCoroutine(ExitDialogueMode());
    }
    }

    private void HandleTags(List<string> currentTags)
    {
        //Looppa per ogni tag e gestiscili di conseguenza
        foreach (string tag in currentTags)
        {
            //parse the tag: in questo modo dividiamo la formula key:value (es: portrait:dr_green_neutral) in due elementi scissi
            string[] splitTag = tag.Split(':');
            if (splitTag.Length !=2)
            {
                Debug.LogError("Tag could not be appropriately parsed" + tag);
            }
            //in questo credo due tag dall'elemento splittato
            //Trim cancella gli spazi vuoti
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                Debug.Log("portrait=" + tagValue);
                    break;
                case LAYOUT_TAG:
                Debug.Log("layout=" + tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled:" + tag);
                    break;      

            }


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

    StartCoroutine(SelectFirstChoice());

    }

    private IEnumerator SelectFirstChoice()
    {
        //Event System richiede in unity prima di essere svuotato, e poi di aspettare prima di attivare un nuovo elemento
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);

    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }

}

