using UnityEditor;
using UnityEngine;

namespace World.Quests
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Quest))]
    public class QuestDrawer : Editor
    {

        public override void OnInspectorGUI()
        {

            var questId = serializedObject.FindProperty("questId");
            var questName = serializedObject.FindProperty("questName");
            var description = serializedObject.FindProperty("description");

            var hasReward = serializedObject.FindProperty("hasReward");
            var reward = serializedObject.FindProperty("reward");

            questId.stringValue = EditorGUILayout.TextField("Id", questId.stringValue);
            questName.stringValue = EditorGUILayout.TextField("Name", questName.stringValue);
            EditorGUILayout.LabelField("Description");
            description.stringValue = EditorGUILayout.TextArea(description.stringValue);

            hasReward.boolValue = EditorGUILayout.Toggle("Has reward?", hasReward.boolValue);
            if (hasReward.boolValue)
            {
                EditorGUILayout.PropertyField(reward);
            }

            serializedObject.ApplyModifiedProperties();

        }

    }

}