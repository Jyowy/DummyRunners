using UnityEngine;
using World.Rewards;

namespace World.Quests
{

    [CreateAssetMenu()]
    public class Quest : ScriptableObject
    {

        public string GetQuestId() => questId;

        [SerializeField]
        private string questId = "QUEST_";
        [SerializeField]
        private string questName = "";
        [SerializeField]
        private string description = "";

        [SerializeField]
        private bool hasReward = false;

        [SerializeField]
        private Reward reward = null;

        public string GetQuestName() => questName;
        public string GetQuestDescription() => description;

        public bool Is(Quest quest) => questId.Equals(quest.questId);

        public bool HasReward() => hasReward;
        public Reward GetReward() => reward;

    }

}