using Common;
using Stadiums.ExtraFases;
using Unity.Mathematics;
using UnityEngine;

namespace Stadiums.Runners
{

    public class PulseModule : MonoBehaviour
    {

        [SerializeField]
        private ParticleSystem effect = null;

        [SerializeField]
        private float power = 5f;
        [SerializeField]
        private float duration = 0.5f;
        [SerializeField]
        private float cooldownDuration = 5f;
        [SerializeField]
        private float effectRadius = 5f;
        [SerializeField]
        private float stunDuration = 0.75f;

        private Runner owner = null;
        private TimedAction pulse;
        private TimedAction cooldown;

        private LayerMask runnersLayerMask = 0;

        public bool IsActive => !pulse.Completed;

        public float GetCooldownProgress()
            => cooldown.GetProgress();

        public void SetPower(float power)
            => this.power = power;

        public void SetRadius(float radius)
            => effectRadius = radius;

        public void SetStunDuration(float duration)
            => stunDuration = duration;

        public void SetCooldownDuration(float duration)
            => cooldownDuration = duration;

        public bool CanActivate() => cooldown.Completed;

        private void Awake()
        {
            owner = transform.parent.GetComponent<Runner>();
            if (owner == null)
            {
                Debug.LogErrorFormat("PulseModule {0}'s parent {1} does not contain a Runner component",
                    name, transform.parent != null ? transform.parent.name : "<null parent>");
            }

            pulse.Set(duration);
            pulse.Finish();
            cooldown.Set(cooldownDuration);
        }

        public void Activate(LayerMask runnersLayerMask)
        {
            if (!CanActivate())
                return;

            this.runnersLayerMask = runnersLayerMask;

            float speed = effectRadius / duration;
            var velocity = effect.velocityOverLifetime;
            velocity.radial = speed;
            var main = effect.main;
            main.startLifetime = duration;
            effect.Play();

            PerformPulse();

            pulse.Restart();
            cooldown.Restart();
        }

        public void Restart()
        {
            pulse.Finish();
            cooldown.Restart();
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            if (IsActive)
            {
                PerformPulse();
                pulse.Update(dt);
            }
            else if (!cooldown.Completed)
            {
                cooldown.Update(dt);
            }
        }

        private void PerformPulse()
        {
            float progress = pulse.GetProgress();
            float currentRadius = math.lerp(0f, effectRadius, progress);

            Vector2 position = transform.position;
            var colliders = PhysicsUtils.GetAllCollidersInRadius(position, currentRadius, runnersLayerMask);
            if (colliders == null)
                return;

            bool pulseRepelled = false;
            for (int i = 0; i < colliders.Length; ++i)
            {
                var parent = colliders[i].transform.parent;
                if (parent == null
                    || parent.gameObject == owner.gameObject
                    || !parent.TryGetComponent(out Runner runner)
                    || runner.Team == owner.Team)
                    continue;

                pulseRepelled |= !runner.ReceivePulse(position, power, stunDuration);
            }

            if (pulseRepelled)
            {
                owner.Stun(stunDuration);
                CancelPulse();
            }

            if (LevelManager.IsABallGame()
                && !owner.HasTheBall()
                && !GoalBallController.IsBallInMyTeam(owner.Team))
            {
                float distance = math.distance(position, GoalBallController.GetBallCollider().ClosestPoint(position));
                if (distance <= currentRadius)
                {
                    var ballPosition = GoalBallController.GetBallPosition();
                    Vector2 direction = (ballPosition - position).normalized;
                    GoalBallController.ShootBall(direction, power);
                }
            }
        }

        private void CancelPulse()
        {
            pulse.Finish();
            effect.Stop();
            effect.SetParticles(null);
        }

    }

}