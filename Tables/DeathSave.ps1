	<#
.SYNOPSIS
	This function “rolls” a d20 for a Death Save. This is a special kind of saving throw used when a character has 0 (zero) hit points remaining. If
	the player rolls a natural 1 (one), a "Critical Failure," this constitutes 2 (two) death save failures, 3 (three) death save failures and your
	character dies. If a player rolls a natural 20 (twenty), a "Critical Success," the player's character regains 1 (one) hit point. If the player rolls
	3 (three) successful Death Saves, the player becomes stable (Death Save successes/failures are reset when a character is stable or regains hit
	points). The successes/failures do not need to be consecutive, keep track of both until you collect three of a kind. In 5th Edition
	Dungeons & Dragons, a successful Death Save is 10 (ten) or higher. Additionally, if you take any damage at 0 (zero) hit points you suffer a
	Death Save failure. If the damage is from a critical hit, the player's character suffers 2 (two) Death Save failures. If the damage equals or exeeds
	the character's hit point max, the player's character suffers instant death.  

.DESCRIPTION
	Author: Orval Roller (@cysecgunz on the Twitterz)
	License: Open Gaming License v1.0 / GNU General Public License (GPL 3.0)
	Required Dependencies: None
	Optional Dependencies: None

.EXAMPLE
	Roll-DeathSave
    	You rolled a 10. Death Save success!
#>
	$deathSave = get-random -minimum 1 -maximum 21
    	if($deathSave -eq 20){
        	Write-Host "You rolled a Natural $deathSave for your Deatch Save! Regain 1 Hit Point!" -foregroundcolor green
    	}
    	elseif($deathSave -eq 1){
        	Write-Host "You rolled a Natural $deathSave for your Death Save! That's two Death Save failures for you pal!" -foregroundcolor red
    	}
    	elseif($deathSave -ge 10){
        	Write-Host "You rolled $deathSave for your Death Save! Death Save success! Don't count your chickens before they hatch!" -foregroundcolor green
    	}
    	else
    	{
        	Write-Host "You rolled $deathSave for your Death Save! Death Save failure! Sucks to suck!" -foregroundcolor yellow    
    	}