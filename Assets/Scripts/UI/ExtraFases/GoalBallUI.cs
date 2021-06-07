using Common;
using Game;
using Stadiums.ExtraFases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class GoalBallUI : Menu
    {

        [SerializeField]
        private TextMeshProUGUI teamAScore = null;
        [SerializeField]
        private TextMeshProUGUI teamBScore = null;
        [SerializeField]
        private Image teamAFrame = null;
        [SerializeField]
        private Image teamBFrame = null;
        [SerializeField]
        private Color ownTeamColor = Color.cyan;
        [SerializeField]
        private Color rivalTeamColor = Color.magenta;

        [SerializeField]
        private TextMeshProUGUI remainingTime = null;
        [SerializeField]
        private float lastSeconds = 10f;
        [SerializeField]
        private Color normalTimeColor = Color.white;
        [SerializeField]
        private Color goldTimeColor = Color.red;

        protected override void OnShow()
        {
            enabled = true;

            int playerTeam = GoalBallController.GetPlayerTeam();
            teamAFrame.color = playerTeam == 0
                ? ownTeamColor
                : rivalTeamColor;
            teamBFrame.color = playerTeam == 1
                ? ownTeamColor
                : rivalTeamColor;

            UpdateUI();
        }

        protected override void OnHide()
        {
            teamAScore.text = "";
            teamBScore.text = "";
            remainingTime.text = "";

            enabled = false;
        }

        private void FixedUpdate()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            float time = GoalBallController.GetCurrentTime();
            GoalBallController.GetCurrentScore(out int ascore, out int bscore);

            teamAScore.text = ascore.ToString();
            teamBScore.text = bscore.ToString();
            remainingTime.text = GameUtils.GetTimeFormatted(time, false);
            remainingTime.color = time > lastSeconds
                ? normalTimeColor
                : goldTimeColor;
        }

    }

}