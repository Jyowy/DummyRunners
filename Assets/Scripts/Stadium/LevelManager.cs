using Cinemachine;
using Common;
using Game;
using Player;
using Player.Skills;
using Stadiums.ExtraFases;
using Stadiums.Races;
using Stadiums.Runners;
using UI.Menus;
using Unity.Mathematics;
using UnityEngine;

namespace Stadiums
{

    public class LevelManager : SingletonBehaviour<LevelManager>
    {

        [SerializeField]
        private RacePauseMenu pauseMenu = null;
        [SerializeField]
        private CountdownSplash countdownSplash = null;
        [SerializeField]
        private float countdownDuration = 3.1f;
        [SerializeField]
        private EnergyBatteryUI turboUI = null;
        [SerializeField]
        private SkillUI pulseUI = null;
        [SerializeField]
        private SkillUI shieldUI = null;

        [SerializeField]
        private new CinemachineVirtualCamera camera = null;

        private bool levelLoaded = false;
        private Level currentLevel = null;
        private bool isPlaying = false;
        private string currentStadium = null;
        private string currentLevelId = null;
        private LevelType currentLevelType = LevelType.Race;

        private Player.Player player = null;

        private EnergyBattery turboBattery = null;
        private PulseModule pulseModule = null;
        private ShieldModule shieldModule = null;

        public static void SetPlayer(Player.Player player)
        {
            if (!(Instance is LevelManager instance))
                return;

            instance.player = player;
            instance.OnPlayerSet();
        }

        public static Player.Player GetPlayer()
        {
            if (!(Instance is LevelManager instance))
                return null;

            return instance.player;
        }

        public static bool IsABallGame()
        {
            if (!(Instance is LevelManager instance))
                return false;

            return instance.currentLevelType == LevelType.GoalBallGame;
        }

        private void OnPlayerSet()
        {
            var playerRunner = player.Runner;
            turboBattery = playerRunner.GetEnergyBattery();
            float minValue = turboBattery.GetMinValue();
            float maxValue = turboBattery.GetMaxValue();
            turboUI.Initialize(minValue, maxValue);
            pulseModule = playerRunner.GetPulseModule();
            shieldModule = playerRunner.GetShieldModule();

            camera.Follow = playerRunner.transform;
        }

        protected override void OnInstantiated()
        {
            HideUI();
        }

        private void ShowUI()
        {
            turboUI.Show();
            pulseUI.Show();
            shieldUI.Show();

            if (turboBattery != null)
            {
                turboBattery.onConsuming += turboUI.SetConsuming;
            }
        }

        private void HideUI()
        {
            turboUI.Hide();
            pulseUI.Hide();
            shieldUI.Hide();

            if (turboBattery != null)
            {
                turboBattery.onConsuming -= turboUI.SetConsuming;
            }
        }

        public static void LoadLevel(string stadiumId, string levelId, LevelType levelType)
        {
            if (!(Instance is LevelManager instance))
                return;

            Debug.LogFormat("LevelManager: Load level stadium {0}, level {1}, which is a {2}",
                stadiumId, levelId, levelType);

            instance.levelLoaded = true;
            instance.isPlaying = false;
            instance.currentStadium = stadiumId;
            instance.currentLevelId = levelId;
            instance.currentLevelType = levelType;
        }

        public static void UnloadLevel()
        {
            if (!(Instance is LevelManager instance))
                return;

            Debug.LogFormat("LevelManager: UnloadLevel");

            AudioManager.StopMusic();

            instance.levelLoaded = false;
            instance.isPlaying = false;
            instance.currentStadium = null;
            instance.currentLevelId = null;
            instance.currentLevelType = LevelType.Race;
            SceneLoader.UnloadLevel();
        }

        public static void StartCountdown(System.Action onFinished)
        {
            if (!(Instance is LevelManager instance))
                return;

            instance.countdownSplash.Show(instance.countdownDuration, onFinished);
        }


        public static void ShowPauseMenu()
        {
            if (!(Instance is LevelManager instance))
                return;

            instance.pauseMenu.Show(RetryLevel, ExitGame);
        }

        public static void RetryLevel()
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded)
                return;

            instance.HideUI();
            if (instance.currentLevelType == LevelType.Race)
            {
                RaceController.RetryRace();
            }
            else if (instance.currentLevelType == LevelType.GoalBallGame)
            {
                GoalBallController.RetryGame();
            }
        }

        public static void StartLevel()
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded
                || instance.isPlaying)
                return;

            instance.isPlaying = true;
            instance.ShowUI();
            if (instance.currentLevelType == LevelType.Race)
            {
                RaceController.StartRace();
            }
            else if (instance.currentLevelType == LevelType.GoalBallGame)
            {
                GoalBallController.StartGame();
            }
        }

        public static void LevelFinished()
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded
                || !instance.isPlaying)
                return;

            instance.isPlaying = false;
            instance.HideUI();
        }

        public static void ExitGame()
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded)
                return;

            instance.HideUI();
            if (instance.currentLevelType == LevelType.Race)
            {
                RaceController.ExitRace();
            }
            else if (instance.currentLevelType == LevelType.GoalBallGame)
            {
                GoalBallController.ExitGame();
            }

            UnloadLevel();
        }

        public static void StartRace(Race race)
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded
                || instance.currentLevelType != LevelType.Race)
                return;

            RaceController.InitializeRace(race);
        }

        public static void RaceCompleted(int position)
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded
                || !instance.isPlaying)
                return;

            instance.camera.Follow = null;
            GameDataManager.RaceFinished(instance.currentStadium, instance.currentLevelId, position);

            LevelFinished();
        }

        public static void StartGoalBallGame(GoalBallGame game)
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded
                || instance.currentLevelType != LevelType.GoalBallGame)
                return;

            Debug.LogFormat("StartGoalBallGame {0}", game != null ? game.name : "null");
            GoalBallController.InitializeGame(game);
        }

        public static void GoalBallGameCompleted(int winTeam, int playerTeam)
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded
                || !instance.isPlaying)
                return;

            if (instance.currentLevelType == LevelType.GoalBallGame)
            {
                GameDataManager.BallGameFinished(instance.currentStadium, instance.currentLevelId, playerTeam == winTeam);
            }

            LevelFinished();
        }

        private void FixedUpdate()
        {
            if (!isPlaying)
                return;

            float turboValue = turboBattery.GetCurrentValue();
            turboUI.UpdateUI(turboValue);
            float pulseCooldownProgress = pulseModule.GetCooldownProgress();
            pulseUI.UpdateCooldownProgress(pulseCooldownProgress);
            float shieldCooldownProgress = shieldModule.GetCooldownProgress();
            shieldUI.UpdateCooldownProgress(shieldCooldownProgress);

            if (isCameraShaking)
            {
                float dt_unscaled = Time.fixedUnscaledDeltaTime;
                cameraShake.Update(dt_unscaled);
                if (cameraShake.Completed)
                    NextShakeCycle();

                Vector2 position = math.lerp(lastCameraPoint, nextCameraPoint, cameraShake.GetProgress());
                cameraTransposer.m_ScreenX = position.x;
                cameraTransposer.m_ScreenY = position.y;

                if (shakeTotalTime >= shakeDuration)
                {
                    FinishShaking();
                }
                shakeTotalTime += dt_unscaled;
            }
        }

        private bool isCameraShaking = false;
        private TimedAction cameraShake;
        private Vector2 cameraOriginalPosition = Vector2.zero;
        private Vector2 lastCameraPoint = Vector2.zero;
        private Vector2 nextCameraPoint = Vector2.zero;
        private CinemachineFramingTransposer cameraTransposer = null;
        private float shakePower = 0f;
        private float shakeCycleDuration = 0f;
        private float shakeTotalTime = 0f;
        private float shakeDuration = 0f;
        private Vector2 cameraShakeDirection = Vector2.zero;
        private Vector2 cameraOriginalDamping = Vector2.zero;

        public static void ShakeCamera(float shakePower, int cycles, float duration)
        {
            if (!(Instance is LevelManager instance)
                || !instance.levelLoaded)
                return;

            instance.isCameraShaking = true;
            instance.cameraTransposer = instance.camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            instance.cameraOriginalPosition.x = instance.cameraTransposer.m_ScreenX;
            instance.cameraOriginalPosition.y = instance.cameraTransposer.m_ScreenY;
            instance.cameraOriginalDamping.x = instance.cameraTransposer.m_XDamping;
            instance.cameraOriginalDamping.y = instance.cameraTransposer.m_YDamping;

            instance.shakePower = shakePower;
            instance.shakeCycleDuration = duration / cycles;
            instance.shakeTotalTime = 0f;
            instance.shakeDuration = duration;
            instance.cameraTransposer.m_XDamping = 0.125f;
            instance.cameraTransposer.m_YDamping = 0.125f;

            instance.StartShakeCycle();
        }

        private void StartShakeCycle()
        {
            cameraShakeDirection =
                GameUtils.GetRandomBool()
                ? Vector2.right
                : Vector2.left;

            NextShakeCycle();
        }

        private void NextShakeCycle()
        {
            cameraShakeDirection = cameraShakeDirection == Vector2.right
                ? Vector2.left
                : Vector2.right;

            float shakePower = math.lerp(0f, this.shakePower, 1f - math.clamp(shakeTotalTime / shakeDuration, 0f, 1f));
            lastCameraPoint.Set(cameraTransposer.m_ScreenX, cameraTransposer.m_ScreenY);
            nextCameraPoint = cameraOriginalPosition + cameraShakeDirection * shakePower;

            cameraShake.Set(shakeCycleDuration);
        }

        private void FinishShaking()
        {
            isCameraShaking = false;
            cameraTransposer.m_ScreenX = cameraOriginalPosition.x;
            cameraTransposer.m_ScreenY = cameraOriginalPosition.y;
            cameraTransposer.m_XDamping = cameraOriginalDamping.x;
            cameraTransposer.m_YDamping = cameraOriginalDamping.y;
        }

    }

}