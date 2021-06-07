using Common;
using World;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Player
{

    public class InteractionModule : MonoBehaviour
    {

        [SerializeField]
        private float radius = 3f;
        [SerializeField]
        private LayerMask layers = 0;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.yellow;
            Handles.DrawWireArc(transform.position, Vector3.back, Vector3.up, 360f, radius);
        }
#endif

        public bool TryToInteract()
        {
            Debug.LogFormat("Try to interact");
            bool interaction = false;

            Vector2 center = transform.position;
            var collisions = PhysicsUtils.GetAllCollidersInRadius(center, radius, layers);
            Debug.LogFormat("Searching for interactives returned {0} collisions. Center at {1}, radius of {2}",
                collisions != null ? collisions.Length : 0, transform.position, radius);
            if (collisions != null
                && collisions.Length > 0)
            {
                float closestDistance = Mathf.Infinity;
                Interactable closestInteractable = null;
                for (int i = 0; i < collisions.Length; ++i)
                {
                    if (!collisions[i].TryGetComponent(out Interactable interactable))
                        continue;

                    float distance = math.distance(center, collisions[i].ClosestPoint(center));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
                
                if (closestInteractable != null)
                {
                    Debug.LogFormat("Interaction with {0}", closestInteractable.name);
                    closestInteractable.Interact();
                    interaction = true;
                }

            }

            return interaction;
        }

    }

}