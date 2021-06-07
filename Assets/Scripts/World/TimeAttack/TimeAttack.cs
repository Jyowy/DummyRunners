using System.Collections.Generic;
using UnityEngine;
using World.Level;

namespace World.TimeAttacks
{

    public class TimeAttack : MonoBehaviour
    {

        [SerializeField]
        private TimeAttackData data = null;
        [SerializeField]
        private Route route = null;
        [SerializeField]
        private SpawnPoint startPoint = null;
        [SerializeField]
        private AreaLimits limits = null;

        public TimeAttackData GetData() => data;
        public List<Vector2> GetRoutePoints() => route.points;
        public SpawnPoint GetSpawnPoint() => startPoint;

        public AreaLimits GetLimits() => limits;

    }

}