using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Map
{
    public class MapGrid : MonoManager<MapGrid>
    {
        private int width;
        private int height;
        
        private List<GridNode> nodes = new List<GridNode>();

        protected override void OnAwake()
        {
            CreateNodes(new Vector2(10, 10));
        }

        public void Initialize(int gridWidth, int gridHeight)
        {
            width = gridWidth;
            height = gridHeight;
        }

        public GridNode GetNode(Vector2Int position)
        {
            if (position.x < 0 || position.x >= width) return null;
            if (position.y < 0 || position.y >= height) return null;

            var index = position.x * height + (position.y + 1);
            return nodes[index];
        }
        
        public GridNode GetNode(Vector2Int position, Direction direction) => GetNode(position + DirectionHelper.ParseDirection(direction));

        private void CreateNodes(Vector2 dimensions)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    var newNode = new GridNode(x, y);
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = transform;
                    cube.transform.position = new Vector3(x, 0, y);
                }
            }
        }
    }
}