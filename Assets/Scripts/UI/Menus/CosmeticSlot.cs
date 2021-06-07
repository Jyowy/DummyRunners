using UnityEngine;
using UnityEngine.UI;

namespace Player.Cosmetics
{

    public class CosmeticSlot : MonoBehaviour
    {

        [SerializeField]
        private Button button = null;
        [SerializeField]
        private Image icon = null;
        [SerializeField]
        private Image equipedFrame = null;

        private Cosmetic cosmetic = null;
        private System.Action<Cosmetic> onSelected = null;

        public Cosmetic GetCosmetic() => cosmetic;

        public Selectable GetSelectable() => button;

        public void Show(Cosmetic cosmetic, System.Action<Cosmetic> onSelected, bool equiped)
        {
            this.cosmetic = cosmetic;
            this.onSelected = onSelected;
            this.icon.sprite = cosmetic.GetPreviewSprite();

            equipedFrame.enabled = equiped;

            gameObject.SetActive(true);

            button.onClick.AddListener(OnSelected);
        }

        public GameObject GetFocusable()
            => button.gameObject;

        public void SetNavigation(Selectable up, Selectable down, Selectable left, Selectable right)
        {
            var navigation = button.navigation;
            navigation.selectOnUp = up;
            navigation.selectOnDown = down;
            navigation.selectOnLeft = left;
            navigation.selectOnRight = right;
            button.navigation = navigation;
        }

        public void SetEquiped(bool equiped)
        {
            equipedFrame.enabled = equiped;
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            cosmetic = null;
            onSelected = null;

            button.onClick.RemoveAllListeners();
        }

        private void OnSelected()
        {
            onSelected?.Invoke(cosmetic);
        }

    }

}