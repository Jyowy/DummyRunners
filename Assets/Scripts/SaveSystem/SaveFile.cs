using System.Runtime.Serialization;
using Player;

namespace SaveSystem
{

    [System.Serializable]
    public class SaveFile
    {

        public bool IsValid { get; private set; } = false;

        public int saveSlotIndex = 0;
        public string fileName = "Jade";
        public PlayerData playerData = new PlayerData();

        public SaveFile() { }

        public SaveFile(SaveFile other)
        {
            IsValid = other.IsValid;

            saveSlotIndex = other.saveSlotIndex;
            fileName = other.fileName;
            playerData = new PlayerData(other.playerData);
        }

        public SaveFile(SaveFilePreset preset)
        {
            IsValid = true;

            saveSlotIndex = preset.saveSlot;
            fileName = preset.fileName;
            playerData = new PlayerData(preset.playerData);
        }

#if UNITY_EDITOR
        // Only for development (right?)
        [OnDeserialized]
        private void OnDeserialized()
        {
            if (fileName == null)
                fileName = "NoName";
            if (playerData == null)
                playerData = new PlayerData();
            else
                playerData.Check();

            IsValid = true;
        }
#endif

    }

}