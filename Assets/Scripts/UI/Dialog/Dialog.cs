using System.Collections.Generic;

namespace UI.DialogSystem
{

    public enum AnswerReactionType
    {
        Next,
        JumpTo,
        Finish
    }

    public enum DialogEvent
    {
        Accept,
        Decline,
        CompleteQuest,
        Shop,
        TimeAttack
    }

    [System.Serializable]
    public struct Answer
    {
        public string message;
        public AnswerReactionType reaction;
        public int jumpTo;
        public bool hasEvent;
        public DialogEvent dialogEvent;
    }

    [System.Serializable]
    public struct Dialog
    {
        public string id;
        public int index;
        public string speaker;
        public string message;

        public bool end;

        public bool hasEvent;
        public DialogEvent dialogEvent;
        
        public int nextIndex;
        public bool hasAnswers;
        public List<Answer> answers;

        //*
        public void Clear()
        {
            id = "";
            index = 0;
            speaker = "";
            message = "";
            end = false;
            nextIndex = 0;
            hasAnswers = false;
            if (answers != null)
            {
                answers.Clear();
                answers = null;
            }
        }
        //*/

#if UNITY_EDITOR
        public bool expanded;
        public bool answersExpanded;
#endif
    }

}