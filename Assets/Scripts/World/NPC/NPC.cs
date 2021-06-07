using System.Collections.Generic;
using Player;
using Player.Cosmetics;
using UI.DialogSystem;
using UnityEngine;
using World.Level;
using World.Quests;
using World.TimeAttacks;

namespace World.NPC
{

    public class NPC : Interactable
    {

        [SerializeField]
        private string npcName = "";
        [SerializeField]
        private DialogSequence dialogSequence = null;

        [SerializeField]
        private DialogSequence prevQuestDialogSequence = null;
        [SerializeField]
        private DialogSequence questActiveDialogSequence = null;
        [SerializeField]
        private DialogSequence questFinishedDialogSequence = null;

        [SerializeField]
        private Quest quest = null;
        [SerializeField]
        private List<Cosmetic> catalog = null;
        [SerializeField]
        private TimeAttack timeAttack = null;

        protected override void OnInteraction()
        {
            if (quest == null)
            {
                DialogController.Open(dialogSequence, OnDialogEvent);
            }
            else
            {
                if (PlayerManager.HasCompletedQuest(quest))
                {
                    DialogController.Open(questFinishedDialogSequence, OnDialogEvent);
                }
                else if (PlayerManager.IsQuestActive(quest))
                {
                    DialogController.Open(questActiveDialogSequence, OnDialogEvent);
                }
                else
                {
                    DialogController.Open(prevQuestDialogSequence, OnDialogEvent);
                }
            }
        }

        private void OnDialogEvent(DialogEvent dialogEvent)
        {
            if (quest != null)
            {
                if (dialogEvent == DialogEvent.Accept)
                {
                    PlayerManager.GiveQuest(quest);
                }
                else if (dialogEvent == DialogEvent.CompleteQuest)
                {
                    PlayerManager.CompleteQuest(quest);
                }
            }
            else if (catalog != null
                && dialogEvent == DialogEvent.Shop)
            {
                DialogController.Pause();
                WorldManager.ShowCosmeticShop(catalog, DialogController.Resume);
            }
            else if (timeAttack != null
                && dialogEvent == DialogEvent.TimeAttack)
            {
                DialogController.Close();
                TimeAttackPlayer.ShowTimeAttack(timeAttack);
            }
        }

    }

}