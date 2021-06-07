using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.DialogSystem
{

    public class AnswerVisualizer : MonoBehaviour
    {

        [SerializeField]
        private Button button = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI message = null;

        private int index = 0;
        private System.Action<int> onSelected = null;

        private bool selectable = true;

        public void Initialize(Answer answer, int index, System.Action<int> onSelected)
        {
            selectable = true;

            message.text = answer.message;
            this.index = index;
            this.onSelected = onSelected;

            button.onClick.AddListener(Selected);
        }

        public void SetNavigation(AnswerVisualizer prev, AnswerVisualizer next)
        {
            var navigation = button.navigation;

            navigation.selectOnUp = prev.button;
            navigation.selectOnLeft = prev.button;

            navigation.selectOnDown = next.button;
            navigation.selectOnRight = next.button;

            button.navigation = navigation;
        }

        public void Focus()
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        public void Select()
        {
            Selected();
        }

        private void Selected()
        {
            Debug.LogFormat("Answer '{0}' selected (is selectable {1})", message, selectable);

            if (!selectable)
                return;

            onSelected?.Invoke(index);
            StopListening();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }    

        public void Hide()
        {
            gameObject.SetActive(false);
            StopListening();
        }

        private void StopListening()
        {
            selectable = false;
            button.onClick.RemoveAllListeners();
        }

    }

}