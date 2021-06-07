using Player;
using TMPro;
using UnityEngine;

namespace UI.Menus
{

    public class PlayerDataPage : MenuPage
    {

        [SerializeField]
        private TextMeshProUGUI playerName = null;
        [SerializeField]
        private TextMeshProUGUI playerMoney = null;

        protected override void OnShow()
        {
            //var dataManager = PlayerManager.GetPlayerInstance().GetDataManager();
            //var playerData = dataManager.GetData();
            //var wallet = dataManager.GetWallet();

            //playerName.text = playerData.playerName;

            //playerMoney.text = wallet.FormattedMoney;
        }

    }

}