using System;

using UnityEngine;


namespace Sharedlib
{
    internal class Maths
    {
        /// <summary>
        /// The acceleration due to gravity in m/s².
        /// </summary>
        public const float GRAVITY = 9.81f;

        /// <summary>
        /// The circle constant Tau (τ), equal to 2π (~6.2832). Represents a full rotation in radians.
        /// </summary>
        public const float Tau = 6.283185307179586476925286766559f;

        /// <summary>
        /// The mathematical constant Pi (π), cast to a single-precision float.
        /// </summary>
        public const float PI = (float)Math.PI;

        /// <summary>
        /// Conversion factor from degrees to radians (π / 180).
        /// </summary>
        public const float DEG_2_RAD = (float)Math.PI / 180f;

        /// <summary>
        /// Conversion factor from radians to degrees (180 / π).
        /// </summary>
        public const float RAD_2_DEG = 57.29578f;

        /// <summary>
        /// Converts a bearing in degrees [0, 360) to a signed heading in the range (-180, 180].
        /// Values greater than or equal to 180 are shifted by subtracting 360.
        /// </summary>
        /// <param name="degrees">The input angle in degrees, expected in the range [0, 360).</param>
        /// <returns>The equivalent signed angle in the range (-180, 180].</returns>
        public static float HemiCircle(float degrees)
        {
            return degrees >= 180 ? degrees - 360 : degrees;
        }

        /// <summary>
        /// Converts a signed heading in degrees to an unsigned bearing in the range [0, 360).
        /// Negative values are shifted by adding 360.
        /// </summary>
        /// <param name="degrees">The input angle in degrees, expected in the range (-180, 180].</param>
        /// <returns>The equivalent unsigned angle in the range [0, 360).</returns>
        public static float ReverseHemiCircle(float degrees)
        {
            return degrees < 0 ? 360 + degrees : degrees;
        }

        /// <summary>
        /// Calculates the signed centripetal acceleration given a linear velocity and an angular velocity.
        /// The sign is determined by the Y component of the angular velocity vector.
        /// </summary>
        /// <param name="velocity">The linear velocity vector in m/s.</param>
        /// <param name="angularVelocity">The angular velocity vector in rad/s.</param>
        /// <returns>
        /// The signed centripetal acceleration in m/s², computed as
        /// |velocity| × |angularVelocity| × sign(angularVelocity.y).
        /// </returns>
        public static float CalculateCentripetalAcceleration(Vector3 velocity, Vector3 angularVelocity)
        {
            return velocity.magnitude * angularVelocity.magnitude* Mathf.Sign(angularVelocity.y);
        }

        /// <summary>
        /// Linearly maps a value from one range to another.
        /// </summary>
        /// <param name="x">The input value to map.</param>
        /// <param name="xMin">The minimum of the input range.</param>
        /// <param name="xMax">The maximum of the input range.</param>
        /// <param name="yMin">The minimum of the output range.</param>
        /// <param name="yMax">The maximum of the output range.</param>
        /// <returns>The value mapped linearly from [<paramref name="xMin"/>, <paramref name="xMax"/>] to [<paramref name="yMin"/>, <paramref name="yMax"/>].</returns>
        public static float MapRange(float x, float xMin, float xMax, float yMin, float yMax)
        {
            return yMin + (yMax - yMin) * (x - xMin) / (xMax - xMin);
        }

        /// <summary>
        /// Linearly maps a value from one range to another, clamping the result to the output range.
        /// </summary>
        /// <param name="x">The input value to map.</param>
        /// <param name="xMin">The minimum of the input range.</param>
        /// <param name="xMax">The maximum of the input range.</param>
        /// <param name="yMin">The minimum of the output range.</param>
        /// <param name="yMax">The maximum of the output range.</param>
        /// <returns>
        /// The value mapped linearly from [<paramref name="xMin"/>, <paramref name="xMax"/>] to
        /// [<paramref name="yMin"/>, <paramref name="yMax"/>], clamped within the output range.
        /// </returns>
        public static float EnsureMapRange(float x, float xMin, float xMax, float yMin, float yMax)
        {
            return Mathf.Max(Mathf.Min(MapRange(x, xMin, xMax, yMin, yMax), Mathf.Max(yMin, yMax)), Mathf.Min(yMin, yMax));
        }


        /// <summary>
        /// Limits an angle to a maximum output value by mapping it from a given input range.
        /// Angles outside the input range are wrapped before mapping.
        /// </summary>
        /// <param name="degrees">The input angle in degrees.</param>
        /// <param name="inputRange">The half-range of the expected input (e.g., 180 for a full circle).</param>
        /// <param name="max">The maximum absolute value of the output angle.</param>
        /// <returns>The clamped and mapped angle in degrees, within [-<paramref name="max"/>, <paramref name="max"/>].</returns>
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
        
        /// <summary>
        /// Returns the value of <paramref name="x"/> with the sign of <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The value whose magnitude is used.</param>
        /// <param name="y">The value whose sign is applied.</param>
        /// <returns>A float with the magnitude of <paramref name="x"/> and the sign of <paramref name="y"/>.</returns>
        public static float CopySign(float x, float y)
        {
            return Mathf.Sign(y) * Mathf.Abs(x);
        }

        /// <summary>
        /// Transforms a world-space vector into the local space of the given rotation.
        /// </summary>
        /// <param name="qT">The rotation quaternion representing the local frame's orientation.</param>
        /// <param name="v">The world-space vector to transform.</param>
        /// <returns>The vector expressed in the local space defined by <paramref name="qT"/>.</returns>
        public static Vector3 LocalizeVector(Quaternion qT, Vector3 v)
        {
            var q = Quaternion.Inverse(qT) * new Quaternion(v.x, v.y, v.z, 0) * qT;
            return new Vector3(q.x, q.y, q.z);
        }

    }


    
}
