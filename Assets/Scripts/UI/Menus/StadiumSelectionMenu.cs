using Game;
using System.Collections.Generic;
using UI.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace Stadiums
{

    public class StadiumSelectionMenu : Menu
    {

        [SerializeField]
        private Transform stadiumSlotsRoot = null;

        [SerializeField]
        private Button goToTitleMenuButton = null;

        private readonly List<StadiumSlot> stadiumSlots = new List<StadiumSlot>();

        protected override void OnAwake()
        {
            int childCount = stadiumSlotsRoot.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                var child = stadiumSlotsRoot.GetChild(i);
                if (child == null
                    || !child.TryGetComponent(out StadiumSlot slot))
                    continue;

                stadiumSlots.Add(slot);
            }
        }

        protected override void OnShow()
        {
            stadiumSlots.ForEach((x) => x.Show(OnStadiumSelected));
            goToTitleMenuButton.onClick.AddListener(OnGoToTitleMenu);

            AudioManager.MenuMusic();
        }

        protected override void InitialFocus()
        {
            if (stadiumSlots.Count == 0)
            {
                Focus(goToTitleMenuButton.gameObject);
                return;
            }

            var stadiumSlot = stadiumSlots[0];
            Focus(stadiumSlot.GetButton().gameObject);
            stadiumSlot.Select();
        }

        protected override void OnHide()
        {
            stadiumSlots.ForEach((x) => x.Hide());
            goToTitleMenuButton.onClick.RemoveAllListeners();
        }

        private void OnStadiumSelected(StadiumMenu stadiumMenu)
        {
            enabled = false;
            stadiumMenu.Show();
        }

        private void OnGoToTitleMenu()
        {
            Hide();
            GameManager.GoToTitleMenu();
        }

    }

}