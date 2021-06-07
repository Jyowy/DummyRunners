using Game;
using SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class SaveFilesMenu : Menu
    {

        [SerializeField]
        private TitleMenu titleMenu = null;
        [SerializeField]
        private CreateNewSaveFileMenu createSaveFileMenu = null;

        [SerializeField]
        private int maxSaveSlots = 5;
        [SerializeField]
        private Transform saveSlotRoot = null;
        [SerializeField]
        private SaveSlotUI saveSlotTemplate = null;

        [SerializeField]
        private Button createSlotButton = null;
        [SerializeField]
        private Button loadSlotButton = null;
        [SerializeField]
        private Button deleteSlotButton = null;
        [SerializeField]
        private Button cancelButton = null;

        [SerializeField]
        private Color normalSlotColor = Color.gray;
        [SerializeField]
        private Color selectedSlotColor = Color.yellow;

        private readonly List<SaveSlotUI> saveSlots = new List<SaveSlotUI>();

        private bool canSelectSlot = false;

        private int selectedSlotIndex = 0;

        protected override void OnAwake()
        {
            for (int i = 0; i < maxSaveSlots; ++i)
            {
                SaveSlotUI saveSlot = Instantiate(saveSlotTemplate, saveSlotRoot);
                saveSlots.Add(saveSlot);
            }
        }

        protected override void OnShow()
        {
            Debug.LogFormat("SaveFilesMenu OnShow");
            var saveFiles = GameManager.GetSaveFiles();

            for (int i = 0, saveFilesIndex = 0; i < maxSaveSlots; ++i)
            {
                var saveSlot = saveSlots[i];
                if (saveFilesIndex >= saveFiles.Count
                    || saveFiles[saveFilesIndex].saveSlotIndex > i)
                {
                    saveSlot.ShowEmptySlot(i, OnEmptySlotSelected);
                }
                else
                {
                    saveSlot.ShowUsedSlot(i, saveFiles[saveFilesIndex], OnUsedSlotSelected);
                    saveFilesIndex++;
                }

                Debug.LogFormat("Added save file slot at index {0}", i);

                saveSlot.SetColor(normalSlotColor);
                saveSlots.Add(saveSlot);
            }

            UpdateSlots();

            canSelectSlot = true;

            createSlotButton.onClick.AddListener(OnCreateGameSlot);
            createSlotButton.interactable = false;
            loadSlotButton.onClick.AddListener(OnLoadSaveSlot);
            loadSlotButton.interactable = false;
            deleteSlotButton.onClick.AddListener(OnDeleteSlot);
            deleteSlotButton.interactable = false;

            cancelButton.onClick.AddListener(OnCancel);

            createSlotButton.gameObject.SetActive(saveFiles.Count < saveSlots.Count);
            loadSlotButton.gameObject.SetActive(!createSlotButton.gameObject.activeSelf);
        }

        protected override void OnHide()
        {
            canSelectSlot = false;

            for (int i = 0; i < saveSlots.Count; ++i)
            {
                SaveSlotUI saveSlot = saveSlots[i];
                saveSlot.Hide();
            }

            createSlotButton.onClick.RemoveAllListeners();
            createSlotButton.interactable = false;
            loadSlotButton.onClick.RemoveAllListeners();
            loadSlotButton.interactable = false;
            deleteSlotButton.onClick.RemoveAllListeners();
            deleteSlotButton.interactable = false;
            cancelButton.onClick.RemoveAllListeners();
        }

        protected override void InitialFocus()
        {
            if (saveSlots.Count > 0)
            {
                Focus(saveSlots[0].GetButton().gameObject);
                saveSlots[0].Select();
            }
            else
            {
                base.InitialFocus();
            }
        }

        private void UpdateSlots()
        {
            if (saveSlots.Count == 0)
            {
                UpdateButtonNavigation(createSlotButton, null);
                UpdateButtonNavigation(loadSlotButton, null);
                UpdateButtonNavigation(deleteSlotButton, null);
                UpdateButtonNavigation(cancelButton, null);

                Focus(cancelButton.gameObject);
            }
            else
            {
                var firstSlot = saveSlots[0].GetButton();
                UpdateButtonNavigation(createSlotButton, firstSlot);
                UpdateButtonNavigation(loadSlotButton, firstSlot);
                UpdateButtonNavigation(deleteSlotButton, firstSlot);
                UpdateButtonNavigation(cancelButton, firstSlot);
            }

            if (saveSlots.Count == 1)
            {
                saveSlots[0].SetNavigation(null, null);
            }
            else if (saveSlots.Count > 1)
            {
                int last = saveSlots.Count - 1;
                saveSlots[0].SetNavigation(saveSlots[last], saveSlots[1]);
                saveSlots[last].SetNavigation(saveSlots[last - 1], saveSlots[0]);
                for (int i = 1; i < saveSlots.Count - 1; ++i)
                {
                    saveSlots[i].SetNavigation(saveSlots[i - 1], saveSlots[i + 1]);
                }
            }
        }

        private void UpdateButtonNavigation(Button button, Selectable left)
        {
            var navigation = button.navigation;
            navigation.selectOnLeft = left;
            button.navigation = navigation;
        }

        private void OnEmptySlotSelected(int slotIndex)
        {
            Debug.LogFormat("OnEmptySlotSelected {0}", slotIndex);

            if (selectedSlotIndex >= 0)
                saveSlots[selectedSlotIndex].SetColor(normalSlotColor);
            saveSlots[slotIndex].SetColor(selectedSlotColor);

            selectedSlotIndex = slotIndex;
            createSlotButton.gameObject.SetActive(true);
            createSlotButton.interactable = true;
            loadSlotButton.gameObject.SetActive(false);
            loadSlotButton.interactable = false;
            deleteSlotButton.interactable = false;
            Focus(createSlotButton.gameObject);

            saveSlots.ForEach((slot) => slot.SetRightNavigation(createSlotButton));
            var nav = cancelButton.navigation;
            nav.selectOnUp = createSlotButton;
            cancelButton.navigation = nav;

            var slotButton = saveSlots[slotIndex].GetButton();
            UpdateButtonNavigation(createSlotButton, slotButton);
            UpdateButtonNavigation(cancelButton, slotButton);
        }

        private void OnUsedSlotSelected(int slotIndex)
        {
            Debug.LogFormat("OnUsedSlotSelected {0}", slotIndex);

            if (selectedSlotIndex >= 0)
                saveSlots[selectedSlotIndex].SetColor(normalSlotColor);
            saveSlots[slotIndex].SetColor(selectedSlotColor);

            selectedSlotIndex = slotIndex;
            createSlotButton.gameObject.SetActive(false);
            createSlotButton.interactable = false;
            loadSlotButton.gameObject.SetActive(true);
            loadSlotButton.interactable = true;
            deleteSlotButton.interactable = true;
            Focus(loadSlotButton.gameObject);

            saveSlots.ForEach((slot) => slot.SetRightNavigation(loadSlotButton));
            var nav = cancelButton.navigation;
            nav.selectOnUp = deleteSlotButton;
            cancelButton.navigation = nav;
            var slotButton = saveSlots[slotIndex].GetButton();
            UpdateButtonNavigation(loadSlotButton, slotButton);
            UpdateButtonNavigation(deleteSlotButton, slotButton);
            UpdateButtonNavigation(cancelButton, slotButton);
        }

        private void OnCreateGameSlot()
        {
            if (!canSelectSlot)
                return;

            canSelectSlot = false;
            createSaveFileMenu.Show(CreateGameSlot, CancelGameSlotCreation);
        }

        private void CreateGameSlot(string characterName)
        {
            int selectedSlotIndex = this.selectedSlotIndex;
            Hide();
            SaveFilesManager.CreateNewSaveFile(selectedSlotIndex, characterName);
            GameManager.LoadSaveFile(selectedSlotIndex);
        }

        private void CancelGameSlotCreation()
        {
            canSelectSlot = true;
            Focus(createSlotButton.gameObject);
        }

        private void OnLoadSaveSlot()
        {
            if (!canSelectSlot)
                return;

            canSelectSlot = false;
            int selectedSlotIndex = this.selectedSlotIndex;
            Hide();
            GameManager.LoadSaveFile(selectedSlotIndex);
        }

        private void OnDeleteSlot()
        {
            if (!canSelectSlot)
                return;

            enabled = false;
            GameManager.ShowYesNoPopup("Are you sure you want to delete save this file?",
                "Yes", "No", OnConfirmDeleteSlot, () => enabled = true);
        }

        private void OnConfirmDeleteSlot()
        {
            SaveFilesManager.DeleteSaveFile(selectedSlotIndex);
            saveSlots[selectedSlotIndex].ShowEmptySlot(selectedSlotIndex, OnEmptySlotSelected);

            enabled = true;
            InitialFocus();
        }

        protected override void OnCancel()
        {
            //titleMenu.Show();
            titleMenu.enabled = true;
            Hide();
        }

    }

}