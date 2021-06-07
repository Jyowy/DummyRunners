using Stadiums.Runners;
using UnityEngine;

namespace Stadiums.Races
{

    [RequireComponent(typeof(Collider2D))]
    public class RaceHole : MonoBehaviour
    {

        [SerializeField]
        private SpawnPoint respawnPosition = null;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent == null
                || !collision.transform.parent.TryGetComponent(out Runner runner))
                return;

            RaceController.RunnerFellInHole(runner, respawnPosition);
        }

    }

}