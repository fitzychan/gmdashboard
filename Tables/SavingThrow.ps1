	<#
.SYNOPSIS
	This function “rolls” a d20 for a saving throw (Strength,Dexterity,Constitution,Intelligence,Wisdom,or Charisma) and adds the character's
	ability modifier. This is similar to an ability check.

.DESCRIPTION
	Author: Orval Roller (@cysecgunz on the Twitterz)
	License: Open Gaming License v1.0 / GNU General Public License (GPL 3.0)
	Required Dependencies: None
	Optional Dependencies: None

.PARAMETER Ability
	Abilities are STR (Strength), DEX (Dexterity), CON (Constitution), INT (Intelligence), WIS (Wisdom), and CHA (Charisma).

.PARAMETER Modifier
	Ability modifiers are an integer ranging from -5 (minus five) to +10 (plus ten).    

.EXAMPLE
	Roll-AbilityCheck -Ability STR -Modifier 3
    	You rolled 16 on your strength check!
#>
[cmdletbinding()]    
	param(

    	[Parameter(Mandatory=$TRUE, ValueFromPipeline=$TRUE)]
    	[ValidateSet('STR','DEX','CON','INT','WIS','CHA')]
    	[string]
    	$ability,
    	[Parameter(Mandatory=$TRUE, ValueFromPipeline=$TRUE)]
    	[ValidateRange(-5,10)]
    	[int]
    	$modifier
     	)

	$abilityTable = @{
    	"STR" = "strength"
    	"DEX" = "dexterity"
    	"CON" = "constitution"
    	"INT" = "intelligence"
    	"WIS" = "wisdom"
    	"CHA" = "charisma"
	}    
	[string]$abilityFullname = $abilityTable.item("$ability")

	[int]$abilityRoll = get-random -minimum 1 -maximum 21
    
	[int]$savingThrow = $abilityRoll + $modifier

	Write-Host "You rolled $savingThrow on your $abilityFullname saving throw!" -foregroundcolor green
