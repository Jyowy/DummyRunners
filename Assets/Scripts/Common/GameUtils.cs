using Unity.Mathematics;
using UnityEngine;

namespace Common
{

    public static class GameUtils
    {

        public static string GetMoneyFormatted(float money) =>
            string.Format("{0:N0}.- RCI", money);

        public static string GetTimeFormatted(float time, bool display_cseconds = true)
        {
            int minutes = (int)math.floor(time / 60f);
            time -= minutes * 60f;

            int seconds = (int)math.floor(time);
            time -= seconds;

            string formatted;
            if (display_cseconds)
            {
                int cseconds = (int)math.floor(time * 100f);
                formatted = string.Format("{0:D}:{1:D2}.{2:D2}",
                    minutes, seconds, cseconds);
            }
            else
            {
                formatted = string.Format("{0:D}:{1:D2}",
                    minutes, seconds);
            }
            return formatted;
        }

        public static string GetTimeAsCountdownFormatted(float time, float secondsSize, float csecondsSize)
        {
            int seconds = (int)math.floor(time);
            time -= seconds;

            int cseconds = (int)math.floor(time * 100f);

            string formatted = string.Format("<size={2}>{0:D}</size><size={3}>.{1:D2}</size>",
                seconds, cseconds,
                secondsSize, csecondsSize);
            return formatted;
        }

        public static string GetPositionFormatted(int position)
        {
            string suffix = "th";

            int lastDigit = position % 10;

            if (lastDigit == 1)
                suffix = "st";
            else if (lastDigit == 2)
                suffix = "nd";
            else if (lastDigit == 3)
                suffix = "rd";

            return string.Format("{0}{1}", position, suffix);
        }

        private static System.Random random;
        private static bool randomInitialized = false;

        private static System.Random RandomInstance
        {
            get
            {
                if (!randomInitialized)
                {
                    Debug.LogWarningFormat("Initialize random seed!");
                    randomInitialized = true;
                    int seed = (int)(math.clamp(UnityEngine.Random.value, 0.01f, 0.99f) * 11111707f);
                    random = new System.Random(seed);
                }
                return random;
            }
        }

        public static int GetRandomNumber(int maxValue)
            => RandomInstance.Next() % maxValue;

        public static float GetRandomNumber(float maxValue)
            => UnityEngine.Mathf.Repeat((float)RandomInstance.NextDouble(), maxValue);

        public static bool GetRandomBool(float trueRatio = 0.5f)
        {
            return GetRandomNumber(1f) < trueRatio;
        }

        public static Color GetRandomColor(bool includeAlpha = false)
        {
            return new Color(GetRandomNumber(1f), GetRandomNumber(1f), GetRandomNumber(1f),
                includeAlpha ? GetRandomNumber(1f) : 1f);
        }

    }

}