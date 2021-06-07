using UnityEngine;

namespace Stadiums.Runners
{

    [CreateAssetMenu]
    public class RunnerData : ScriptableObject
    {

        public string runnnerName = "Runner";

        public float gravity = 5f;
        public float friction = 1f;

        public float baseSpeed = 25f;
        public float startWalkTime = 0.5f;
        public float jumpPower = 20f;
        public float onAirHorizontalSpeed = 15f;
        public float turboMultiplier = 2f;

        public float pulsePower = 5f;
        public float pulseRadius = 4f;
        public float pulseStunDuration = 2f;
        public float pulseStunReduction = 0.1f;
        public float pulseCooldown = 5f;

        public float shieldDuration = 0.5f;
        public float shieldCooldown = 4f;

    }

}