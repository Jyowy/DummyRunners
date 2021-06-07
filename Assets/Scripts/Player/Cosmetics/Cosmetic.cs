using UnityEngine;

namespace Player.Cosmetics
{

    public enum CostumSlot
    {
        Head
    }

    public class Cosmetic : MonoBehaviour
    {

        [SerializeField]
        private string cosmeticId = "";
        [SerializeField]
        private string cosmeticName = "";
        [SerializeField]
        private string description = "";
        [SerializeField]
        private CostumSlot slot = CostumSlot.Head;
        [SerializeField]
        private Sprite preview = null;
        [SerializeField]
        private float price = 100f;

        public string GetCosmeticId() => cosmeticId;
        public string GetCosmeticName() => cosmeticName;
        public string GetCosmeticDescription() => description;
        public CostumSlot GetCosmeticSlot() => slot;
        public Sprite GetPreviewSprite() => preview;
        public float GetNormalPrice() => price;

    }

}