using Stadiums.ExtraFases;
using TMPro;
using UnityEngine;

namespace UI.Menus
{

    public class GoalBallResults : ResultsMenu
    {

        [SerializeField]
        private TextMeshProUGUI teamAScore = null;
        [SerializeField]
        private TextMeshProUGUI teamBScore = null;

        [SerializeField]
        private Color ownScoreColor = Color.cyan;
        [SerializeField]
        private Color rivalScoreColor = Color.yellow;

        [SerializeField]
        private GameObject teamAWinner = null;
        [SerializeField]
        private GameObject teamBWinner = null;

        protected override void OnShow()
        {
            base.OnShow();

            GoalBallController.GetCurrentScore(out int ascore, out int bscore);
            teamAScore.text = ascore.ToString();
            teamBScore.text = bscore.ToString();

            int playerTeam = GoalBallController.GetPlayerTeam();
            teamAScore.color = playerTeam == 0 ? ownScoreColor : rivalScoreColor;
            teamBScore.color = playerTeam == 1 ? ownScoreColor : rivalScoreColor;

            teamAWinner.SetActive(ascore > bscore);
            teamBWinner.SetActive(bscore > ascore);

            teamAWinner.GetComponentInChildren<TextMeshProUGUI>().color = teamAScore.color;
            teamBWinner.GetComponentInChildren<TextMeshProUGUI>().color = teamBScore.color;
        }

        protected override void OnHide()
        {
            base.OnHide();

            teamAScore.text = "";
            teamBScore.text = "";
            teamAWinner.SetActive(false);
            teamBWinner.SetActive(false);
        }

    }

}