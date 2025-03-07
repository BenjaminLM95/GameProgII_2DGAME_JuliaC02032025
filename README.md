
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
If the player reaches the exit tile, the game loads a new level with a fresh map (or rather it should, I made logic for it but could not implement it properly).

---------- TERMINAL ----------
build game : dotnet build
run game in VS Code : dotnet run
open Content.mgcb : dotnet mgcb-editor .//Content/Content.mgcb
