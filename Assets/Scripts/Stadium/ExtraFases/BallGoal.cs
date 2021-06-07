using Common;
using UnityEngine;

namespace Stadiums.ExtraFases
{

    public class BallGoal : MonoBehaviour
    {

        [SerializeField]
        private TriggerArea trigger = null;

        private int team = -1;
        private bool active = false;

        public void Activate(int team)
        {
            active = true;
            this.team = team;
            trigger.Activate(OnTriggerEnter2D, null);
        }

        public void Deactivate()
        {
            active = false;
            team = -1;
            trigger.Deactivate();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.LogFormat("Collision {0} (parent {1}) entered goal {2}. Is active? {3}",
                collision.name, collision.transform.parent.name, name, active);

            if (!active)
                return;

            GoalBallController.GoalAgainstTeam(team);
            Deactivate();
        }

    }

}