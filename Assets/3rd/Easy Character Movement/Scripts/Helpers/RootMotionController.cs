using UnityEngine;

namespace ECM.Helpers
{
    /// <summary>
    ///
    /// RootMotionController.
    /// 
    /// Helper component to get 'Animator' root-motion velocity vector (animVelocity).
    /// This must be attached to a game object with an 'Animator' component.
    /// 
    /// </summary>

    [RequireComponent(typeof(Animator))]
    public sealed class RootMotionController : MonoBehaviour
    {
        #region FIELDS

        private Animator _animator;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The animation velocity vector.
        /// </summary>

        public Vector3 animVelocity { get; private set; }

        #endregion

        #region MONOBEHAVIOUR

        public void Awake()
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
            {
                Debug.LogError(
                    string.Format(
                        "RootMotionController: There is no 'Animator' attached to the '{0}' game object.\n" +
                        "Please attach a 'Animator' to the '{0}' game object",
                        name));
            }
        }

        public void OnAnimatorMove()
        {
            // Compute movement velocity from animation

            var deltaTime = Time.deltaTime;
            if (deltaTime > 0.0f)
                animVelocity = _animator.deltaPosition / deltaTime;
        }

        #endregion
    }
}