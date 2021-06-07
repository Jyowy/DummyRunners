using Player.Skills;
using UnityEngine;

namespace Player
{

    public class TurboModule : MonoBehaviour
    {

        [SerializeField]
        private float speedMultiplier = 2f;

        [SerializeField]
        private EnergyBattery battery = null;

        public bool IsActive { get; private set; } = false;

        public void SetSpeedSpeed(float multiplier)
            => speedMultiplier = multiplier;

        public EnergyBattery GetEnergyBattery()
            => battery;

        public void Restart()
        {
            battery.Restart();
        }

        private void Start()
        {
            battery.Initialize(OnDeactivate, null);
        }

        public bool Activate()
        {
            if (IsActive
                || !battery.CanActivate())
                return false;
            IsActive = true;
            battery.StartConsuming();
            return true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            battery.StopConsuming();
            OnDeactivate();
        }

        private void OnDeactivate()
        {
            IsActive = false;
        }

        public float GetSpeedTurboMultiplier()
        {
            return !IsActive
                ? 1f
                : speedMultiplier;
        }

    }

}