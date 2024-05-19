using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Game;
using Managers;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Map
{
    public class MapManager : MonoManager<MapManager>
    {
        public MapConfig Config;
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public IReadOnlyList<Tile> AllTiles => tiles;

        private List<Tile> tiles = new List<Tile>();

        protected override void OnAwake()
        {
            MapWidth = Config.MapDimensions.x;
            MapHeight = Config.MapDimensions.y;
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
                    var tileMesh = Instantiate(Config.TilePrefab);
                    tileMesh.transform.parent = transform;
                    tileMesh.transform.position = new Vector3(x, 0, y);
                    tiles.Add(new Tile(x, y, tileMesh.GetComponent<MeshRenderer>()));
                }
            }
        }
    }
}