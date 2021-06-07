using Common;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.TimeAttacks;

namespace UI.Menus
{

    public class TimeAttackMenu : Menu
    {

        [SerializeField]
        private TextMeshProUGUI timeAttackName = null;
        [SerializeField]
        private TextMeshProUGUI timeAttackDescription = null;

        [SerializeField]
        private TextMeshProUGUI bestTime = null;

        [SerializeField]
        private GameObject newRecord = null;

        [SerializeField]
        private RewardSlotUI rewardSlotTemplate = null;
        [SerializeField]
        private Transform goldRewardSlot = null;
        [SerializeField]
        private Transform silverRewardSlot = null;
        [SerializeField]
        private Transform bronzeRewardSlot = null;
        [SerializeField]
        private Transform finishRewardSlot = null;

        [SerializeField]
        private TextMeshProUGUI goldTime = null;
        [SerializeField]
        private TextMeshProUGUI silverTime = null;
        [SerializeField]
        private TextMeshProUGUI bronzeTime = null;

        [SerializeField]
        private Button startButton = null;
        [SerializeField]
        private Button cancelButton = null;

        private RewardSlotUI goldReward = null;
        private RewardSlotUI silverReward = null;
        private RewardSlotUI bronzeReward = null;
        private RewardSlotUI finishReward = null;

        private System.Action onStart = null;
        private System.Action onCancel = null;

        private void Awake()
        {
            goldReward = Instantiate(rewardSlotTemplate, goldRewardSlot);
            silverReward = Instantiate(rewardSlotTemplate, silverRewardSlot);
            bronzeReward = Instantiate(rewardSlotTemplate, bronzeRewardSlot);
            finishReward = Instantiate(rewardSlotTemplate, finishRewardSlot);
        }

        public void Show(TimeAttackData data, System.Action onStart, System.Action onCancel)
        {
            this.onStart = onStart;
            this.onCancel = onCancel;

            SetTimeAttackData(data);

            Show();
        }

        public void ShowTimeAttackFinished(TimeAttackData data, float time, System.Action onStart, System.Action onCancel)
        {
            this.onStart = onStart;
            this.onCancel = onCancel;

            SetTimeAttackData(data);
            CheckNewTime(data, time);

            Show();
        }

        protected override void OnShow()
        {
            startButton.onClick.AddListener(OnStart);
            cancelButton.onClick.AddListener(OnExit);
        }

        private void SetTimeAttackData(TimeAttackData data)
        {
            timeAttackName.text = data.timeAttackName;
            timeAttackDescription.text = data.description;

            //var timeRecord = PlayerManager.GetPlayerInstance().GetDataManager().GetTimeRecord();
            //float time = timeRecord.GetTime(data.id);

            //bool firstTime = !timeRecord.HasRecord(data.id);
            //bestTime.text = !firstTime
            //    ? GameUtils.GetTimeFormatted(time)
            //    : "-";

            //goldTime.text = GameUtils.GetTimeFormatted(data.goldTime);
            //goldReward.Show(data.goldReward, data.goldTime >= time);
            //
            //silverTime.text = GameUtils.GetTimeFormatted(data.silverTime);
            //silverReward.Show(data.silverReward, data.silverTime >= time);
            //
            //bronzeTime.text = GameUtils.GetTimeFormatted(data.bronzeTime);
            //bronzeReward.Show(data.bronzeReward, data.bronzeTime >= time);
            //
            //finishReward.Show(data.finishReward, !firstTime);

            newRecord.SetActive(false);
        }

        private void CheckNewTime(TimeAttackData data, float time)
        {
            //var timeRecord = PlayerManager.GetPlayerInstance().GetDataManager().GetTimeRecord();
            //float prevTime = timeRecord.GetTime(data.id);
            //bool hadAnyRecord = timeRecord.HasRecord(data.id);
            //bool isNewRecord = timeRecord.TryToRecordTime(data.id, time);

            //newRecord.SetActive(!hadAnyRecord
            //    || isNewRecord);
            //bestTime.text = GameUtils.GetTimeFormatted(timeRecord.GetTime(data.id));
            //
            //// Finish reward
            //if (!hadAnyRecord)
            //{
            //    finishReward.SetJustAchieved();
            //    PlayerManager.GetReward(data.finishReward);
            //}
            //// Bronze reward
            //if (prevTime > data.bronzeTime
            //    && time <= data.bronzeTime)
            //{
            //    bronzeReward.SetJustAchieved();
            //    PlayerManager.GetReward(data.bronzeReward);
            //}
            //// Silver reward
            //if (prevTime > data.silverTime
            //    && time <= data.silverTime)
            //{
            //    silverReward.SetJustAchieved();
            //    PlayerManager.GetReward(data.silverReward);
            //}
            //// Gold reward
            //if (prevTime > data.goldTime
            //    && time <= data.goldTime)
            //{
            //    goldReward.SetJustAchieved();
            //    PlayerManager.GetReward(data.goldReward);
            //}
        }

        protected override void OnHide()
        {
            onStart = null;
            onCancel = null;
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.RemoveAllListeners();

            bestTime.text = "";

            goldReward.Hide();
            silverReward.Hide();
            bronzeReward.Hide();
            finishReward.Hide();
        }

        private void OnStart()
        {
            var onStart = this.onStart;
            Hide();
            onStart?.Invoke();
        }

        protected override void OnCancel()
        {
            OnExit();
        }

        private void OnExit()
        {
            var onCancel = this.onCancel;
            Hide();
            onCancel?.Invoke();
        }

    }

}