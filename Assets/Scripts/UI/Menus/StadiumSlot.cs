using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class StadiumSlot : MonoBehaviour
    {

        [SerializeField]
        private StadiumMenu stadiumMenu = null;
        [SerializeField]
        private Button button = null;

        private System.Action<StadiumMenu> onSelected = null;

        public Button GetButton() => button;

        public void Show(System.Action<StadiumMenu> onSelected)
        {
            gameObject.SetActive(true);
            this.onSelected = onSelected;
            button.onClick.AddListener(OnSelected);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            onSelected = null;
            button.onClick.RemoveAllListeners();
        }

        public void Select()
        {
            button.Select();
        }

        private void OnSelected()
        {
            onSelected?.Invoke(stadiumMenu);
        }


    }

}