using UnityEngine;

namespace Utility
{
    public static class DirectionHelper
    {
        public static Vector2Int ParseDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2Int.up,
                Direction.Right => Vector2Int.right,
                Direction.Down => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                _ => Vector2Int.zero
            };
        }
    }
}