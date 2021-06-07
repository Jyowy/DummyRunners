using Stadiums.Runners;
using UnityEngine;

namespace Stadiums.Races
{

    [RequireComponent(typeof(Collider2D))]
    public class RaceEndArea : MonoBehaviour
    {

        public System.Action<Runner> runnerReachedEnd = null;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "EndArea");
        }
#endif

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var parent = collision.transform.parent;
            if (parent == null
                || !parent.TryGetComponent(out Runner runner))
                return;

            runnerReachedEnd?.Invoke(runner);
        }

    }

}