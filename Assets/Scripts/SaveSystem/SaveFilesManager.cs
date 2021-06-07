using Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveSystem
{

    public static class SaveFilesManager
    {

        private static readonly string folder = "/Data/SaveFiles";
        private static readonly string extension = "jjsf";

        private static string Path => Application.dataPath + folder;

        private static string GetFilePath(string fileName)
            => Path + "/" + fileName + "." + extension;

        private readonly static List<SaveFile> saveFilesCache = new List<SaveFile>();
        private static bool initialized = false;

        public static List<SaveFile> GetSaveFiles()
        {
            if (!initialized)
            {
                Initialize();
            }

            return saveFilesCache;
        }

        public static void Initialize()
        {
            if (!CheckDirectory())
                return;

            saveFilesCache.Clear();
            var files = Directory.GetFiles(Path, string.Format("*.{0}", extension));
            int count = files != null ? files.Length : 0;
            for (int i = 0; i < count; ++i)
            {
                bool success;
                SaveFile saveFile;

                string filePath = files[i];

                try
                {
                    var file = File.OpenRead(filePath);
                    if (file == null)
                        continue;

                    BinaryFormatter bf = new BinaryFormatter();
                    saveFile = (SaveFile)bf.Deserialize(file);
                    success = saveFile.IsValid;

                    file.Close();
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarningFormat("Error while trying to deserialize file {0}: {1}",
                        filePath, exception.Message);
                    continue;
                }

                saveFilesCache.Add(saveFile);
            }

            saveFilesCache.Sort((a, b) => a.saveSlotIndex.CompareTo(b.saveSlotIndex));

            initialized = true;
        }

        private static bool CheckDirectory()
        {
            bool exists = true;

            if (!Directory.Exists(Path))
            {
                var directory = Directory.CreateDirectory(Path);
                exists = directory.Exists;
                if (!directory.Exists)
                {
                    Debug.LogErrorFormat("Couldn't create directory {0}",
                        Path);
                }
            }

            return exists;
        }

        public static SaveFile CreateNewSaveFile(int slotIndex, string playerName)
        {
            Debug.LogFormat("CreateNewSaveFile");

            var saveFileTemplate = GameManager.GetSaveFileTemplate();

            string fileName = string.Format("SaveSlot_{0}", slotIndex);
            string filePath = GetFilePath(fileName);

            SaveFile newSaveFile = new SaveFile(saveFileTemplate)
            {
                saveSlotIndex = slotIndex,
                fileName = fileName,
                playerData = new Player.PlayerData()
                {
                    playerName = playerName
                }
            };
            Debug.LogFormat("FileName available: {0} (path {1})",
                fileName, filePath);

            SaveFile(newSaveFile);

            saveFilesCache.Add(newSaveFile);

            return newSaveFile;
        }

        public static SaveFile GetSaveFile(int slotIndex)
        {
            return saveFilesCache.Find((x) => x.saveSlotIndex == slotIndex);
        }

        public static void SaveFile(SaveFile saveFile)
        {
            if (!saveFile.IsValid
                || !CheckDirectory())
                return;

            string filePath = GetFilePath(saveFile.fileName);
            FileStream file;

            if (!File.Exists(filePath)
                && !CreateFile(filePath))
            {
                Debug.LogFormat("Couldn't create save file {0}", filePath);
                return;
            }

            try
            {
                file = File.OpenWrite(filePath);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, saveFile);

                file.Close();
            }
            catch (System.Exception exception)
            {
                Debug.LogFormat("Error while trying to open file {0} in write mode: {1}",
                    filePath, exception.Message);
                return;
            }

            Debug.LogFormat("File saved correctly at path '{0}'", filePath);
        }

        private static bool CreateFile(string filePath)
        {
            bool success = false;

            try
            {
                var file = File.Create(filePath);
                success = File.Exists(filePath);
                if (file != null)
                {
                    file.Close();
                }
            }
            catch (Exception exception)
            {
                Debug.LogErrorFormat("Error while creating file {0}: {1}",
                    filePath, exception.Message);
            }

            return success;
        }

        public static void DeleteSaveFile(int slotIndex)
        {
            var file = GetSaveFile(slotIndex);
            if (file == null)
                return;

            string filePath = GetFilePath(file.fileName);
            saveFilesCache.Remove(file);
            DeleteSaveFile(filePath);
        }

        private static void DeleteSaveFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception exception)
            {
                Debug.LogErrorFormat("Error while deleting file {0}: {1}",
                    filePath, exception.Message);
            }
        }

    }

}