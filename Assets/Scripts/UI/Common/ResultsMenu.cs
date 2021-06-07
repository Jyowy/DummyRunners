using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class ResultsMenu : Menu
    {

        [SerializeField]
        private GameObject win = null;
        [SerializeField]
        private GameObject lose = null;

        [SerializeField]
        private Button retryButton = null;
        [SerializeField]
        private Button exitButton = null;

        private System.Action onRetry = null;
        private System.Action onExit = null;

        public void Show(bool playerWin, System.Action onRetry, System.Action onExit)
        {
            win.SetActive(playerWin);
            lose.SetActive(!playerWin);
            this.onRetry = onRetry;
            this.onExit = onExit;

            Show();
        }

        protected override void OnShow()
        {
            retryButton.onClick.AddListener(OnRetry);
            exitButton.onClick.AddListener(OnExit);
        }

        protected override void OnHide()
        {
            win.SetActive(false);
            lose.SetActive(false);
            retryButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        private void OnRetry()
        {
            var onRetry = this.onRetry;
            Hide();
            onRetry?.Invoke();
        }

        private void OnExit()
        {
            var onExit = this.onExit;
            Hide();
            onExit?.Invoke();
        }

    }

}