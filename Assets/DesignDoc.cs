/*

Requirements:
10 Days of Time
-Create a core game loop
-limit number of core mechanics to 3

 
Game Design:

1 Main Mechanic - Core idea
2 Supporting Mechanics - Not as intense to code, but support and help the main mechanic to shine
3 Require a basic main menu -> allow the game to be replayed without closing and re-opening the game/exiting the game


The player finds themselves in a land, and after traversing it a little bit, teaching the player basic movement, they find their bazooka
on the ground. They would then use this in platforming challenges and puzzles to uncover secrets and collect items, providing an ultimate goal.
The game could be replayed in an attempt to complete the game as fast as possible.


Technical Design:

//Identify all the core systems that the game will require

A slow moving 2D player controller with a bazooka that can be used for rocket jumps or recoil (newton's 3rd law) - MAIN
A form of camera locking system which will lock the camera to the room the player is in - SUPPORT
A collectible system providing an ultimate goal for the player - SUPPORT

//Estimations
    Time estimates to know how long it will take things to build

2D player controller can take not much more than like an hour, refining it could take like 1 day
The Bazooka controller would take like 3 days, refining the recoil and explosion values
A camera locking system would take 1-2 days
A collectible system shouldn't take longer than 1 day
creating a full level could take about 4-5 days

//Style

Each function must have a <summary> block explaining what it does and what it's parameters are
Each script and class constants/instance variables must be named something descriptive as to keep variable names easy to understand
Each Serialized Field must have an explanation that is visible in Unity to be able to easily know what each field means
Add null checks and throwing errors in start loops

*/