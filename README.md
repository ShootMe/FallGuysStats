# Fall Guys Stats
Simple program to generate stats for the game Fall Guys. Reads the games log file to track how you are doing.

## Download
  - [FallGuysStats.zip](https://raw.githubusercontent.com/ShootMe/FallGuysStats/master/FallGuyStats.zip)
  
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

## Changelog
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