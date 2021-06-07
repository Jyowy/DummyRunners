using Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Cosmetics
{

    public class CosmeticShop : UI.Menus.Menu
    {

        [SerializeField]
        private TextMeshProUGUI selectedCosmeticName = null;
        [SerializeField]
        private TextMeshProUGUI selectedCosmeticDescription = null;
        [SerializeField]
        private Image selectedCosmeticIcon = null;

        [SerializeField]
        private TextMeshProUGUI playerMoney = null;
        [SerializeField]
        private TextMeshProUGUI price = null;

        [SerializeField]
        private Button buyButton = null;
        [SerializeField]
        private Button cancelButton = null;

        [SerializeField]
        private Transform cosmeticsRoot = null;
        [SerializeField]
        private CosmeticSlot cosmeticSlotTemplate = null;

        private readonly List<CosmeticSlot> slots = new List<CosmeticSlot>();

        private Cosmetic selectedCosmetic = null;

        private List<Cosmetic> catalog = null;
        private Wallet wallet = null;

        private System.Action onClose = null;

        private void Awake()
        {
            DeselectCosmetic();
        }

        public void Show(List<Cosmetic> catalog, System.Action onClose)
        {
            this.onClose = onClose;

            //var dataManager = PlayerManager.GetPlayerInstance().GetDataManager();
            //var suitcase = dataManager.GetSuitcase();
            //wallet = dataManager.GetWallet();
            UpdatePlayerMoney();

            this.catalog = catalog;
            //for (int i = 0; i < catalog.Count; ++i)
            //{
            //    if (!suitcase.HasCosmetic(catalog[i]))
            //        continue;
            //
            //    catalog.RemoveAt(i);
            //    i--;
            //}

            while (slots.Count < catalog.Count)
            {
                CosmeticSlot cosmeticSlot = Instantiate(cosmeticSlotTemplate, cosmeticsRoot);
                slots.Add(cosmeticSlot);
            }

            for (int i = 0; i < catalog.Count; ++i)
            {
                slots[i].Show(catalog[i], OnSelected, false);
            }

            buyButton.onClick.AddListener(OnBuy);
            cancelButton.onClick.AddListener(Hide);

            Show();
        }

        protected override void InitialFocus()
        {
            if (catalog.Count > 0)
            {
                Debug.LogFormat("Focus on {0}", slots[0].gameObject);
                Focus(slots[0].GetFocusable());
                OnSelected(slots[0].GetCosmetic());
            }
        }

        protected override void OnHide()
        {
            slots.ForEach((x) => x.Hide());

            DeselectCosmetic();
            buyButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            onClose?.Invoke();
        }

        protected override void OnCancel()
            => Hide();

        private void OnSelected(Cosmetic cosmetic)
        {
            Debug.LogFormat("OnCosmeticSelected {0}", cosmetic);

            if (cosmetic == null)
                return;

            selectedCosmetic = cosmetic;

            selectedCosmeticName.text = cosmetic.GetCosmeticName();
            selectedCosmeticName.enabled = true;
            selectedCosmeticDescription.text = cosmetic.GetCosmeticDescription();
            selectedCosmeticDescription.enabled = true;
            selectedCosmeticIcon.sprite = cosmetic.GetPreviewSprite();
            selectedCosmeticIcon.enabled = true;

            price.enabled = true;
            price.text = GameUtils.GetMoneyFormatted(cosmetic.GetNormalPrice());

            buyButton.interactable = wallet.HasEnoughMoney(selectedCosmetic.GetNormalPrice());
        }

        private void OnBuy()
        {
            float price = selectedCosmetic.GetNormalPrice();
            if (selectedCosmetic == null
                || !wallet.HasEnoughMoney(price))
                return;

            //var suitcase = PlayerManager.GetPlayerInstance().GetDataManager().GetSuitcase();
            //wallet.Spend(price);
            //suitcase.AddCosmetic(selectedCosmetic);

            RemoveCosmetic(selectedCosmetic);
            DeselectCosmetic();

            UpdatePlayerMoney();
        }

        private void DeselectCosmetic()
        {
            selectedCosmetic = null;
            selectedCosmeticName.text = "";
            selectedCosmeticName.enabled = false;
            selectedCosmeticDescription.text = "";
            selectedCosmeticDescription.enabled = false;
            selectedCosmeticIcon.sprite = null;
            selectedCosmeticIcon.enabled = false;

            price.text = "";
            price.enabled = false;

            buyButton.interactable = false;
        }

        private void RemoveCosmetic(Cosmetic cosmetic)
        {
            if (cosmetic == null)
                return;

            int index = slots.FindIndex((x) => x.GetCosmetic() == cosmetic);
            if (index < 0)
                return;

            slots[index].Hide();
            slots.RemoveAt(index);
            catalog.RemoveAt(index);
        }

        private void UpdatePlayerMoney()
        {
            playerMoney.text = wallet.FormattedMoney;
        }

    }

}