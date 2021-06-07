using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class CreateNewSaveFileMenu : Menu
    {

        [SerializeField]
        private TMP_InputField nameInputField = null;
        [SerializeField]
        private Button confirmButton = null;
        [SerializeField]
        private Button cancelButton = null;

        private System.Action<string> onConfirm = null;
        private System.Action onCancel = null;

        protected override void OnAwake()
        {
            nameInputField.text = "";
            nameInputField.richText = false;
        }

        public void Show(System.Action<string> onConfirm, System.Action onCancel)
        {
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;

            Show();
        }

        protected override void OnShow()
        {
            confirmButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(OnCancel);
        }

        protected override void OnHide()
        {
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }

        private void OnConfirm()
        {
            string name = GetName();
            bool isAValidName = IsValidName(name);
            confirmButton.interactable = isAValidName;

            if (!isAValidName)
                return;

            string message = string.Format("You're name is gonna be: \"{0}\"", name);
            GameManager.ShowYesNoPopup(message, "Accept", "Cancel", OnNameAccepted, OnNameNotAccepted);
        }

        private void OnNameAccepted()
        {
            string name = GetName();
            var onConfirm = this.onConfirm;
            Hide();
            onConfirm?.Invoke(name);
        }

        private void OnNameNotAccepted()
        {
            Focus(nameInputField.gameObject);
        }

        protected override void OnCancel()
        {
            var onCancel = this.onCancel;
            Hide();
            onCancel?.Invoke();
        }

        private bool nameInputFieldFocused = false;

        private void Update()
        {
            if (nameInputFieldFocused
                && !nameInputField.isFocused)
            {
                string name = GetName();
                confirmButton.interactable = IsValidName(name);
            }

            nameInputFieldFocused = nameInputField.isFocused;
        }

        private readonly static char[] charsToTrim = new char[] { '\r', '\n', '\t', '\'' };

        private string GetName()
            => nameInputField.text
                .Trim(charsToTrim)
                .TrimStart(' ')
                .TrimEnd(' ');

        private static bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

    }

}