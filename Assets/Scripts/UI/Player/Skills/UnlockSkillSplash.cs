using UnityEngine;
using UnityEngine.UI;
using Player.Skills;
using Player;

namespace UI.Menus
{

    public class UnlockSkillSplash : Menu
    {

        [SerializeField]
        private Image skillIcon = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI skillName = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI skillDescription = null;

        private Skill skill = null;

        public void Show(Skill skill)
        {
            this.skill = skill;
            string name = skill.GetSkillName();
            string description = skill.GetSkillDescription();
            var sprite = skill.GetSprite();

            skillName.text = name;
            skillDescription.text = description;
            skillIcon.sprite = sprite;

            Show();
        }

        protected override void OnHide()
        {
            Debug.LogFormat("OnHide()");
            if (skill == null)
                return;

            //PlayerManager.UnlockSkill(skill.GetSkillType());

            skill = null;

            skillIcon.sprite = null;
            skillName.text = "";
            skillDescription.text = "";
        }

        protected override void OnAccept()
        {
            Hide();
        }

        protected override void OnCancel()
        {
            Hide();
        }

    }

}