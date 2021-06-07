using Player.Skills;
using UnityEngine;
using System.Collections.Generic;
using World.Rewards;
using Player.Cosmetics;

namespace Player
{

    public class PlayerDataManager : MonoBehaviour
    {

        [SerializeField]
        private SkillManager skillManager = null;
        [SerializeField]
        private CostumWearer wearer = null;

        private PlayerData playerData = new PlayerData();
        private Journal journal = new Journal();
        private Wallet wallet = new Wallet();
        private TimeRecord timeRecord = new TimeRecord();
#if UNITY_EDITOR
        // Only for development
        [SerializeField]
#endif
        private CosmeticSuitcase suitcase = new CosmeticSuitcase();

        public bool CanWalkOnWalls() => skillManager.IsSkillAvailable(SkillEnum.WalkOnWalls);
        public bool CanJumpOnWalls() => CanWalkOnWalls();

        public PlayerData GetData() => playerData;
        public SkillManager GetSkillManager() => skillManager;
        public Journal GetJournal() => journal;
        public Wallet GetWallet() => wallet;
        public CostumWearer GetWearer() => wearer;
        public CosmeticSuitcase GetSuitcase() => suitcase;
        public TimeRecord GetTimeRecord() => timeRecord;

        public void SetData(
                PlayerData playerData,
                List<SkillData> skillsData,
                Journal journal,
                Wallet wallet,
                CosmeticSuitcase suitcase,
                TimeRecord timeRecord
            )
        {
            this.playerData = playerData;
            this.skillManager.SetSkillsData(skillsData);
            this.journal = journal;
            this.wallet = wallet;
            this.suitcase = suitcase;
            this.timeRecord = timeRecord;

            journal.Initialize();
            suitcase.Initialize();
            wearer.SetSuitcaseCostum(suitcase.GetCostum());
        }

#if UNITY_EDITOR
        private void Awake()
        {
            journal.Initialize();
            suitcase.Initialize();
            wearer.SetSuitcaseCostum(suitcase.GetCostum());
        }
#endif

        public void GetReward(Reward reward)
        {
            if (reward.ContainsMoney())
            {
                float money = reward.GetMoney();
                wallet.Receive(money);
            }

            if (reward.UnlocksSkill())
            {
                SkillEnum skillToUnlock = reward.GetSkill();
                var skill = skillManager.GetSkill(skillToUnlock);
                PlayerManager.ShowSkillUnlockedSplash(skill);
            }

            if (reward.ContainsCosmetic())
            {
                Cosmetic cosmetic = reward.GetCosmetic();
                suitcase.AddCosmetic(cosmetic);
            }
        }

    }

}