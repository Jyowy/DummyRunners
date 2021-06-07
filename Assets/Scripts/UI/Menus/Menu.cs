using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.Menus
{

    public class Menu : MonoBehaviour
    {

        private static readonly float showMenuDelay = 0.1f;

        [SerializeField]
        private Canvas canvas = null;
        [SerializeField]
        private bool isInteractive = true;
        [SerializeField]
        private bool startVisible = false;
        [SerializeField]
        private bool stopGameTime = false;

        [SerializeField]
        private GameObject defaultElement = null;

        public bool IsVisible() => canvas.enabled;

        private void Awake()
        {
            OnAwake();

            if (startVisible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void OnDestroy()
        {
            Hide();
        }

        protected virtual void OnAwake() { }

        private void OnEnable()
        {
            if (isInteractive)
            {
                EnableInput();
            }
        }

        private void OnDisable()
        {
            if (isInteractive)
            {
                DisableInput();
            }
        }

        public void Show()
        {
            //Debug.LogFormat("Show menu {0}", name);

            if (stopGameTime)
                GameManager.StopGameTime();

            if (isInteractive)
            {
                EnableInput();
            }

            enabled = true;
            StartCoroutine(Show_Impl());
        }

        private IEnumerator Show_Impl()
        {
            yield return new WaitForSecondsRealtime(showMenuDelay);

            canvas.enabled = true;
            OnShow();
            if (isInteractive)
            {
                InitialFocus();
            }
        }

        protected virtual void InitialFocus()
        {
            Debug.LogFormat("Select default element {0}",
                defaultElement != null ? defaultElement.name : "<nothing>");
            Focus(defaultElement);
        }

        protected void Focus(GameObject element)
        {
            EventSystem.current.SetSelectedGameObject(element);
        }

        protected virtual void OnShow() { }

        public void Hide()
        {
            canvas.enabled = false;
            enabled = false;
            prevGameObjectSelected = null;

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);

            if (stopGameTime)
                GameManager.ResumeGameTime();

            if (isInteractive)
            {
                DisableInput();
            }

            OnHide();
        }

        protected virtual void OnHide() { }

        private void EnableInput()
        {
            //Debug.LogFormat("Enable Input {0}", name);
            GameManager.SetMenuInputEnable(gameObject, true);
            var menuMap = GameManager.GetMenuActionMap();
            if (menuMap != null)
            {
                menuMap.FindAction("Navigation").performed += OnNavigation;
                menuMap.FindAction("Accept").performed += OnAccept;
                menuMap.FindAction("Cancel").performed += OnCancel;
                menuMap.FindAction("LeftPage").performed += OnLeftPage;
                menuMap.FindAction("RightPage").performed += OnRightPage;
            }
        }

        private void DisableInput()
        {
            //Debug.LogFormat("Disable Input {0}", name);
            GameManager.SetMenuInputEnable(gameObject, false);
            var menuMap = GameManager.GetMenuActionMap();
            if (menuMap != null)
            {
                menuMap.FindAction("Navigation").performed -= OnNavigation;
                menuMap.FindAction("Accept").performed -= OnAccept;
                menuMap.FindAction("Cancel").performed -= OnCancel;
                menuMap.FindAction("LeftPage").performed -= OnLeftPage;
                menuMap.FindAction("RightPage").performed -= OnRightPage;
            }
        }

        private void OnNavigation(InputAction.CallbackContext context)
        {
            OnNavigation_Impl(context.ReadValue<Vector2>());
        }
        protected virtual void OnNavigation_Impl(Vector2 navigation)
        {
            if (EventSystem.current.currentSelectedGameObject == null
                && GameManager.GetFocusedMenu() == gameObject)
            {
                InitialFocus();
            }
        }

        private void OnAccept(InputAction.CallbackContext context)
        {
            OnAccept();
        }
        protected virtual void OnAccept() { }

        private void OnCancel(InputAction.CallbackContext context)
        {
            OnCancel();
        }
        protected virtual void OnCancel() { }

        protected void OnLeftPage(InputAction.CallbackContext context)
        {
            OnLeftPage();
        }
        protected virtual void OnLeftPage() { }

        protected void OnRightPage(InputAction.CallbackContext context)
        {
            OnRightPage();
        }
        protected virtual void OnRightPage() { }

        private GameObject prevGameObjectSelected = null;

        private void Update()
        {
            if (!isInteractive)
                return;

            if (EventSystem.current.currentSelectedGameObject == null
                && prevGameObjectSelected != null
                && prevGameObjectSelected.activeInHierarchy)
            {
                Debug.LogFormat("Menu {0} resets to prev game object selected", name);
                EventSystem.current.SetSelectedGameObject(prevGameObjectSelected);
            }

            if (EventSystem.current.currentSelectedGameObject != null
                && EventSystem.current.currentSelectedGameObject.activeInHierarchy)
                prevGameObjectSelected = EventSystem.current.currentSelectedGameObject;
        }

    }

}