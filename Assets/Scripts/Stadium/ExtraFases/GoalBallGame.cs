using Game;
using UnityEngine;

namespace Stadiums.ExtraFases
{

    public class GoalBallGame : Level
    {

        [SerializeField]
        private BallGoal teamAGoal = null;
        [SerializeField]
        private BallGoal teamBGoal = null;
        [SerializeField]
        private Ball ball = null;
        [SerializeField]
        private SpawnPoint ballStartPoint = null;
        [SerializeField]
        private float timeLimit = 120f;

        public Ball GetBall()
            => ball;

        public SpawnPoint GetBallStartPoint()
            => ballStartPoint;

        public float GetTimeLimit()
            => timeLimit;

        public BallGoal GetTeamAGoal() => teamAGoal;
        public BallGoal GetTeamBGoal() => teamBGoal;

        private void Start()
        {
#if UNITY_EDITOR
            if (GameManager.DebugMode)
            {
                LevelManager.LoadLevel("", "", LevelType.GoalBallGame);
            }
#endif
            LevelManager.StartGoalBallGame(this);
        }

    }

}