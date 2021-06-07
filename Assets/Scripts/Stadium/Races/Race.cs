using Game;
using UnityEngine;

namespace Stadiums.Races
{

    public class Race : Level
    {

        [SerializeField]
        private RaceEndArea endArea = null;
        public RaceEndArea GetEndArea() => endArea;

        public void Start()
        {
#if UNITY_EDITOR
            if (GameManager.DebugMode)
            {
                LevelManager.LoadLevel("", "", LevelType.Race);
            }
#endif
            LevelManager.StartRace(this);
        }

    }

}