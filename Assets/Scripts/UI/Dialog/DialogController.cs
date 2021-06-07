using Common;
using UnityEngine;

namespace UI.DialogSystem
{

    public class DialogController : SingletonBehaviour<DialogController>
    {

        [SerializeField]
        private DialogVisualizer dialogVisualizer = null;

        private DialogSequence dialogSequence = null;
        private Dialog currentDialog;

        private System.Action<DialogEvent> onDialogEvent;

        public static void Open(DialogSequence dialogSequence, System.Action<DialogEvent> onDialogEvent = null)
        {
            if (!(Instance is DialogController instance))
                return;

            instance.OpenImpl(dialogSequence, onDialogEvent);
        }

        private void OpenImpl(DialogSequence dialogSequence, System.Action<DialogEvent> onDialogEvent)
        {
            if (dialogSequence == null)
                return;

            this.dialogSequence = dialogSequence;
            this.onDialogEvent = onDialogEvent;

            currentDialog = dialogSequence.GetFirstDialog();
            dialogVisualizer.SetCallbacks(OnDialogFinished, OnAnswerSelected);
            dialogVisualizer.Show(currentDialog);
        }

        public static void Close()
        {
            if (!(Instance is DialogController instance))
                return;

            instance.CloseImpl();
        }

        private void CloseImpl()
        {
            if (!dialogVisualizer.IsVisible())
                return;

            dialogVisualizer.RemoveCallbacks();
            dialogVisualizer.Hide();
        }

        public static void Pause()
        {
            if (!(Instance is DialogController instance))
                return;

            instance.dialogVisualizer.Pause();
        }

        public static void Resume()
        {
            if (!(Instance is DialogController instance))
                return;

            instance.dialogVisualizer.Resume();
        }

        private void OnDialogFinished()
        {
            if (currentDialog.hasEvent)
            {
                onDialogEvent?.Invoke(currentDialog.dialogEvent);
            }

            if (!currentDialog.end)
            {
                NextDialog();
            }
            else
            {
                DialogSequenceFinished();
            }
        }

        private void OnAnswerSelected(int answerIndex)
        {
            Answer answer = currentDialog.answers[answerIndex];

            if (answer.hasEvent)
            {
                onDialogEvent?.Invoke(answer.dialogEvent);
            }

            if (answer.reaction == AnswerReactionType.Next)
            {
                OnDialogFinished();
            }
            else if (answer.reaction == AnswerReactionType.JumpTo)
            {
                GoToDialog(answer.jumpTo);
            }
            else
            {
                DialogSequenceFinished();
            }
        }

        private void NextDialog()
        {
            GoToDialog(currentDialog.nextIndex);
        }

        private void GoToDialog(int index)
        {
            Dialog dialog = dialogSequence.GetDialog(index);
            currentDialog = dialog;
            dialogVisualizer.Show(dialog);
        }

        private void DialogSequenceFinished()
        {
            Close();
        }

    }

}