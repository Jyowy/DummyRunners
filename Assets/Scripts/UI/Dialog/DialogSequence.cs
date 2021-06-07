using System.Collections.Generic;
using UnityEngine;

namespace UI.DialogSystem
{

    [CreateAssetMenu]
    public class DialogSequence : ScriptableObject
    {

        [SerializeField]
        private string sequenceId = "Unnamed";
        [SerializeField]
        private List<Dialog> dialogs = null;

        public string GetSequenceId() => sequenceId;

        public Dialog GetFirstDialog() => dialogs[0];
        public Dialog GetDialog(int index) => dialogs[index];

    }

}