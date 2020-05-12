	<#
.SYNOPSIS
	This function “rolls” one d20 for Initiative.

.DESCRIPTION
	Author: Orval Roller (@cysecgunz on the Twitterz)
	License: Open Gaming License v1.0 / GNU General Public License (GPL 3.0)
	Required Dependencies: None
	Optional Dependencies: None    

.EXAMPLE
	Roll-Initiative
    	You rolled a Natural 20!
#>
	$Initiative = get-random -minimum 1 -maximum 21
    	if($Initiative -eq 20){
        	Write-Host "You rolled a Natural 20!" -foregroundcolor green
    	}
    	else
    	{
         	Write-Host "Your rolled a $Initiative for initiative!" -ForegroundColor green   
    	}
