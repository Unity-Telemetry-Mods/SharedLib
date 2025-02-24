using System;

using UnityEngine;


namespace TelemetryLib
{
    internal class Maths
    {
        public const float PI = (float)Math.PI;

        public const float DEG_2_RAD = (float)Math.PI / 180f;

        public const float RAD_2_DEG = 57.29578f;

        public static float HemiCircle(float angle)
        {
            return angle >= 180 ? angle - 360 : angle;
        }

        public static float ReverseHemiCircle(float angle)
        {
            return angle < 0 ? 360 + angle : angle;
        }

        //public float CalculateCentripetalAcceleration(Vector3 velocity, Vector3 angularVelocity)
        //{
        //    var Fc = velocity.Length() * angularVelocity.Length();

        //    return Fc * (angularVelocity.Y >= 0 ? -1 : 1);

        //}

        public static double MapRange(double x, double xMin, double xMax, double yMin, double yMax)
        {
            return yMin + (yMax - yMin) * (x - xMin) / (xMax - xMin);
        }

        public static double EnsureMapRange(double x, double xMin, double xMax, double yMin, double yMax)
        {
            return Math.Max(Math.Min(MapRange(x, xMin, xMax, yMin, yMax), Math.Max(yMin, yMax)), Math.Min(yMin, yMax));
        }


        /// <summary>
        /// Limit angle to a maximum value 
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double LimitAngle(double degrees, float inputRange, float max)
        {
            double v = 0;
            if (Math.Abs(degrees) <= inputRange)
            {
                v = degrees;
            }
            else
            {
                v = (180 - Math.Abs(degrees)) * (degrees < 0 ? -1 : 1);
            }


            return EnsureMapRange(v, -inputRange, inputRange, -max, max);
        }
        
        public static float CopySign(float x, float y)
        {
            return Math.Sign(y) * Math.Abs(x);
        }

        public static Vector3 LocalizeVector(Quaternion qT, Vector3 v)
        {
            var q = Quaternion.Inverse(qT) * new Quaternion(v.x, v.y, v.z, 0) * qT;
            return new Vector3(q.x, q.y, q.z);
        }

    }


    
}
