using UI.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{

    public class EnergyBatteryUI : Menu
    {

        [SerializeField]
        private Slider slider = null;
        [SerializeField]
        private Image sliderFill = null;

        [SerializeField]
        private Material unavailableMaterial = null;
        [SerializeField]
        private Color unavailableColor = Color.gray;
        [SerializeField]
        private Material availableMaterial = null;
        [SerializeField]
        private Color availableColor = Color.white;
        [SerializeField]
        private Material consumingMaterial = null;
        [SerializeField]
        private Color consumingColor = Color.white;

        private float minValue = 0f;
        private bool isConsuming = false;

        public void Initialize(float minValue, float maxValue)
        {
            this.minValue = minValue;
            slider.maxValue = maxValue;
        }

        public void SetConsuming(bool consuming)
        {
            isConsuming = consuming;
            if (isConsuming)
            {
                sliderFill.material = consumingMaterial;
                sliderFill.color = consumingColor;
            }
        }

        public void UpdateUI(float value)
        {
            slider.value = value;

            if (isConsuming)
                return;

            sliderFill.material = value < minValue
                ? unavailableMaterial
                : availableMaterial;
            sliderFill.color = value < minValue
                ? unavailableColor
                : availableColor;
        }

    }

}