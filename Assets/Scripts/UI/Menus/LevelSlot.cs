using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Stadiums
{

    public class LevelSlot : MonoBehaviour
    {

        [SerializeField]
        private LevelData levelData = null;
        [SerializeField]
        private Button button = null;

        private System.Action<LevelSlot> onSelected = null;

        public Button GetButton() => button;
        public LevelData GetData() => levelData;

        public void Show(System.Action<LevelSlot> onSelected)
        {
            this.onSelected = onSelected;
            button.onClick.AddListener(OnSelected);
        }

        public void Hide()
        {
            button.onClick.RemoveAllListeners();
        }

        public void SetColor(Color color)
        {
            var colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }

        public void Select()
        {
            OnSelected();
        }

        private void OnSelected()
        {
            onSelected?.Invoke(this);
        }

    }

}