<#
.SYNOPSIS
	This function “rolls” one or more dice depending on the chosen attack weapon.

.DESCRIPTION
	Author: Orval Roller (@cysecgunz on the Twitterz)
	License: Open Gaming License v1.0 / GNU General Public License (GPL 3.0)
	Required Dependencies: None
	Optional Dependencies: None

.PARAMETER Weapon
	All available weapons are referenced in “weapons.csv.” Selecting the weapon will request the dice number and type from “weapons.csv.”    

.PARAMETER Modifier
	Add this based on strong you are 

.PARAMETER Proficiency
	Add this based on how well you can swing that weapon
.EXAMPLE
	Roll-Damage -Weapon Mace
    	Your mace did 4 bludgeoning damage!
#>

[cmdletbinding()]
    
	param(
   	 
    	[Parameter(Mandatory=$TRUE, ValueFromPipeline=$TRUE)]   	 
    	[ValidateSet('club','dagger','greatclub','handaxe','javelin','light-hammer','mace','quarterstaff','quaterstaff-versatile',`
                	'sickle','spear','light-crossbow','dart','shortbow','sling','battleaxe','battleaxe-versatile','flail','glaive',`
                	'greataxe','greatsword','halberd','lance','longsword','longsword-versatile','maul','morningstar','pike','rapier',`
                	'scimitar','shortsword','trident','war-pick','warhammer','warhammer-versatile','whip','blowgun','hand-crossbow',`
                	'heavy-crossbow','longbow')]
                	<#
                    	ValidateSet is a list of values that can be entered for Roll-Damage weapon parameter,
                    	this also provides tab-complete functionality to the cmdlet, as it will only display
                    	the weapons available for the cmdlet.
                	#>
    	[string]
    	$weapon,
        [Parameter(Mandatory=$TRUE, ValueFromPipeline=$TRUE)]
        [ValidateRange(-5,10)]
        [int]
        $modifier,
        [Parameter(Mandatory=$TRUE, ValueFromPipeline=$TRUE)]
        [ValidateRange(0,10)]
        [int]
        $proficiency
    	)

	[int]$Global:damage = $null #This line sets the Global variable damage to "null."

	<#
	This takes the list of weapons in CSV format and converts them (similar to pointing Import-CSV to a file). This approach
	was taken so the function is not reliant upon a CSV file.
	#>
    
	$weaponList = @"
        	weaponName,diceNumber,diceType,damageType
        	club,1,4,bludgeoning
        	dagger,1,4,piercing
        	greatclub,1,8,bludgeoning
        	handaxe,1,6,slashing
        	javelin,1,6,piercing
        	light-hammer,1,4,bludgeoning
        	mace,1,6,bludgeoning
        	quarterstaff,1,6,bludgeoning
        	quarterstaff-versatile,1,8,bludgeoning
        	sickle,1,4,slashing
        	spear,1,6,piercing
        	light-crossbow,1,8,piercing
        	dart,1,4,piercing
        	shortbow,1,6,piercing
        	sling,1,4,bludgeoning
        	battleaxe,1,8,slashing
        	battleaxe-versatile,1,10,slashing
        	flail,1,8,bludgeoning
        	glaive,1,10,slashing
        	greataxe,1,12,slashing
        	greatsword,2,6,slashing
        	halberd,1,10,slashing
        	lance,1,12,piercing
        	longsword,1,8,slashing
        	longsword-versatile,1,10,slashing
        	maul,2,6,bludgeoning
        	morningstar,1,8,piercing
        	pike,1,10,piercing
        	rapier,1,8,piercing
        	scimitar,1,6,slashing
        	shortsword,1,6,piercing
        	trident,1,6,piercing
        	war-pick,1,8,piercing
        	warhammer,1,8,bludgeoning
        	warhammer-versatile,1,10,bludgeoning
        	whip,1,4,slashing
        	blowgun,1,1,piercing
        	hand-crossbow,1,6,piercing
        	heavy-crossbow,1,10,piercing
        	longbow,1,8,piercing

"@ | ConvertFrom-CSV -delimiter ","

	[int]$diceNumber = $weaponList | Where-Object weaponName -eq $weapon | Select-Object -ExpandProperty diceNumber
    
	[int]$diceType = $weaponList | Where-Object weaponName -eq $weapon | Select-Object -ExpandProperty diceType
    
	$damageType = $weaponList | Where-Object weaponName -eq $weapon | Select-Object -ExpandProperty damageType
   	 
    	for ($i=1; $i -le $diceNumber; $i++) {
       	 
        	[int]$diceRoll = get-random -minimum 1 -maximum ($diceType + 1)
       	 
        	$damage = $damage + $diceRoll #This line adds the values from each dice roll in the loop.
       	 
        	}
            $damageTotal = ($damage) + ($modifier) + ($proficency)
       	 
	Write-Host "Your $weapon did $damageTotal $damageType damage!" -foregroundcolor green