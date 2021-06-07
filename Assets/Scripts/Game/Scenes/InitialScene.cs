using System.Collections;
using UI;
using UnityEngine;

namespace Game
{

    public class InitialScene : MonoBehaviour
    {

        [SerializeField]
        private CanvasFader splash = null;

        private bool initializing = false;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            if (initializing)
                yield break;

            initializing = true;
            splash.enabled = true;

            splash.FadeInOut();
            while (!splash.Finished)
                yield return null;

            GameManager.GoToTitleMenu();

            Destroy(this, 0.5f);
        }

    }

}
