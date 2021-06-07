using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Skills
{

    public class SkillManager : MonoBehaviour
    {

        [SerializeField]
        private EnergyBattery battery = null;
        [SerializeField]
        private List<Skill> skills = new List<Skill>();

        public EnergyBattery GetEnergyBattery() => battery;

        private List<SkillData> skillsData = null;

        public List<SkillData> GetSkillsData()
        {
            return skillsData;
        }

        public void SetSkillsData(List<SkillData> skillsData)
        {
            for (int i = 0; i < skillsData.Count; ++i)
            {
                SkillEnum skillType = skillsData[i].skill;
                var skill = skills.Find((x) => x.GetSkillType() == skillType);
                skill.SetSkillData(skillsData[i]);
            }
            this.skillsData = skillsData;
        }

        private void Start()
        {
            //for (int i = 0; i < skills.Count; ++i)
            //{
            //    skills[i].Initialize(
            //        battery.HasEnoughEnergy,
            //        battery.Consume,
            //        battery.StartConsuming,
            //        battery.StopConsuming
            //    );
            //    battery.batteryEmpty += skills[i].OnEnergyBatteryEmpty;
            //}

            if (skillsData == null)
            {
                skillsData = new List<SkillData>();
                for (int i = 0; i < skills.Count; ++i)
                {
                    SkillData skillData = new SkillData()
                    {
                        skill = skills[i].GetSkillType(),
                        unlocked = skills[i].IsUnlocked
                    };
                    skillsData.Add(skillData);
                }
            }

            //SetUIVisible(HasAnySkillUnlocked());
        }

        private void OnDestroy()
        {
            for (int i = 0; i < skills.Count; ++i)
            {
                battery.batteryEmpty -= skills[i].OnEnergyBatteryEmpty;
            }

            skills.Clear();
        }

        public bool HasAnySkillUnlocked()
            => skills.Find((x) => x.IsUnlocked) != null;

        public bool IsSkillAvailable(SkillEnum skill)
        {
            var ownedSkill = GetSkill(skill);
            return ownedSkill != null
                && ownedSkill.IsAvailable();
        }

        public void UnlockSkill(SkillEnum skillType)
        {
            var skill = GetSkill(skillType);
            if (skill == null)
                return;

            if (!HasAnySkillUnlocked())
            {
                //SetUIVisible(true);
            }

            Debug.LogFormat("Skill {0} unlocked!", skillType);
            skill.Unlock();

            int index = skillsData.FindIndex((x) => x.skill == skillType);
            bool found = index >= 0;

            if (found)
            {
                var skillData = skillsData[index];
                skillData.unlocked = true;
                skillsData[index] = skillData;
            }
            else
            {
                var skillData = new SkillData()
                {
                    skill = skillType,
                    unlocked = true
                };
                skillsData.Add(skillData);
            }
        }

        public void SetTemporaryLock(SkillEnum skill, bool temporaryBlock)
        {
            var ownedSkill = GetSkill(skill);
            if (ownedSkill == null
                || !ownedSkill.IsUnlocked)
                return;

            if (temporaryBlock)
            {
                ownedSkill.Block();
            }
            else
            {
                ownedSkill.Unblock();
            }
        }
        
        public Skill GetSkill(SkillEnum skill)
            => skills.Find((x) => x.GetSkillType() == skill);
        /*
        public void SetUIVisible(bool visible)
            => battery.SetVisible(visible);
        //*/

    }

}