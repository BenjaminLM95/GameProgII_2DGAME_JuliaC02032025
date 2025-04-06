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

            //InitializeLevel();
            LoadNextLevel();
        }

        public void InitializeLevel()
        {
            // generate random map
            // spawn enemies
            // track if it's a boss enemy
        }
        public void LoadNextLevel()
        {
            // handle respawning
            //bool isBossFloor = currentLevel % 3 == 0;
            //bool isBossFloor = true;
            //if (isBossFloor)
            //{
            //    EnemySpawner.RespawnBoss();
            //    Debug.WriteLine($"LevelManager: spawning boss on level: {currentLevel}");
            //}
            EnemySpawner.RespawnEnemies(currentLevel);
            Debug.WriteLine($"LevelManager: tracking current level as: {currentLevel}");
        }
        public void GoToNextLevel()
        {
            currentLevel++;
            LoadNextLevel();
        }
    }
}
