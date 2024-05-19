using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Map;
using UnityEngine;

namespace Pathfinding
{
    public class Path
    {
        private readonly Node StartNode;
        private readonly Node EndNode;

        private readonly HashSet<Node> openSet = new HashSet<Node>();
        private readonly HashSet<Node> closedSet = new HashSet<Node>();

        public Path(Tile startTile, Tile endTile)
        {
            StartNode = new Node(startTile);
            EndNode = new Node(endTile);

            openSet.Add(StartNode);
        }
        
        public async Task<PathResult> Calculate(CancellationTokenSource cancellationToken)
        {
            if (StartNode == EndNode) return PathResult.Completed;

            Node currentNode = null;
            
            while (openSet.Count > 0 && currentNode != EndNode)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.LogWarning("Path calculation cancelled.");
                    return PathResult.Failed;
                }
                
                currentNode = GetMinimumCostOpenNode();
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                CalculateNodeNeighbours(currentNode);
                await Task.Delay(10);
                ColorNodes();
            }
            return currentNode != EndNode ? PathResult.Failed : new PathResult(true, ConstructTilesQueue(currentNode));
        }

        private static Queue<Tile> ConstructTilesQueue(Node currentNode)
        {
            var temporaryList = new List<Tile>();

            var parentNode = currentNode.Parent;
            currentNode.Tile.ChangeColor(Color.blue);
            while (parentNode != null)
            {
                parentNode.Tile.ChangeColor(Color.blue);
                temporaryList.Add(parentNode.Tile);
                parentNode = parentNode.Parent;
            }

            temporaryList.Reverse();
            var queue = new Queue<Tile>(temporaryList);
            return queue;
        }

        private void ColorNodes()
        {
            foreach (var node in openSet)
            {
                node.Tile.ChangeColor(Color.green);
            }
        }

        private void CalculateNodeNeighbours(Node parentNode)
        {
            foreach (var tile in parentNode.Tile.GetNeighbours())
            {
                if (tile == null) continue;
                if (closedSet.Any(node => node.Tile == tile)) continue;
                if (tile.Blocked)
                {
                    closedSet.Add(new Node(tile));
                    continue;
                }
                if (openSet.Any(node => node.Tile == tile)) continue;
                var newNode = new Node(tile)
                {
                    Steps = parentNode.Steps + 1,
                    Heuristic = Vector2Int.Distance(tile.Position, EndNode.Tile.Position),
                    Parent = parentNode
                };
                openSet.Add(newNode);
            }
        }

        private Node GetMinimumCostOpenNode()
        {
            float bestCost = float.MaxValue;
            Node bestNode = null;
            foreach (var node in openSet)
            {
                if (node.Cost < bestCost)
                {
                    bestCost = node.Cost;
                    bestNode = node;
                }
            }
            return bestNode;
        }
    }
}