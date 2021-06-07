using Stadiums;
using System.Collections.Generic;

namespace Player
{

    [System.Serializable]
    public class PlayerLevelData
    {
        public string levelId = "";
        public LevelType levelType = LevelType.Race;

        public int playerPosition = -1;
        public bool goalBallWon = false;

        public PlayerLevelData(string levelId, LevelType levelType)
        {
            this.levelId = levelId;
            this.levelType = levelType;
        }

        public PlayerLevelData(PlayerLevelData other)
        {
            levelId = other.levelId;
            levelType = other.levelType;

            playerPosition = other.playerPosition;
            goalBallWon = other.goalBallWon;
        }
    }

    [System.Serializable]
    public class PlayerStadiumData
    {
        public string stadiumId = "";
        public List<PlayerLevelData> levelDatas = new List<PlayerLevelData>();

        public PlayerStadiumData(string stadiumId)
        {
            this.stadiumId = stadiumId;
        }

        public PlayerStadiumData(PlayerStadiumData other)
        {
            stadiumId = other.stadiumId;
            other.levelDatas.ForEach((x) => levelDatas.Add(new PlayerLevelData(x)));
        }
    }

    [System.Serializable]
    public class PlayerData
    {

        public string playerName = "Jade";
        public List<PlayerStadiumData> stadiumDatas = new List<PlayerStadiumData>();

        public PlayerData() { }

        public PlayerData(PlayerDataPreset preset)
        {
            playerName = preset.playerName;
            preset.stadiumDatas.ForEach((x) => stadiumDatas.Add(new PlayerStadiumData(x)));
        }

        public PlayerData(PlayerData other)
        {
            playerName = other.playerName;
            other.stadiumDatas.ForEach((x) => stadiumDatas.Add(new PlayerStadiumData(x)));
        }

        public void Check()
        {
            if (playerName == null)
                playerName = "Jade";
            if (stadiumDatas == null)
                stadiumDatas = new List<PlayerStadiumData>();
        }

    }

}