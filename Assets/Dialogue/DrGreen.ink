INCLUDE globals.ink

~ playEmote("exclamation")

Hello there! #speaker:Dr.Green #portrait: dr_green_neutral #layout:left
-> main

=== main ===
How are you feeling today?
+ [Happy]
    ~ playEmote("exclamation")
    That makes me feel <color=\#F8FF30>happy</color> as well! #portrait: dr_green_happy
+ [Sad]
    Oh, well that makes me <color=\#5B81FF>sad</color> too. #portrait: dr_green_sad
    
- Don't trust him, he's <b><color=\#FF1E35>not</color></b> a real doctor! #speaker:Ms.Yellow #portrait: ms_yellow_neutral #layout:right

~ playEmote("question")
Well, do you have any more questions? #speaker:Dr.Green #portrait: dr_green_neutral #layout:left
+ [Yes]
    -> main
+ [No]
    Goodbye then!
    -> END