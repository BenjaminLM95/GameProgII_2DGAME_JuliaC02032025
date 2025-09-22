using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class LevelManager : Component
    {
        // track level number
        // respawn all enemy types
        // every few levels spawn a boss
        // handle map generation as well
        public static LevelManager Instance { get; private set; }
        public LevelManager() 
        {
            Instance = this;
        }
        // ---------- REFERENCES ---------- //       
        private Globals globals;
        private Enemy enemy;
        private MapSystem mapSystem;        


        // ---------- VARIABLES ---------- //
        private int currentLevel;

        public override void Start()
        {
            globals = Globals.Instance;

            mapSystem = globals._mapSystem;
            if (mapSystem == null)
            {
                Debug.WriteLine("LevelManager: ERROR - mapsystem is null!");
                return;
            }
            currentLevel = mapSystem.levelNumber;
            Debug.WriteLine($"LevelManager: tracking current starting level as: {currentLevel}");

            LoadNextLevel();
        }

        public void LoadNextLevel()
        {
            currentLevel = mapSystem.GetLevelNumber();

            // Log current level
            Debug.WriteLine($"LevelManager: Current Level: {currentLevel}");

                EnemySpawner.RespawnEnemies(currentLevel);  // regular enemies
            


            TurnManager.Instance?.ResetTurns();

            Debug.WriteLine($"LevelManager: Tracking current level as: {currentLevel}");
        }
    }
}
