using Common;
using Player;
using Stadiums;
using UnityEngine;

namespace Game
{

    public class GameDataManager : SingletonBehaviour<GameDataManager>
    {

        private PlayerData playerData = null;

        protected override void OnInstantiated()
        {
            playerData = GameManager.GetPlayerData();
        }

        public static PlayerLevelData GetPlayerLevelData(string stadiumId, string levelId)
        {
            if (!(Instance is GameDataManager instance)
#if UNITY_EDITOR
                || GameManager.DebugMode
#endif
                )
                return null;

            var stadiumData = instance.playerData.stadiumDatas.Find((x) => x.stadiumId.Equals(stadiumId));
            return stadiumData?.levelDatas.Find((x) => x.levelId.Equals(levelId));
        }

        public static void RaceFinished(string stadiumId, string levelId, int playerPosition)
        {
            if (!(Instance is GameDataManager instance)
#if UNITY_EDITOR
                || GameManager.DebugMode
#endif
                )
                return;

            var stadiumData = instance.playerData.stadiumDatas.Find((x) => x.stadiumId.Equals(stadiumId));
            if (stadiumData == null)
            {
                Debug.LogFormat("Stadium data was null so create an instance of {0}", stadiumId);
                stadiumData = new PlayerStadiumData(stadiumId);
                instance.playerData.stadiumDatas.Add(stadiumData);
            }

            var levelData = stadiumData.levelDatas.Find((x) => x.levelId.Equals(levelId));
            if (levelData == null)
            {
                Debug.LogFormat("Level data was null so create an instance of {0}", levelId);
                levelData = new PlayerLevelData(levelId, LevelType.Race);
                stadiumData.levelDatas.Add(levelData);
            }
            if (levelData.playerPosition < 1
                || levelData.playerPosition > playerPosition)
            {
                levelData.playerPosition = playerPosition;
            }
        }

        public static void BallGameFinished(string stadiumId, string levelId, bool win)
        {
            if (!(Instance is GameDataManager instance)
#if UNITY_EDITOR
                || GameManager.DebugMode
#endif
                )
                return;

            var stadiumData = instance.playerData.stadiumDatas.Find((x) => x.stadiumId.Equals(stadiumId));
            if (stadiumData == null)
            {
                Debug.LogFormat("Stadium data was null so create an instance of {0}", stadiumId);
                stadiumData = new PlayerStadiumData(stadiumId);
                instance.playerData.stadiumDatas.Add(stadiumData);
            }

            var levelData = stadiumData.levelDatas.Find((x) => x.levelId.Equals(levelId));
            if (levelData == null)
            {
                Debug.LogFormat("Level data was null so create an instance of {0}", levelId);
                levelData = new PlayerLevelData(levelId, LevelType.GoalBallGame);
                stadiumData.levelDatas.Add(levelData);
            }

            if (!levelData.goalBallWon
                || win)
            {
                levelData.goalBallWon = win;
            }
        }

    }

}