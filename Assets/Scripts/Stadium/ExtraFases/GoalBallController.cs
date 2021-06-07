using Common;
using Game;
using Stadiums.Runners;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Menus;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stadiums.ExtraFases
{

    public class GoalBallController : SingletonBehaviour<GoalBallController>
    {

        [SerializeField]
        private float respawnTime = 0.5f;
        [SerializeField]
        private GoalSplash goalSplash = null;
        [SerializeField]
        private float goalCameraShakePower = 0.25f;
        [SerializeField]
        private int goalCameraShakeFrequency = 10;
        [SerializeField]
        private float goalCameraShakeDuration = 0.25f;
        [SerializeField]
        private float goalSlowMotionScale = 0.1f;
        [SerializeField]
        private float goalSlowMotionDuration = 1.5f;
        [SerializeField]
        private float goalPauseTime = 1f;

        // Debug
        [SerializeField]
        private GoalBallGame debugGame = null;
        [SerializeField]
        private Player.Player playerPrefab = null;
        [SerializeField]
        private Runner runnerPrefab = null;
        [SerializeField]
        private CanvasFader screenFader = null;
        [SerializeField]
        private List<RunnerData> runnerDatasDebug = null;

        [SerializeField]
        private GoalBallUI ui = null;
        [SerializeField]
        private GoalBallResults gameResults = null;

        [SerializeField]
        private float ballStartLunchPower = 5f;

        [SerializeField]
        private bool skipStart = true;

        private GoalBallGame game = null;

        private bool isGameActive = false;
        private float gameTime = 0f;

        private Player.Player player = null;
        private Ball ball = null;
        private BallGoal teamAGoal = null;
        private BallGoal teamBGoal = null;

        private int totalRunners = 0;
        private int runnersPerTeam = 0;
        private readonly List<RunnerController> controllers = new List<RunnerController>();
        private readonly List<RunnerController> teamAControllers = new List<RunnerController>();
        private readonly List<RunnerController> teamBControllers = new List<RunnerController>();
        private int teamAPoints = 0;
        private int teamBPoints = 0;

        private GameObject runnersRoot = null;

        public static void GetCurrentScore(out int teamAPoints, out int teamBPoints)
        {
            teamAPoints = 0;
            teamBPoints = 0;
            if (!(Instance is GoalBallController instance))
                return;

            teamAPoints = instance.teamAPoints;
            teamBPoints = instance.teamBPoints;
        }

        public static float GetCurrentTime()
        {
            if (!(Instance is GoalBallController instance))
                return 0f;

            return instance.gameTime;
        }

        public static int GetPlayerTeam()
        {
            if (!(Instance is GoalBallController instance))
                return -1;

            return instance.player.Runner.Team;
        }

        public static Vector2 GetBallPosition()
        {
            if (!(Instance is GoalBallController instance))
                return Vector2.zero;

            return instance.ball.transform.position;
        }

        public static Collider2D GetBallCollider()
        {
            if (!(Instance is GoalBallController instance))
                return null;

            return instance.ball.GetCollider();
        }

        public static Vector2 GetBallVelocity()
        {
            if (!(Instance is GoalBallController instance))
                return Vector2.zero;

            return instance.ball.GetVelocity();
        }

        public static bool IsBallInMyTeam(int myTeam)
        {
            if (!(Instance is GoalBallController instance))
                return false;

            return instance.ball.GetTeam() == myTeam;
        }

        public static Vector2 GetRivalGoalPosition(int myTeam)
        {
            if (!(Instance is GoalBallController instance))
                return Vector2.zero;

            return myTeam == 0
                ? instance.teamBGoal.transform.position
                : instance.teamAGoal.transform.position;
        }

        private void Start()
        {
            // Debug
            if (debugGame != null)
            {
                InitializeGame(debugGame);
            }
        }

        public static void InitializeGame(GoalBallGame game)
        {
            if (!(Instance is GoalBallController instance)
                || game == null)
                return;

            Debug.LogFormat("InitializeGame {0}", game.name);
            instance.game = game;
            instance.InitializeGame();
        }

        public static void ExitGame()
        {
            if (!(Instance is GoalBallController instance))
                return;

            Debug.LogFormat("GoalBallController ExitGame");
            instance.isGameActive = false;
            instance.ui.Hide();
            instance.gameResults.Hide();
            instance.goalSplash.Hide();
            instance.RemoveRunners();
            instance.runnersRoot = null;
        }

        private void InitializeGame()
        {
            Debug.LogFormat("Initialize game");

            GameObject runnersRoot = GetRunnersRoot();
            var levelScene = SceneLoader.GetLevelScene();
            var rootObjects = levelScene.GetRootGameObjects();
            int rootCount = rootObjects != null ? rootObjects.Length : 0;
            for (int i = 0; i < rootCount; ++i)
            {
                if (rootObjects[i].name == "RunnersRoot")
                {
                    runnersRoot = rootObjects[i];
                    break;
                }
            }
            if (runnersRoot == null)
            {
                runnersRoot = new GameObject("RunnersRoot");
                SceneManager.MoveGameObjectToScene(runnersRoot, levelScene);
            }

            int count = runnersRoot.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                var children = runnersRoot.transform.GetChild(i);
                Destroy(children.gameObject, 0.1f);
            }
            runnersRoot.transform.DetachChildren();

            var startPoints = game.GetStartPoints();

            int raceSlots = startPoints.Count;
            runnersPerTeam = 0;
            controllers.Clear();
            teamAControllers.Clear();
            teamBControllers.Clear();
            teamAPoints = 0;
            teamBPoints = 0;
            gameTime = 0f;

            for (int i = 0; i < raceSlots - 1; ++i)
            {
                GameObject newRunner = new GameObject();
                GoalBallRunnerAI runnerAI = newRunner.AddComponent<GoalBallRunnerAI>();
                newRunner.transform.parent = runnersRoot.transform;
                var runner = GameObject.Instantiate(runnerPrefab, runnerAI.transform);
                runner.SetData(runnerDatasDebug[i]);

                runner.name = string.Format("Runner_{0}", i);
                runnerAI.SetRunner(runner);
                controllers.Add(runnerAI);
            }

            player = GameObject.Instantiate(playerPrefab, runnersRoot.transform);
            var playerRunner = GameObject.Instantiate(runnerPrefab, player.transform);
            playerRunner.SetData(runnerDatasDebug[raceSlots - 1]);
            playerRunner.name = "Player";
            player.SetRunner(playerRunner);
            controllers.Add(player);
            LevelManager.SetPlayer(player);

            ball = game.GetBall();
            teamAGoal = game.GetTeamAGoal();
            teamBGoal = game.GetTeamBGoal();

            totalRunners = controllers.Count;

            SetRunnersInStartPosition();

            runnersPerTeam = (int)math.ceil(totalRunners / 2.0f);
            for (int i = 0; i < runnersPerTeam; ++i)
            {
                teamAControllers.Add(controllers[i]);
                controllers[i].Runner.SetTeam(0);
            }
            for (int i = runnersPerTeam; i < totalRunners; ++i)
            {
                teamBControllers.Add(controllers[i]);
                controllers[i].Runner.SetTeam(1);
            }

            if (!skipStart)
            {
                PlayIntro();
            }
            else
            {
                LevelManager.StartLevel();
            }
        }

        private GameObject GetRunnersRoot()
        {
            if (this.runnersRoot != null)
                return this.runnersRoot;

            var levelScene = SceneLoader.GetLevelScene();
            GameObject runnersRoot = null;

            var rootObjects = levelScene.GetRootGameObjects();
            int rootCount = rootObjects != null ? rootObjects.Length : 0;
            for (int i = 0; i < rootCount; ++i)
            {
                if (rootObjects[i].name == "RunnersRoot")
                {
                    runnersRoot = rootObjects[i];
                    break;
                }
            }
            if (runnersRoot == null)
            {
                runnersRoot = new GameObject("RunnersRoot");
                SceneManager.MoveGameObjectToScene(runnersRoot, levelScene);
            }

            this.runnersRoot = runnersRoot;
            return runnersRoot;
        }

        private void RemoveRunners()
        {
            if (runnersRoot == null)
                return;

            int count = runnersRoot.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                var children = runnersRoot.transform.GetChild(i);
                Destroy(children.gameObject, 0.1f);
            }
            runnersRoot.transform.DetachChildren();

            teamAControllers.Clear();
            teamBControllers.Clear();
            controllers.Clear();

            player = null;
        }

        private void PlayIntro()
        {
            game.PlayIntro(OnIntroFinished);
        }

        private void OnIntroFinished()
        {
            LevelManager.StartLevel();
        }

        public static void StartGame()
        {
            if (!(Instance is GoalBallController instance))
                return;

            Debug.LogFormat("Goal Ball started! ({0} players)", instance.totalRunners);
            instance.gameTime = instance.game.GetTimeLimit();
            instance.teamAPoints = 0;
            instance.teamBPoints = 0;
            instance.NextRound();
        }

        private void SetControllersActive(bool active)
        {
            for (int i = 0; i < controllers.Count; ++i)
            {
                controllers[i].SetActive(active);
            }
        }

        public static void GoalAgainstTeam(int team)
        {
            if (!(Instance is GoalBallController instance)
                || !instance.isGameActive)
                return;

            int teamWhoMarked = 1 - team;
            instance.Goal(teamWhoMarked);
        }

        private void Goal(int teamWhoMarked)
        {
            if (teamWhoMarked == 0)
                teamAPoints++;
            else
                teamBPoints++;

            goalSplash.Show(teamWhoMarked, player.Runner.Team);

            isGameActive = false;
            ball.Goal();
            LevelManager.ShakeCamera(goalCameraShakePower, goalCameraShakeFrequency, goalCameraShakeDuration);

            bool goldGoal = gameTime == 0f
                && teamAPoints != teamBPoints;
            if (!goldGoal)
            {
                GameManager.SlowMotion(
                    goalSlowMotionScale,
                    goalSlowMotionDuration,
                    NextRound
                );
            }
            else
            {
                GameManager.SlowMotion(
                    goalSlowMotionScale,
                    goalSlowMotionDuration,
                    GameFinished
                );
            }
        }

        private void NextRound()
            => StartCoroutine(NextRound_Imp());

        private IEnumerator NextRound_Imp()
        {
            RoundFinished();

            screenFader.FadeIn();
            while (!screenFader.Finished)
                yield return null;

            SetControllersActive(false);
            SetRunnersInStartPosition();
            goalSplash.Hide();

            ui.Show();

            screenFader.FadeOut();
            while (!screenFader.Finished)
                yield return null;

            bool countdownFinished = false;
            LevelManager.StartCountdown(() => countdownFinished = true);
            while (!countdownFinished)
                yield return null;

            StartRound();
        }

        private void StartRound()
        {
            AudioManager.BallGameMusic(teamAPoints == 0 && teamBPoints == 0);

            isGameActive = true;
            SetControllersActive(true);
            ball.Activate();
            ball.Shoot(Vector2.up, ballStartLunchPower);
            teamAGoal.Activate(0);
            teamBGoal.Activate(1);
        }

        private void RoundFinished()
        {
            isGameActive = false;
            SetControllersActive(false);
            ball.Deactivate();
            teamAGoal.Deactivate();
            teamBGoal.Deactivate();
        }

        public static void RunnerFellInHole(Runner runner, SpawnPoint respawnPoint)
        {
            if (!(Instance is GoalBallController instance))
                return;

            instance.StartCoroutine(instance.RunnerFellInHole_Imp(runner, respawnPoint));
        }

        private IEnumerator RunnerFellInHole_Imp(Runner runner, SpawnPoint respawnPoint)
        {
            if (runner.transform.parent == null
                || !runner.transform.parent.TryGetComponent(out RunnerController controller))
                yield break;

            controller.SetActive(false);
            controller.gameObject.SetActive(false);

            bool isPlayer = player.gameObject == runner.transform.parent.gameObject;
            if (isPlayer)
            {
                screenFader.FadeIn();
                while (!screenFader.Finished)
                    yield return null;
            }
            else
            {
                yield return new WaitForSeconds(respawnTime);
            }

            controller.SetPosition(respawnPoint);

            if (isPlayer)
            {
                screenFader.FadeOut();
                while (!screenFader.Finished)
                    yield return null;
            }

            controller.gameObject.SetActive(true);
            controller.SetActive(true);
        }

        private void SetRunnersInStartPosition()
        {
            var startPositions = game.GetStartPoints();
            for (int i = 0; i < totalRunners; ++i)
            {
                controllers[i].SetPosition(startPositions[i]);
                controllers[i].SetActive(false);
            }

            var ballStartPosition = game.GetBallStartPoint();
            ball.SetPosition(ballStartPosition);
        }

        public static void RetryGame()
        {
            if (!(Instance is GoalBallController instance))
                return;

            LevelManager.LevelFinished();
            instance.isGameActive = false;
            instance.InitializeGame();
        }

        private void FixedUpdate()
        {
            if (!isGameActive)
                return;

            gameTime = math.max(gameTime - Time.fixedDeltaTime, 0f);
            if (gameTime == 0f
                && teamAPoints != teamBPoints)
            {
                GameFinished();
            }
        }

        private void GameFinished()
        {
            isGameActive = false;
            SetControllersActive(false);
            ShowGameResults();
        }

        private void ShowGameResults()
        {
            int playerTeam = player.Runner.Team;
            int winnerTeam = teamAPoints > teamBPoints
                ? 0
                : 1;
            LevelManager.GoalBallGameCompleted(winnerTeam, playerTeam);
            gameResults.Show(winnerTeam == playerTeam, LevelManager.RetryLevel, LevelManager.ExitGame);
        }

        public static bool AmIAboutToReceiveAPulse(Runner runner)
        {
            if (!(Instance is GoalBallController instance)
                || runner.GetShieldModule().IsActive)
                return false;

            bool pulseInComing = false;

            int runnerTeam = runner.Team;
            var rivalRunners = runnerTeam == 0
                ? instance.teamBControllers
                : instance.teamAControllers;

            for (int i = 0; i < rivalRunners.Count; ++i)
            {
                var collider = runner.GetCollider();
                var rivalRunner = rivalRunners[i].Runner;
                if (!rivalRunner.IsPulseActive
                    || !rivalRunner.IsPointInReachOfPulse(collider.ClosestPoint(rivalRunner.transform.position)))
                {
                    continue;
                }

                pulseInComing = true;
                break;
            }

            return pulseInComing;
        }

        public static float GetDistanceToClosestRival(Runner runner)
        {
            if (!(Instance is GoalBallController instance))
                return Mathf.Infinity;

            var rivalTeam = runner.Team == 0
                ? instance.teamBControllers
                : instance.teamAControllers;

            float closest = Mathf.Infinity;
            rivalTeam.ForEach(
                (rival) =>
                {
                    float distance = math.distancesq(
                        rival.Runner.transform.position,
                        runner.transform.position
                    );
                    closest = math.min(distance, closest);
                }
            );

            return math.sqrt(closest);
        }

        public static void ShootBall(Vector2 direction, float power)
        {
            if (!(Instance is GoalBallController instance))
                return;

            instance.ball.Shoot(direction, power);
        }

    }

}