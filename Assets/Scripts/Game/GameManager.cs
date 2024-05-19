using System;
using Data;
using Managers;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoManager<GameManager>
    {
        public event Action Destroyed;
        
        public readonly Options Options = new Options();

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }
    }
}