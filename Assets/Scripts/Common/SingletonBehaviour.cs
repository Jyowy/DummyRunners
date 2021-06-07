#if UNITY_EDITOR
using Game;
#endif
using UnityEngine;

namespace Common
{

    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {

        protected static SingletonBehaviour<T> Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarningFormat("Multiple instances of {0} detected {2}, previous {1}", this.GetType(), Instance.name, name);
                return;
            }

            Instance = this;
            OnInstantiated();
        }

        protected virtual void OnInstantiated() { }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            OnDestroyed();
            Instance = null;
        }

        protected virtual void OnDestroyed() { }

    }

}