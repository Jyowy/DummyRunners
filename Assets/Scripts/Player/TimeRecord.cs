using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    [System.Serializable]
    public class TimeRecord
    {

        [HideInInspector]
        [SerializeField]
        private List<string> recordIds = new List<string>();
        [HideInInspector]
        [SerializeField]
        private List<float> recordTimes = new List<float>();

        public TimeRecord() { }

        public TimeRecord(TimeRecord other)
        {
            recordIds = new List<string>(other.recordIds);
            recordTimes = new List<float>(other.recordTimes);
        }

        public bool TryToRecordTime(string timeAttackId, float time)
        {
            bool betterTime = false;
        
            int index = recordIds.FindIndex((x) => x.Equals(timeAttackId));
            if (index < 0)
            {
                recordIds.Add(timeAttackId);
                recordTimes.Add(Mathf.Infinity);
                index = recordIds.Count - 1;
            }
        
            if (time < recordTimes[index])
            {
                betterTime = true;
                recordTimes[index] = time;
            }
        
            return betterTime;
        }
        
        public bool HasRecord(string timeAttackId)
            => recordIds.Contains(timeAttackId);
        
        public float GetTime(string timeAttackId)
        {
            float time = Mathf.Infinity;
        
            int index = recordIds.FindIndex((x) => x.Equals(timeAttackId));
            if (index >= 0)
            {
                time = recordTimes[index];
            }
        
            return time;
        }

    }

}