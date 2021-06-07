using UnityEngine;
using Player;
using TMPro;
using System.Collections.Generic;
using World.Quests;

namespace UI.Menus
{

    public class JournalPage : MenuPage
    {

        [SerializeField]
        private TextMeshProUGUI noActiveQuestsMessage = null;
        [SerializeField]
        private TextMeshProUGUI noCompletedQuestsMessage = null;

        [SerializeField]
        private TextMeshProUGUI noQuestSelected = null;
        [SerializeField]
        private TextMeshProUGUI selectedQuestName = null;
        [SerializeField]
        private TextMeshProUGUI selectedQuestDescription = null;

        [SerializeField]
        private Transform activeQuestsRoot = null;
        [SerializeField]
        private Transform completedQuestsRoot = null;
        [SerializeField]
        private QuestSlot questSlotTemplate = null;

        private readonly List<QuestSlot> slots = new List<QuestSlot>();

        private bool hasQuests = false;

        protected override void OnShow()
        {
            //var playerDataManager = PlayerManager.GetPlayerInstance().GetDataManager();
            //var journal = playerDataManager.GetJournal();

            //var activeQuests = journal.GetActiveQuestsCache();
            //var completedQuests = journal.GetCompletedQuestsCache();

            //Debug.LogFormat("Journal active quests is null? {0}",
            //    activeQuests == null);
            //
            //int activeCount = activeQuests.Count;
            //int completedCount = completedQuests.Count;

            //noActiveQuestsMessage.enabled = activeCount == 0;
            //noCompletedQuestsMessage.enabled = completedCount == 0;

            //int totalQuests = activeCount + completedCount;
            //while (slots.Count < totalQuests)
            //{
            //    QuestSlot questSlot = Instantiate(questSlotTemplate, completedQuestsRoot);
            //    slots.Add(questSlot);
            //}

            //int slotIndex = 0;
            //for (int i = 0; i < activeCount; ++i, ++slotIndex)
            //{
            //    QuestSlot questSlot = slots[slotIndex];
            //    questSlot.transform.SetParent(activeQuestsRoot);
            //    questSlot.Show(activeQuests[i], false, OnQuestSelected);
            //}
            //
            //for (int i = 0; i < completedCount; ++i, ++slotIndex)
            //{
            //    QuestSlot questSlot = slots[slotIndex];
            //    questSlot.transform.SetParent(completedQuestsRoot);
            //    questSlot.Show(completedQuests[i], true, OnQuestSelected);
            //}
            //
            //hasQuests = totalQuests > 0;
            //noQuestSelected.enabled = !hasQuests;

            selectedQuestName.text = "";
            selectedQuestName.enabled = false;
            selectedQuestDescription.text = "";
            selectedQuestDescription.enabled = false;
        }

        public override void InitialFocus()
        {
            if (hasQuests)
            {
                Focus(slots[0].GetFocusable());
                OnQuestSelected(slots[0].Quest);
            }
        }

        protected override void OnHide()
        {
            slots.ForEach((x) => x.Hide());

            hasQuests = false;

            selectedQuestName.text = "";
            selectedQuestName.enabled = false;
            selectedQuestDescription.text = "";
            selectedQuestDescription.enabled = false;
        }

        private void OnQuestSelected(Quest quest)
        {
            selectedQuestName.text = quest.GetQuestName();
            selectedQuestName.enabled = true;
            selectedQuestDescription.text = quest.GetQuestDescription();
            selectedQuestDescription.enabled = true;
        }

    }

}