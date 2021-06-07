using Common;
using UnityEngine;

namespace Stadiums.Runners
{

    public class ShieldModule : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem effect = null;

        [SerializeField]
        private float duration = 1f;
        [SerializeField]
        private float cooldownDuration = 5f;

        public bool IsActive => !shield.Completed;

        private TimedAction shield;
        private TimedAction cooldown;

        public float GetCooldownProgress()
            => cooldown.GetProgress();

        public void SetDuration(float duration)
            => this.duration = duration;

        public void SetCooldownDuration(float duration)
            => cooldownDuration = duration;

        public bool CanActivate() => cooldown.Completed;

        private void Awake()
        {
            shield.Set(duration);
            shield.Finish();
            cooldown.Set(cooldownDuration);
        }

        public void Activate()
        {
            if (!CanActivate())
                return;

            effect.Play();

            shield.Restart();
            cooldown.Restart();
        }

        public void Restart()
        {
            shield.Finish();
            cooldown.Restart();
            effect.Stop();
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            if (IsActive)
            {
                shield.Update(dt);
                if (shield.Completed)
                {
                    effect.Stop();
                }
            }
            else if (!cooldown.Completed)
            {
                cooldown.Update(dt);
            }
        }

    }

}