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

        public void Quit() => Application.Quit();

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }
    }
}