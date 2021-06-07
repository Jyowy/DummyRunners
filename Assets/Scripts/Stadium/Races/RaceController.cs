using Common;
using Game;
using Player;
using Stadiums.Runners;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Menus;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stadiums.Races
{

    public class RaceController : SingletonBehaviour<RaceController>
    {

        [SerializeField]
        private float respawnTime = 0.5f;

        // Debug
        [SerializeField]
        private Race debugRace = null;
        [SerializeField]
        private Player.Player playerPrefab = null;
        [SerializeField]
        private Runner runnerPrefab = null;
        [SerializeField]
        private RaceUI ui = null;
        [SerializeField]
        private CanvasFader screenFader = null;
        [SerializeField]
        private List<RunnerData> runnerDatasDebug = null;

        [SerializeField]
        private RaceResults raceResults = null;

        [SerializeField]
        private bool skipStart = true;

        private Race race = null;

        private bool isRaceActive = false;
        private float raceTime = 0f;

        private Player.Player player = null;

        private int totalRunners = 0;
        private readonly List<RunnerController> remainingControllers = new List<RunnerController>();
        private readonly List<Runner> remainingRunners = new List<Runner>();
        private readonly List<Runner> runnersOrder = new List<Runner>();
        private readonly List<float> goalTimes = new List<float>();

        private GameObject runnersRoot = null;

        public static List<Runner> GetRunnersOrder()
        {
            if (!(Instance is RaceController instance))
                return null;

            return instance.runnersOrder;
        }

        public static List<float> GetGoalTimes()
        {
            if (!(Instance is RaceController instance))
                return null;

            return instance.goalTimes;
        }


        private void Start()
        {
            // Debug
            if (debugRace != null)
            {
                InitializeRace(debugRace);
            }
        }

        public static void InitializeRace(Race race)
        {
            if (!(Instance is RaceController instance))
                return;

            Debug.LogFormat("InitializeRace {0}", race.name);
            instance.race = race;
            instance.InitializeRace();
        }

        public static void ExitRace()
        {
            if (!(Instance is RaceController instance))
                return;

            instance.isRaceActive = false;
            instance.ui.Hide();
            instance.raceResults.Hide();
            instance.RemoveRunners();
            instance.runnersRoot = null;
        }

        public static float GetCurrentTime()
        {
            if (!(Instance is RaceController instance))
                return 0f;

            return instance.raceTime;
        }

        private void InitializeRace()
        {
            GameObject runnersRoot = GetRunnersRoot();
            RemoveRunners();

            var startPoints = race.GetStartPoints();

            int raceSlots = startPoints.Count;

            for (int i = 0; i < raceSlots - 1; ++i)
            {
                var position = startPoints[i];
                GameObject newRunner = new GameObject();
                RaceRunnerAI runnerAI = newRunner.AddComponent<RaceRunnerAI>();
                newRunner.transform.parent = runnersRoot.transform;

                var runner = GameObject.Instantiate(runnerPrefab, runnerAI.transform);
                runner.SetData(runnerDatasDebug[i]);
                runner.name = string.Format("Runner_{0}", i);
                runner.SetTeam(i);

                runnerAI.SetRunner(runner);
                runnerAI.SetPosition(position);
                runnerAI.SetActive(false);
                remainingRunners.Add(runner);
                remainingControllers.Add(runnerAI);

                runnersOrder.Add(runner);
            }

            int lastPosition = raceSlots - 1;
            var lastStartPosition = startPoints[raceSlots - 1];
            player = GameObject.Instantiate(playerPrefab, runnersRoot.transform);

            var playerRunner = GameObject.Instantiate(runnerPrefab, player.transform);
            playerRunner.SetData(runnerDatasDebug[raceSlots - 1]);
            var playerData = GameManager.GetPlayerData();
            playerRunner.name = playerData != null ? playerData.playerName : "Player";
            playerRunner.SetTeam(lastPosition);

            player.SetRunner(playerRunner);
            player.SetPosition(lastStartPosition);
            remainingRunners.Add(playerRunner);
            remainingControllers.Add(player);
            player.SetActive(false);
            LevelManager.SetPlayer(player);

            runnersOrder.Add(playerRunner);

            totalRunners = remainingRunners.Count;

            if (!skipStart)
            {
                PlayIntro();
            }
            else
            {
                OnIntroFinished();
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

            remainingRunners.Clear();
            remainingControllers.Clear();
            runnersOrder.Clear();
            goalTimes.Clear();

            player = null;
        }

        private void PlayIntro()
        {
            race.PlayIntro(OnIntroFinished);
        }

        private void OnIntroFinished()
        {
            if (screenFader == null)
                return;

            screenFader.FadeInOut(() => LevelManager.StartCountdown(LevelManager.StartLevel));
        }

        public static void StartRace()
        {
            if (!(Instance is RaceController instance))
                return;

            Debug.LogFormat("Race started! ({0} runners)", instance.totalRunners);

            for (int i = 0; i < instance.remainingControllers.Count; ++i)
            {
                instance.remainingControllers[i].SetActive(true);
            }

            instance.isRaceActive = true;
            instance.raceTime = 0f;

            instance.ui.Show();

            GameManager.SetPlayerInputEnable(true);
            AudioManager.RaceMusic();

            var endArea = instance.race.GetEndArea();
            endArea.runnerReachedEnd += instance.RunnerReachedEnd;
        }

        public static void RunnerFellInHole(Runner runner, SpawnPoint respawnPoint)
        {
            if (!(Instance is RaceController instance))
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

        public static void RetryRace()
        {
            if (!(Instance is RaceController instance))
                return;

            LevelManager.LevelFinished();
            instance.isRaceActive = false;
            instance.InitializeRace();
        }

        private void RunnerReachedEnd(Runner runner)
        {
            if (!remainingRunners.Contains(runner))
                return;

            goalTimes.Add(raceTime);

            if (player.Runner == runner)
            {
                PlayerJustReachedGoal();
            }

            int index = remainingRunners.IndexOf(runner);
            remainingRunners.RemoveAt(index);
            //remainingControllers[index].SetActive(false);
            remainingControllers.RemoveAt(index);
            Debug.LogFormat("Runner {0} reached the goal at time {1:N1}s",
                runner.name, raceTime);

            if (remainingRunners.Count == 0)
            {
                isRaceActive = false;
            }
        }

        private void PlayerJustReachedGoal()
        {
            int playerPosition = goalTimes.Count;
            LevelManager.RaceCompleted(playerPosition);
            raceResults.Show(playerPosition == 1, RetryRace, LevelManager.ExitGame);
        }

        private void FixedUpdate()
        {
            if (!isRaceActive)
                return;

            raceTime += Time.fixedDeltaTime;
            CalculateRacePositions();
        }

        private readonly List<KeyValuePair<Runner, float>> runnersOrderAux = new List<KeyValuePair<Runner, float>>();

        private void CalculateRacePositions()
        {
            int remainingCount = remainingControllers.Count;
            var endArea = race.GetEndArea();
            for (int i = 0; i < remainingCount; ++i)
            {
                var runner = remainingRunners[i];
                float distance = math.distancesq(runner.transform.position, endArea.transform.position);
                runnersOrderAux.Add(new KeyValuePair<Runner, float>(runner, distance));
            }

            runnersOrderAux.Sort((a, b) => a.Value.CompareTo(b.Value));

            int racePositionOffset = totalRunners - remainingCount;
            for (int i = 0; i < remainingCount; ++i)
            {
                runnersOrder[racePositionOffset + i] = runnersOrderAux[i].Key;
            }

            runnersOrderAux.Clear();
        }

        public static int GetPlayerPosition()
        {
            if (!(Instance is RaceController instance))
                return -1;

            return GetRunnerPosition(instance.player.Runner);
        }

        public static int GetRunnerPosition(Runner runner)
        {
            if (!(Instance is RaceController instance))
                return -1;

            return instance.runnersOrder.IndexOf(runner);
        }

        public static bool IsInFirstPosition(Runner runner)
        {
            if (!(Instance is RaceController instance))
                return false;
            int position = GetRunnerPosition(runner);
            if (position < 0)
                return false;

            int firstPosition = instance.totalRunners - instance.remainingRunners.Count;
            return position == firstPosition; 
        }

        public static float GetDistanceToNextRunnerAtTheRear(Runner runner)
        {
            if (!(Instance is RaceController instance))
                return Mathf.Infinity;

            int position = GetRunnerPosition(runner);
            int lastPosition = instance.totalRunners - 1;
            if (position < 0
                || position == lastPosition)
                return Mathf.Infinity;

            return math.distance(
                instance.runnersOrder[position].transform.position,
                instance.runnersOrder[position + 1].transform.position
            );
        }

        public static bool AmIAboutToReceiveAPulse(Runner runner)
        {
            if (!(Instance is RaceController instance)
                || runner.GetShieldModule().IsActive)
                return false;

            bool pulseInComing = false;

            int position = GetRunnerPosition(runner);
            int firstPosition = instance.totalRunners - instance.remainingRunners.Count;

            for (int i = position - 1; i >= firstPosition; --i)
            {
                var collider = runner.GetCollider();
                var nextRunner = instance.runnersOrder[i];

                if (!nextRunner.IsPulseActive
                    || !nextRunner.IsPointInReachOfPulse(collider.ClosestPoint(nextRunner.transform.position)))
                {
                    continue;
                }

                pulseInComing = true;
                break;
            }

            return pulseInComing;
        }

    }

}