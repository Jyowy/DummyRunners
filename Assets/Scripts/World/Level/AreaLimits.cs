using UnityEngine;

namespace World.Level
{

    [RequireComponent(typeof(Collider2D))]
    public class AreaLimits : MonoBehaviour
    {

        private bool active = false;

        private System.Action onExitted = null; 

        public void Activate(System.Action onExitted)
        {
            active = true;
            this.onExitted = onExitted;
        }

        public void Deactivate()
        {
            active = false;
            onExitted = null;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.LogFormat("AreaLimits {0} OnTriggerExit2D", name);
            if (!active)
                return;

            onExitted?.Invoke();
        }

    }

}