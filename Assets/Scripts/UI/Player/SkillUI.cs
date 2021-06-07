using UI.Menus;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{

    public class SkillUI : Menu
    {

        [SerializeField]
        private Slider slider = null;
        [SerializeField]
        private Image frame = null;
        [SerializeField]
        private Image fill = null;
        [SerializeField]
        private Color availableColor = Color.red;
        [SerializeField]
        private Color unavailableColor = Color.gray;

        private void Awake()
        {
            slider.value = 0f;
        }

        public void UpdateCooldownProgress(float progress)
        {
            progress = math.clamp(progress, 0f, 1f);
            slider.value = progress;

            fill.color = progress < 1f
                ? unavailableColor
                : availableColor;
            Color frameColor = fill.color * 0.25f;
            frameColor.a = 1f;
            frame.color = frameColor;
        }

    }

}
