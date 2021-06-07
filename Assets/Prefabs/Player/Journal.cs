using World.Quests;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Player
{

    [Serializable]
    public class Journal
    {

        [SerializeField]
        private List<string> activeQuests = new List<string>();
        [SerializeField]
        private List<string> completedQuests = new List<string>();

        [NonSerialized]
        private List<Quest> activeQuestsCache = new List<Quest>();
        [NonSerialized]
        private List<Quest> completedQuestsCache = new List<Quest>();

        public List<Quest> GetActiveQuestsCache() => activeQuestsCache;
        public List<Quest> GetCompletedQuestsCache() => completedQuestsCache;

        public Journal() { }

        public Journal(Journal other)
        {
            activeQuests = new List<string>(other.activeQuests);
            completedQuests = new List<string>(other.completedQuests);
        }

        public void Initialize()
        {
            InitializeCache();
        }

        private void InitializeCache()
        {
            activeQuestsCache = new List<Quest>();
            completedQuestsCache = new List<Quest>();

            activeQuests.ForEach(
                (id) =>
                {
                    Quest quest = QuestArchive.GetQuest(id);
                    if (quest != null)
                    {
                        activeQuestsCache.Add(quest);
                    }
                }
            );

            completedQuests.ForEach(
                (id) =>
                {
                    Quest quest = QuestArchive.GetQuest(id);
                    if (quest != null)
                    {
                        completedQuestsCache.Add(quest);
                    }
                }
            );
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest))
                return;

            activeQuests.Add(quest.GetQuestId());
            activeQuestsCache.Add(quest);
            Debug.LogFormat("Journal: Quest {0} added", quest.GetQuestName());
        }

        public void CompleteQuest(Quest quest)
        {
            if (!IsQuestActive(quest))
                return;

            int index = GetQuestIndex(activeQuests, quest);
            activeQuests.RemoveAt(index);
            activeQuestsCache.RemoveAt(index);
            completedQuests.Add(quest.GetQuestId());
            completedQuestsCache.Add(quest);

            Debug.LogFormat("Journal: Quest {0} completed", quest.GetQuestName());

            var reward = quest.GetReward();
            PlayerManager.GetReward(reward);
        }

        public bool HasQuest(Quest quest)
            => ContainsQuest(completedQuests, quest)
                || ContainsQuest(activeQuests, quest);

        public bool IsQuestActive(Quest quest)
            => ContainsQuest(activeQuests, quest);

        public bool HasCompletedQuest(Quest quest)
            => ContainsQuest(completedQuests, quest);

        private static bool ContainsQuest(List<string> questList, Quest quest)
        {
            string id = quest.GetQuestId();
            return questList.Find((x) => x == id) != null;
        }

        private static int GetQuestIndex(List<string> questList, Quest quest)
        {
            string id = quest.GetQuestId();
            return questList.FindIndex((x) => x == id);
        }

    }

}