using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    internal class Pathfinding : Component
    {
        public PathNode[,] nodeMap;
        public List<PathNode> unexploredNodes = new List<PathNode>();
        public Point startingPoint, targetPoint;

        public override void Start()
        {
            Debug.WriteLine("Pathfinding: START");
        }

        public void InitializePathfinding(char[,] map, Point startingFrom, Point goal)
        {
            nodeMap = new PathNode[map.GetLength(0), map.GetLength(1)];

            for (int x = 0; x < map.Length; x++)
            {
                for (int y = 0; y < map.Length; y++)
                {

                }
            }
        }

        public void ExploredNode()
        {
            // find best F cost surrounding it
        }
    }

    // checking neighbors

    // [ ] [X Y+1 ] [ ]
    // [X Y-1 ] [X Y ] [X Y+1 ]
    // [ ] [X Y-1 ] [ ]

    // when checking neoghbors, only read neighbors if X, Y are one of them 0
    // like if X != 0 && Y != 0 continue;

    /// <summary>
    /// 
    /// </summary>
    public class PathNode
    {
        // H COST IS DISTANCE FROM END NODE
        // G COST IS DISTANCE FROM STARTING NODE
        // F COST IS H COST + G COST

        /// <summary>
        /// 
        /// </summary>
        public int HCost, FCost, GCost;
        /// <summary>
        /// 
        /// </summary>
        public Point position, exploredFrom;
        /// <summary>
        /// 
        /// </summary>
        public bool hasBeencalculated, isWalkable, hasclosed = false;

        /// <summary>
        /// 
        /// </summary>
        public PathNode() // constructor
        {
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            // calculate if costs are -1 and bools false to reset pathfinding
        }
    }
}
// Calculate cost from start (check surrounding 'nodes' or tiles)
// cost to end/enemy
// move towards lowest cost (no diagonal)
// need to account for moving objects (keep calculating in update during turn)
// youtube Sebastian Lague pathfinding video on A star pathfinding