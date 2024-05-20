using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Configuration/MapConfig", fileName = "Map Config")]
    public class MapConfig : ScriptableObject
    {
        public GameObject TilePrefab;
        public Material NormalMaterial;
        public Material PathMaterial;
        public Material VisitedMaterial;
        public Material BlockedMaterial;
        public Material BrokenMaterial;
        public Vector2Int MapDimensions;
    }
}