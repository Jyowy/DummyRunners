
using Unity.Mathematics;
using UnityEngine;

namespace Common
{

    public struct TimedAction
    {

        public float timeLimit { get; private set; }
        public float currentTime { get; private set; }
        public bool Completed => currentTime >= timeLimit;

        private AnimationCurve curve;

        public void Set(float timeLimit, float startTime = 0f, AnimationCurve curve = null)
        {
            this.timeLimit = math.max(timeLimit, 0f);
            currentTime = math.clamp(startTime, 0f, this.timeLimit);
            this.curve = curve;
        }

        public void ChangeTimeLimit(float newTimeLimit)
        {
            timeLimit = math.max(newTimeLimit, 0f);
            currentTime = math.clamp(currentTime, 0f, timeLimit);
        }

        public void SetCurrentTime(float newCurrentTime) =>
            currentTime = math.clamp(newCurrentTime, 0f, timeLimit);

        public void SetProgress(float progress) =>
            currentTime = math.lerp(0f, timeLimit, math.clamp(progress, 0f, 1f));

        public void Restart() => currentTime = 0f;

        public void Finish() => currentTime = timeLimit;

        public void Clear()
        {
            timeLimit = 0f;
            currentTime = 0f;
            curve = null;
        }

        public bool Update(float dt)
        {
            currentTime = math.clamp(currentTime + dt, 0f, timeLimit);
            return Completed;
        }

        public float GetProgress(bool evaluated = true)
        {
            if (timeLimit == 0f)
                return 0f;

            float progress = currentTime / timeLimit;

            if (curve != null
                && evaluated)
            {
                progress = curve.Evaluate(progress);
            }

            return progress;
        }

        public float RemainingTime() => timeLimit - currentTime;

    }

}