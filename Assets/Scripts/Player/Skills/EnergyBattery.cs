using Unity.Mathematics;
using UnityEngine;

namespace Player.Skills
{

    public class EnergyBattery : MonoBehaviour
    {

        public System.Action batteryAvailable = null;
        public System.Action batteryEmpty = null;
        public System.Action<bool> onConsuming = null;

        [SerializeField]
        private float rechargingPerSecond = 5f;
        [SerializeField]
        private float consumingPerSecond = 25f;

        private static readonly float maxValue = 100f;

        private float currentValue = 0f;
        private float MinValue => maxValue * 0.1f;

        public float UpdateUI()
            => currentValue;

        public float GetMaxValue() => maxValue;
        public float GetMinValue() => MinValue;
        public float GetCurrentValue() => currentValue;

        public void Restart()
        {
            currentValue = 0f;
        }

        public void Initialize(System.Action batteryEmpty, System.Action batteryAvailable)
        {
            this.batteryEmpty = batteryEmpty;
            this.batteryAvailable = batteryAvailable;
        }

        private void Start()
        {
            Restart();
        }

        public bool CanActivate() => currentValue > MinValue;
        public bool IsFull => currentValue == maxValue;
        public bool IsActive { get; private set; } = false;

        public void FullCharge()
        {
            currentValue = maxValue;
            UpdateUI();
        }

        public bool StartConsuming()
        {
            if (IsActive
                || currentValue < MinValue)
                return false;

            IsActive = true;
            onConsuming?.Invoke(true);
            return true;
        }

        public void StopConsuming()
        {
            if (!IsActive)
                return;

            IsActive = false;
            onConsuming?.Invoke(false);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (IsActive)
            {
                currentValue = math.max(currentValue - consumingPerSecond * dt, 0f);
                if (currentValue == 0f)
                {
                    OnRunOutOfEnergy();
                }

                UpdateUI();
            }
            else if (!IsFull)
            {
                float value = currentValue;

                currentValue = math.min(currentValue + rechargingPerSecond * dt, maxValue);
                if (value < MinValue
                    && currentValue >= MinValue)
                {
                    OnBatteryAvailable();
                }

                UpdateUI();
            }
        }

        private void OnRunOutOfEnergy()
        {
            StopConsuming();
            batteryEmpty?.Invoke();
        }

        private void OnBatteryAvailable()
        {
            batteryAvailable?.Invoke();
        }

    }

}