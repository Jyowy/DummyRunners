using Common;
using Player.Cosmetics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Rewards;

namespace UI.Menus
{

    public class RewardSlotUI : MonoBehaviour
    {

        [SerializeField]
        private GameObject moneySlot = null;
        [SerializeField]
        private TextMeshProUGUI money = null;

        [SerializeField]
        private GameObject energySlot = null;

        [SerializeField]
        private GameObject cosmeticSlot = null;
        [SerializeField]
        private Image cosmeticIcon = null;

        [SerializeField]
        private GameObject achievedFrame = null;
        [SerializeField]
        private GameObject justAchievedFrame = null;

        public void Show(Reward reward, bool achieved)
        {
            gameObject.SetActive(true);

            SetMoneySlot(reward.GetMoney());
            SetEnergySlot(reward.ContainsEnergySlot());
            SetCosmeticSlot(reward.GetCosmetic());

            achievedFrame.SetActive(achieved);
            justAchievedFrame.SetActive(false);
        }

        public void SetJustAchieved()
        {
            justAchievedFrame.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            SetMoneySlot(0f);
            SetEnergySlot(false);
            SetCosmeticSlot(null);

            achievedFrame.SetActive(false);
            justAchievedFrame.SetActive(false);
        }

        private void SetMoneySlot(float money)
        {
            moneySlot.SetActive(money > 0f);
            this.money.text = GameUtils.GetMoneyFormatted(money);
        }

        private void SetEnergySlot(bool hasEnergySlot)
        {
            energySlot.SetActive(hasEnergySlot);
        }

        private void SetCosmeticSlot(Cosmetic cosmetic)
        {
            cosmeticSlot.SetActive(cosmetic != null);
            cosmeticIcon.sprite = cosmetic != null
                ? cosmetic.GetPreviewSprite()
                : null;
        }

    }

}