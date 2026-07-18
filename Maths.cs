using System;

using UnityEngine;


namespace TelemetryLib
{
    internal class Maths
    {
        /// <summary>
        /// Standard gravitational acceleration constant (m/s²).
        /// </summary>
        public const float GRAVITY = 9.81f;

        /// <summary>
        /// The mathematical constant Tau (2π), representing a full circle in radians.
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
        /// Converts a degree value in the range [0, 360) to the half-circle range [-180, 180).
        /// Values greater than or equal to 180 are shifted by -360.
        /// </summary>
        /// <param name="degrees">The angle in degrees, expected in the range [0, 360).</param>
        /// <returns>The equivalent angle in the range [-180, 180).</returns>
        public static float HemiCircle(float degrees)
        {
            return degrees >= 180 ? degrees - 360 : degrees;
        }

        /// <summary>
        /// Converts a degree value in the half-circle range [-180, 180) back to the full-circle range [0, 360).
        /// Negative values are shifted by +360.
        /// </summary>
        /// <param name="degrees">The angle in degrees, expected in the range [-180, 180).</param>
        /// <returns>The equivalent angle in the range [0, 360).</returns>
        public static float ReverseHemiCircle(float degrees)
        {
            return degrees < 0 ? 360 + degrees : degrees;
        }

        /// <summary>
        /// Calculates the signed centripetal acceleration from a velocity and angular velocity vector.
        /// The sign is determined by the Y component of the angular velocity.
        /// </summary>
        /// <param name="velocity">The linear velocity vector.</param>
        /// <param name="angularVelocity">The angular velocity vector.</param>
        /// <returns>
        /// The signed centripetal acceleration, computed as the product of the magnitudes of
        /// <paramref name="velocity"/> and <paramref name="angularVelocity"/>, signed by the Y axis of rotation.
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
        /// <returns>The value mapped into the output range [<paramref name="yMin"/>, <paramref name="yMax"/>].</returns>
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
        /// The mapped value clamped to the bounds of [<paramref name="yMin"/>, <paramref name="yMax"/>].
        /// </returns>
        public static float EnsureMapRange(float x, float xMin, float xMax, float yMin, float yMax)
        {
            return Mathf.Max(Mathf.Min(MapRange(x, xMin, xMax, yMin, yMax), Mathf.Max(yMin, yMax)), Mathf.Min(yMin, yMax));
        }

        /// <summary>
        /// Limits an angle to a maximum output range, remapping values that fall outside the
        /// given input range using a reflected/folded approach.
        /// </summary>
        /// <param name="degrees">The input angle in degrees.</param>
        /// <param name="inputRange">
        /// The symmetrical half-range of the input (i.e. valid input is [-<paramref name="inputRange"/>, <paramref name="inputRange"/>]).
        /// Values outside this range are reflected before mapping.
        /// </param>
        /// <param name="max">The maximum absolute value of the output range.</param>
        /// <returns>
        /// The angle clamped and mapped to the range [-<paramref name="max"/>, <paramref name="max"/>].
        /// </returns>
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
        /// <param name="y">The value whose sign is applied to <paramref name="x"/>.</param>
        /// <returns>A float with the magnitude of <paramref name="x"/> and the sign of <paramref name="y"/>.</returns>
        public static float CopySign(float x, float y)
        {
            return Mathf.Sign(y) * Mathf.Abs(x);
        }

        /// <summary>
        /// Transforms a world-space vector into a local coordinate space defined by the given rotation.
        /// </summary>
        /// <param name="qT">The rotation quaternion representing the local coordinate frame.</param>
        /// <param name="v">The world-space vector to localize.</param>
        /// <returns>The vector expressed in the local coordinate space of <paramref name="qT"/>.</returns>
        public static Vector3 LocalizeVector(Quaternion qT, Vector3 v)
        {
            var q = Quaternion.Inverse(qT) * new Quaternion(v.x, v.y, v.z, 0) * qT;
            return new Vector3(q.x, q.y, q.z);
        }

        /// <summary>
        /// Linearly interpolates between two values, clamping the interpolant to the range [0, 1].
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation factor, clamped to [0, 1].</param>
        /// <returns>The interpolated value between <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }

        /// <summary>
        /// Calculates the interpolant <c>t</c> that produces <paramref name="value"/> when lerping from
        /// <paramref name="a"/> to <paramref name="b"/>, clamped to [0, 1].
        /// Returns <c>0</c> if <paramref name="a"/> and <paramref name="b"/> are approximately equal.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="value">The value within the range to find the interpolant for.</param>
        /// <returns>A value in [0, 1] representing the relative position of <paramref name="value"/> between <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static float InverseLerp(float a, float b, float value)
        {
            return Mathf.Approximately(a, b) ? 0f : Mathf.Clamp01((value - a) / (b - a));
        }
    }


    
}
