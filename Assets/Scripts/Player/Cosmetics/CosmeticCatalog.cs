using Common;
using System.Collections.Generic;
using System.IO;
//using UnityEditor;
using UnityEngine;

namespace Player.Cosmetics
{

    public class CosmeticCatalog : SingletonBehaviour<CosmeticCatalog>
    {

        [SerializeField]
        private string folder = "Assets/Prefabs/Cosmetics";

        private readonly List<Cosmetic> catalog = new List<Cosmetic>();

        protected override void OnInstantiated()
        {
            Debug.LogFormat("Load all cosmetics");

            catalog.Clear();

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
                Cosmetic cosmetic;
                try
                {
                    cosmetic = AssetDatabase.LoadAssetAtPath<Cosmetic>(filePath);
                }
                catch (System.Exception exception)
                {
                    Debug.LogErrorFormat("Error while trying to read file '{0}': {1}",
                        filePath, exception.Message);
                    continue;
                }

                if (cosmetic == null)
                    continue;

                // Repetition check
                if (catalog.Contains(cosmetic))
                {
                    int index = catalog.IndexOf(cosmetic);
                    Cosmetic repeatedCosmetic = catalog[index];
                    Debug.LogWarningFormat("Cosmetic '{0}'-'{1}' (file {4}) has same id as already stored cosmetic '{2}'-'{3}' so it is ignored!",
                        cosmetic.GetCosmeticId(), cosmetic.GetCosmeticName(), repeatedCosmetic.GetCosmeticId(), repeatedCosmetic.GetCosmeticName(),
                        filePath);
                    continue;
                }

                catalog.Add(cosmetic);
                Debug.LogFormat("Catalog '{0}'-'{1}' (file {2}) added successfully!",
                        cosmetic.GetCosmeticId(), cosmetic.GetCosmeticName(), filePath);

                //*/
            }

            if (catalog.Count == 0)
            {
                Debug.LogWarningFormat("Folder {0} doesn't contain any cosmetic",
                    path);
                return;
            }
        }

        public static Cosmetic GetCosmetic(string id)
        {
            if (!(Instance is CosmeticCatalog instance))
                return null;

            return instance.catalog.Find((x) => x.GetCosmeticId().Equals(id));
        }

    }

}