using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class YesNoPopup : Menu
    {

        [SerializeField]
        private TextMeshProUGUI message = null;
        [SerializeField]
        private Button yesButton = null;
        [SerializeField]
        private TextMeshProUGUI yesText = null;
        [SerializeField]
        private Button noButton = null;
        [SerializeField]
        private TextMeshProUGUI noText = null;

        private System.Action onYes = null;
        private System.Action onNo = null;

        public void Show(string message, string yes, string no, System.Action onYes, System.Action onNo)
        {
            this.message.text = message;
            yesText.text = yes;
            noText.text = no;

            this.onYes = onYes;
            this.onNo = onNo;

            Show();
        }

        protected override void OnShow()
        {
            yesButton.onClick.AddListener(OnYes);
            noButton.onClick.AddListener(OnNo);
        }

        protected override void OnHide()
        {
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
        }

        private void OnYes()
        {
            var onYes = this.onYes;
            Hide();
            onYes?.Invoke();
        }

        private void OnNo()
        {
            var onNo = this.onNo;
            Hide();
            onNo?.Invoke();
        }

        protected override void OnCancel()
        {
            OnNo();
        }

    }

}