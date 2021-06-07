using System.Collections.Generic;
using TMPro;
using UI.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Cosmetics
{

    public class CosmeticsPage : MenuPage
    {

        [SerializeField]
        private Image selectedCosmeticIcon = null;
        [SerializeField]
        private TextMeshProUGUI selectedCosmeticName = null;
        [SerializeField]
        private TextMeshProUGUI selectedCosmeticDescription = null;
        [SerializeField]
        private TextMeshProUGUI noCosmeticSelected = null;

        [SerializeField]
        private Button equipButton = null;
        [SerializeField]
        private TextMeshProUGUI equipText = null;
        [SerializeField]
        private TextMeshProUGUI unequipText = null;

        [SerializeField]
        private GridLayoutGroup grid = null;
        [SerializeField]
        private CosmeticSlot cosmeticSlotTemplate = null;
        [SerializeField]
        private TextMeshProUGUI noCosmeticsMessage = null;

        private readonly List<CosmeticSlot> cosmeticSlots = new List<CosmeticSlot>();

        private CostumWearer costumWearer = null;
        private Costum costumReference = null;
        private Cosmetic selectedCosmetic = null;
        private bool isSelectedCosmeticEquiped = false;

        private CosmeticSuitcase suitcase = null;

        protected override void OnShow()
        {
            //var playerDataManager = PlayerManager.GetPlayerInstance().GetDataManager();

            //suitcase = playerDataManager.GetSuitcase();
            //costumWearer = playerDataManager.GetWearer();
            //costumReference = costumWearer.GetCostum();

            var cosmetics = suitcase.GetCosmetics();

            int count = cosmetics.Count;
            while (cosmeticSlots.Count < count)
            {
                CosmeticSlot cosmeticSlot = Instantiate(cosmeticSlotTemplate, grid.transform);
                cosmeticSlots.Add(cosmeticSlot);
            }

            for (int i = 0; i < cosmetics.Count; ++i)
            {
                Cosmetic cosmetic = cosmetics[i];
                cosmeticSlots[i].Show(cosmetic, OnSelected, costumReference.IsThisCosmeticEquiped(cosmetic));
            }

            selectedCosmeticName.text = null;
            selectedCosmeticDescription.text = null;
            selectedCosmeticIcon.sprite = null;
            selectedCosmeticIcon.enabled = false;

            noCosmeticSelected.enabled = true;
            equipButton.interactable = false;
            unequipText.enabled = false;
            equipButton.onClick.AddListener(OnEquip);

            noCosmeticsMessage.enabled = cosmetics.Count == 0;

            SetUpNavigation();
        }

        private void SetUpNavigation()
        {
            var navigation = equipButton.navigation;
            navigation.selectOnLeft =
                cosmeticSlots.Count > 0
                ? cosmeticSlots[0].GetSelectable()
                : null;
            equipButton.navigation = navigation;

            if (cosmeticSlots.Count == 0)
                return;

            float width = grid.GetComponent<RectTransform>().rect.width;
            float cellWidth = grid.cellSize.x;
            int cols = (int)(width / cellWidth);
            int rows = cosmeticSlots.Count / cols;
            int remaining = cosmeticSlots.Count - cols * rows;

            Selectable up = null;
            Selectable down = null;
            Selectable left = null;
            Selectable right = null;

            int slotIndex = 0;
            int count = cosmeticSlots.Count;
            for (int i = 0; i < rows; ++i)
            {
                if (i > 0)
                    up = cosmeticSlots[slotIndex - cols].GetSelectable();
                else
                    up = null;

                if ((i < rows - 1)
                    || (remaining > 0 && (slotIndex + cols) < count))
                    down = cosmeticSlots[slotIndex + cols].GetSelectable();
                else
                    down = null;

                left = null;
                right = cosmeticSlots[slotIndex + 1].GetSelectable();
                cosmeticSlots[slotIndex].SetNavigation(up, down, left, right);
                slotIndex++;

                for (int j = 1; j < cols - 1; ++j, ++slotIndex)
                {
                    if (i > 0)
                        up = cosmeticSlots[slotIndex - cols].GetSelectable();
                    else
                        up = null;

                    if ((i < rows - 1)
                        || (remaining > 0 && (slotIndex + cols) < count))
                        down = cosmeticSlots[slotIndex + cols].GetSelectable();
                    else
                        down = null;

                    left = cosmeticSlots[slotIndex - 1].GetSelectable();
                    right = cosmeticSlots[slotIndex + 1].GetSelectable();
                    cosmeticSlots[slotIndex].SetNavigation(up, down, left, right);
                }

                if (i > 0)
                    up = cosmeticSlots[slotIndex - cols].GetSelectable();
                else
                    up = null;

                if ((i < rows - 1)
                    || (remaining > 0 && (slotIndex + cols) < count))
                    down = cosmeticSlots[slotIndex + cols].GetSelectable();
                else
                    down = null;

                left = cosmeticSlots[slotIndex - 1].GetSelectable();
                right = equipButton;
                cosmeticSlots[slotIndex].SetNavigation(up, down, left, right);
                slotIndex++;
            }

            if (remaining > 0)
            {
                down = null;

                if (rows > 0)
                    up = cosmeticSlots[slotIndex - cols].GetSelectable();
                else
                    up = null;

                left = null;
                right = remaining > 1
                    ? cosmeticSlots[slotIndex + 1].GetSelectable()
                    : equipButton;
                cosmeticSlots[slotIndex].SetNavigation(up, down, left, right);
                slotIndex++;

                for (int j = 1; j < remaining - 1; ++j, ++slotIndex)
                {
                    if (rows > 0)
                        up = cosmeticSlots[slotIndex - cols].GetSelectable();
                    else
                        up = null;

                    left = cosmeticSlots[slotIndex - 1].GetSelectable();
                    right = cosmeticSlots[slotIndex + 1].GetSelectable();
                    cosmeticSlots[slotIndex].SetNavigation(up, down, left, right);
                }

                if (remaining > 1)
                {
                    up = rows > 0
                        ? cosmeticSlots[slotIndex - cols].GetSelectable()
                        : null;
                    left = cosmeticSlots[slotIndex - 1].GetSelectable();
                    right = equipButton;
                    cosmeticSlots[slotIndex].SetNavigation(up, down, left, right);
                }
            }
        }

        public override void InitialFocus()
        {
            var cosmetics = suitcase.GetCosmetics();
            if (cosmetics.Count > 0)
            {
                Focus(cosmeticSlots[0].GetFocusable());
                OnSelected(cosmetics[0]);
            }
        }

        protected override void OnHide()
        {
            cosmeticSlots.ForEach((x) => x.Hide());

            noCosmeticSelected.enabled = false;
            selectedCosmeticName.text = null;
            selectedCosmeticDescription.text = null;
            selectedCosmeticIcon.sprite = null;
            selectedCosmeticIcon.enabled = false;

            unequipText.enabled = false;
            noCosmeticsMessage.enabled = false;

            equipButton.interactable = false;
            equipButton.onClick.RemoveAllListeners();
        }

        private void OnSelected(Cosmetic cosmetic)
        {
            if (cosmetic == null)
                return;

            noCosmeticSelected.enabled = false;
            equipButton.interactable = true;

            selectedCosmetic = cosmetic;
            selectedCosmeticName.text = cosmetic.GetCosmeticName();
            selectedCosmeticName.enabled = true;
            selectedCosmeticDescription.text = cosmetic.GetCosmeticDescription();
            selectedCosmeticDescription.enabled = true;
            selectedCosmeticIcon.sprite = cosmetic.GetPreviewSprite();
            selectedCosmeticIcon.enabled = true;

            CheckSelectedCosmeticEquiped();
        }

        private void OnEquip()
        {
            if (selectedCosmetic == null)
                return;

            if (!isSelectedCosmeticEquiped)
            {
                costumWearer.EquipCosmetic(selectedCosmetic);
            }
            else
            {
                costumWearer.UnequipCosmetic(selectedCosmetic);
            }

            CheckEquipedCosmetics();
        }

        private void CheckSelectedCosmeticEquiped()
        {
            isSelectedCosmeticEquiped = costumReference.IsThisCosmeticEquiped(selectedCosmetic);
            equipText.enabled = !isSelectedCosmeticEquiped;
            unequipText.enabled = isSelectedCosmeticEquiped;
        }

        private void CheckEquipedCosmetics()
        {
            CheckSelectedCosmeticEquiped();

            cosmeticSlots.ForEach(
                (slot) =>
                {
                    Cosmetic cosmetic = slot.GetCosmetic();
                    slot.SetEquiped(costumReference.IsThisCosmeticEquiped(cosmetic));
                }
            );
        }

    }

}