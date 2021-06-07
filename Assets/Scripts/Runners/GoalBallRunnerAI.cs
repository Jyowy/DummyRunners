using Common;
using Stadiums.ExtraFases;
using Stadiums.Races;
using UnityEngine;

namespace Stadiums.Runners
{

    public class GoalBallRunnerAI : RunnerController
    {

        protected override void UpdateInput(float dt, ref RunnerInput runnerInput, ref RunnerState state)
        {
            if (!active)
                return;

            runnerInput.move = true;
            Vector2 runnerPosition = Runner.transform.position;

            float distance = GoalBallController.GetDistanceToClosestRival(Runner);
            runnerInput.pulseActivation = Runner.IsDistanceInReachOfPulse(distance)
                && !Runner.HasTheBall();

            bool pulseInComing = GoalBallController.AmIAboutToReceiveAPulse(Runner);

            runnerInput.shieldActivation = false;

            if (!Runner.IsStunned
                && pulseInComing)
            {
                var shield = Runner.GetShieldModule();
                if (!shield.IsActive)
                {
                    if (Runner.GetShieldModule().CanActivate())
                    {
                        runnerInput.shieldActivation = true;
                    }
                    else if (Runner.HasTheBall()
                        && Runner.GetPulseModule().CanActivate())
                    {
                        Vector2 rivalGoal = GoalBallController.GetRivalGoalPosition(Runner.Team);
                        runnerInput.pulseActivation = true;
                        runnerInput.direction = ((rivalGoal - runnerPosition).normalized + Vector2.up).normalized;
                    }
                }
            }

            if (Runner.HasTheBall())
            {
                Vector2 rivalGoal = GoalBallController.GetRivalGoalPosition(Runner.Team);
                runnerInput.faceRight = rivalGoal.x > runnerPosition.x;

                runnerInput.activateTurbo = true;
                runnerInput.shieldActivation = true;

                Debug.LogFormat("Runner {0} has the ball! It's from team {1} so goes to rival goal at {2}",
                    Runner.name, Runner.Team, rivalGoal);
            }
            else
            {
                Vector2 ballPosition = GoalBallController.GetBallPosition();
                runnerInput.faceRight = ballPosition.x > runnerPosition.x;
            }
        }

        protected override bool CanUpdateState()
        {
            return true;
        }

        protected override void OnStateUpdated(RunnerOutput runnerOutput)
        {
        }

    }

}