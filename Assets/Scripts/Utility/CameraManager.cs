using System;
using Managers;
using Map;
using UnityEngine;

namespace Utility
{
    public class CameraManager : MonoManager<CameraManager>
    {
        private void Start()
        {
            transform.position = new Vector3(MapManager.Current.MapWidth / 2, MapManager.Current.MapHeight, 0);
        }
    }
}