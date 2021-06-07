using Common;
using TMPro;
using UnityEngine;

namespace Stadiums.Races
{

    public class RaceResultSlotUI : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI runnerPosition = null;
        [SerializeField]
        private TextMeshProUGUI runnerName = null;
        [SerializeField]
        private TextMeshProUGUI time = null;

        [SerializeField]
        private GameObject playerFrame = null;

        public void Show(int position, string runnerName, float time, bool player)
        {
            gameObject.SetActive(true);

            runnerPosition.text = GameUtils.GetPositionFormatted(position);
            this.runnerName.text = runnerName;
            this.time.text = GameUtils.GetTimeFormatted(time);

            playerFrame.SetActive(player);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            runnerPosition.text = "";
            runnerName.text = "";
            time.text = "";

            playerFrame.SetActive(false);
        }

    }

}