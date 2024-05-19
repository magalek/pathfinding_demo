using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using Utility;

namespace Map
{
    public class Tile 
    {
        public readonly Vector2Int Position;

        public bool Blocked { get; private set; }

        private readonly MapManager manager;

        private readonly MeshRenderer Renderer;

        private CancellationTokenSource cancelationToken;
        
        public Tile(int x, int y, MeshRenderer renderer)
        {
            Position = new Vector2Int(x, y);
            manager = MapManager.Current;
            Renderer = renderer;
        }

        public Tile(Vector2Int position, MeshRenderer renderer) : this(position.x, position.y, renderer) { }
        
        public void ChangeBlockedState()
        {
            Blocked = !Blocked;
            ChangeColor(Blocked ? Color.black : Color.white);
        }
        

        public void ChangeColor(Color color)
        {
            Renderer.material.color = color;
        }
        
        public async void ChangeColor(Color color, int durationMs)
        {
            var material = Renderer.material;
            var lastColor = material.color;
            material.color = color;
            cancelationToken = new CancellationTokenSource();
            await Task.Delay(durationMs, cancelationToken.Token);
            material.color = lastColor;
        }

        public IEnumerable<Tile> GetNeighbours()
        {
            yield return manager.GetTile(Position, Direction.Up);
            yield return manager.GetTile(Position, Direction.Right);
            yield return manager.GetTile(Position, Direction.Down);
            yield return manager.GetTile(Position, Direction.Left);
        }
        
        ~Tile()
        {
            cancelationToken?.Cancel();
        }
    }
}