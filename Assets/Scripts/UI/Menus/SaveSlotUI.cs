using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SaveSystem;

namespace UI.Menus
{

    public class SaveSlotUI : MonoBehaviour
    {

        [SerializeField]
        private Button button = null;
        [SerializeField]
        private GameObject usedSlot = null;
        [SerializeField]
        private TextMeshProUGUI slotNumber = null;
        [SerializeField]
        private TextMeshProUGUI fileName = null;

        private int slotIndex = 0;
        private System.Action<int> onSelected = null;

        public Button GetButton() => button;

        public void ShowEmptySlot(int slotIndex, System.Action<int> onEmptySlot)
        {
            Show(slotIndex, false, null, onEmptySlot);
        }

        public void ShowUsedSlot(int slotIndex, SaveFile saveFile, System.Action<int> onUsedSlot)
        {
            Show(slotIndex, true, saveFile.playerData.playerName, onUsedSlot);
        }

        private void Show(int slotIndex, bool usedSlot, string characterName, System.Action<int> onSelected)
        {
            gameObject.SetActive(true);

            this.slotIndex = slotIndex;
            this.onSelected = onSelected;

            slotNumber.text = (slotIndex + 1).ToString();
            fileName.text = usedSlot
                ? characterName
                : "Empty slot";

            this.usedSlot.SetActive(usedSlot);

            button.onClick.AddListener(OnSelected);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            fileName.text = "";
            onSelected = null;
            usedSlot.SetActive(false);
            button.onClick.RemoveAllListeners();
        }

        public void SetColor(Color color)
        {
            var colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }

        public void Select()
        {
            OnSelected();
        }

        public void SetNavigation(SaveSlotUI up, SaveSlotUI down)
        {
            var navigation = button.navigation;

            navigation.selectOnUp = up != null ? up.button : null;
            navigation.selectOnDown = down != null ? down.button : null;

            button.navigation = navigation;
        }

        public void SetRightNavigation(Selectable selectable)
        {
            var navigation = button.navigation;

            navigation.selectOnRight = selectable;

            button.navigation = navigation;
        }

        private void OnSelected()
        {
            Debug.LogFormat("SaveSlot {0} OnSelected: callback is null? {1}",
                slotIndex, onSelected == null);
            onSelected?.Invoke(slotIndex);
        }

    }

}