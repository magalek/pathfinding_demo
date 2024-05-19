using System;
using Map;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pathfinding
{
    public class Node
    {
        public event Action<Node> Invalidated;
        
        public readonly Tile Tile;
        
        public float Steps { get; set; }
        
        public float Heuristic { get; set; }
        
        public float Cost => Steps + Heuristic;

        public Node Parent;

        public Node(Tile tile)
        {
            Tile = tile;
            Tile.BecameBlocked += OnTileBecameBlocked;
        }

        public void SetParent(Node parent)
        {
            Parent = parent;
        }

        private void OnTileBecameBlocked() => Invalidated?.Invoke(this);

        public static bool operator ==(Node a, Node b) => a?.Tile == b?.Tile;

        public static bool operator !=(Node a, Node b) => !(a == b);

        ~Node()
        {
            Tile.BecameBlocked -= OnTileBecameBlocked;
            Invalidated = null;
        }
    }
}