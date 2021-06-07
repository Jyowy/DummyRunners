using Common;
using Game;
using TMPro;
using UnityEngine;

namespace UI.Menus
{

    public class CountdownSplash : Menu
    {

        [SerializeField]
        private TextMeshProUGUI countdown = null;
        [SerializeField]
        private GameObject goMessageSplash = null;
        [SerializeField]
        private float goMessageDuration = 1f;

        [SerializeField]
        private float secondsFontSize = 30f;
        [SerializeField]
        private float csecondsFontSize = 10f;

        float countdownTime = 0f;

        private TimedAction goMessage;

        private System.Action onFinished = null;

        public void Show(float countdownDuration, System.Action onFinished)
        {
            enabled = true;
            countdownTime = countdownDuration;
            countdown.enabled = true;
            UpdateCountdownUI();
            goMessageSplash.SetActive(false);
            this.onFinished = onFinished;

            AudioManager.PlayCountdown();

            Show();
        }

        protected override void OnHide()
        {
            enabled = false;
            countdownTime = 0;
            countdown.enabled = false;
            countdown.text = "";
            goMessageSplash.SetActive(false);
            onFinished = null;
            goMessage.Clear();
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;

            if (countdownTime >= 1f)
            {
                countdownTime = Mathf.Max(countdownTime - dt, 0f);
                UpdateCountdownUI();

                if (countdownTime < 1f)
                {
                    ShowGoMessageSplash();
                }
            }
            else if (goMessage.Update(dt))
            {
                GoMessageFinished();
            }
        }

        private void UpdateCountdownUI()
        {
            countdown.text = GameUtils.GetTimeAsCountdownFormatted(countdownTime, secondsFontSize, csecondsFontSize);
        }

        private void ShowGoMessageSplash()
        {
            countdown.enabled = false;
            countdown.text = "";
            goMessageSplash.SetActive(true);
            goMessage.Set(goMessageDuration);
        }

        private void GoMessageFinished()
        {
            System.Action onFinished = this.onFinished;
            Hide();
            onFinished?.Invoke();
        }

    }

}