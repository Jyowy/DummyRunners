using UnityEditor;
using UnityEngine;

namespace World.Rewards
{

    [CustomPropertyDrawer(typeof(Reward))]
    public class RewardDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var containsMoney = property.FindPropertyRelative("containsMoney");
            var unlocksSkill = property.FindPropertyRelative("unlocksSkill");
            var containsCosmetic = property.FindPropertyRelative("containsCosmetic");

            int lines = 1 + 4
                + (containsMoney.boolValue ? 1 : 0)
                + (unlocksSkill.boolValue ? 1 : 0)
                + (containsCosmetic.boolValue ? 1 : 0);

            return lines * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            float indexMargin = 17.5f;
            Rect propertyRect = new Rect(position.x, position.y, position.width, height);

            EditorGUI.LabelField(propertyRect, label, EditorStyles.boldLabel);
            propertyRect.y += propertyRect.height;

            propertyRect.x += indexMargin;
            propertyRect.width -= indexMargin;

            var containsMoney = property.FindPropertyRelative("containsMoney");
            var containsEnergySlot = property.FindPropertyRelative("containsEnergySlot");
            var unlocksSkill = property.FindPropertyRelative("unlocksSkill");
            var containsCosmetic = property.FindPropertyRelative("containsCosmetic");

            containsMoney.boolValue = GUI.Toggle(propertyRect, containsMoney.boolValue, "Contains money?");
            propertyRect.y += height;
            if (containsMoney.boolValue)
            {
                propertyRect.x += indexMargin;
                propertyRect.width -= indexMargin;

                var money = property.FindPropertyRelative("money");
                EditorGUI.PropertyField(propertyRect, money);
                propertyRect.y += height;

                propertyRect.x -= indexMargin;
                propertyRect.width += indexMargin;
            }

            containsEnergySlot.boolValue = GUI.Toggle(propertyRect, containsEnergySlot.boolValue, "Contains an energy slot?");
            propertyRect.y += height;

            unlocksSkill.boolValue = GUI.Toggle(propertyRect, unlocksSkill.boolValue, "Unlocks skill?");
            propertyRect.y += height;
            if (unlocksSkill.boolValue)
            {
                propertyRect.x += indexMargin;
                propertyRect.width -= indexMargin;

                var skill = property.FindPropertyRelative("skill");
                EditorGUI.PropertyField(propertyRect, skill);
                propertyRect.y += height;

                propertyRect.x -= indexMargin;
                propertyRect.width += indexMargin;
            }

            containsCosmetic.boolValue = GUI.Toggle(propertyRect, containsCosmetic.boolValue, "Contains cosmetic?");
            propertyRect.y += height;
            if (containsCosmetic.boolValue)
            {
                propertyRect.x += indexMargin;
                propertyRect.width -= indexMargin;

                var cosmetic = property.FindPropertyRelative("cosmetic");
                EditorGUI.PropertyField(propertyRect, cosmetic);
                propertyRect.y += height;

                propertyRect.x -= indexMargin;
                propertyRect.width += indexMargin;
            }
        }

    }

}