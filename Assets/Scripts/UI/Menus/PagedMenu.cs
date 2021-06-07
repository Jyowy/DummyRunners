using Common;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace UI.Menus
{

    public class PagedMenu : Menu
    {

        [SerializeField]
        private TextMeshProUGUI pageTitle = null;

        [SerializeField]
        private List<MenuPage> pages = new List<MenuPage>();

        [SerializeField]
        private int defaultPageIndex = 0;
        [SerializeField]
        private float movePageCooldown = 0.25f;

        private MenuPage currentPage = null;
        private int currentPageIndex = 0;

        private TimedAction cooldown;

        private void Awake()
        {
            currentPageIndex = math.max(defaultPageIndex, 0);
            HidePages();
        }

        protected override void OnShow()
        {
            OpenPage(currentPageIndex);
        }

        protected override void OnHide()
        {
            HidePages();
        }

        private void OpenPage(int index)
        {
            Debug.LogFormat("Open page with index {0}", index);
            index = math.clamp(index, 0, pages.Count - 1);

            if (currentPageIndex != index)
            {
                currentPage.Hide();
            }

            currentPageIndex = index;
            currentPage = pages[currentPageIndex];
            pageTitle.text = currentPage.GetPageName();
            currentPage.Show();

            currentPage.InitialFocus();
        }

        private void HidePages()
        {
            pages.ForEach((x) => x.Hide());
        }

        protected override void OnLeftPage()
        {
            if (!cooldown.Completed)
                return;

            int newPageIndex = (int)Mathf.Repeat(currentPageIndex - 1, pages.Count);
            OpenPage(newPageIndex);

            cooldown.Set(movePageCooldown);
        }

        protected override void OnRightPage()
        {
            if (!cooldown.Completed)
                return;

            int newPageIndex = (int)Mathf.Repeat(currentPageIndex + 1, pages.Count);
            OpenPage(newPageIndex);

            cooldown.Set(movePageCooldown);
        }

        protected override void InitialFocus()
        {
            currentPage.InitialFocus();
        }

        private void Update()
        {
            if (cooldown.Completed)
                return;

            cooldown.Update(Time.deltaTime);
        }

    }

}