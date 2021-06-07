using UnityEngine;

namespace Player.Cosmetics
{

    public class CostumWearer : MonoBehaviour
    {

        [SerializeField]
        private Transform headSlot = null;

        private Cosmetic headCosmetic = null;

#if UNITY_EDITOR
        // Only for development
        [SerializeField]
#endif
        private Costum costum = null;

        public Costum GetCostum() => costum;

#if UNITY_EDITOR
        // Only for development
        private void Awake()
        {
            SetSuitcaseCostum(costum);
        }
#endif

        public void EquipCosmetic(Cosmetic cosmetic)
        {
            if (cosmetic == null)
                return;

            Debug.LogFormat("Equip cosmetic {0}", cosmetic);
            CostumSlot cosmeticSlot = cosmetic.GetCosmeticSlot();
            if (cosmeticSlot == CostumSlot.Head)
            {
                EquipCosmeticInSlot(headSlot, ref headCosmetic, cosmetic, ref costum.head);
            }
        }

        public void UnequipCosmetic(Cosmetic cosmetic)
        {
            if (cosmetic == null
                || !costum.IsThisCosmeticEquiped(cosmetic))
                return;

            CostumSlot cosmeticSlot = cosmetic.GetCosmeticSlot();
            if (cosmeticSlot == CostumSlot.Head)
            {
                UnequipCosmeticFromSlot(ref headCosmetic, ref costum.head);
            }
        }

        public void SetSuitcaseCostum(Costum costum)
        {
            Debug.LogFormat("SetStuicaseCostum");
            if (costum == null)
                return;

            this.costum = costum;
            EquipCosmetic(costum.headCosmetic);
        }

        private void UnequipCosmeticFromSlot(ref Cosmetic slot, ref string costumSlot)
        {
            if (slot == null)
                return;

            GameObject.DestroyImmediate(slot.gameObject);
            slot = null;
            costumSlot = null;
        }

        private void EquipCosmeticInSlot(Transform slotRoot, ref Cosmetic slot, Cosmetic cosmetic, ref string costumSlot)
        {
            Debug.LogFormat("EquipCosmeticInSlot {0}", cosmetic);

            if (cosmetic == null)
                return;

            UnequipCosmeticFromSlot(ref slot, ref costumSlot);

            slot = GameObject.Instantiate(cosmetic, slotRoot);
            costumSlot = cosmetic.GetCosmeticId();

            Debug.LogFormat("Equiped cosmetic {0}", cosmetic);
        }

    }

}