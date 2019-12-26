#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ECM.Components
{
    /// <summary>
    /// Ground detection default implementation.
    /// This is a feature rich class, capable of detect if the character is on 'ground', and provide additional
    /// information about it.
    /// </summary>

    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class GroundDetection : BaseGroundDetection
    {
        /// <summary>
        /// Perform a downwards sphere cast from capsule's bottom sphere
        /// </summary>
        /// <param name="position">A probing position. This can be different from character's position.</param>
        /// <param name="rotation">A probing rotation. This can be different from character's rotation.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the sweep.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <returns>True when intersects ANY 'ground' collider, otherwise false.</returns>

        private bool BottomSphereCast(Vector3 position, Quaternion rotation, out RaycastHit hitInfo, float distance,
            float backstepDistance = kBackstepDistance)
        {
            var radius = capsuleCollider.radius;
    
            var height = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);
            var center = capsuleCollider.center - Vector3.up * height;

            var origin = position + rotation * center;
            var down = rotation * Vector3.down;

            return SphereCast(origin, radius, down, out hitInfo, distance, backstepDistance);
        }

        /// <summary>
        /// Perfrom a downwards raycast from character's position (capsule's bottom).
        /// </summary>
        /// <param name="position">A probing position. This can be different from character's position.</param>
        /// <param name="rotation">A probing rotation. This can be different from character's rotation.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the sweep.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <returns>True when intersects ANY 'ground' collider, otherwise false.</returns>

        private bool BottomRaycast(Vector3 position, Quaternion rotation, out RaycastHit hitInfo, float distance,
            float backstepDistance = kBackstepDistance)
        {
            var down = rotation * Vector3.down;
            return Raycast(position, down, out hitInfo, distance, backstepDistance) &&
                   SimulateSphereCast(position, rotation, hitInfo.normal, out hitInfo, distance, backstepDistance);
        }

        /// <summary>
        /// Simulate a sphere cast using a raycast.
        /// This is needed to retrieve the correct capsule contact info when using bottom raycast.
        /// </summary>
        /// <param name="position">A probing position. This can be different from character's position.</param>
        /// <param name="rotation">A probing rotation. This can be different from character's rotation.</param>
        /// <param name="normal">The current contact normal.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="distance">The length of the sweep.</param>
        /// <param name="backstepDistance">Probing backstep distance to avoid initial overlaps.</param>
        /// <returns>True when intersects ANY 'ground' collider, otherwise false.</returns>
        
        private bool SimulateSphereCast(Vector3 position, Quaternion rotation, Vector3 normal, out RaycastHit hitInfo,
            float distance = Mathf.Infinity, float backstepDistance = kBackstepDistance)
        {
            var origin = position;
            var up = rotation * Vector3.up;

            var angle = Vector3.Angle(normal, up) * Mathf.Deg2Rad;
            if (angle > 0.0001f)
            {
                var radius = capsuleCollider.radius;

                var x = Mathf.Sin(angle) * radius;
                var y = (1.0f - Mathf.Cos(angle)) * radius;

                var right = Vector3.Cross(normal, up);
                var tangent = Vector3.Cross(right, normal);

                origin += Vector3.ProjectOnPlane(tangent, up).normalized * x + up * y;
            }

            return Raycast(origin, -up, out hitInfo, distance, backstepDistance);
        }

        /// <summary>
        /// Check if we are standing on a ledge or a step and update 'grounding' info.
        /// </summary>
        /// <param name="position">A probing position. This can be different from character's position.</param>
        /// <param name="rotation">A probing rotation. This can be different from character's rotation.</param>
        /// <param name="distance">The cast distance.</param>
        /// <param name="point">The current contact point.</param>
        /// <param name="normal">The current contact normal.</param>
        /// <param name="groundHitInfo">If found any 'ground', hitInfo will contain more information about it.</param>
        
        private void DetectLedgeAndSteps(Vector3 position, Quaternion rotation, ref GroundHit groundHitInfo,
            float distance, Vector3 point, Vector3 normal)
        {
            Vector3 up = rotation * Vector3.up, down = -up;
            var projectedNormal = Vector3.ProjectOnPlane(normal, up).normalized;

            var nearPoint = point + projectedNormal * kHorizontalOffset;
            var farPoint = point - projectedNormal * kHorizontalOffset;

            var ledgeStepDistance = Mathf.Max(kMinLedgeDistance, Mathf.Max(stepOffset, distance));

            RaycastHit nearHitInfo;
            var nearHit = Raycast(nearPoint, down, out nearHitInfo, ledgeStepDistance);
            var isNearGroundValid = nearHit && Vector3.Angle(nearHitInfo.normal, up) < groundLimit;

            RaycastHit farHitInfo;
            var farHit = Raycast(farPoint, down, out farHitInfo, ledgeStepDistance);
            var isFarGroundValid = farHit && Vector3.Angle(farHitInfo.normal, up) < groundLimit;

            // Flush

            if (farHit && !isFarGroundValid)
            {
                groundHitInfo.surfaceNormal = farHitInfo.normal;

                // Attemp to retrieve the 'ground' below us

                RaycastHit secondaryHitInfo;
                if (BottomRaycast(position, rotation, out secondaryHitInfo, distance))
                {
                    // Update ground info and return

                    groundHitInfo.SetFrom(secondaryHitInfo);
                    groundHitInfo.surfaceNormal = secondaryHitInfo.normal;
                }

                return;
            }

            // Steps

            if (isNearGroundValid && isFarGroundValid)
            {
                // Choose nearest normal

                groundHitInfo.surfaceNormal =
                    (point - nearHitInfo.point).sqrMagnitude < (point - farHitInfo.point).sqrMagnitude
                        ? nearHitInfo.normal
                        : farHitInfo.normal;

                // Check if max vertical distance between points is steep enough to be considered as a step

                var nearHeight = Vector3.Dot(point - nearHitInfo.point, up);
                var farHeight = Vector3.Dot(point - farHitInfo.point, up);

                var height = Mathf.Max(nearHeight, farHeight);
                if (height > kMinLedgeDistance && height < stepOffset)
                {
                    groundHitInfo.isOnStep = true;
                    groundHitInfo.stepHeight = height;
                }

                return;
            }

            // Ledges

            // If only one of the near / far rays hit we are on a ledge

            var isOnLedge = isNearGroundValid != isFarGroundValid;
            if (!isOnLedge)
                return;

            // On ledge, compute ledge info and check which side of the ledge we are

            groundHitInfo.surfaceNormal = isFarGroundValid ? farHitInfo.normal : nearHitInfo.normal;

            groundHitInfo.ledgeDistance = Vector3.ProjectOnPlane(point - position, up).magnitude;

            if (isFarGroundValid && groundHitInfo.ledgeDistance > ledgeOffset)
            {
                // On possible ledge 'empty' side,
                // cast downwards using de ledgeOffset as radius

                groundHitInfo.isOnLedgeEmptySide = true;

                var radius = ledgeOffset;
                var offset = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);

                var bottomSphereCenter = capsuleCollider.center - Vector3.up * offset;
                var bottomSphereOrigin = position + rotation * bottomSphereCenter;

                RaycastHit hitInfo;
                if (SphereCast(bottomSphereOrigin, radius, down, out hitInfo, Mathf.Max(stepOffset, distance)))
                {
                    var verticalSquareDistance = Vector3.Project(point - hitInfo.point, up).sqrMagnitude;
                    if (verticalSquareDistance <= stepOffset * stepOffset)
                        groundHitInfo.isOnLedgeEmptySide = false;
                }
            }

            groundHitInfo.isOnLedgeSolidSide = !groundHitInfo.isOnLedgeEmptySide;
        }

        public override bool ComputeGroundHit(Vector3 position, Quaternion rotation, ref GroundHit groundHitInfo,
            float distance = Mathf.Infinity)
        {
            // Downward cast from campsule's bottom sphere (filter any 'wall')

            RaycastHit hitInfo;
            if (BottomSphereCast(position, rotation, out hitInfo, distance) &&
                Vector3.Angle(hitInfo.normal, rotation * Vector3.up) < 89.0f)
            {
                // Update ground hit info

                groundHitInfo.SetFrom(hitInfo);

                // Check if standing on a ledge or step

                DetectLedgeAndSteps(position, rotation, ref groundHitInfo, distance, hitInfo.point, hitInfo.normal);

                // Inform we found 'ground' and if is valid

                groundHitInfo.isOnGround = true;
                groundHitInfo.isValidGround = !groundHitInfo.isOnLedgeEmptySide &&
                                              Vector3.Angle(groundHitInfo.surfaceNormal, Vector3.up) < groundLimit;

                return true;
            }

            // If initial sphere cast fails, or found a 'wall',
            // fallback to a raycast from character's bottom position

            if (!BottomRaycast(position, rotation, out hitInfo, distance))
                return false;

            // If found 'ground', update ground info and return

            groundHitInfo.SetFrom(hitInfo);
            groundHitInfo.surfaceNormal = hitInfo.normal;

            groundHitInfo.isOnGround = true;
            groundHitInfo.isValidGround = Vector3.Angle(groundHitInfo.surfaceNormal, Vector3.up) < groundLimit;

            return true;
        }

        public override bool FindGround(Vector3 direction, out RaycastHit hitInfo, float distance = Mathf.Infinity,
            float backstepDistance = kBackstepDistance)
        {
            var radius = capsuleCollider.radius;

            var height = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);
            var center = capsuleCollider.center - Vector3.up * height;

            var origin = transform.TransformPoint(center);

            if (!SphereCast(origin, radius, direction, out hitInfo, distance, backstepDistance) ||
                Vector3.Angle(hitInfo.normal, Vector3.up) >= 89.0f)
                return false;

            var p = transform.position - transform.up * hitInfo.distance;
            var q = transform.rotation;

            var groundHitInfo = new GroundHit(hitInfo);
            DetectLedgeAndSteps(p, q, ref groundHitInfo, castDistance, hitInfo.point, hitInfo.normal);

            groundHitInfo.isOnGround = true;
            groundHitInfo.isValidGround = !groundHitInfo.isOnLedgeEmptySide &&
                                          Vector3.Angle(groundHitInfo.surfaceNormal, Vector3.up) < groundLimit;

            return groundHitInfo.isOnGround && groundHitInfo.isValidGround;
        }

        protected override void DrawGizmos()
        {
#if UNITY_EDITOR

            // Draw 'ground' hit info

            base.DrawGizmos();

            // Draw 'grounding' volume

            var radius = capsuleCollider.radius;

            var center = capsuleCollider.center;
            var offset = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);
            
            if (!Application.isPlaying)
                offset += castDistance;

            var color = new Color(0.5f, 1.0f, 0.6f);
            if (Application.isPlaying)
                color = isOnGround ? (isValidGround ? Color.green : Color.blue) : Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            Gizmos.color = color;
            Gizmos.DrawWireSphere(center - Vector3.up * offset, radius * 1.01f);

            Gizmos.matrix = Matrix4x4.identity;

            // When on ledge, show ledge offset radius and state (solid / empty side)

            Handles.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            var standingOnLedge = isOnLedgeSolidSide || isOnLedgeEmptySide;
            if (standingOnLedge)
            {
                Handles.color = isOnLedgeSolidSide
                    ? new Color(0.0f, 1.0f, 0.0f, 0.25f)
                    : new Color(1.0f, 0.0f, 0.0f, 0.25f);

                Handles.DrawSolidDisc(Vector3.zero, Vector3.up, ledgeOffset);
            }

            Handles.matrix = Matrix4x4.identity;

#endif
        }
    }
}
