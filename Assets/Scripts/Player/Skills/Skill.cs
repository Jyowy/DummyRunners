using UnityEngine;

namespace Player.Skills
{

    public enum SkillEnum
    {
        None,
        DoubleJump,
        WalkOnWalls
    }

    public abstract class Skill : MonoBehaviour
    {

        [SerializeField]
        private string skillName = "";
        [SerializeField]
        private string description = "";
        [SerializeField]
        private Sprite sprite = null;

        [SerializeField]
        private bool unlocked = false;

        [SerializeField]
        private int slotConsume = 1;
        [SerializeField]
        private bool consumeOverTime = false;
        [SerializeField]
        private float consumeSlotEach = 1.5f;

        public abstract SkillEnum GetSkillType();

        public string GetSkillName() => skillName;
        public string GetSkillDescription() => description;
        public Sprite GetSprite() => sprite;

        public bool IsUnlocked => unlocked;
        public bool IsTemporaryBlocked { get; private set; } = false;
        public virtual bool IsAvailable() =>
            unlocked
            && !IsTemporaryBlocked;

        private System.Func<int, bool> hasEnoughEnergy = null;
        private System.Func<int, bool> consumeEnergy = null;
        private System.Func<float, bool> startConsuming = null;
        private System.Action stopConsuming = null;

        public void Initialize(
            System.Func<int, bool> hasEnoughEnergy,
            System.Func<int, bool> consumeEnergy,
            System.Func<float, bool> startConsuming,
            System.Action stopConsuming)
        {
            this.hasEnoughEnergy = hasEnoughEnergy;
            this.consumeEnergy = consumeEnergy;
            this.startConsuming = startConsuming;
            this.stopConsuming = stopConsuming;
        }

        public SkillData GetSkillData()
            => new SkillData()
                {
                    skill = GetSkillType(),
                    unlocked = unlocked
                };

        public void SetSkillData(SkillData data)
        {
            if (data.skill != GetSkillType())
                return;
            unlocked = data.unlocked;
        }

        protected bool HasEnoughEnergy()
            => hasEnoughEnergy.Invoke(slotConsume);

        protected bool ConsumeEnergy()
        {
            bool success = false;

            if (!consumeOverTime)
            {
                success = consumeEnergy.Invoke(slotConsume);
            }
            else
            {
                success = startConsuming.Invoke(consumeSlotEach);
            }

            return success;
        }

        protected void StopConsumingEnergy()
            => stopConsuming.Invoke();

        public virtual void OnEnergyBatteryEmpty() { }

        public void Unlock()
        {
            if (unlocked)
                return;

            unlocked = true;
        }

        public void Lock()
        {
            if (!unlocked)
                return;

            unlocked = false;
        }

        public void Unblock()
        {
            if (IsTemporaryBlocked)
                return;

            IsTemporaryBlocked = true;
        }

        public void Block()
        {
            if (!IsTemporaryBlocked)
                return;

            IsTemporaryBlocked = false;
        }

    }

}