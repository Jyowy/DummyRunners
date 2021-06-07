using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class AudioManager : SingletonBehaviour<AudioManager>
    {

        [SerializeField]
        private AudioSource soundtrackAudioSource = null;

        [SerializeField]
        private AudioClip menuSoundtrack = null;
        [SerializeField]
        private AudioClip raceSoundtrack = null;
        [SerializeField]
        private AudioClip ballGameSoundtrack = null;

        [SerializeField]
        private AudioSource uiAudioSource = null;

        [SerializeField]
        private AudioClip onSelectedSound = null;
        [SerializeField]
        private AudioClip onClickSound = null;

        [SerializeField]
        private AudioClip onGoalSound = null;
        [SerializeField]
        private AudioClip explosionSound = null;
        [SerializeField]
        private AudioClip countdownSound = null;

        [SerializeField]
        private float clickSoundCooldown = 0.15f;
        [SerializeField]
        private float selectSoundCooldown = 0.1f;

        private TimedAction clickSound;
        private TimedAction selectSound;

        protected override void OnInstantiated()
        {
            soundtrackAudioSource.clip = null;
            soundtrackTimes.Add(menuSoundtrack, 0f);
            soundtrackTimes.Add(raceSoundtrack, 0f);
            soundtrackTimes.Add(ballGameSoundtrack, 0f);
        }

        public static void PlayAudio(AudioClip clip)
        {
            if (!(Instance is AudioManager instance)
                || clip == null)
                return;

            Debug.LogFormat("PlayAudio {0}", clip.name);
            instance.uiAudioSource.PlayOneShot(clip);
        }


        public static void MenuMusic()
        {
            if (!(Instance is AudioManager instance))
                return;

            instance.OnMenu();
        }

        public static void RaceMusic()
        {
            if (!(Instance is AudioManager instance))
                return;

            instance.OnRace();
        }

        public static void BallGameMusic(bool restart)
        {
            if (!(Instance is AudioManager instance))
                return;

            instance.OnBallGame(restart);
        }

        private Dictionary<AudioClip, float> soundtrackTimes = new Dictionary<AudioClip, float>();

        public static void StopMusic()
        {
            if (!(Instance is AudioManager instance))
                return;

            instance.OnStopMusic();
        }

        private void OnStopMusic()
        {
            if (soundtrackAudioSource.clip != null)
            {
                soundtrackTimes[soundtrackAudioSource.clip] = soundtrackAudioSource.time;
            }
            soundtrackAudioSource.Stop();
            soundtrackAudioSource.clip = null;
        }

        private void OnMenu()
        {
            SetSoundtrack(menuSoundtrack, false);
        }

        private void OnRace()
        {
            SetSoundtrack(raceSoundtrack);
        }

        private void OnBallGame(bool restart)
        {
            SetSoundtrack(ballGameSoundtrack, restart);
        }

        private void SetSoundtrack(AudioClip clip, bool restart = true)
        {
            if (soundtrackAudioSource.clip == clip
                && soundtrackAudioSource.isPlaying)
                return;

            if (soundtrackAudioSource.clip != null)
            {
                soundtrackTimes[soundtrackAudioSource.clip] = soundtrackAudioSource.time;
            }

            soundtrackAudioSource.clip = clip;
            soundtrackAudioSource.Play();
            if (!restart)
            {
                soundtrackAudioSource.time = soundtrackTimes[clip];
            }
            else
            {
                soundtrackAudioSource.time = 0;
            }
        }

        public static void PlayOnSelected()
        {
            if (!(Instance is AudioManager instance))
                return;

            instance.OnSelected();
        }

        public static void PlayOnClick()
        {
            if (!(Instance is AudioManager instance))
                return;

            instance.OnClick();
        }

        public static void PlayGoal()
        {
            if (!(Instance is AudioManager instance))
                return;

            AudioManager.PlayAudio(instance.onGoalSound);
            AudioManager.PlayAudio(instance.explosionSound);
        }

        public static void PlayCountdown()
        {
            if (!(Instance is AudioManager instance))
                return;

            AudioManager.PlayAudio(instance.countdownSound);
        }

        private void OnClick()
        {
            if (!clickSound.Completed)
                return;

            AudioManager.PlayAudio(onClickSound);
            clickSound.Set(clickSoundCooldown);
        }

        private void OnSelected()
        {
            if (!clickSound.Completed
                || !selectSound.Completed)
                return;
            AudioManager.PlayAudio(onSelectedSound);
            selectSound.Set(clickSoundCooldown);
        }

        private void Update()
        {
            if (!clickSound.Completed)
            {
                clickSound.Update(Time.unscaledDeltaTime);
            }
            if (!selectSound.Completed)
            {
                selectSound.Update(Time.unscaledDeltaTime);
            }
        }

    }

}