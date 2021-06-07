using Common;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{

    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {

        [SerializeField]
        private string titleSceneName = null;
        [SerializeField]
        private string gamePersistentSceneName = null;
        [SerializeField]
        private string gameSceneName = null;
        [SerializeField]
        private CanvasFader screenFader = null;

#if UNITY_EDITOR
        private int titleMenuLevel => GameManager.DebugMode
            ? 0
            : 1;
        private int persistentGameLevel => GameManager.DebugMode
            ? 0
            : 1;
        private int stadiumMenuLevel => GameManager.DebugMode
            ? 0
            : 2;
        private int gameLevelLevel => GameManager.DebugMode
            ? 0
            : 2;
#else
        private readonly int titleMenuLevel = 1;
        private readonly int persistentGameLevel = 1;
        private readonly int stadiumMenuLevel = 2;
        private readonly int gameLevelLevel = 2;
#endif

        private bool busy = false;

        public static void GoToTitleMenu()
        {
            if (!(Instance is SceneLoader instance)
                || instance.busy)
                return;

            instance.StartCoroutine(instance.LoadScene(instance.titleMenuLevel, instance.titleSceneName));
        }

        public static void StartGame()
        {
            if (!(Instance is SceneLoader instance)
                || instance.busy)
                return;

#if UNITY_EDITOR
            if (GameManager.DebugMode)
                LoadLevel(instance.gameSceneName);
            else
#endif
            instance.StartCoroutine(instance.LoadScene(instance.persistentGameLevel, instance.gamePersistentSceneName, instance.gameSceneName));
        }

        public static void LoadLevel(string sceneName)
        {
            if (!(Instance is SceneLoader instance)
                || instance.busy)
                return;

            instance.StartCoroutine(instance.LoadScene(instance.gameLevelLevel, sceneName));
        }

        public static void UnloadLevel()
        {
            if (!(Instance is SceneLoader instance)
                || instance.busy)
                return;

            instance.StartCoroutine(instance.LoadScene(instance.stadiumMenuLevel, instance.gameSceneName));
        }

        public static Scene GetLevelScene()
        {
            if (!(Instance is SceneLoader instance)
                || SceneManager.sceneCount < instance.gameLevelLevel)
                throw new System.Exception("No level scene loaded");

            return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }

        private IEnumerator LoadScene(int level, params string []sceneNames)
        {
            if (busy
                || sceneNames == null
                || sceneNames.Length == 0)
                yield break;

            busy = true;

            screenFader.FadeIn();
            while (!screenFader.Finished)
                yield return null;

#if UNITY_EDITOR
            if (GameManager.DebugMode)
            {
                yield return SceneManager.LoadSceneAsync(sceneNames[0], LoadSceneMode.Single);

                screenFader.FadeOut();
                while (!screenFader.Finished)
                    yield return null;

                busy = false;

                yield break;
            }
#endif

            while (SceneManager.sceneCount > level)
            {
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
            }

            for (int i = 0; i < sceneNames.Length; ++i)
            {
                string sceneName = sceneNames[i];
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

            screenFader.FadeOut();
            while (!screenFader.Finished)
                yield return null;

            busy = false;
        }

    }

}
