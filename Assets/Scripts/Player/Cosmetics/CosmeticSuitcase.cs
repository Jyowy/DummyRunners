using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Cosmetics
{

    [System.Serializable]
    public class Costum
    {
        public string head = null;

        [NonSerialized]
        public Cosmetic headCosmetic = null;

        public Costum() { }
        
        public Costum(Costum other)
        {
            head = other.head;
        }

        public bool IsThisCosmeticEquiped(Cosmetic cosmetic)
        {
            return cosmetic.GetCosmeticId().Equals(head);
        }
    }

    [System.Serializable]
    public class CosmeticSuitcase
    {

        [SerializeField]
        private List<string> cosmetics = new List<string>();
        [SerializeField]
        private Costum costum = new Costum();

        public Costum GetCostum() => costum;

        [NonSerialized]
        private List<Cosmetic> cosmeticsCache = new List<Cosmetic>();

        public List<Cosmetic> GetCosmetics() => cosmeticsCache;

        public CosmeticSuitcase()
        {
            InitializeCache();
        }

        public CosmeticSuitcase(CosmeticSuitcase other)
        {
            cosmetics = new List<string>(other.cosmetics);
            costum = new Costum(other.costum);

            InitializeCache();
        }

        public void Initialize()
        {
            InitializeCache();
        }

        private void InitializeCache()
        {
            cosmeticsCache = new List<Cosmetic>();
            cosmetics.ForEach(
                (id) =>
                {
                    Cosmetic cosmetic = CosmeticCatalog.GetCosmetic(id);
                    if (cosmetic != null)
                    {
                        cosmeticsCache.Add(cosmetic);
                    }
                }
            );

            costum.headCosmetic = GetCosmetic(costum.head);
        }

        public bool HasCosmetic(Cosmetic cosmetic)
            => cosmeticsCache.Contains(cosmetic);

        public void AddCosmetic(Cosmetic cosmetic)
        {
            if (HasCosmetic(cosmetic))
                return;

            cosmetics.Add(cosmetic.GetCosmeticId());
            cosmeticsCache.Add(cosmetic);
        }

        public Cosmetic GetCosmetic(string id)
            => cosmeticsCache.Find((x) => x.GetCosmeticId().Equals(id));

    }

}