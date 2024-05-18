using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public static class ManagerLoader
    {
        private static readonly Dictionary<Type, IMonoManager> ManagersDictionary = new Dictionary<Type, IMonoManager>();

        public static void RegisterMonoManager(IMonoManager manager)
        {
            if (ManagersDictionary.ContainsKey(manager.GetType()))
            {
                Debug.LogWarning($"Another instance of {manager.GetType()} tried registering - destroying.");
                manager.Destroy();
                return;
            }
            ManagersDictionary[manager.GetType()] = manager;
        }
        
        public static object Get(Type managerType) => ManagersDictionary.TryGetValue(managerType, out IMonoManager manager) ? manager : null;

        public static object Get<T>() => Get(typeof(T));
    }
}