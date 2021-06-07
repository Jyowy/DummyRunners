using System.Collections.Generic;
using Cinemachine;
using Common;
using Player.Cosmetics;
using UI.Menus;
using UnityEngine;
using World.TimeAttacks;

namespace World
{

    public class WorldManager : SingletonBehaviour<WorldManager>
    {

        [SerializeField]
        private PauseMenu pauseMenu = null;
        [SerializeField]
        private PlayerMenu playerMenu = null;
        [SerializeField]
        private CosmeticShop cosmeticShop = null;
        [SerializeField]
        private new CinemachineVirtualCamera camera = null;

        public static CinemachineVirtualCamera GetPlayerCamera() =>
            Instance is WorldManager instance
            ? instance.camera
            : null;

        public static void ShowPauseMenu()
        {
            if (!(Instance is WorldManager instance))
                return;

            if (TimeAttackPlayer.IsTimeAttackPlaying())
            {
                TimeAttackPlayer.PauseTimeAttack();
            }
            else
            {
                instance.pauseMenu.Show();
            }
        }

        public static void ShowPlayerMenu()
        {
            if (!(Instance is WorldManager instance)
                || TimeAttackPlayer.IsTimeAttackPlaying())
                return;

            instance.playerMenu.Show();
        }

        public static void ShowCosmeticShop(List<Cosmetic> catalog, System.Action onClose = null)
        {
            if (!(Instance is WorldManager instance))
                return;

            instance.cosmeticShop.Show(catalog, onClose);
        }

    }

}