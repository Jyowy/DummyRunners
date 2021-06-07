using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{

    public class EnergyBatterySlotUI : MonoBehaviour
    {

        [SerializeField]
        private Slider chargingSlider = null;
        [SerializeField]
        private Image fullSprite = null;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetEmpty()
        {
            chargingSlider.gameObject.SetActive(false);
            fullSprite.enabled = false;
        }

        public void SetCharging()
        {
            chargingSlider.gameObject.SetActive(true);
            fullSprite.enabled = false;

            chargingSlider.value = 0f;
        }

        public void SetFull()
        {
            chargingSlider.gameObject.SetActive(false);
            fullSprite.enabled = true;
        }

        public void UpdateCharging(float progress)
        {
            chargingSlider.value = progress;
        }

    }

}