using Common;
using Player;
using SaveSystem;
using Stadiums.Races;
using System.Collections.Generic;
using UI.Menus;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game
{

    public class GameManager : SingletonBehaviour<GameManager>
    {

        [SerializeField]
        private InputActionAsset gameInput = null;

        [SerializeField]
        private SaveFilePreset newSaveFileTemplate = null;

        [SerializeField]
        private YesNoPopup yesNoPoupu = null;

        [SerializeField]
        private LayerMask scenaryLayerMask = 0;
        [SerializeField]
        private LayerMask runnersLayer = 0;

        [SerializeField]
        private bool playerInputEnabledDebug = false;

#if UNITY_EDITOR
        [SerializeField]
        private bool debugMode = false;

        private static bool debugModeSet = false;

        public static bool DebugMode { get; private set; } = false;
#endif

        private InputActionMap menuInputMap = null;
        private InputActionMap playerInputMap = null;

        private SaveFile currentSaveFile = null;

        public static PlayerData GetPlayerData()
        {
            if (!(Instance is GameManager instance)
                || instance.currentSaveFile == null
                || !instance.currentSaveFile.IsValid)
                return null;

            return instance.currentSaveFile.playerData;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLoaded()
        {
            debugModeSet = false;
            DebugMode = false;
        }

        private void RemoveDebugObjects(Scene a, Scene b)
        {
            if (!DebugMode)
            {
                GameObject debug = GameObject.Find("Debug");
                if (debug != null)
                {
                    GameObject.Destroy(debug);
                }
            }
        }
#endif

        protected override void OnInstantiated()
        {
#if UNITY_EDITOR
            if (!debugModeSet)
            {
                debugModeSet = true;
                DebugMode = debugMode;

                SceneManager.activeSceneChanged += RemoveDebugObjects;
            }
#endif

            menuInputMap = gameInput.FindActionMap("Menu");
            playerInputMap = gameInput.FindActionMap("Player");

            menuInputMap.Disable();
            playerInputMap.Enable();

            SaveFilesManager.Initialize();
        }

        public static void GoToTitleMenu()
        {
            GameManager.SaveFile();
            GameManager.UnloadSaveFile();
            SceneLoader.GoToTitleMenu();
        }

        public static void StartGame()
        {
            SceneLoader.StartGame();
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void ShowYesNoPopup(string message, string yes, string no, System.Action onYes, System.Action onNo)
        {
            if (!(Instance is GameManager instance))
                return;

            instance.yesNoPoupu.Show(message, yes, no, onYes, onNo);
        }

        public static LayerMask GetScenaryLayerMark()
        {
            if (!(Instance is GameManager instance))
            {
                return 0;
            }

            return instance.scenaryLayerMask;
        }

        public static LayerMask GetRunnersLayerMark()
        {
            if (!(Instance is GameManager instance))
            {
                return 0;
            }

            return instance.runnersLayer;
        }

        public static SaveFilePreset GetSaveFileTemplate()
        {
            if (!(Instance is GameManager instance))
            {
                return null;
            }

            return instance.newSaveFileTemplate;
        }

        public static List<SaveFile> GetSaveFiles() => SaveFilesManager.GetSaveFiles();

        public static void LoadSaveFile(int slotIndex)
        {
            if (!(Instance is GameManager instance))
                return;

            instance.currentSaveFile = SaveFilesManager.GetSaveFile(slotIndex);
            StartGame();
        }

        public static void UnloadSaveFile()
        {
            if (!(Instance is GameManager instance))
                return;

            instance.currentSaveFile = null;
        }

        public static void SaveFile()
        {
            if (!(Instance is GameManager instance)
                || instance.currentSaveFile == null
                || !instance.currentSaveFile.IsValid)
                return;

            SaveFilesManager.SaveFile(instance.currentSaveFile);
        }

        private readonly static List<GameObject> menuInputResponsibles = new List<GameObject>();

        public static GameObject GetFocusedMenu()
        {
            if (menuInputResponsibles.Count == 0)
                return null;

            return menuInputResponsibles[menuInputResponsibles.Count - 1];
        }

        public static void SetMenuInputEnable(GameObject responsible, bool enable)
        {
            if (!(Instance is GameManager instance))
                return;

            if (!enable)
            {
                menuInputResponsibles.Remove(responsible);
                if (menuInputResponsibles.Count == 0)
                {
                    //Debug.LogWarning("PlayerInput enabled");
                    instance.menuInputMap.Disable();
                    instance.playerInputMap.Enable();
                }
            }
            else if (!menuInputResponsibles.Contains(responsible))
            {
                if (menuInputResponsibles.Count == 0)
                {
                    //Debug.LogWarning("PlayerInput disabled");
                    instance.playerInputMap.Disable();
                    instance.menuInputMap.Enable();
                }

                menuInputResponsibles.Add(responsible);
            }
        }

        public static void SetPlayerInputEnable(bool enable)
        {
            if (!(Instance is GameManager instance))
                return;

            if (!enable)
            {
                instance.playerInputMap.Disable();
            }
            else if (menuInputResponsibles.Count == 0)
            {
                instance.playerInputMap.Enable();
            }
            else
            {
                Debug.LogErrorFormat("Someone tried to enable player's input but could't");
                Debug.LogFormat("Active menus: {0}", menuInputResponsibles.Count);
                menuInputResponsibles.ForEach((x) => Debug.LogFormat("Menu {0}", x.name));
            }
        }

        public static InputActionMap GetMenuActionMap() =>
            Instance != null
                ? (Instance as GameManager).menuInputMap
                : null;

        public static InputActionMap GetPlayerActionMap() =>
            Instance != null
                ? (Instance as GameManager).playerInputMap
                : null;

        public static void StopGameTime() =>
            Time.timeScale = 0f;

        public static void ResumeGameTime() =>
            Time.timeScale = 1f;

        public static void SlowMotion(float timeScale, float duration, System.Action onFinished = null)
        {
            if (!(Instance is GameManager instance)
                || !instance.slowMotion.Completed)
                return;

            Time.timeScale = timeScale;
            instance.originalFixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime *= timeScale;
            instance.slowMotion.Set(duration);
            instance.onSlowMotionFinished = onFinished;
        }

        public static void FinishSlowMotion()
        {
            if (!(Instance is GameManager instance))
                return;

            Time.timeScale = 1f;
            Time.fixedDeltaTime = instance.originalFixedDeltaTime;

            var onSlowMotionFinished = instance.onSlowMotionFinished;
            instance.onSlowMotionFinished = null;
            onSlowMotionFinished?.Invoke();
        }

        private TimedAction slowMotion;
        private System.Action onSlowMotionFinished = null;
        private float originalFixedDeltaTime = 0.2f;

        private void FixedUpdate()
        {
            playerInputEnabledDebug = playerInputMap.enabled;

            if (!slowMotion.Completed)
            {
                slowMotion.Update(Time.fixedUnscaledDeltaTime);
                if (slowMotion.Completed)
                {
                    FinishSlowMotion();
                }
            }
        }

        public static void StartRace(Race race)
        {
            RaceController.InitializeRace(race);
        }

        public static void RaceEnded()
        {
            QuitGame();
        }

    }

}