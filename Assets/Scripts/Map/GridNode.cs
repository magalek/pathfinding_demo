using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Map
{
    public class GridNode 
    {
        public readonly Vector2Int Position;
        
        public bool Occupied { get; private set; }

        private readonly MapGrid Grid;

        public GridNode(int x, int y)
        {
            Position = new Vector2Int(x, y);
            Grid = MapGrid.Current;
        }

        public GridNode(Vector2Int position) : this(position.x, position.y) { }
        
        public void Occupy()
        {
            if (Occupied) return;
            
            Occupied = true;
        }

        public void Free()
        {
            Occupied = false;
        }

        public IEnumerable<GridNode> GetNeighbours()
        {
            yield return Grid.GetNode(Position, Direction.Up);
            yield return Grid.GetNode(Position, Direction.Right);
            yield return Grid.GetNode(Position, Direction.Down);
            yield return Grid.GetNode(Position, Direction.Left);
        }
    }
}