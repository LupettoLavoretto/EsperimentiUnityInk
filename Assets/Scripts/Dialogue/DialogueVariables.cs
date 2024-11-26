using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;


public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    public DialogueVariables (TextAsset loadGlobalsJSON)
    {
        Story globalStoryVariables = new Story(loadGlobalsJSON.text);

        //initialize the dictionary
        variables = new Dictionary<string, Ink.Runtime.Object> ();
        foreach (string name in globalStoryVariables.variablesState)
        {
            Ink.Runtime.Object value = globalStoryVariables.variablesState.GetVariableWithName(name);
            variables.Add (name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public void StartListening (Story story)
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }


    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

        private void VariablesToStory(Story story)
        {
            foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }

  
        }
}
