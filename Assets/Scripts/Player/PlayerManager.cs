using Common;
using World.Quests;
using World;
using Player.Skills;
using UnityEngine;
using World.Rewards;
using UI.Menus;
using World.Level;

namespace Player
{

    public class PlayerManager : SingletonBehaviour<PlayerManager>
    {

        [SerializeField]
        private Player playerPrefab = null;

        [SerializeField]
        private EnergyBatteryUI batteryUI = null;
        [SerializeField]
        private UnlockSkillSplash skillUnlockedSplash = null;

        private Player playerInstance = null;
        private PlayerDataManager dataManager = null;

        public static Player GetPlayerInstance()
        {
            if (!(Instance is PlayerManager instance))
                return null;

            return instance.playerInstance;
        }

        public static void InstantiatePlayer(Transform root, SpawnPoint spawnPoint)
        {
            if (!(Instance is PlayerManager instance))
                return;

            instance.InstantiatePlayerImpl(root, spawnPoint.transform.position, spawnPoint.FaceRight);

        }

        private void InstantiatePlayerImpl(Transform root, Vector2 position, bool faceRight)
        {
            if (playerInstance != null)
            {
                DestroyImmediate(playerInstance.gameObject);
                playerInstance = null;
                dataManager = null;
            }

            playerInstance = Instantiate(playerPrefab, position, Quaternion.identity, root);
            //playerInstance.FaceRight(faceRight);

            var camera = WorldManager.GetPlayerCamera();
            camera.Follow = playerInstance.transform;
        }

        public static bool HasQuest(Quest quest)
        {
            if (!(Instance is PlayerManager instance))
                return false;

            return instance.dataManager.GetJournal().HasQuest(quest);
        }

        public static bool IsQuestActive(Quest quest)
        {
            if (!(Instance is PlayerManager instance))
                return false;

            return instance.dataManager.GetJournal().IsQuestActive(quest);
        }

        public static bool  HasCompletedQuest(Quest quest)
        {
            if (!(Instance is PlayerManager instance))
                return false;

            return instance.dataManager.GetJournal().HasCompletedQuest(quest);
        }

        public static void GiveQuest(Quest quest)
        {
            if (!(Instance is PlayerManager instance))
                return;

            instance.dataManager.GetJournal().AddQuest(quest);
        }

        public static void CompleteQuest(Quest quest)
        {
            if (!(Instance is PlayerManager instance))
                return;

            instance.dataManager.GetJournal().CompleteQuest(quest);
        }

        public static void GetReward(Reward reward)
        {
            if (!(Instance is PlayerManager instance))
                return;

            instance.dataManager.GetReward(reward);
        }

        public static void ShowSkillUnlockedSplash(Skill skill)
        {
            if (!(Instance is PlayerManager instance))
                return;

            instance.skillUnlockedSplash.Show(skill);
        }

        public static void SetPosition(SpawnPoint spawnPoint)
        {
            if (!(Instance is PlayerManager instance))
                return;

            instance.playerInstance.transform.position = spawnPoint.transform.position;
            //instance.playerInstance.FaceRight(spawnPoint.FaceRight);
        }

    }

}