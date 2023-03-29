# Table of Contents
  - [Fall Guys Stats](#fall-guys-stats)
    - [Download](#download)
    - [Usage](#usage)
    - [Theme](#theme)
      - [Light Theme](#light-theme)
      - [Dark Theme](#dark-theme)
  - [Multilingual Support](#multilingual-support)
  - [Overlay](#overlay)
    - [Hotkey](#hotkey)
    - [Create your own overlays](#create-your-own-overlays)
    - [How to Overlay Background Image Customized](#how-to-overlay-background-image-customized)
  - [Profile](#profile)
    - [Linking Profiles and Shows](#linking-profiles-and-shows)
    - [Deleting shows or moving shows to another profile](#deleting-shows-or-moving-shows-to-another-profile)
  - [Changelog](#changelog)

# Fall Guys Stats
Simple program to generate stats for the game Fall Guys. Reads the games log file to track how you are doing.

## Download
　　<a href="https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuysStats.zip">![FallGuysStats.zip](Resources/FallGuysStats-download.svg)</a>
  - Or, if you have a false detection problem with your virus program, download the version below with the automatic update feature removed.

　　<a href="https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuysStatsManualUpdate.zip">![FallGuysStats.zip](Resources/FallGuysStatsManualUpdate-download.svg)</a>

## Usage
  - Extract zip to it's own folder
  - Run the program while playing Fall Guys to see new stats.
  - Only updates after a show has been completed and results are given.


### Main Window
![Fall Guys Stats](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/mainWindow.png)


### Rounds Stats List
![Fall Guys Level Stats](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/levelWindow.png)

## Theme
  - FallGuysStats supports two themes, Light and Dark.

### Light Theme
![Fall Guys Stats Light Theme](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/mainWindowLightTheme.png)

![Fall Guys Stats Light Theme](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/levelWindowLightTheme.png)

### Dark Theme
![Fall Guys Stats Dark Theme](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/mainWindowDarkTheme.png)

![Fall Guys Stats Dark Theme](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/levelWindowDarkTheme.png)

## Multilingual Support
  - FallGuysStats supports the following languages.
    - 🇺🇸 English
    - 🇫🇷 French
    - 🇰🇷 Korean
    - 🇯🇵 Japanese
    - 🇨🇳 Simplified Chinese

## Overlay
![Overlay](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/overlay.png)

### Hotkey
  - Hit Keys **'Ctrl + Shift + D'** to set the overlay to its default size.
  - Hit Keys **'T'** to toggle background colors.
  - Hit Keys **'F'** to flip the Display.
  - Hit Keys **'P'** to change the profile order.
  - Hit Keys **'1'** through **'9'** to select profiles 1 through 9.
  - Hit Keys **'Shift + mouse wheel Up and Down'** to select a profile up or down.
  - Hit Keys **'Shift + (Up, Left) and (Down, Right) arrow keys'** to select a profile up or down.
  - Hit Keys **'C'** to shows the number of users by platform.
  - Hit Keys **'R'** the round name shows the colored badge for the round type.


### Create your own overlays
![Customized Overlay](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/customizedOverlay.png)


### How to Overlay Background Image Customized
  - **Step 1.** Edit the **background.png** and **tab.png** in the Overlay folder of the FallGuysStats folder as desired.


  - **Step 2.** Rename the edited image as below.
    - **{my_image_name}** must be the same for both files.
      - **{my_image_name}**.png
      - tab_**{my_image_name}**.png


  - **Step 3.** Place the image inside the Overlay folder in the FallGuysStats folder.


  - **Step 4.** You can see that the background image you added appears first in the Background Image of the Overlay item in Settings.


  - **Step 5.** Select and save the added image.

## Profile

### Linking Profiles and Shows
  - Link your profile and show so your profile automatically changes when the show starts.


  - Profile Settings

![Shows](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/profileAndShowLinkage.png)


  - Settings - Automatically change to linked profile

![Shows](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/automaticProfileChange.png)


### Deleting shows or moving shows to another profile


  - On the main screen, click the Shows label at the top.
  - Highlight any number of shows and hit the 'DEL' key or right-click to manage the show through the 'Delete' and 'Move show data' menus.

![Shows](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/Properties/showsWindow.png)

## Changelog
  - 1.141
    - Upgrade Win Per Day Charts
    - Finals bug fix
  - 1.140
    - Update all packages due to package vulnerabilities
  - 1.139
    - Bug fixes and Correct typos
  - 1.138
    - Added theme option and change overlay background image option
  - 1.137
    - Added a function to automatically select a linked profile when a show starts by linking a profile with a show
  - 1.136
    - Overlay position fixed function button addition and changed the graph to make it look better
  - 1.135
    - Bug fixes and multilanguage updates
  - 1.134
    - Many updates from the community. Multilanguage / Profile Editing / Various Fixes
  - 1.133
    - Add Bean Hill Zone and fix names (Thanks to iku55 & Foolyfish)
  - 1.132
    - Season 9 (aka Season 3)
  - 1.131
    - Update by ThreesFG to fix date parsing
  - 1.130
    - Update by ThreesFG to fix log parsing for new update
  - 1.129
    - Try and fix Leading Light
  - 1.128
    - Update Season filter
  - 1.127
    - Season 8 (aka Season 2)
  - 1.126
    - Fix some log parsing
  - 1.125
    - Move Blast Ball to final category
  - 1.124
    - Season 7 (aka Season 1)
  - 1.123
    - Fix group play stats
  - 1.122
    - Add Sweet Thieves
  - 1.121
    - Possibly fix Sum Fruit
  - 1.120
    - Season 6 update
  - 1.119
    - Round name fix
  - 1.118
    - Season 5 update
  - 1.117
    - Season 4.5 update
  - 1.116
    - Fix for round names
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