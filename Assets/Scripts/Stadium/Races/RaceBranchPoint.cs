using Stadiums.Runners;
using UnityEngine;

namespace Stadiums.Races
{

    [RequireComponent(typeof(Collider2D))]
    public class RaceBranchPoint : MonoBehaviour
    {

        [SerializeField]
        private RunnerInput input = new RunnerInput();
        [SerializeField]
        private float ratio = 0.5f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent == null
                || !collision.transform.parent.TryGetComponent(out Runner runner)
                || runner.transform.parent == null
                || !runner.transform.parent.TryGetComponent(out RaceRunnerAI ai))
                return;

            ai.SetBranchInput(input, ratio);
        }

    }

}