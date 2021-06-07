using Player.Cosmetics;
using Player.Skills;
using UnityEngine;

namespace World.Rewards
{

    [System.Serializable]
    public class Reward
    {

        [SerializeField]
        private bool containsMoney = false;
        [SerializeField]
        private float money = 0f;

        [SerializeField]
        private bool containsEnergySlot = false;

        [SerializeField]
        private bool unlocksSkill = false;
        [SerializeField]
        private SkillEnum skill = SkillEnum.None;

        [SerializeField]
        private bool containsCosmetic = false;
        [SerializeField]
        private Cosmetic cosmetic = null;

        public bool ContainsMoney() => containsMoney;
        public float GetMoney()
        {
            if (!containsMoney)
                return 0f;

            return money;
        }

        public bool ContainsEnergySlot() => containsEnergySlot;

        public bool UnlocksSkill() => unlocksSkill;
        public SkillEnum GetSkill()
        {
            if (!unlocksSkill)
                return SkillEnum.None;

            return skill;
        }

        public bool ContainsCosmetic() => containsCosmetic;
        public Cosmetic GetCosmetic() =>
            containsCosmetic ? cosmetic : null;

    }

}