using UnityEngine;

namespace ECM.Examples.Common
{
    public static class Utils
    {
        /// <summary>
        /// Sinusoidal ease function.
        /// </summary>

        public static float EaseInOut(float time, float duration)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * time / duration) - 1.0f);
        }

        /// <summary>
        /// Wrap an angle in degrees around 360 degrees.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>

        public static float WrapAngle(float degrees)
        {
            if (degrees > 360.0f)
                degrees -= 360.0f;
            else if (degrees < 0.0f)
                degrees += 360.0f;

            return degrees;
        }
    }
}