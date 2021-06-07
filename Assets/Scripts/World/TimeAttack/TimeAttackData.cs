using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World.Rewards;

namespace World.TimeAttacks
{

    [CreateAssetMenu]
    public class TimeAttackData : ScriptableObject
    {

        public string id = "TIME_ATTACK_";
        public string timeAttackName = "Time Attack ";
        public string description = "";

        public float goldTime = 30f;
        public float silverTime = 45f;
        public float bronzeTime = 60f;

        public Reward goldReward = null;
        public Reward silverReward = null;
        public Reward bronzeReward = null;
        public Reward finishReward = null;

    }

}