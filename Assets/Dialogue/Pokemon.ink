INCLUDE globals.ink



{ pokemon_name == "": -> main| -> alredy_chose}

=== main ===
Which pokemon do you choose? #speaker:Ms.Yellow #portrait: ms_yellow_happy #layout:left
+ [Charmander]
    Charmander! #speaker:Charmander #portrait:Charmander #layout:right
    -> chosen("Charmander")
+ [Bulbasaur]
    Bulbasaur! #speaker:Bulbasaur #portrait:Bulbasaur #layout:right
    -> chosen("Bulbasaur")
+ [Squirtle]
    Squirtle! #speaker:Squirtle #portrait:Squirtle #layout:right
    -> chosen("Squirtle")
    
=== chosen(pokemon) ===
~ pokemon_name = pokemon
You chose {pokemon}! #speaker:Ms.Yellow #portrait: ms_yellow_happy #layout:left
+ [Charmander]
-> END

=== alredy_chose ===
You already chose {pokemon_name} #speaker:Ms.Yellow #portrait: ms_yellow_happy #layout:left
Do you want to change your mind?
    + No, i'm fine -> continue
    + Yes! -> main
    -
-> END

=== continue


-> END