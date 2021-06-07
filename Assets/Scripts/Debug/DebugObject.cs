#if UNITY_EDITOR

using UnityEngine;

namespace Game
{

    public class DebugObject : MonoBehaviour
    {

        private static DebugObject instance = null;

        private void Awake()
        {
            if (instance == null)
            {
                if (GameManager.DebugMode)
                {
                    instance = this;
                }
            }

            if (instance != this)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

    }

}

#endif