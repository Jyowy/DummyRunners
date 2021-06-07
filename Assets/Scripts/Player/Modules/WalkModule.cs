using Common;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{

    public class WalkModule : MonoBehaviour
    {

        private static readonly float startBaseSpeed = 5f;

        [SerializeField]
        private float baseSpeed = 10f;
        [SerializeField]
        private float slopeMultiplier = 1.5f;
        [SerializeField]
        private float startTime = 0.5f;
        [SerializeField]
        private AnimationCurve startCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private TimedAction start;

        private float SpeedThreshold => baseSpeed * 0.9f;

        public void SetBaseSpeed(float baseSpeed)
            => this.baseSpeed = baseSpeed;

        public void SetStartTime(float startTime)
            => this.startTime = startTime;

        private void Start()
        {
            start.Set(startTime, 0f, startCurve);
        }

        public void Walk(float dt, Rigidbody2D rigidbody, bool facingRight, Vector2 floorDirection, float multiplier)
        {
            Vector2 direction = facingRight
                ? floorDirection
                : -floorDirection;

            float walkSpeed = baseSpeed;
            float currentSpeed =
                rigidbody.velocity.x * math.sign(direction.x);

            if (currentSpeed < 0f
                || (start.Completed && currentSpeed < SpeedThreshold))
            {
                float progress = math.clamp(currentSpeed, 0f, baseSpeed) / baseSpeed;
                start.SetCurrentTime(start.timeLimit * progress);
            }

            if (!start.Completed)
            {
                walkSpeed = math.lerp(startBaseSpeed, baseSpeed, start.GetProgress());
                start.Update(dt);
            }

            float slope = math.abs(math.clamp(direction.y, -0.6f, 0f)) / 0.6f;
            float slopeMultiplier = math.lerp(1f, this.slopeMultiplier, slope);

            rigidbody.velocity = direction * walkSpeed * multiplier * slopeMultiplier;
        }

    }

}