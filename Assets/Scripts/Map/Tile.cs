using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Map
{
    public class Tile
    {
        public event Action BecameBlocked;
        
        public readonly Vector3 WalkablePosition;
        
        public readonly Vector2Int Position;

        public bool Blocked { get; private set; }

        private readonly MapManager manager;

        private readonly MeshRenderer Renderer;

        public Tile(int x, int y, MeshRenderer renderer)
        {
            Position = new Vector2Int(x, y);
            manager = MapManager.Current;
            Renderer = renderer;
            WalkablePosition = new Vector3(x, renderer.transform.localScale.y / 2, y);
        }

        public void ChangeBlockedState()
        {
            Blocked = !Blocked;
            if (Blocked)
            {
                Renderer.material = MapManager.Current.Config.BlockedMaterial;
                BecameBlocked?.Invoke();
            }
            else
                Renderer.material = MapManager.Current.Config.NormalMaterial;
        }

        public void SetAsPath() => Renderer.material = MapManager.Current.Config.PathMaterial;
        public void SetAsVisited() => Renderer.material = MapManager.Current.Config.VisitedMaterial;
        public void SetAsBroken()
        {
            if (Blocked) return;
            Renderer.material = MapManager.Current.Config.BrokenMaterial;
        }

        public void ResetMaterial() => Renderer.material = MapManager.Current.Config.NormalMaterial;

        public IEnumerable<Tile> GetNeighbours()
        {
            yield return manager.GetTile(Position, Direction.Up);
            yield return manager.GetTile(Position, Direction.Right);
            yield return manager.GetTile(Position, Direction.Down);
            yield return manager.GetTile(Position, Direction.Left);
        }
    }
}