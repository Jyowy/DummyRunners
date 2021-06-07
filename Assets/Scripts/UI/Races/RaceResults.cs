using Common;
using Stadiums;
using Stadiums.Races;
using TMPro;
using UnityEngine;

namespace UI.Menus
{

    public class RaceResults : ResultsMenu
    {

        [SerializeField]
        private TextMeshProUGUI finalPosition = null;

        [SerializeField]
        private RaceResultSlotUI runnerResultTemplate = null;
        [SerializeField]
        private Transform runnerResultsRoot = null;

        private bool AllRunnersResult => currentResults == totalRunners;
        private int currentResults = 0;
        private int totalRunners = 0;

        protected override void OnShow()
        {
            base.OnShow();

            var finalOrder = RaceController.GetRunnersOrder();
            var goalTimes = RaceController.GetGoalTimes();

            totalRunners = finalOrder.Count;
            currentResults = goalTimes.Count;

            var player = LevelManager.GetPlayer();

            for (int i = 0; i < currentResults; ++i)
            {
                var runner = finalOrder[i];
                RaceResultSlotUI ui = GameObject.Instantiate(runnerResultTemplate, runnerResultsRoot.transform);
                int position = i + 1;
                string name = runner.name;
                float time = goalTimes[i];
                bool isPlayer = runner == player.Runner;
                ui.Show(position, name, time, isPlayer);

                if (isPlayer)
                {
                    finalPosition.text = GameUtils.GetPositionFormatted(position);
                }
            }
        }

        protected override void OnHide()
        {
            base.OnHide();

            totalRunners = 0;
            currentResults = 0;

            int count = runnerResultsRoot.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                var child = runnerResultsRoot.transform.GetChild(i);
                child.GetComponent<RaceResultSlotUI>().Hide();
                GameObject.Destroy(child.gameObject, 0.1f);
            }
            runnerResultsRoot.transform.DetachChildren();
        }

        private void Update()
        {
            if (!AllRunnersResult)
            {
                var goalTimes = RaceController.GetGoalTimes();
                int newResults = goalTimes.Count;

                if (newResults > currentResults)
                {
                    var finalOrder = RaceController.GetRunnersOrder();

                    for (int i = currentResults; i < newResults; ++i)
                    {
                        RaceResultSlotUI ui = GameObject.Instantiate(runnerResultTemplate, runnerResultsRoot.transform);
                        int position = i + 1;
                        string name = finalOrder[i].name;
                        float time = goalTimes[i];
                        ui.Show(position, name, time, false);
                    }

                    currentResults = newResults;
                }
            }
        }

    }

}