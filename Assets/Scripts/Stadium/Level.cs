using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Stadiums
{

    public class Level : MonoBehaviour
    {

        [SerializeField]
        private List<SpawnPoint> startPoints = null;

        [SerializeField]
        private PlayableDirector intro = null;

        private System.Action onIntroFinished = null;

        public List<SpawnPoint> GetStartPoints() => startPoints;

        private bool isPlaying = false;

        private InputAction skipIntroAction = null;

        public void PlayIntro(System.Action onFinished)
        {
            if (isPlaying)
                return;

            isPlaying = true;

            skipIntroAction = new InputAction(binding: "/*/<button>");
            skipIntroAction.performed += (_) => intro.time = intro.duration;
            skipIntroAction.Enable();

            this.onIntroFinished = onFinished;
            intro.stopped += IntroFinished;
            intro.enabled = true;
            intro.timeUpdateMode = DirectorUpdateMode.Manual;
            intro.Play();
        }

        private void IntroFinished(PlayableDirector _)
        {
            if (!isPlaying)
                return;

            isPlaying = false;

            skipIntroAction.Dispose();
            skipIntroAction = null;
            intro.stopped -= IntroFinished;
            var onFinished = this.onIntroFinished;
            intro.enabled = false;
            this.onIntroFinished = null;

            onFinished?.Invoke();
        }

        private void FixedUpdate()
        {
            if (!isPlaying)
                return;

            intro.time += Time.fixedDeltaTime;
            intro.Evaluate();
            if (intro.time >= intro.duration)
            {
                IntroFinished(null);
            }
        }

    }

}