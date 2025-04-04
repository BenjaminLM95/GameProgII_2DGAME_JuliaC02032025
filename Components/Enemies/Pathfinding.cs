using Microsoft.VisualBasic;
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
        // ---------- REFERENCES ---------- //
        private Globals globals;
        private TileMap tileMap;

        // ---------- VARIABLES ---------- //
        // represents the walkable and non-walkable areas of the map
        public PathNode[,] nodeMap; 

        // List to track unexplored nodes during the pathfinding
        public List<PathNode> unexploredNodes = new List<PathNode>();

        // Start and goal points for pathfinding
        public Point startingPoint, targetPoint;

        // ---------- METHODS ---------- //
        public override void Start()
        {
            globals = globals ?? Globals.Instance; // globals
            if (globals == null)
            {
                Debug.WriteLine("Pathfinding: globals is NULL!");
                throw new InvalidOperationException("Globals instance could not be initialized");
            }

            Debug.WriteLine("Pathfinding: START");
            tileMap = globals._mapSystem?.Tilemap; // get the TileMap instance from the scene
            tileMap = GameObject.FindObjectOfType<TileMap>();
            if (tileMap == null)
            {
                Debug.WriteLine("Pathfinding: Could not find TileMap in the scene!");
            }
            else {
                Debug.WriteLine($"Pathfinding: Found TileMap. Map dimensions - Width: {tileMap.mapWidth}, Height: {tileMap.mapHeight}");
            }
        }

        /// <summary>
        /// Initialize the pathfinding system, setting up the node map
        /// </summary>
        /// <param name="tileMap"></param>
        /// <param name="debug"></param>
        public void InitializePathfinding(TileMap tileMap, bool debug = false) 
        {
            if (tileMap == null)
            {
                if (debug) Debug.WriteLine("Pathfinding: TileMap is NULL");
                return;
            }

            if (debug) Debug.WriteLine($"Pathfinding: Initializing with map dimensions - Width: {tileMap.mapWidth}, Height: {tileMap.mapHeight}");

            // Initialize the node map with dimensions based on tile map
            nodeMap = new PathNode[tileMap.mapWidth, tileMap.mapHeight];

            int walkableTiles = 0;
            int nonWalkableTiles = 0;

            // Loop through each tile in the map
            for (int x = 0; x < tileMap.mapWidth; x++)
            {
                for (int y = 0; y < tileMap.mapHeight; y++)
                {
                    var currentTile = tileMap.GetTileAt(x, y);
                    if (currentTile == null)
                    {
                        if (debug) Debug.WriteLine($"Pathfinding: Tile at ({x},{y}) is NULL!");
                        continue;
                    }

                    // Check if the tile is walkable based on its texture
                    bool isWalkable = currentTile != null && currentTile.Texture == tileMap.floorTexture;

                    // Create a new PathNode representing this tile
                    nodeMap[x, y] = new PathNode
                    {
                        position = new Point(x, y),
                        isWalkable = isWalkable
                    };

                    // Count the number of walkable and non-walkable tiles
                    if (isWalkable)
                        walkableTiles++;
                    else
                        nonWalkableTiles++;
                }
            }
            // Debug log initialization result
            if (debug) Debug.WriteLine($"Pathfinding: Initialization Complete");
            if (debug) Debug.WriteLine($"Pathfinding: Total Tiles - Walkable: {walkableTiles}, Non-Walkable: {nonWalkableTiles}");
            if (debug) Debug.WriteLine($"Pathfinding: NodeMap is NULL: {nodeMap == null}");
            if (debug) Debug.WriteLine($"Pathfinding: NodeMap Dimensions - {nodeMap?.GetLength(0)}x{nodeMap?.GetLength(1)}");
        }

        /// <summary>
        /// Find a path from the start point to the goal point using A* algorithm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public List<Point> FindPath(Point start, Point goal, bool debug = false)
        {
            if (debug) Debug.WriteLine("Pathfinding: FindPath method called");

            if (nodeMap == null)
            {
                if (debug) Debug.WriteLine("Pathfinding: nodeMap is NULL");
                return null;
            }

            foreach (var node in nodeMap) // Reset all nodes in the map
            {
                node.Reset(); // when entering new level, node becomes null
            }

            // Get the start and goal nodes
            PathNode startNode = nodeMap[start.X, start.Y];
            PathNode goalNode = nodeMap[goal.X, goal.Y];

            // Set the starting node's cost values
            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, goalNode);
            startNode.FCost = startNode.GCost + startNode.HCost;

            // Open and closed lists for A* algorithm
            List<PathNode> openList = new List<PathNode> { startNode };
            HashSet<PathNode> closedList = new HashSet<PathNode>();

            // Perform A* search
            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList); // Get the node with the lowest F cost

                if (currentNode == goalNode) // If goal node is reached, retrace and return the path
                {
                    return RetracePath(startNode, goalNode);
                }

                // Move current node to the closed list
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Process each neighbor of the current node
                foreach (PathNode neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    // Calculate the G cost for the neighbor
                    int tentativeGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (tentativeGCost < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        // Update the neighbor's cost values
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = GetDistance(neighbor, goalNode);
                        neighbor.FCost = neighbor.GCost + neighbor.HCost;
                        neighbor.exploredFrom = currentNode.position;

                        // Add neighbor to open list if not already present
                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
            if (debug) Debug.WriteLine("Pathfinding: FindPath completed");
            return null; // Path not found
        }

        // Retrace the path from the goal node to the start node
        private List<Point> RetracePath(PathNode startNode, PathNode endNode)
        {
            List<Point> path = new List<Point>();
            PathNode currentNode = endNode;

            // Traverse the path by following the exploredFrom links
            while (currentNode != startNode)
            {
                path.Add(currentNode.position);
                currentNode = nodeMap[currentNode.exploredFrom.X, currentNode.exploredFrom.Y];
            }

            // Reverse the path to get it from start to goal
            path.Reverse();
            return path;
        }

        // Calculate the distance between two nodes
        private int GetDistance(PathNode a, PathNode b)
        {
            int dx = Math.Abs(a.position.X - b.position.X);
            int dy = Math.Abs(a.position.Y - b.position.Y);
            return dx + dy;
        }

        // Get the node with the lowest F cost from a list
        private PathNode GetLowestFCostNode(List<PathNode> nodeList)
        {
            PathNode lowestNode = nodeList[0];
            foreach (var node in nodeList)
            {
                if (node.FCost < lowestNode.FCost)
                {
                    lowestNode = node;
                }
            }
            return lowestNode;
        }

        // Get the walkable neighbors of a node
        private List<PathNode> GetNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();
            Point[] directions = new Point[]
            {
                new Point(0, 1),   // Down
                new Point(0, -1),  // Up
                new Point(1, 0),   // Right
                new Point(-1, 0)   // Left
            };

            // Check each direction for walkable neighbors
            foreach (Point dir in directions)
            {
                int checkX = node.position.X + dir.X;
                int checkY = node.position.Y + dir.Y;

                // Ensure the neighbor is within the bounds of the map
                if (checkX >= 0 && checkX < nodeMap.GetLength(0) &&
                    checkY >= 0 && checkY < nodeMap.GetLength(1))
                {
                    PathNode neighbor = nodeMap[checkX, checkY];

                    // If ignoring obstacles, all tiles are valid
                    if (neighbor.isWalkable)
                    {
                        neighbors.Add(neighbor); // Only add walkable neighbors
                    }
                }
            }
            return neighbors;
        }
    }

    // ========== PATH NODE CLASS ========== //
    // PathNode represents a single tile in the pathfinding system
    public class PathNode
    {
        // H COST IS DISTANCE FROM END NODE
        // G COST IS DISTANCE FROM STARTING NODE
        // F COST IS H COST + G COST

        public int HCost, FCost, GCost;
        public Point position, exploredFrom;
        public bool hasBeencalculated, isWalkable, hasclosed = false;

        public void Reset()
        {
            // Reset the node's cost values for a new search
            // calculate if costs are -1 and bools false to reset pathfinding
            GCost = int.MaxValue;
            HCost = int.MaxValue;
            FCost = int.MaxValue;
            exploredFrom = Point.Zero;
        }
    }
}
