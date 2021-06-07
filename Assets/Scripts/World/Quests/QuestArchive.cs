using Common;
using System.Collections.Generic;
using System.IO;
//using UnityEditor;
using UnityEngine;

namespace World.Quests
{

    public class QuestArchive : SingletonBehaviour<QuestArchive>
    {

        [SerializeField]
        private string folder = "Assets/Data/Quests";

        private readonly List<Quest> archive = new List<Quest>();

        protected override void OnInstantiated()
        {
            LoadAllQuests();
        }

        public static Quest GetQuest(string id)
        {
            if (!(Instance is QuestArchive instance))
                return null;

            return instance.archive.Find((x) => x.GetQuestId().Equals(id));
        }

        private void LoadAllQuests()
        {
            Debug.LogFormat("Load all quests");

            archive.Clear();

            string path = folder;
            string[] files;
            int fileCount;
            try
            {
                if (!Directory.Exists(path))
                {
                    Debug.LogErrorFormat("Folder {0} doesn't exist", path);
                    return;
                }

                files = Directory.GetFiles(path);
                fileCount = files != null ? files.Length : 0;
            }
            catch (System.Exception exception)
            {
                Debug.LogErrorFormat("Error while trying to acces folder {0}: {1}",
                    path, exception.Message);
                return;
            }

            for (int i = 0; i < fileCount; ++i)
            {
                /*
                string filePath = files[i];
                Quest quest;
                try
                {
                    quest = AssetDatabase.LoadAssetAtPath<Quest>(filePath);
                }
                catch (System.Exception exception)
                {
                    Debug.LogErrorFormat("Error while trying to read file '{0}': {1}",
                        filePath, exception.Message);
                    continue;
                }
                    
                if (quest == null)
                    continue;

                // Repetition check
                if (archive.Contains(quest))
                {
                    int index = archive.IndexOf(quest);
                    Quest repeatedQuest = archive[index];
                    Debug.LogWarningFormat("Quest '{0}'-'{1}' (file {4}) has same id as already stored quest '{2}'-'{3}' so it is ignored!",
                        quest.GetQuestId(), quest.GetQuestName(), repeatedQuest.GetQuestId(), repeatedQuest.GetQuestName(),
                        filePath);
                    continue;
                }

                archive.Add(quest);
                Debug.LogFormat("Quest '{0}'-'{1}' (file {2}) added successfully!",
                        quest.GetQuestId(), quest.GetQuestName(), filePath);

                //*/
            }

            if (archive.Count == 0)
            {
                Debug.LogWarningFormat("Folder {0} doesn't contain any quest",
                    path);
                return;
            }
        }

    }

}