using Common;
using Player;
using TMPro;
using UnityEngine;
using World.TimeAttacks;

namespace UI.Menus
{

    public class TimeAttackUI : MonoBehaviour
    {

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private TextMeshProUGUI bestTimeText = null;
        [SerializeField]
        private TextMeshProUGUI currentTimeText = null;

        [SerializeField]
        private TextMeshProUGUI goldTimeText = null;
        [SerializeField]
        private TextMeshProUGUI silverTimeText = null;
        [SerializeField]
        private TextMeshProUGUI bronzeTimeText = null;

        private void Awake()
        {
            Hide();
        }

        public void Show(TimeAttackData data)
        {
            enabled = true;
            canvas.enabled = true;

            //var timeRecord = PlayerManager.GetPlayerInstance().GetDataManager().GetTimeRecord();
            //bool firstTime = !timeRecord.HasRecord(data.id);
            //float bestTime = timeRecord.GetTime(data.id);

            //bestTimeText.text = !firstTime
            //    ? GameUtils.GetTimeFormatted(bestTime)
            //    : "-";

            goldTimeText.text = GameUtils.GetTimeFormatted(data.goldTime);
            silverTimeText.text = GameUtils.GetTimeFormatted(data.silverTime);
            bronzeTimeText.text = GameUtils.GetTimeFormatted(data.bronzeTime);

            currentTimeText.text = GameUtils.GetTimeFormatted(0f);
        }

        public void Hide()
        {
            enabled = false;
            canvas.enabled = false;
        }

        private void Update()
        {
            float currentTime = TimeAttackPlayer.GetCurrentTime();

            currentTimeText.text = GameUtils.GetTimeFormatted(currentTime);
        }

    }

}