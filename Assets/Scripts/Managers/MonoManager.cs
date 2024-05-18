using UnityEngine;

namespace Managers
{
    public class MonoManager<TManager> : MonoBehaviour, IMonoManager where TManager : IMonoManager
    {
        public static TManager Current => (TManager)ManagerLoader.Get<TManager>();
        
        private void Awake()
        {
            ManagerLoader.RegisterMonoManager(this);
            OnAwake();
        }
        
        protected virtual void OnAwake(){}

        void IMonoManager.Destroy()
        {
            Destroy(gameObject);
        }
    }
}