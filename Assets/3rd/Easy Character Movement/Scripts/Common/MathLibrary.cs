using UnityEngine;

namespace ECM.Common
{
    public static class MathLibrary
    {
        /// <summary>
        /// Returns the direction adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>
        /// <param name="direction">The direction to be adjusted.</param>
        /// <param name="normal">The surface normal.</param>
        /// <param name="up">The given up-axis.</param>
        
        public static Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            var right = Vector3.Cross(direction, up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }

        /// <summary>
        /// Returns the direction adjusted to be tangent to a specified surface normal relatively to the world's up axis.
        /// </summary>
        /// <param name="direction">The direction to be adjusted.</param>
        /// <param name="normal">The surface normal.</param>

        public static Vector3 GetTangent(Vector3 direction, Vector3 normal)
        {
            return GetTangent(direction, normal, Vector3.up);
        }

        /// <summary>
        /// Projects a given point onto the plane defined by plane origin and plane normal.
        /// </summary>
        /// <param name="point">The point to be projected.</param>
        /// <param name="planeOrigin">A point on the plane.</param>
        /// <param name="planeNormal">The plane normal.</param>

        public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            var toPoint = point - planeOrigin;
            var toPointProjected = Vector3.Project(toPoint, planeNormal.normalized);

            return point - toPointProjected;
        }
    }
}

