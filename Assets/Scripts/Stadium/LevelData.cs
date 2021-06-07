using UnityEngine;

namespace Stadiums
{

    public enum LevelType
    {
        Race,
        GoalBallGame
    }

    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {

        public LevelType type = LevelType.Race;

        public string levelId = "";
        public string levelName = "";
        public string levelDescription = "";
        public string levelSceneName = "";

    }

}