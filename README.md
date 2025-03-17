
# MonoGame 2D Map System - JuliaC

This is a tile-based game featuring a Scene-GameObject-Component system and a randomized tilemap with obstacles, start, and exit tiles.

I made it so the Scene holds a list of GameObjects, and GameObjects hold lists of Components. The Components are in their own folder, and most of the references are handled by GameManager.

Scene: Manages and updates a list of GameObject instances.
GameObject: Represents an object in the scene, holding a list of Component instances.
Component: Defines behaviors that a GameObject can have (e.g., movement, rendering).
GameManager: Contains helper classes but does not manage game logic directly.

I made both a MapSystem and Tilemap component, Tilemap to set up and load a grid of sprite components, and MapSystem to hold behaviors like generation and specific tile checks.
The player does their own checks for collisions, recognizing obstacles and running into them.
 
The game starts by generating a random tilemap or loading a predefined map from the MyMaps folder. Predefined maps use chars and convert them into their corresponding Textures.

The player navigates the map while avoiding obstacles.

Combat
Based on a bool that checks if it is the player's turn, depending on wether its player/enemy check adjacent 32 x 32 pixel tiles for the opponent. 
If the opponent is adjacent, attacks and switches turn
Turn indicator is drawn above currently active gameobject

Enemy
Enemies start at 1 count and go up to 10 maximum. They move towards player by tile (32 x 32). 
They check if next to player and attack, take damage, or recover from stun on turn

MapSystem
Obstacle dimensions are specified in GenerateMap()

---------- TERMINAL ----------
build game : dotnet build
run game in VS Code : dotnet run
open Content.mgcb : dotnet mgcb-editor .//Content/Content.mgcb
