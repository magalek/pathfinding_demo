using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Managers;
using Pathfinding;
using UnityEngine;
using Utility;

namespace Map
{
    public class MapManager : MonoManager<MapManager>
    {
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        
        private List<Tile> tiles = new List<Tile>();

        private CancellationTokenSource cancellationToken;

        protected override void OnAwake()
        {
            Initialize(10, 10);
            
        }

        public async void StartPath()
        {
            cancellationToken = new CancellationTokenSource();
            var result = await new Path(tiles[2], tiles[99]).Calculate(cancellationToken);
            Debug.Log(result.Successful);
        }
        
        public void Initialize(int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            CreateTiles();
        }

        public Tile GetTile(Vector2Int position)
        {
            if (position.x < 0 || position.x >= MapWidth) return null;
            if (position.y < 0 || position.y >= MapHeight) return null;

            var index = (position.x * MapHeight) + position.y;
            return tiles[index];
        }
        
        public Tile GetTile(Vector2Int position, Direction direction) => GetTile(position + DirectionHelper.ParseDirection(direction));

        private void CreateTiles()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = transform;
                    cube.transform.position = new Vector3(x, 0, y);
                    tiles.Add(new Tile(x, y, cube.GetComponent<MeshRenderer>()));
                }
            }
        }

        private void OnDestroy()
        {
            cancellationToken?.Cancel();
        }
    }
}