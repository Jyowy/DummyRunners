using UnityEngine;
using UnityEngine.Events;

namespace World
{

    public class Interactable : MonoBehaviour
    {

        public UnityEvent onInteraction;

        public void Interact()
        {
            onInteraction?.Invoke();
            OnInteraction();
        }

        protected virtual void OnInteraction() { }

    }

}