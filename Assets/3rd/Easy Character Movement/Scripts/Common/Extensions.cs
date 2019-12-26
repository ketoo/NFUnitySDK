using UnityEngine;

namespace ECM.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Checks whether value is near to zero within a tolerance.
        /// </summary>

        public static bool isZero(this float value)
        {
            const float tolerance = 0.0000000001f;

            return Mathf.Abs(value) < tolerance;
        }

        /// <summary>
        /// Returns a copy of given vector with only X component of the vector.
        /// </summary>

        public static Vector3 onlyX(this Vector3 vector3)
        {
            vector3.y = 0.0f;
            vector3.z = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with only Y component of the vector.
        /// </summary>

        public static Vector3 onlyY(this Vector3 vector3)
        {
            vector3.x = 0.0f;
            vector3.z = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with only Z component of the vector.
        /// </summary>

        public static Vector3 onlyZ(this Vector3 vector3)
        {
            vector3.x = 0.0f;
            vector3.y = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with only X and Z components of the vector.
        /// </summary>

        public static Vector3 onlyXZ(this Vector3 vector3)
        {
            vector3.y = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>

        public static bool isZero(this Vector3 vector3)
        {
            return vector3.sqrMagnitude < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>

        public static bool isExceeding(this Vector3 vector3, float magnitude)
        {
            // Allow 1% error tolerance, to account for numeric imprecision.

            const float errorTolerance = 1.01f;

            return vector3.sqrMagnitude > magnitude * magnitude * errorTolerance;
        }

        /// <summary>
        /// Returns a copy of given vector with a magnitude of 1,
        /// and outs its magnitude before normalization.
        /// 
        /// If the vector is too small to be normalized a zero vector will be returned.
        /// </summary>

        public static Vector3 normalized(this Vector3 vector3, out float magnitude)
        {
            magnitude = vector3.magnitude;
            if (magnitude > 9.99999974737875E-06)
                return vector3 / magnitude;

            magnitude = 0.0f;

            return Vector3.zero;
        }

        /// <summary>
        /// Returns a copy of given vector with its magnitude clamped to 0 and 1,
        /// and outs its magnitude before clamp.
        /// </summary>

        public static Vector3 clamped(this Vector3 vector3, out float magnitude)
        {
            magnitude = vector3.magnitude;

            return magnitude > 1.0f ? vector3 / magnitude : vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with its magnitude clamped to maxLength.
        /// </summary>

        public static Vector3 clampedTo(this Vector3 vector3, float maxLength)
        {
            if (vector3.sqrMagnitude > maxLength * (double) maxLength)
                return vector3.normalized * maxLength;

            return vector3;
        }

        /// <summary>
        /// Transform a given vector to be realtive to target transform.
        /// Eg: Use to perform movement relative to camera's view direction.
        /// </summary>

        public static Vector3 relativeTo(this Vector3 vector3, Transform target, bool onlyLateral = true)
        {
            var forward = onlyLateral ? target.forward.onlyXZ() : target.forward;

            return Quaternion.LookRotation(forward) * vector3;
        }
    }
}