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
        public PathNode[,] nodeMap;
        public List<PathNode> unexploredNodes = new List<PathNode>();
        public Point startingPoint, targetPoint;

        public override void Start()
        {
            Debug.WriteLine("Pathfinding: START");
        }

        public void InitializePathfinding(char[,] map)
        {
            nodeMap = new PathNode[map.GetLength(0), map.GetLength(1)];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    nodeMap[x, y] = new PathNode
                    {
                        position = new Point(x, y),
                        isWalkable = map[x, y] != 'X' // meaning wall or obstacles
                    };
                }
            }
        }

        public List<Point> FindPath(Point start, Point goal)
        {
            if (nodeMap == null) return null;

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

        private int GetDistance(PathNode a, PathNode b)
        {
            int dx = Math.Abs(a.position.X - b.position.X);
            int dy = Math.Abs(a.position.Y - b.position.Y);
            return dx + dy;
        }

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
// Calculate cost from start (check surrounding 'nodes' or tiles)
// cost to end/enemy
// move towards lowest cost (no diagonal)
// need to account for moving objects (keep calculating in update during turn)
// youtube Sebastian Lague pathfinding video on A star pathfinding
// checking neighbors

// [ ] [X Y+1 ] [ ]
// [X Y-1 ] [X Y ] [X Y+1 ]
// [ ] [X Y-1 ] [ ]

// when checking neoghbors, only read neighbors if X, Y are one of them 0
// like if X != 0 && Y != 0 continue;