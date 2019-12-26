using System;
using UnityEngine;

namespace ECM.Components
{
    [Obsolete("This has been replaced for the more feature-rich GroundDetection component. " +
              "Please feel free to remove this class as this will be removed in next update.")]
    public sealed class BoxGroundDetection : BaseGroundDetection
    {
        public override bool ComputeGroundHit(Vector3 position, Quaternion rotation, ref GroundHit groundHitInfo,
            float distance = Mathf.Infinity)
        {
            throw new System.NotImplementedException();
        }

        public override bool FindGround(Vector3 direction, out RaycastHit hitInfo, float distance = Mathf.Infinity,
            float backstepDistance = kBackstepDistance)
        {
            throw new System.NotImplementedException();
        }
    }
}