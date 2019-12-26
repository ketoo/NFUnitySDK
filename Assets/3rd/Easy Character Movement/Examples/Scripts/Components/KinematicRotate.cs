using ECM.Examples.Common;
using UnityEngine;

namespace ECM.Examples.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicRotate : MonoBehaviour
    {
        #region FIELDS

        [SerializeField]
        private float _rotationSpeed = 30.0f;

        #endregion

        #region PRIVATE FIELDS

        private Rigidbody _rigidbody;

        private float _angle;

        #endregion

        #region PROPERTIES

        public float rotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = Mathf.Clamp(value, -360.0f, 360.0f); }
        }

        public float angle
        {
            get { return _angle; }
            set { _angle = Utils.WrapAngle(value); }
        }

        #endregion

        #region MONOBEHAVIOUR

        public void OnValidate()
        {
            rotationSpeed = _rotationSpeed;
        }

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        public void FixedUpdate()
        {
            angle += rotationSpeed * Time.deltaTime;
            
            var rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            _rigidbody.MoveRotation(rotation);
        }

        #endregion
    }
}
