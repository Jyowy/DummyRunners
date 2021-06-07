using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class RacePauseMenu : Menu
    {

        [SerializeField]
        private Button resumeButton = null;
        [SerializeField]
        private Button retryButton = null;
        [SerializeField]
        private Button abortButton = null;

        private System.Action onRetry = null;
        private System.Action onExit = null;

        public void Show(System.Action onRetry, System.Action onAbort)
        {
            this.onRetry = onRetry;
            this.onExit = onAbort;

            resumeButton.onClick.AddListener(Hide);
            retryButton.onClick.AddListener(OnRetry);
            abortButton.onClick.AddListener(OnAbort);

            Show();
        }

        protected override void OnHide()
        {
            onRetry = null;
            onExit = null;

            resumeButton.onClick.RemoveAllListeners();
            retryButton.onClick.RemoveAllListeners();
            abortButton.onClick.RemoveAllListeners();
        }

        protected override void OnCancel()
        {
            Hide();
        }

        private void OnRetry()
        {
            var onRetry = this.onRetry;

            Hide();
            onRetry?.Invoke();
        }

        private void OnAbort()
        {
            var onAbort = this.onExit;

            Hide();
            onAbort?.Invoke();
        }

    }

}