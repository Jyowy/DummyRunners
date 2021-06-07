using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class PauseMenu : Menu
    {

        [SerializeField]
        private Button resumeButton = null;
        [SerializeField]
        private Button saveButton = null;
        [SerializeField]
        private Button titleMenuButton = null;

        private bool unloadingLevel = false;

        protected override void OnShow()
        {
            resumeButton.onClick.AddListener(OnResume);
            saveButton.onClick.AddListener(OnSave);
            titleMenuButton.onClick.AddListener(OnTitleMenu);
        }

        protected override void OnHide()
        {
            resumeButton.onClick.RemoveAllListeners();
            saveButton.onClick.RemoveAllListeners();
            titleMenuButton.onClick.RemoveAllListeners();
        }

        protected override void OnCancel()
        {
            OnResume();
        }

        private void OnResume()
        {
            if (unloadingLevel)
                return;

            Hide();
        }

        private void OnSave()
        {
            if (unloadingLevel)
                return;

            Debug.LogFormat("OnSave");
            GameManager.SaveFile();
        }

        private void OnTitleMenu()
        {
            if (unloadingLevel)
                return;

            Hide();
            unloadingLevel = true;
            SceneLoader.GoToTitleMenu();
        }

    }

}