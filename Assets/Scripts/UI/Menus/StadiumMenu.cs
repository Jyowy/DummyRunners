using Common;
using Game;
using Stadiums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{

    public class StadiumMenu : Menu
    {

        [SerializeField]
        private StadiumSelectionMenu selectionMenu = null;

        [SerializeField]
        private StadiumData stadiumData = null;
        [SerializeField]
        private Transform levelSlotsRoot = null;

        [SerializeField]
        private GameObject playerPositionRoot = null;
        [SerializeField]
        private TextMeshProUGUI playerPosition = null;
        [SerializeField]
        private GameObject levelWon = null;

        [SerializeField]
        private Button startLevelButton = null;
        [SerializeField]
        private Button exitStadiumButton = null;

        [SerializeField]
        private Color selectedColor = Color.yellow;
        [SerializeField]
        private Color nonSelectedColor = Color.white;

        private readonly List<LevelSlot> levelSlots = new List<LevelSlot>();

        private LevelSlot selectedSlot = null;

        public StadiumData GetStadiumData()
            => stadiumData;

        private LevelData selectedLevelData = null;

        protected override void OnAwake()
        {
            int childCount = levelSlotsRoot.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                var child = levelSlotsRoot.GetChild(i);
                if (child == null
                    || !child.TryGetComponent(out LevelSlot slot))
                    continue;

                levelSlots.Add(slot);
            }

            startLevelButton.interactable = false;
        }

        protected override void OnShow()
        {
            levelSlots.ForEach((slot)
                =>
                {
                    slot.Show(OnLevelSelected);
                    slot.SetColor(nonSelectedColor);
                }
            );

            startLevelButton.onClick.AddListener(OnStartLevel);
            exitStadiumButton.onClick.AddListener(OnExit);
        }

        protected override void InitialFocus()
        {
            if (levelSlots.Count == 0)
            {
                Focus(exitStadiumButton.gameObject);
                return;
            }

            Focus(levelSlots[0].GetButton().gameObject);
            levelSlots[0].Select();
        }

        protected override void OnHide()
        {
            selectedLevelData = null;

            playerPositionRoot.SetActive(false);
            levelWon.SetActive(false);

            startLevelButton.interactable = false;
            startLevelButton.onClick.RemoveAllListeners();
            exitStadiumButton.onClick.RemoveAllListeners();
        }

        private void OnLevelSelected(LevelSlot levelSlot)
        {
            if (levelSlot == null)
                return;

            if (selectedSlot != null)
                selectedSlot.SetColor(nonSelectedColor);

            levelSlot.SetColor(selectedColor);
            selectedSlot = levelSlot;

            var levelData = levelSlot.GetData();
            selectedLevelData = levelData;
            startLevelButton.interactable = true;

            var playerLevelData = GameDataManager.GetPlayerLevelData(stadiumData.stadiumId, levelData.levelId);
            if (levelData.type == LevelType.Race)
            {
                if (playerLevelData == null)
                {
                    playerPositionRoot.SetActive(false);
                    levelWon.SetActive(false);

                    playerPosition.text = "";
                }
                else
                {
                    int position = playerLevelData.playerPosition;
                    playerPosition.text = GameUtils.GetPositionFormatted(position);

                    playerPositionRoot.SetActive(true);
                    levelWon.SetActive(position == 1);
                }
            }
            else if (levelData.type == LevelType.GoalBallGame)
            {
                playerPositionRoot.SetActive(false);
                levelWon.SetActive(
                    playerLevelData != null
                    && playerLevelData.goalBallWon
                );
            }

            Focus(startLevelButton.gameObject);
        }

        private void OnStartLevel()
        {
            LevelData data = selectedLevelData;
            if (data == null)
                return;

            LevelManager.LoadLevel(stadiumData.stadiumId, data.levelId, data.type);
            AudioManager.StopMusic();
            Hide();
            selectionMenu.Hide();
            SceneLoader.LoadLevel(data.levelSceneName);
        }

        private void OnExit()
        {
            Hide();
            selectionMenu.enabled = true;
        }

    }

}