using System;
using System.Collections.Generic;
using Map;
using UnityEngine;

namespace Pathfinding
{
    public class PathResult
    {
        public event Action Invalidated
        {
            add => Path.Invalidated += value;
            remove => Path.Invalidated -= value;
        }
        public event Action Broken
        {
            add => Path.Broken += value;
            remove => Path.Broken -= value;
        }

        public static PathResult Failed => new PathResult(false, null, null);
        public static PathResult Completed => new PathResult(true, null, null, new Queue<Tile>());
        
        public readonly bool Successful;
        public readonly Queue<Tile> TilesQueue;
        public readonly Tile Destination;

        private readonly Path Path;
        
        public PathResult(bool successful, Path path, Tile destination, Queue<Tile> tilesQueue = null)
        {
            Successful = successful;
            TilesQueue = tilesQueue;
            Path = path ?? Path.Empty;
            Destination = destination;
            if (Path != null) Path.Broken += OnPathBroken;
        }

        private void OnPathBroken()
        {
            foreach (var tile in TilesQueue)
            {
                tile.SetAsBroken();
            }
        }
    }
}