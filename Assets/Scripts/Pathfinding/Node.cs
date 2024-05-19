using Map;
using Unity.VisualScripting;

namespace Pathfinding
{
    public class Node
    {
        public readonly Tile Tile;
        
        public float Steps { get; set; }
        
        public float Heuristic { get; set; }
        
        public float Cost => Steps + Heuristic;

        public Node Parent;

        public Node(Tile tile)
        {
            Tile = tile;
        }

        public void SetParent(Node parent)
        {
            Parent = parent;
        }

        public static bool operator ==(Node a, Node b) => a?.Tile == b?.Tile;

        public static bool operator !=(Node a, Node b) => !(a == b);
    }
}