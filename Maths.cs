using System;

using UnityEngine;


namespace TelemetryLib
{
    internal class Maths
    {
        public const float GRAVITY = 9.81f;

        public const float Tau = 6.283185307179586476925286766559f;

        public const float PI = (float)Math.PI;

        public const float DEG_2_RAD = (float)Math.PI / 180f;

        public const float RAD_2_DEG = 57.29578f;

        public static float HemiCircle(float degrees)
        {
            return degrees >= 180 ? degrees - 360 : degrees;
        }

        public static float ReverseHemiCircle(float degrees)
        {
            return degrees < 0 ? 360 + degrees : degrees;
        }

        public static float CalculateCentripetalAcceleration(Vector3 velocity, Vector3 angularVelocity)
        {
            return velocity.magnitude * angularVelocity.magnitude* Mathf.Sign(angularVelocity.y);
        }

        public static float MapRange(float x, float xMin, float xMax, float yMin, float yMax)
        {
            return yMin + (yMax - yMin) * (x - xMin) / (xMax - xMin);
        }

        public static float EnsureMapRange(float x, float xMin, float xMax, float yMin, float yMax)
        {
            return Mathf.Max(Mathf.Min(MapRange(x, xMin, xMax, yMin, yMax), Mathf.Max(yMin, yMax)), Mathf.Min(yMin, yMax));
        }


        /// <summary>
        /// Limit degrees to a maximum value 
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float LimitAngle(float degrees, float inputRange, float max)
        {
            float v = 0;
            if (Mathf.Abs(degrees) <= inputRange)
            {
                v = degrees;
            }
            else
            {
                v = (180 - Mathf.Abs(degrees)) * (degrees < 0 ? -1 : 1);
            }


            return EnsureMapRange(v, -inputRange, inputRange, -max, max);
        }
        
        public static float CopySign(float x, float y)
        {
            return Mathf.Sign(y) * Mathf.Abs(x);
        }

        public static Vector3 LocalizeVector(Quaternion qT, Vector3 v)
        {
            var q = Quaternion.Inverse(qT) * new Quaternion(v.x, v.y, v.z, 0) * qT;
            return new Vector3(q.x, q.y, q.z);
        }

    }


    
}
