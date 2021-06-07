using Player;
using UnityEngine;

namespace SaveSystem
{

    [CreateAssetMenu]
    public class SaveFilePreset : ScriptableObject
    {

        public int saveSlot = 0;
        public string fileName = "";
        public PlayerData playerData = null;

    }

}