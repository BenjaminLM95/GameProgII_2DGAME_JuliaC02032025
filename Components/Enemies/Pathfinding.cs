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
{   // BETTER COMMENTS!!!
    internal class Pathfinding : Component
    {
        public PathNode[,] nodeMap;
        public List<PathNode> unexploredNodes = new List<PathNode>();
        public Point startingPoint, targetPoint;

        private TileMap tileMap;

        public override void Start()
        {
            Debug.WriteLine("Pathfinding: START");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void InitializePathfinding(TileMap tileMap) // need to call this...
        {
            if (tileMap == null)
            {
                Debug.WriteLine("Pathfinding: TileMap is NULL");
                return;
            }
            nodeMap = new PathNode[tileMap.mapWidth, tileMap.mapHeight];

            for (int x = 0; x < tileMap.mapWidth; x++)
            {
                for (int y = 0; y < tileMap.mapHeight; y++)
                {
                    var currentTile = tileMap.GetTileAt(x, y);

                    nodeMap[x, y] = new PathNode
                    {
                        position = new Point(x, y),
                        isWalkable = currentTile != null && currentTile.Texture == tileMap.floorTexture
                    };
                }
            }

            Debug.WriteLine("Pathfinding: nodeMap initialized using TileMap");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public List<Point> FindPath(Point start, Point goal)
        {
            

            if (nodeMap == null)
            {
                Debug.WriteLine("Pathfinding: nodeMap is NULL");
                return null;
            }

            foreach (var node in nodeMap)
            {
                node.Reset();
            }

            PathNode startNode = nodeMap[start.X, start.Y];
            PathNode goalNode = nodeMap[goal.X, goal.Y];

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, goalNode);
            startNode.FCost = startNode.GCost + startNode.HCost;

            List<PathNode> openList = new List<PathNode> { startNode };
            HashSet<PathNode> closedList = new HashSet<PathNode>();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);

                if (currentNode == goalNode)
                {
                    return RetracePath(startNode, goalNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (tentativeGCost < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = GetDistance(neighbor, goalNode);
                        neighbor.FCost = neighbor.GCost + neighbor.HCost;
                        neighbor.exploredFrom = currentNode.position;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return null; // Path not found
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private List<Point> RetracePath(PathNode startNode, PathNode endNode)
        {
            List<Point> path = new List<Point>();
            PathNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.position);
                currentNode = nodeMap[currentNode.exploredFrom.X, currentNode.exploredFrom.Y];
            }
            path.Reverse();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int GetDistance(PathNode a, PathNode b)
        {
            int dx = Math.Abs(a.position.X - b.position.X);
            int dy = Math.Abs(a.position.Y - b.position.Y);
            return dx + dy;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<PathNode> GetNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();
            Point[] directions = new Point[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };

            foreach (Point dir in directions)
            {
                int checkX = node.position.X + dir.X;
                int checkY = node.position.Y + dir.Y;

                if (checkX >= 0 && checkX < nodeMap.GetLength(0) && checkY >= 0 && checkY < nodeMap.GetLength(1))
                {
                    neighbors.Add(nodeMap[checkX, checkY]);
                }
            }
            return neighbors;
        }
    }

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
            // calculate if costs are -1 and bools false to reset pathfinding
            GCost = int.MaxValue;
            HCost = int.MaxValue;
            FCost = int.MaxValue;
            exploredFrom = Point.Zero;
        }
    }
}
