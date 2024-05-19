using System.Collections.Generic;
using Map;

namespace Pathfinding
{
    public struct PathResult
    {
        public static PathResult Failed => new PathResult(false);
        public static PathResult Completed => new PathResult(true, new Queue<Tile>());
        
        public readonly bool Successful;
        public readonly Queue<Tile> TilesQueue;
        
        public PathResult(bool successful, Queue<Tile> tilesQueue = null)
        {
            Successful = successful;
            TilesQueue = tilesQueue;
        }
    }
}