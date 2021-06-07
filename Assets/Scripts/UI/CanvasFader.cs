using System.Collections;
using UnityEngine;

namespace UI
{

    public class CanvasFader : MonoBehaviour
    {

        [SerializeField]
        private Canvas canvas = null;
        [SerializeField]
        private CanvasGroup canvasGroup = null;
        [SerializeField]
        private bool startVisible = false;

        [SerializeField]
        private float fadeInDuration = 1f;
        [SerializeField]
        private float fadeOutDuration = 1f;
        [SerializeField]
        private float holdFadeInDuration = 0f;

        private void Awake()
        {
            if (startVisible)
            {
                canvas.enabled = true;
                canvasGroup.alpha = 1f;
            }
            else
            {
                canvas.enabled = false;
                canvasGroup.alpha = 0f;
            }
        }

        public bool Finished => !busy;

        private bool busy = false;

        private void StartProcess()
        {
            busy = true;
        }

        private void FinishProcess()
        {
            busy = false;
        }

        public void FadeIn(System.Action onFinished = null)
        {
            if (busy)
                return;

            StartProcess();
            StartCoroutine(FadeInImplementation(onFinished));
        }

        public void FadeOut(System.Action onFinished = null)
        {
            Debug.LogFormat("FadeOut (onFinished is null? {0}). Is busy? {1}",
                onFinished == null, busy);
            if (busy)
                return;

            StartProcess();
            StartCoroutine(FadeOutImplementation(onFinished));
        }

        public void FadeInOut(System.Action onFinished = null)
        {
            if (busy)
                return;

            StartProcess();
            StartCoroutine(FadeInOutImplementation(onFinished));
        }

        private IEnumerator FadeInImplementation(System.Action onFinished = null)
        {
            canvas.enabled = true;

            float time = fadeInDuration * canvasGroup.alpha;
            while (time < fadeInDuration)
            {
                if (canvasGroup == null)
                    yield break;

                canvasGroup.alpha = time / fadeInDuration;

                yield return null;
                time += Time.unscaledDeltaTime;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            FinishProcess();

            onFinished?.Invoke();
        }

        private IEnumerator FadeOutImplementation(System.Action onFinished = null)
        {
            canvasGroup.interactable = false;

            float time = fadeOutDuration * (1f - canvasGroup.alpha);
            while (time < fadeOutDuration)
            {
                if (canvasGroup == null)
                    yield break;

                canvasGroup.alpha = 1f - time / fadeOutDuration;

                yield return null;
                time += Time.unscaledDeltaTime;
            }

            canvasGroup.alpha = 0f;
            canvas.enabled = false;

            FinishProcess();

            onFinished?.Invoke();
        }

        private IEnumerator FadeInOutImplementation(System.Action onFinished = null)
        {
            yield return FadeInImplementation();
            StartProcess();
            yield return new WaitForSeconds(holdFadeInDuration);
            yield return FadeOutImplementation(onFinished);
        }

    }

}