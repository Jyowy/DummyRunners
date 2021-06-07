using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    [CreateAssetMenu]
    public class PlayerDataPreset : ScriptableObject
    {

        public string playerName = "";
        public List<PlayerStadiumData> stadiumDatas = new List<PlayerStadiumData>();

    }

}