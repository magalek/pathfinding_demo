using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game;
using Map;
using UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pathfinding
{
    public class Path
    {
        public event Action Invalidated;
        public event Action Broken;
        
        private readonly Node StartNode;
        private readonly Node EndNode;

        private readonly HashSet<Node> openSet = new HashSet<Node>();
        private readonly HashSet<Node> closedSet = new HashSet<Node>();

        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        
        public Path(Tile startTile, Tile endTile)
        {
            StartNode = new Node(startTile);
            EndNode = new Node(endTile);

            openSet.Add(StartNode);
            GameManager.Current.Destroyed += Cancel;
        }

        public void Cancel() => cancellationToken?.Cancel();

        public async Task<PathResult> Calculate()
        {
            if (StartNode == EndNode) return PathResult.Completed;

            foreach (var tile in MapManager.Current.AllTiles)
            {
                if (!tile.Blocked) tile.ResetMaterial();
            }
            
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
            }
            GameManager.Current.Destroyed -= Cancel;
            return currentNode != EndNode ? PathResult.Failed : new PathResult(true, this, EndNode.Tile, ConstructTilesQueue(currentNode));
        }

        private Queue<Tile> ConstructTilesQueue(Node currentNode)
        {
            var temporaryList = new List<Tile>();

            temporaryList.Add(currentNode.Tile);
            currentNode.Invalidated += OnNodeInvalidated;
            var parentNode = currentNode.Parent;
            while (parentNode != null && parentNode != StartNode)
            {
                parentNode.Invalidated += OnNodeInvalidated;
                temporaryList.Add(parentNode.Tile);
                parentNode = parentNode.Parent;
            }

            foreach (var tile in temporaryList)
            {
                if (GameManager.Current.Options.PathDrawingOption.Value) tile.SetAsPath();
                 
            }

            temporaryList.Reverse();
            var queue = new Queue<Tile>(temporaryList);
            return queue;
        }

        private void OnNodeInvalidated(Node node)
        {
            if (node == EndNode) Broken?.Invoke();
            else Invalidated?.Invoke();
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

        ~Path()
        {
            GameManager.Current.Destroyed -= Cancel;
            Cancel();
        }
    }
}