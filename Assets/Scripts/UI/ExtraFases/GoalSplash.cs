using Common;
using Stadiums.ExtraFases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class GoalSplash : Menu
    {

        [SerializeField]
        private TextMeshProUGUI goalTime = null;
        [SerializeField]
        private GameObject goldGoal = null;

        [SerializeField]
        private Image backgroundImage = null;
        [SerializeField]
        private GameObject goalImage = null;

        [SerializeField]
        private TextMeshProUGUI teamAScore = null;
        [SerializeField]
        private TextMeshProUGUI teamBScore = null;

        [SerializeField]
        private float teamGoalFontSize = 85f;
        [SerializeField]
        private float otherTeamFontSize = 65f;
        [SerializeField]
        private Color ownColor = Color.cyan;
        [SerializeField]
        private Color rivalColor = Color.magenta;

        public void Show(int teamWhoMarked, int playerTeam)
        {
            goalImage.SetActive(true);

            float time = GoalBallController.GetCurrentTime();
            goalTime.text = GameUtils.GetTimeFormatted(time);
            goalTime.enabled = time > 0f;
            goldGoal.SetActive(time == 0f);

            GoalBallController.GetCurrentScore(out int ascore, out int bscore);
            teamAScore.text = ascore.ToString();
            teamBScore.text = bscore.ToString();

            teamAScore.fontSize = teamWhoMarked == 0
                ? teamGoalFontSize
                : otherTeamFontSize;
            teamBScore.fontSize = teamWhoMarked == 1
                ? teamGoalFontSize
                : otherTeamFontSize;

            teamAScore.color = playerTeam == 0
                ? ownColor
                : rivalColor;
            teamBScore.color = playerTeam == 1
                ? ownColor
                : rivalColor;

            backgroundImage.color = teamWhoMarked == playerTeam
                ? ownColor
                : rivalColor;

            Show();
        }

        protected override void OnHide()
        {
            goalImage.SetActive(false);
        }

    }

}