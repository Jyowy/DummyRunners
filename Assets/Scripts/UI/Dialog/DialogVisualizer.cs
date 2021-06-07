using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UI.Menus;

namespace UI.DialogSystem
{

    public class DialogVisualizer : Menu
    {

        [SerializeField]
        private TMPro.TextMeshProUGUI speaker = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI message = null;

        [SerializeField]
        private Transform answerContainer = null;
        [SerializeField]
        private AnswerVisualizer answerTemplate = null;

        [SerializeField]
        private float letterSpeed = 0.02f;
        [SerializeField]
        private float commaPause = 0.1f;
        [SerializeField]
        private float dotPause = 0.2f;
        [SerializeField]
        private float paragraphPause = 0.5f;

        private Dialog dialog;
        private readonly List<AnswerVisualizer> answers = new List<AnswerVisualizer>();

        private System.Action dialogFinished = null;
        private System.Action<int> answerSelected = null;

        private bool interactable = false;
        private bool inputPaused = false;

        private bool messageSet = false;
        private int messageIndex = 0;
        private int messageLength = 0;
        private float remainingTime = 0f;

        private bool IsMessageCompleted() => !messageSet
            || messageIndex == messageLength;
        private bool hasAnswers = false;

        private float pauseTime = 0f;

        public void SetCallbacks(System.Action dialogFinished, System.Action<int> answerSelected)
        {
            this.dialogFinished = dialogFinished;
            this.answerSelected = answerSelected;
        }

        public void RemoveCallbacks()
        {
            dialogFinished = null;
            answerSelected = null;
        }

        public void Show(Dialog dialog)
        {
            this.dialog = dialog;
            Initialize();

            if (!IsVisible())
            {
                Show();
            }
        }

        protected override void OnHide()
        {
            base.OnHide();

            Clear();
            dialog = new Dialog();
        }

        private void Clear()
        {
            Debug.Log("interactable = false");

            interactable = false;
            messageSet = false;
            speaker.text = "";
            message.text = "";
            messageIndex = 0;
            messageLength = 0;
            pauseTime = 0f;
            remainingTime = 0f;
            hasAnswers = false;
            answerContainer.gameObject.SetActive(false);
            for (int i = 0; i < answers.Count; ++i)
            {
                answers[i].Hide();
                Destroy(answers[i].gameObject, 0.1f);
                answers[i] = null;
            }
            answers.Clear();
            
            answerContainer.DetachChildren();
        }

        private void Initialize()
        {
            Clear();

            messageIndex = 0;

            Debug.LogFormat("Initialize dialog '{0}', '{1}'", dialog.speaker, dialog.message);
            Debug.Log("interactable = true");
            interactable = true;
            messageSet = true;
            speaker.text = dialog.speaker;
            message.text = "";
            messageIndex = 0;
            messageLength = dialog.message.Length;
            pauseTime = 0f;
            remainingTime = 0f;

            hasAnswers = dialog.hasAnswers
                && dialog.answers.Count > 0;

            if (hasAnswers)
            {
                int count = dialog.answers.Count;
                for (int i = 0; i < count; ++i)
                {
                    var newAnswer = GameObject.Instantiate(answerTemplate, answerContainer);
                    newAnswer.Initialize(dialog.answers[i], i, AnswerSelected);
                    answers.Add(newAnswer);
                }

                if (count > 1)
                {
                    answers[0].SetNavigation(answers[count - 1], answers[1]);
                    for (int i = 1; i < count - 1; ++i)
                    {
                        answers[i].SetNavigation(answers[i - 1], answers[i + 1]);
                    }
                    answers[count - 1].SetNavigation(answers[count - 2], answers[0]);
                }
            }
        }

        private void Update()
        {
            if (IsMessageCompleted())
                return;

            float time = Time.deltaTime
                + remainingTime;

            if (pauseTime > 0f)
            {
                if (pauseTime > time)
                {
                    pauseTime -= time;
                    time = 0f;
                }
                else
                {
                    pauseTime = 0f;
                    time -= pauseTime;
                }
            }

            int newIndex = messageIndex;

            while (time > 0f
                && newIndex < messageLength)
            {
                int skip = GetPauseTime(newIndex, out pauseTime);
                if (pauseTime > 0f)
                {
                    newIndex += skip;
                    if (pauseTime > time)
                    {
                        pauseTime -= time;
                        time = 0f;
                    }
                    else
                    {
                        pauseTime = 0f;
                        time = math.max(time - pauseTime, 0f);
                    }
                }
                else if (time > letterSpeed)
                {
                    time = math.max(time - letterSpeed, 0f);
                    newIndex++;
                }
                else
                {
                    break;
                }
            }

            remainingTime = time;

            if (newIndex != messageIndex)
            {
                UpdateMessage(newIndex);
            }
        }

        private void CompleteMessage()
        {
            if (IsMessageCompleted())
                return;

            UpdateMessage(messageLength);
        }

        private void UpdateMessage(int newIndex)
        {
            if (IsMessageCompleted())
                return;

            int prevIndex = messageIndex;
            messageIndex = math.min(newIndex, messageLength);
            message.text += dialog.message.Substring(prevIndex, newIndex - prevIndex);

            if (IsMessageCompleted())
            {
                MessageCompleted();
            }
        }

        private void MessageCompleted()
        {
            Debug.LogFormat("Message completed!");
            messageIndex = dialog.message.Length;
            if (hasAnswers)
            {
                ShowAnswers();
            }
        }

        private void ShowAnswers()
        {
            Debug.LogFormat("ShowAnswers");
            answerContainer.gameObject.SetActive(true);
            for (int i = 0; i < answers.Count; ++i)
            {
                answers[i].Show();
            }
            answers[0].Focus();
        }

        public void Pause() => inputPaused = true;
        public void Resume() => inputPaused = false;

        protected override void OnAccept()
        {
            Interact();
        }

        protected override void OnCancel()
        {
            Interact();
        }

        private void Interact()
        {
            if (!messageSet
                || !interactable
                || inputPaused)
                return;

            if (!IsMessageCompleted())
            {
                CompleteMessage();
            }
            else if (!hasAnswers)
            {
                interactable = false;
                dialogFinished?.Invoke();
            }
        }

        private void AnswerSelected(int index)
        {
            if (!interactable)
                return;

            Debug.Log("interactable = false");
            interactable = false;
            Focus(null);
            answerSelected?.Invoke(index);
        }

        private int GetPauseTime(int messageIndex, out float pauseTime)
        {
            pauseTime = 0f;
            int prevMessageIndex = messageIndex;

            if (messageIndex < messageLength)
            {
                char c = dialog.message[messageIndex];

                if (c == ',')
                {
                    pauseTime = commaPause;
                    messageIndex++;
                }
                else if (c == '.'
                    || c == '!'
                    || c == '?')
                {
                    pauseTime = dotPause;
                    messageIndex++;

                    if (messageIndex < messageLength)
                    {
                        c = dialog.message[messageIndex];
                        if (c == '\n')
                        {
                            pauseTime = paragraphPause;
                            messageIndex++;
                        }
                    }
                }
            }

            return messageIndex - prevMessageIndex;
        }

    }

}