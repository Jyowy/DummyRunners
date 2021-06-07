using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;

namespace UI.Menus
{

    public class PlayerSkillsPage : MenuPage
    {

        [SerializeField]
        private TextMeshProUGUI noSkillsMessage = null;

        [SerializeField]
        private Transform skillSlotRoot = null;
        [SerializeField]
        private PlayerSkillSlot skillSlotTemplate = null;

        private readonly List<PlayerSkillSlot> skillSlots = new List<PlayerSkillSlot>();

        protected override void OnShow()
        {
            //var dataManager = PlayerManager.GetPlayerInstance().GetDataManager();
            //var skillManager = dataManager.GetSkillManager();
            
            //var skills = skillManager.GetSkillsData();

            int unlockedSkills = 0;

            //skills.ForEach((x) =>
            //    {
            //        if (x.unlocked)
            //        {
            //            unlockedSkills++;
            //        }
            //    }
            //);
            noSkillsMessage.enabled = unlockedSkills == 0;

            while (skillSlots.Count < unlockedSkills)
            {
                PlayerSkillSlot skillSlot = Instantiate(skillSlotTemplate, skillSlotRoot);
                skillSlots.Add(skillSlot);
            }

            //for (int i = 0, slot = 0; i < skills.Count; ++i)
            //{
            //    var skill = skillManager.GetSkill(skills[i].skill);
            //    if (!skill.IsUnlocked)
            //        continue;
            //
            //    skillSlots[slot].Show(skill);
            //    slot++;
            //}
        }

        protected override void OnHide()
        {
            skillSlots.ForEach((x) => x.Hide());
        }

    }

}