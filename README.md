# Fall Guys Stats
Simple program to generate stats for the game Fall Guys. Reads the games log file to track how you are doing.

## Download
  - [FallGuyStats.zip](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuyStats.zip)

  - or if you have problems with false positives in your virus program this one removes the ability to auto update [FallGuyStatsManualUpdate.zip](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuyStatsManualUpdate.zip)
  
## Usage
  - Extract zip to it's own folder
  - Run the program while playing Fall Guys to see new stats.
  - Only updates after a show has been completed and results are given.

![Fall Guys Stats](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/mainWindow.png)

![Fall Guys Level Stats](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/levelWindow.png)

## Overlay
![Overlay](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/overlay.png)

  - Hit 'T' to toggle background colors
  - Hit 'F' to flip the Display

## Deleting Shows
![Shows](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/showsWindow.png)

  - Click the blue Shows label on the main screen
  - Highlight any number of shows and hit the 'DEL' key

## Changelog
  - 1.115
    - Season 4 update
  - 1.114
    - Fix overlay not showing correct stats for Snowball Survivor
  - 1.113
    - Fix Snowball Survival
  - 1.112
    - Update for new game patch
  - 1.111
    - Try and fix false negatives by removing NDI
  - 1.110
    - Fix error reading log file date/time in rare instances
  - 1.109
    - Add ability to track if a final is actually a final
  - 1.108
    - Fix Hex a gone game mode from showing all wins/finals
    - Fix font selector not remembering font
  - 1.107
    - Add Font Chooser for overlay
    - Fix Average times on main grid
  - 1.106
    - Added better options for the cycle stats on overlay in settings
    - Added average finish time to main grid
    - Minor sorting fixes
  - 1.105
    - Grid sorting improvements
    - Display issue with private lobby stats on overlay
  - 1.104
    - Implemented a number of improvements from hunterpankey
  - 1.103
    - Fix log reading during live rounds
  - 1.102
    - Fix issues reading log file in certain cases
    - Made sure private lobbies stats dont show in main screen
  - 1.101
    - Add ability to track private lobbies
  - 1.100
    - Fixes and added levels for Season 3
  - 1.99
    - Hopefully made it so game modes wont affect levels anymore
  - 1.98
    - Logic to handle new game mode
    - Added ability to only show certain stats on overlay instead of having to cycle them
  - 1.97
    - Logic to handle new game mode
  - 1.96
    - Fixed existing levels for the northernlion game mode to show up correctly
  - 1.95
    - Fixed new game mode adding levels that shouldn't be there
  - 1.94
    - Fixed typo in level name
  - 1.93
    - Added ability to rename Hoopsie Legends to Hoopsie Heroes
    - Added logic to save main window size
  - 1.92
    - Added code to handle levels with variations in their name
  - 1.91
    - Added Big Fans Level
  - 1.90
    - Fixed names on overlay
  - 1.89
    - Fixed names for new Slime Event levels
  - 1.88
    - Added more info to AssemblyInfo to possibly help with false positives in AV programs
  - 1.87
    - Fixed level names in level details for gauntlet matches
    - Allowed main window to be resizable
  - 1.86
    - Fixed Level stats grid columns
  - 1.85
    - Finish time on overlay will now become gold when you beat overall best time or green when you beat best time for current filter
    - Time on overlay will now also show the timeout duration
  - 1.84
    - Fixed a filter issue with profiles
  - 1.83
    - Added ability to switch between a Main and Practice profile
  - 1.82
    - Fixed season filter dates
  - 1.81
    - Fixed guantlet levels not showing up on Overlay properly
  - 1.80
    - Added Final Streak to cycle with Win Streak
    - Added new maps
  - 1.79
    - Added option to cycle between Players and Server Ping on overlay
  - 1.78
    - Changed logic when not cycling stats on overlay to show the most interesting stat
    - Added option to show / hide percentages on overlay
  - 1.77
    - Added individual option for Cycle Qualify / Gold and Cycle Fastest / Longest to settings
  - 1.76
    - Moved Season 2 start date to Oct 8th
    - Added ability to choose when starting program to include previous stats or not
  - 1.75
    - Fixed streak count on overlay
  - 1.74
    - Fixed stat calculations for shows crossing filter boundries
    - Added some extra stats to the Wins Per Day popup
    - Added option in settings to show / hide Wins info for overlay
  - 1.73
    - Added options to settings screen for overlay color and flip to make it more visible to the user
    - Added ability to manually resize overlay from the corners
  - 1.72
    - Changed overlay so it stays visible when you minimize amin screen
  - 1.71
    - Changed main screen to show Fastest / Longest qualifications for each level
    - Fixed minor sorting issue in the grids
  - 1.70
    - Cleaned up auto update feature a bit
  - 1.69
    - Program will save last location of main window now and restore it when opened again
  - 1.68
    - Fixed Week / Day filters
    - Added more filter options in settings
    - Added logic to account for new levels that may come up in Season 2
    - Added option to auto update program in settings
  - 1.67
    - Fixed times in database to be stored correctly as utc
  - 1.66
    - Hopefully fixed an issue with times counting down on overlay in rare cases
  - 1.65
    - Added export to MarkDown option
    - Added ability to click on Win% label to toggle columns to show %s and back
  - 1.64
    - Fixed time issue when parsing log
  - 1.63
    - Added export options for both Html and BBCode when right clicking any grid
  - 1.62
    - Fixed some logic when deleting shows while filtered
    - Switched the Longest/Fastest to align with Qualify/Gold
  - 1.61
    - Added logic to reset overlay position if it ended up off screen
    - Tightened up the overlay when hiding the round info
  - 1.60
    - Added option to show tab on overlay for current filter
  - 1.59
    - Try and make sure deleted shows dont get added back accidentally
  - 1.58
    - Fixed rare case when deleting show didnt work
  - 1.57
    - Fixed overlay missing image on startup
  - 1.56
    - Add ability to show / hide information on overlay
  - 1.55
    - Fixed overlay getting out of wack if you change filters a lot
  - 1.54
    - Added mouse hover tool tip on Qualified / Gold / Silver / Bronze to show value as a %
  - 1.53
    - Fixed Filters on overlay not taking into account UTC time
  - 1.52
    - Fixed Time display on overlay not updating sometimes
  - 1.51
    - Fixed an issue around results coming from a previous match
  - 1.50
    - Fixed accidental debug typo
  - 1.49
    - Added filter options to settings page for overlay display
  - 1.48
    - Fixed Gold statistic on overlay, was using wrong medal type
  - 1.47
    - Added Gold statistic to overlay that rotates with Qualify
  - 1.46
    - Fixed overlay display not updating
  - 1.45
    - Cleaned up labels on overlay
  - 1.44
    - Fixed end of round numbers on overlay
  - 1.43
    - Added ability to delete Shows in Shows screen