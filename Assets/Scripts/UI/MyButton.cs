using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{

    [RequireComponent(typeof(Button))]
    public class MyButton : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler
    {

        public void OnSelect(BaseEventData eventData)
        {
            AudioManager.PlayOnSelected();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            AudioManager.PlayOnClick();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.PlayOnClick();
        }
    }

}