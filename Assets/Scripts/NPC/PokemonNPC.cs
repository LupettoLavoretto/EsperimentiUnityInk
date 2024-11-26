using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonNPC : MonoBehaviour
{
    //SerializeField ci permette di avere variabili private, ma accessibili comunque dall'inspector per poterle modificare.

    [SerializeField] private Color defaultColor = Color.white;

    [SerializeField] private Color charmanderColor = Color.red;

    [SerializeField] private Color bulbasaurColor = Color.green;

    [SerializeField] private Color squirtleColor = Color.blue;

    //SpriteRenderer ci aiuta a definire lo sprite su cui agire, da "renderizzare" per eventuali modifiche.
    //Ha diverse opzioni, tra cui colore e materiale
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        //Qui, se ho inteso bene, definiamo una variabile "pokemonName" che coincide con il valore "pokemon_name" che vediamo nel file ink. Tutta la logica è se ("", Charmander...) allora (defaultColor, charmanderColor;)
        string pokemonName = ((Ink.Runtime.StringValue)DialogueManager.GetInstance().GetVariableState("pokemon_name")).value;

        switch (pokemonName)
        {
            case "":
                spriteRenderer.color = defaultColor;
                break;
            case "Charmander":
                spriteRenderer.color = charmanderColor;
                break;
            case "Bulbasaur":
                spriteRenderer.color = bulbasaurColor;
                break;
            case "Squirtle":
                spriteRenderer.color = squirtleColor;
                break;
            default:
                Debug.LogWarning("Pokemon name is not handled by switch statement: " + pokemonName
                    );
                break;
        }
    }
}
