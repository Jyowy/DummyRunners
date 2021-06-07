using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace UI.DialogSystem
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogSequence))]
    public class DialogSequenceDrawer : Editor
    {

        public override void OnInspectorGUI()
        {
            var sequenceName = serializedObject.FindProperty("sequenceId");
            var dialogs = serializedObject.FindProperty("dialogs");

            string sequenceTitle = string.Format("Dialog Sequence {0}", sequenceName.stringValue);
            EditorGUILayout.LabelField(sequenceTitle, EditorStyles.boldLabel);

            int count = dialogs.arraySize;
            count = EditorGUILayout.DelayedIntField("Count", count);

            string []dialogIds = new string[dialogs.arraySize];
            for (int i = 0; i < dialogs.arraySize; ++i)
            {
                dialogIds[i] =
                    dialogs.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("id").stringValue;
            }

            for (int i = 0; i < dialogs.arraySize; ++i)
            {
                var dialog = dialogs.GetArrayElementAtIndex(i);
                var dialogName = dialog.FindPropertyRelative("id");

                var dialogExpanded = dialog.FindPropertyRelative("expanded");

                string dialogTitle = string.Format("Dialog {0}: {1}", i + 1, dialogName.stringValue);
                dialogExpanded.boolValue = EditorGUILayout.Foldout(dialogExpanded.boolValue, dialogTitle, true);
                if (!dialogExpanded.boolValue)
                    continue;

                EditorGUI.indentLevel++;

                var index = dialog.FindPropertyRelative("index");
                var speaker = dialog.FindPropertyRelative("speaker");
                var message = dialog.FindPropertyRelative("message");
                var end = dialog.FindPropertyRelative("end");
                var hasEvent = dialog.FindPropertyRelative("hasEvent");
                var nextIndex = dialog.FindPropertyRelative("nextIndex");
                var hasAnaswers = dialog.FindPropertyRelative("hasAnswers");

                dialogName.stringValue = EditorGUILayout.TextField(string.Format("Dialog {0}", i + 1), dialogName.stringValue);
                index.intValue = i;
                speaker.stringValue = EditorGUILayout.TextField("Speaker", speaker.stringValue);
                EditorGUILayout.LabelField("Message");
                EditorGUI.indentLevel++;
                message.stringValue = EditorGUILayout.TextArea(message.stringValue);
                EditorGUI.indentLevel--;

                end.boolValue = EditorGUILayout.Toggle("Is an end?", end.boolValue);
                if (!end.boolValue)
                {
                    nextIndex.intValue = math.clamp(nextIndex.intValue, 0, dialogIds.Length - 1);
                    nextIndex.intValue = EditorGUILayout.Popup("Next Dialog", nextIndex.intValue, dialogIds);
                }

                hasEvent.boolValue = EditorGUILayout.Toggle("Triggers event?", hasEvent.boolValue);
                if (hasEvent.boolValue)
                {
                    var dialogEvent = dialog.FindPropertyRelative("dialogEvent");
                    EditorGUILayout.PropertyField(dialogEvent, new GUIContent("Event"));
                }

                hasAnaswers.boolValue = EditorGUILayout.Toggle("Contains answers?", hasAnaswers.boolValue);
                if (hasAnaswers.boolValue)
                {
                    var answers = dialog.FindPropertyRelative("answers");
                    var answersExpanded = dialog.FindPropertyRelative("answersExpanded");

                    answersExpanded.boolValue = EditorGUILayout.Foldout(answersExpanded.boolValue, "Answers", true);

                    if (answersExpanded.boolValue)
                    {
                        int answerCount = answers.arraySize;

                        answerCount = EditorGUILayout.DelayedIntField("Count", answerCount);

                        EditorGUI.indentLevel++;
                        for (int j = 0; j < answers.arraySize; ++j)
                        {
                            EditorGUILayout.LabelField(string.Format("Answer {0}", j + 1));
                            EditorGUI.indentLevel++;

                            var answer = answers.GetArrayElementAtIndex(j);
                            var answerMessage = answer.FindPropertyRelative("message");
                            var reaction = answer.FindPropertyRelative("reaction");
                            var answerHasEvent = answer.FindPropertyRelative("hasEvent");

                            answerMessage.stringValue = EditorGUILayout.TextField("Message:", answerMessage.stringValue);
                            EditorGUILayout.PropertyField(reaction);
                            AnswerReactionType reactionType = (AnswerReactionType)reaction.intValue;

                            if (reactionType == AnswerReactionType.JumpTo)
                            {
                                var jumpTo = answer.FindPropertyRelative("jumpTo");
                                jumpTo.intValue = math.clamp(jumpTo.intValue, 0, dialogIds.Length - 1);
                                jumpTo.intValue = EditorGUILayout.Popup("Jump To", jumpTo.intValue, dialogIds);
                            }

                            answerHasEvent.boolValue = EditorGUILayout.Toggle("Triggers event?", answerHasEvent.boolValue);
                            if (answerHasEvent.boolValue)
                            {
                                var answerDialogEvent = answer.FindPropertyRelative("dialogEvent");
                                EditorGUILayout.PropertyField(answerDialogEvent, new GUIContent("Event"));
                            }

                            EditorGUI.indentLevel--;
                        }

                        EditorGUI.indentLevel++;
                        if (GUILayout.Button("+ Answer"))
                        {
                            answerCount++;
                        }

                        if (GUILayout.Button("- Answer"))
                        {
                            answerCount = math.max(answerCount - 1, 0);
                        }
                        EditorGUI.indentLevel--;

                        if (answerCount != answers.arraySize)
                        {
                            int newAnswers = math.max(answerCount - answers.arraySize, 0);
                            answers.arraySize = answerCount;

                            for (int j = answerCount - newAnswers; j < answerCount; ++j)
                            {
                                InitializeAnswer(answers.GetArrayElementAtIndex(j), j, answerCount);
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button("+ Dialog"))
            {
                count++;
            }

            if (GUILayout.Button("- Dialog"))
            {
                count = math.max(count - 1, 0);
            }

            if (count != dialogs.arraySize)
            {
                int newDialgos = math.max(count - dialogs.arraySize, 0);
                dialogs.arraySize = count;

                for (int i = count - newDialgos; i < count; ++i)
                {
                    InitializeDialog(dialogs.GetArrayElementAtIndex(i), i, count);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeDialog(SerializedProperty dialog, int i, int count)
        {
            var id = dialog.FindPropertyRelative("id");
            var index = dialog.FindPropertyRelative("index");
            var speaker = dialog.FindPropertyRelative("speaker");
            var message = dialog.FindPropertyRelative("message");
            var end = dialog.FindPropertyRelative("end");
            var hasEvent = dialog.FindPropertyRelative("hasEvent");
            var dialogEvent = dialog.FindPropertyRelative("dialogEvent");
            var nextIndex = dialog.FindPropertyRelative("nextIndex");
            var hasAnswers = dialog.FindPropertyRelative("hasAnswers");
            var answers = dialog.FindPropertyRelative("answers");
            var answersExpanded = dialog.FindPropertyRelative("answersExpanded");

            id.stringValue = "Unnamed";
            index.intValue = i;
            speaker.stringValue = "";
            message.stringValue = "";
            end.boolValue = i == count - 1;
            hasEvent.boolValue = false;
            dialogEvent.intValue = 0;
            nextIndex.intValue = math.min(i + 1, count - 1);
            hasAnswers.boolValue = false;
            answers.arraySize = 0;
            answersExpanded.boolValue = true;
        }

        private void InitializeAnswer(SerializedProperty answer, int i, int count)
        {
            var message = answer.FindPropertyRelative("message");
            var reaction = answer.FindPropertyRelative("reaction");
            var jumpTo = answer.FindPropertyRelative("jumpTo");
            var eventType = answer.FindPropertyRelative("dialogEvent");

            message.stringValue = "";
            reaction.intValue = 0;
            jumpTo.intValue = math.min(i + 1, count - 1);
            eventType.intValue = 0;
        }

    }

}