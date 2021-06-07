using Common;
using Stadiums.Races;

namespace Stadiums.Runners
{

    public class RaceRunnerAI : RunnerController
    {

        private RunnerInput branchInput;
        private bool branchInputAvailable = false;
        private float takeBranchRatio = 0f;

        public void SetBranchInput(RunnerInput input, float ratio)
        {
            branchInput = input;
            branchInputAvailable = true;
            takeBranchRatio = ratio;
        }

        protected override void UpdateInput(float dt, ref RunnerInput runnerInput, ref RunnerState state)
        {
            if (!active)
                return;

            bool isFirst = RaceController.IsInFirstPosition(Runner);
            float distanceToPrevRunner = RaceController.GetDistanceToNextRunnerAtTheRear(Runner);
            runnerInput.shieldActivation = !Runner.IsStunned
                && RaceController.AmIAboutToReceiveAPulse(Runner);

            bool inRange = Runner.IsDistanceInReachOfPulse(distanceToPrevRunner);

            runnerInput.activateTurbo = !isFirst && true;
            runnerInput.pulseActivation = inRange;

            runnerInput.move = true;
            runnerInput.jump =
                state.jumpAvailable && state.collision.Floor
                && (runnerInput.faceRight && state.collision.right.Contact
                    || !runnerInput.faceRight && state.collision.left.Contact);

            if (branchInputAvailable)
            {
                branchInputAvailable = false;
                bool takeBranch = GameUtils.GetRandomBool(takeBranchRatio);
                if (takeBranch)
                {
                    runnerInput.move = branchInput.move;
                    runnerInput.jump = branchInput.jump;
                    runnerInput.faceRight = branchInput.faceRight;
                }
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