using UnityEngine;

namespace Stadiums.Runners
{

    public abstract class RunnerController : MonoBehaviour
    {

        private Runner runner = null;

        public void SetRunner(Runner runner)
            => this.runner = runner;

        public Runner Runner => runner;

        private RunnerState state;
        private RunnerInput runnerInput;
        private RunnerOutput runnerOutput;

        protected bool active { get; private set; } = false;

        public void SetActive(bool active)
        {
            this.active = active;

            runner.Restart();
            ClearInput();
        }

        public void SetPosition(SpawnPoint spawnPoint)
        {
            runner.SetPosition(spawnPoint);
            runnerInput.faceRight = spawnPoint.FaceRight;
        }

        private void Awake()
        {
            runnerInput.faceRight = true;
            ClearInput();

            runnerOutput.facingRight = true;
            runnerOutput.moved = false;
            runnerOutput.jumped = false;
            runnerOutput.onAir = false;
            runnerOutput.isClimbingCorner = false;
            runnerOutput.turboActive = false;
            runnerOutput.pulseActive = false;
            runnerOutput.shieldActive = false;

            OnAwake();
        }

        protected virtual void OnAwake() { }

        private void ClearInput()
        {
            runnerInput.move = false;
            runnerInput.jump = false;
            runnerInput.activateTurbo = false;
            runnerInput.pulseActivation = false;
            runnerInput.shieldActivation = false;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            ClearInput();
            runner.GetState(dt, out RunnerState state);
            UpdateInput(dt, ref runnerInput, ref state);

            if (!active
                || !CanUpdateState())
                return;

            runner.UpdateState(dt, ref state, ref runnerInput, out runnerOutput);
            runnerInput.faceRight = runnerOutput.facingRight;

            OnStateUpdated(runnerOutput);
        }

        protected abstract bool CanUpdateState();

        protected abstract void UpdateInput(float dt, ref RunnerInput runnerInput, ref RunnerState runnerState);

        protected abstract void OnStateUpdated(RunnerOutput runnerOutput);

    }

}