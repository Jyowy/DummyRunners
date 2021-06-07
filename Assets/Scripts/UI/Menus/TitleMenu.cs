using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class TitleMenu : Menu
    {

        [SerializeField]
        private Button startGameButton = null;
        [SerializeField]
        private Button exitGameButton = null;

        [SerializeField]
        private SaveFilesMenu loadGameMenu = null;

        protected override void OnShow()
        {
            startGameButton.onClick.AddListener(OnStartGame);
            exitGameButton.onClick.AddListener(OnExit);

            AudioManager.MenuMusic();
        }

        protected override void OnHide()
        {
            startGameButton.onClick.RemoveListener(OnStartGame);
            exitGameButton.onClick.RemoveListener(OnExit);
        }

        private void OnStartGame()
        {
            Debug.LogFormat("OnStartGame");
            enabled = false;
            loadGameMenu.Show();
        }

        private void OnExit()
        {
            enabled = false;
            GameManager.ShowYesNoPopup("Exit game?", "Yes", "No", GameManager.QuitGame, () => enabled = true);
        }

    }

}