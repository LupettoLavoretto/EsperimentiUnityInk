using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
//Bloccone di parametri vari
    [Header("Parametri")]
    //Questo definisce il ritmo di comparsa delle singole lettere
    [SerializeField] private float typingSpeed = 0.04f;

    //Questo ci serve per avere un tracciamento di tutte le variabili che sia sempre accessibile
    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    //Qui definiamo gli elementi della User Interface
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    //Variabili varie
    private TextMeshProUGUI[] choicesText;

    private Animator layoutAnimator;

    private Story currentStory;

    public bool dialogueIsPlaying {get; private set;}

    private bool canContinueToNextLine = false;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueVariables dialogueVariables;

    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;


private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Trovati più di un Dialogue Manager nella scena");
        }
        instance = this;

            dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

public static DialogueManager GetInstance()
    {
        return instance;
    }

private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        //get the Layout Animator
        layoutAnimator = dialoguePanel.GetComponent<Animator>();
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
        if (canContinueToNextLine && 
        currentStory.currentChoices.Count ==0 
        && InputManager.GetInstance().GetSubmitPressed())
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

            dialogueVariables.StartListening(currentStory);

        //reset portrait, layout, and speaker
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");


        ContinueStory();

    }
private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

private void ContinueStory()
    {
            if (currentStory.canContinue)
    {
        //set text for current dialogue line
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }
        
        displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
        


        HandleTags(currentStory.currentTags);

    }
    else
    {
        //Se non c'è testo, esci dal dialogo
        StartCoroutine(ExitDialogueMode());
    }
    }

private IEnumerator DisplayLine(string line)
    {
        //empty the dialogue text
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        //display each Letter one at a time
        foreach (char letter in line.ToCharArray()
        )
        {
            //se il player clicca il tasto submit, la riga viene mostrata per intero
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }
            //check for rich text tag, if found, add it without waiting
            if (letter == '<'|| isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            

            }
            //if not rich text, add the next letter and wait a small time
            else
            {
                dialogueText.maxVisibleCharacters ++;
                yield return new WaitForSeconds(typingSpeed);
            }



        }
        continueIcon.SetActive(true);
        // mostra le scelte, se esistono, per questa linea di dialogo
        DisplayChoices();

        canContinueToNextLine = true;

    }

private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
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
                //Dato che lo speaker è solo testo, gli diciamo di mostrare il testo del nome personaggio
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                //Play è un metodo che attiva l'animazione, e gli diciamo di attivare l'animazione legata al tag
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
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
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            InputManager.GetInstance().RegisterSubmitPressed();
            ContinueStory();
        }
        
    }

public Ink.Runtime.Object GetVariableState(string variableName) 
    { 
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null) 
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

public void OnApplicationQuit()
    {
        if (dialogueVariables != null) {
            dialogueVariables.SaveVariables();
        }
        
    }

}

