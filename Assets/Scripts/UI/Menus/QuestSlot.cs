using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Quests;

namespace UI.Menus
{

    public class QuestSlot : MonoBehaviour
    {

        [SerializeField]
        private Button button = null;
        [SerializeField]
        private Image questCompletedFrame = null;
        [SerializeField]
        private TextMeshProUGUI questName = null;

        public Quest Quest { get; private set; } = null;

        private System.Action<Quest> onQuestSelected;

        public void Show(Quest quest, bool completed, System.Action<Quest> onQuestSelected)
        {
            this.Quest = quest;
            this.onQuestSelected = onQuestSelected;
            questName.text = quest.GetQuestName();

            gameObject.SetActive(true);
            questCompletedFrame.enabled = completed;

            button.onClick.AddListener(OnClick);
        }

        public GameObject GetFocusable()
            => button.gameObject;

        public void Hide()
        {
            gameObject.SetActive(false);
            questName.text = null;
            questCompletedFrame.enabled = false;
            button.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            onQuestSelected?.Invoke(Quest);
        }

    }

}