using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Menus
{

    public class MenuPage : MonoBehaviour
    {

        [SerializeField]
        private string pageName = "Menu";
        [SerializeField]
        private Canvas canvas = null;

        public string GetPageName() => pageName;

        public void Show()
        {
            if (canvas.enabled)
                return;

            canvas.enabled = true;
            OnShow();
        }

        protected virtual void OnShow() { }

        public void Hide()
        {
            if (!canvas.enabled)
                return;

            canvas.enabled = false;
            OnHide();
        }

        public virtual void InitialFocus() { }

        protected virtual void OnHide() { }

        protected void Focus(GameObject element)
        {
            EventSystem.current.SetSelectedGameObject(element);
        }

    }

}