using Common;
using Stadiums.Races;
using TMPro;
using UnityEngine;

namespace UI.Menus
{

    public class RaceUI : MonoBehaviour
    {

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private TextMeshProUGUI currentTimeText = null;

        [SerializeField]
        private TextMeshProUGUI currentPositionText = null;

        private void Awake()
        {
            Hide();
        }

        public void Show()
        {
            enabled = true;
            canvas.enabled = true;

            currentTimeText.text = GameUtils.GetTimeFormatted(0f);
        }

        public void Hide()
        {
            enabled = false;
            canvas.enabled = false;
        }

        private void Update()
        {
            float currentTime = RaceController.GetCurrentTime();
            currentTimeText.text = GameUtils.GetTimeFormatted(currentTime);

            int playerPosition = RaceController.GetPlayerPosition();
            currentPositionText.text = GameUtils.GetPositionFormatted(playerPosition + 1);
        }

    }

}