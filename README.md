
# MonoGame 2D Map System - JuliaC

This is a tile-based game featuring a Scene-GameObject-Component system and a randomized tilemap with obstacles, start, and exit tiles.
I made it so the Scene holds a list of GameObjects, and GameObjects hold lists of Components. The Components are in their own folder, and most of the references are handled by GameManager.

*The Long Version* - summary at the end

Scene: 
	- Manages and updates a list of GameObject instances.
GameObject: 
	- Represents an object in the scene, holding a list of Component instances.
Component: 
	- Defines behaviors that a GameObject can have (e.g., movement, rendering).
GameManager: 
	- Contains helper classes but does not manage game logic directly.
TurnManager: 
	- Replaced the old Combat script, handles turn takers better, 
	- adds and removes by checking active entities
	- also handles some state checking and drawing the turn indicators above the active turn taker.

I made both a MapSystem and Tilemap component, Tilemap to set up and load a grid of sprite components, and MapSystem to hold behaviors like generation and specific tile checks.
The player does their own checks for collisions, recognizing obstacles and running into them.
The player navigates the map while avoiding obstacles.

MapSystem
Obstacle dimensions are specified in GenerateMap()

Enemy Types
Slimes do 5 damage, have 30 health, and have simple movement
Ghosts can go through anything except walls
Skeletons keep a distance to have a line of sight to the player, have more health and do more damage
Bosses have the most health, do 4 actions during combat (nothing, move, shoot, charge)
	- tip for beating bosses, I had items in mind, 
		it would be practically impossible without lightning scrolls & fireballs.
	- Bosses show an indicator above their head that predicts 
		their next action based on their distance to the player.

Menus
Play on the main menu starts the game, Quit quits the game (this applies for all screens)
Any Retry button re-loads the map at level 1 with full health

How To Play: **
	- Right now the boss spawns along with the enemies, 
		if you want to change this, comment out line 351 in Enemy.cs
	- The goal is to kill the boss, you get a blue win screen
	- If you die, you get a red lose screen
	- Using items is recommended, they are; 
		warp scrolls take you to any free floor tile on the map,
		Fireball scrolls shoot a projectile in the direction you LAST moved.
		Lightning scrolls damage every enemy on the map, and are very powerful
		Healing potions heal +20 hp


*Summary - added features*
- damage numbers
- fixed enemy projectile direction and how enemies detect the player
- added full boss functionality (except the nothing indicator does not display, chose to put move indicator)
- fixed turn management, completely re-did combat
- 3 UI screens for Main Menu, End Screen, & Win Screen
- Updated Skeleton & Ghost sprites, added all boss sprites
- Fixed bug with multiple turns/items used, was player input missing previousState check/lock