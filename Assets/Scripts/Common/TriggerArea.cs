using UnityEngine;

namespace Common
{

    [RequireComponent(typeof(Collider2D))]
    public class TriggerArea : MonoBehaviour
    {

        public bool IsActive { get; private set; } = false;

        private System.Action<Collider2D> onTriggerEnter = null;
        private System.Action<Collider2D> onTriggerExit = null;

        public void Activate(System.Action<Collider2D> onTriggerEnter, System.Action<Collider2D> onTriggerExit)
        {
            IsActive = true;
            this.onTriggerEnter = onTriggerEnter;
            this.onTriggerExit = onTriggerExit;
        }

        public void Deactivate()
        {
            IsActive = false;
            this.onTriggerEnter = null;
            this.onTriggerExit = null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.LogFormat("Collision {0}, parent {1} entered trigger area",
                collision.name, transform.parent.name);
            if (!IsActive)
                return;

            onTriggerEnter?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!IsActive)
                return;

            onTriggerExit?.Invoke(collision);
        }

    }

}
