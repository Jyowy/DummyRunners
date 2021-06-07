using Stadiums;
using Stadiums.Runners;
using UnityEngine;

namespace Player
{

    public class Player : RunnerController
    {

        [SerializeField]
        private PlayerInputController inputController = null;

        [SerializeField]
        private Runner debugRunner = null;

        private PlayerInputInfo input = null;

        private bool turboActivated = false;

        protected override void OnAwake()
        {
            if (debugRunner != null)
            {
                SetRunner(debugRunner);
                SetActive(true);
            }
        }

        protected override void UpdateInput(float dt, ref RunnerInput runnerInput, ref RunnerState state)
        {
            input = inputController.GetInputInfo(dt);
            runnerInput.faceRight = input.x > 0f
                || (input.x == 0f && runnerInput.faceRight);

            runnerInput.move = input.x != 0f;
            runnerInput.direction = new Vector2(input.x, input.y);
            runnerInput.jump = input.jump.Pressed;
            if (input.impulse.Pressed)
            {
                turboActivated = !turboActivated;
            }
            runnerInput.activateTurbo = turboActivated;
            runnerInput.pulseActivation = input.skill.Pressed;
            runnerInput.shieldActivation = input.interact.Pressed;
        }

        protected override bool CanUpdateState()
        {
            return !CheckPauseMenu();
        }

        protected override void OnStateUpdated(RunnerOutput runnerOutput)
        {
            if (runnerOutput.jumped)
                input.jump.Use();
            if (turboActivated
                && runnerOutput.pulseActive)
                input.skill.Use();
            input.impulse.Use();
            input.interact.Use();

            turboActivated = runnerOutput.turboActive;
        }

        private bool CheckPauseMenu()
        {
            bool pauseMenuLaunched = false;

            if (input.start.Stored)
            {
                pauseMenuLaunched = true;
                input.start.Use();
                LevelManager.ShowPauseMenu();
            }

            return pauseMenuLaunched;
        }

    }

}