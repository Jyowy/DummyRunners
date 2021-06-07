using Player.Skills;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class PlayerSkillSlot : MonoBehaviour
    {

        [SerializeField]
        private Image skillIcon = null;
        [SerializeField]
        private TextMeshProUGUI skillName = null;
        [SerializeField]
        private TextMeshProUGUI skillDescription = null;

        public void Show(Skill skill)
        {
            gameObject.SetActive(true);
            skillIcon.sprite = skill.GetSprite();
            skillName.text = skill.GetSkillName();
            skillDescription.text = skill.GetSkillDescription();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            skillIcon.sprite = null;
            skillName.text = null;
            skillDescription.text = null;
        }

    }

}