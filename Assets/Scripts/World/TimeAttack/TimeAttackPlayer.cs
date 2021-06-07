using Common;
using Player;
using System.Collections.Generic;
using UI;
using UI.Menus;
using UnityEngine;
using World.Level;

namespace World.TimeAttacks
{

    public class TimeAttackPlayer : SingletonBehaviour<TimeAttackPlayer>
    {

        [SerializeField]
        private TimeAttackMenu timeAttackMenu = null;

        [SerializeField]
        private CanvasFader screenFader = null;

        [SerializeField]
        private CountdownSplash initialCountdownSplash = null;
        [SerializeField]
        private float initialCountdownDuration = 4f;

        [SerializeField]
        private TimeAttackUI ui = null;
        [SerializeField]
        private TimeAttackRoutePoint nextRoutePoint = null;
        [SerializeField]
        private TimeAttackPauseMenu pauseMenu = null;

        private float currentTime = 0f;

        private TimeAttack timeAttack = null;
        private TimeAttackData data = null;
        private List<Vector2> routePoints = null;
        private AreaLimits timeAttackLimits = null;

        private int nextPoint = 0;

        protected override void OnInstantiated()
        {
            enabled = false;
        }

        public static bool IsTimeAttackPlaying()
            => (Instance is TimeAttackPlayer instance)
                && instance.timeAttack != null;

        public static float GetCurrentTime()
        {
            if (!(Instance is TimeAttackPlayer instance))
                return 0f;

            return instance.currentTime;
        }

        public static void ShowTimeAttack(TimeAttack timeAttack)
        {
            Debug.LogFormat("Show TimeAttack");
            if (!(Instance is TimeAttackPlayer instance))
                return;

            instance.ShowTimeAttack_Impl(timeAttack);
        }

        private void ShowTimeAttack_Impl(TimeAttack timeAttack)
        {
            Debug.LogFormat("ShowTimeAttack_Impl");
            enabled = false;

            this.timeAttack = timeAttack;

            data = timeAttack.GetData();
            routePoints = timeAttack.GetRoutePoints();
            timeAttackLimits = timeAttack.GetLimits();
            timeAttackMenu.Show(data, InitializeTimeAttack, ExitTimeAttack);
        }

        private void InitializeTimeAttack()
        {
            Debug.LogFormat("Initialize Time Attack");

            enabled = false;
            currentTime = 0f;
            nextPoint = 0;
            screenFader.FadeIn(PutPlayerInStartPosition);
        }

        private void PutPlayerInStartPosition()
        {
            Debug.LogFormat("Put player in start position");
            PlayerManager.SetPosition(timeAttack.GetSpawnPoint());
            screenFader.FadeOut(StartCountdown);
        }

        private void StartCountdown()
        {
            Debug.LogFormat("Start countdown");
            initialCountdownSplash.Show(initialCountdownDuration, StartTimeAttack);
        }

        private void StartTimeAttack()
        {
            Debug.LogFormat("Start Time Attack");
            enabled = true;

            nextRoutePoint.Activate(OnPointReached);
            ui.Show(data);

            timeAttackLimits.Activate(AbortTimeAttack);
            SetNextPoint();
        }

        public static void PauseTimeAttack()
        {
            Debug.LogFormat("Pause Time Attack");
            if (!(Instance is TimeAttackPlayer instance))
                return;

            instance.PauseTimeAttack_Impl();
        }
        private void PauseTimeAttack_Impl()
        {
            Debug.LogFormat("Pause Time Attack Impl");
            pauseMenu.Show(RetryTimeAttack, AbortTimeAttack);
        }

        private void RetryTimeAttack()
        {
            Debug.LogFormat("Retry Time Attack");
            InitializeTimeAttack();
        }

        public static void ExitTimeAttack()
        {
            Debug.LogFormat("Exit Time Attack");
            if (!(Instance is TimeAttackPlayer instance))
                return;

            instance.ExitTimeAttack_Impl();
        }
        private void ExitTimeAttack_Impl()
        {
            Debug.LogFormat("Exit Time Attack Impl");
            enabled = false;

            timeAttack = null;
            data = null;
            routePoints = null;
            timeAttackLimits = null;
            currentTime = 0f;
            nextPoint = 0;
        }

        private void Update()
        {
            currentTime += Time.deltaTime;
        }

        private void OnPointReached()
        {
            Debug.LogFormat("On Point Reached");

            if (nextPoint < routePoints.Count - 1)
            {
                nextPoint++;
                SetNextPoint();
            }
            else
            {
                FinishTimeAttack();
            }
        }

        private void SetNextPoint()
        {
            Debug.LogFormat("Set Next Point");
            nextRoutePoint.SetNextPoint(routePoints[nextPoint]);
        }

        private void FinishTimeAttack()
        {
            Debug.LogFormat("Finish Time Attack");
            float currentTime = this.currentTime;
            OnTimeAttackFinished();
            timeAttackMenu.ShowTimeAttackFinished(data, currentTime, RetryTimeAttack, ExitTimeAttack);
        }

        private void AbortTimeAttack()
        {
            Debug.LogFormat("Abort Time Attack");
            OnTimeAttackFinished();
            timeAttackMenu.Show(data, RetryTimeAttack, ExitTimeAttack);
        }

        private void OnTimeAttackFinished()
        {
            Debug.LogFormat("On Time Attack Finished");
            ui.Hide();
            enabled = false;
            nextRoutePoint.Deactivate();
            timeAttackLimits.Deactivate();
        }

    }

}