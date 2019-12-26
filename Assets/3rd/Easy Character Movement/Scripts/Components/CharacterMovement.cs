using ECM.Common;
using System.Collections;
using UnityEngine;

namespace ECM.Components
{
    /// <summary>
    /// Character Movement.
    /// 
    /// 'CharacterMovement' is the core of the ECM system and is responsible to perform
    /// all the heavy work to move a character (a.k.a. Character motor),
    /// such as apply forces, impulses, constraints, platforms interaction, etc.
    /// 
    /// This is analogous to the Unity's character controller, but unlike the Unity character controller,
    /// this make use of Rigidbody physics.
    /// 
    /// The controller (eg: 'BaseCharacterController') determines how the Character should be moved,
    /// such as in response from user input, AI, animation, etc.
    /// and feed this information to the 'CharacterMovement' component, which perform the movement. 
    /// </summary>

    public sealed class CharacterMovement : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("The maximum lateral speed this character can move, " +
                 "including movement from external forces like sliding, collisions, etc.")]
        [SerializeField]
        private float _maxLateralSpeed = 10.0f;

        [Tooltip("The maximum rising speed, " +
                 "including movement from external forces like sliding, collisions, etc.")]
        [SerializeField]
        private float _maxRiseSpeed = 20.0f;

        [Tooltip("The maximum falling speed, " +
                 "including movement from external forces like sliding, collisions, etc.")]
        [SerializeField]
        private float _maxFallSpeed = 20.0f;

        [Tooltip("Enable / disable character's custom gravity." +
                 "If enabled the character will be affected by this gravity force.")]
        [SerializeField]
        private bool _useGravity = true;

        [Tooltip("The gravity applied to this character.")]
        [SerializeField]
        private float _gravity = 25.0f;

        [Tooltip("Should the character slide down of a steep slope?")]
        [SerializeField]
        private bool _slideOnSteepSlope;

        [Tooltip("The maximum angle (in degrees) for a walkable slope.")]
        [SerializeField]
        private float _slopeLimit = 45.0f;

        [Tooltip("The amount of gravity to be applied when sliding off a steep slope.")]
        [SerializeField]
        private float _slideGravityMultiplier = 2.0f;

        [Tooltip("When enabled, will force the character to safely follow the walkable 'ground' geometry.")]
        [SerializeField]
        private bool _snapToGround = true;

        [Tooltip("A tolerance of how close to the 'ground' maintain the character.\n" +
                 "0 == no snap at all, 1 == 100% stick to ground.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _snapStrength = 0.5f;

        #endregion

        #region FIELDS
        
        private Coroutine _lateFixedUpdateCoroutine;

        private Rigidbody _rigidbody;

        private Vector3 _normal;

        private float _referenceCastDistance;

        private bool _forceUnground;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The maximum lateral speed this character can move,
        /// including movement from external forces like sliding, collisions, etc.
        /// </summary>

        public float maxLateralSpeed
        {
            get { return _maxLateralSpeed; }
            set { _maxLateralSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The maximum rising speed,
        /// including movement from external forces like sliding, collisions, etc.
        /// </summary>

        public float maxRiseSpeed
        {
            get { return _maxRiseSpeed; }
            set { _maxRiseSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The maximum fall speed,
        /// including movement from external forces like sliding, collisions, etc.
        /// </summary>

        public float maxFallSpeed
        {
            get { return _maxFallSpeed; }
            set { _maxFallSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Enable / disable character's gravity.
        /// If enabled the character will be affected by its custom gravity force.
        /// </summary>

        public bool useGravity
        {
            get { return _useGravity; }
            set
            {
                _useGravity = value;

                // If gravity is disabled,
                // remove any remaining vertical velocity

                if (!_useGravity)
                    velocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            }
        }

        /// <summary>
        /// The amount of gravity to be applied to this character.
        /// We apply gravity manually for more tuning control.
        /// </summary>

        public float gravity
        {
            get { return _gravity; }
            set { _gravity = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Should the character slide down of a steep slope?
        /// </summary>

        public bool slideOnSteepSlope
        {
            get { return _slideOnSteepSlope; }
            set { _slideOnSteepSlope = value; }
        }

        /// <summary>
        /// The maximum angle (in degrees) the slope needs to be before the character starts to slide. 
        /// </summary>

        public float slopeLimit
        {
            get { return _slopeLimit; }
            set { _slopeLimit = Mathf.Clamp(value, 0.0f, 89.0f); }
        }

        /// <summary>
        /// The percentage of gravity that will be applied to the slide.
        /// </summary>

        public float slideGravityMultiplier
        {
            get { return _slideGravityMultiplier; }
            set { _slideGravityMultiplier = Mathf.Max(1.0f, value); }
        }

        /// <summary>
        /// The amount of gravity to apply when sliding.
        /// </summary>

        public float slideGravity
        {
            get { return gravity * slideGravityMultiplier; }
        }

        /// <summary>
        /// If enabled, will prevent the character leaving the ground.
        /// This will cause the character to safely follow the geometry of the ground.
        /// </summary>

        public bool snapToGround
        {
            get { return _snapToGround; }
            set { _snapToGround = value; }
        }

        /// <summary>
        /// The strength of snap to ground.
        /// 0 == no snap at all, 1 == 100% stick to ground.
        /// </summary>

        public float snapStrength
        {
            get { return _snapStrength; }
            set { _snapStrength = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Cached GroundDetection component.
        /// </summary>

        private BaseGroundDetection groundDetection { get; set; }

        /// <summary>
        /// The impact point in world space where the cast hit the 'ground' collider.
        /// If the character is not on 'ground', it represent a point at character's base.
        /// </summary>

        public Vector3 groundPoint
        {
            get { return groundDetection.groundPoint; }
        }

        /// <summary>
        /// The normal of the 'ground' surface.
        /// If the character is not grounded, it will point along character's up axis (transform.up).
        /// </summary>

        public Vector3 groundNormal
        {
            get { return groundDetection.groundNormal; }
        }

        /// <summary>
        /// The real surface normal.
        /// 
        /// This is different from groundNormal, because when SphereCast contacts the edge of a collider
        /// (rather than a face directly on) the hit.normal that is returned is the interpolation of the two normals
        /// of the faces that are joined to that edge.
        /// </summary>

        public Vector3 surfaceNormal
        {
            get { return groundDetection.surfaceNormal; }
        }

        /// <summary>
        /// The distance from the ray's origin to the impact point.
        /// </summary>

        public float groundDistance
        {
            get { return groundDetection.groundDistance; }
        }

        /// <summary>
        /// The Collider that was hit.
        /// This property is null if the cast hit nothing.
        /// </summary>

        public Collider groundCollider
        {
            get { return groundDetection.groundCollider; }
        }

        /// <summary>
        /// The Rigidbody of the collider that was hit.
        /// If the collider is not attached to a rigidbody then it is null.
        /// </summary>

        public Rigidbody groundRigidbody
        {
            get { return groundDetection.groundRigidbody; }
        }

        /// <summary>
        /// Is this character standing on VALID 'ground'?
        /// </summary>

        public bool isGrounded
        {
            get { return groundDetection.isOnGround && groundDetection.isValidGround; }
        }

        /// <summary>
        /// Was (previous fixed frame) the character standing on VALID 'ground'?
        /// </summary>

        public bool wasGrounded
        {
            get
            {
                return groundDetection.prevGroundHit.isOnGround && groundDetection.prevGroundHit.isValidGround;
            }
        }

        /// <summary>
        /// Is this character standing on ANY 'ground'?
        /// </summary>

        public bool isOnGround
        {
            get { return groundDetection.isOnGround; }
        }

        /// <summary>
        /// Was (previous fixed frame) the character standing on ANY 'ground'?
        /// </summary>

        public bool wasOnGround
        {
            get { return groundDetection.prevGroundHit.isOnGround; }
        }

        /// <summary>
        /// Is this character on VALID 'ground'?
        /// </summary>

        public bool isValidGround
        {
            get { return groundDetection.isValidGround; }
        }

        /// <summary>
        /// Is the character standing on a platform? (eg: Kinematic Rigidbody)
        /// </summary>

        public bool isOnPlatform { get; private set; }

        /// <summary>
        /// Is this character standing on the 'solid' side of a ledge?
        /// </summary>

        public bool isOnLedgeSolidSide
        {
            get { return groundDetection.isOnLedgeSolidSide; }
        }

        /// <summary>
        /// Is this character standing on the 'empty' side of a ledge?
        /// </summary>

        public bool isOnLedgeEmptySide
        {
            get { return groundDetection.isOnLedgeEmptySide; }
        }

        /// <summary>
        /// The horizontal distance from the character's bottom position to the ledge contact point.
        /// </summary>

        public float ledgeDistance
        {
            get { return groundDetection.ledgeDistance; }
        }

        /// <summary>
        /// Is the character standing on a step?
        /// </summary>

        public bool isOnStep
        {
            get { return groundDetection.isOnStep; }
        }

        /// <summary>
        /// When on a step, this is the current step height.
        /// </summary>

        public float stepHeight
        {
            get { return groundDetection.stepHeight; }
        }

        /// <summary>
        /// Is the character standing on a slope?
        /// </summary>

        public bool isOnSlope
        {
            get { return groundDetection.isOnSlope; }
        }

        /// <summary>
        /// The 'ground' angle (in degrees) the character is standing on.
        /// </summary>

        public float groundAngle
        {
            get { return groundDetection.groundAngle; }
        }

        /// <summary>
        /// Is a valid slope to walk without slide?
        /// </summary>

        public bool isValidSlope
        {
            get { return !slideOnSteepSlope || groundAngle < slopeLimit; }
        }

        /// <summary>
        /// Is the character sliding off a steep slope?
        /// </summary>

        public bool isSliding { get; private set; }

        /// <summary>
        /// The velocity of the platform the character is standing on,
        /// zero (Vector3.zero) if not on a platform.
        /// </summary>

        public Vector3 platformVelocity { get; private set; }

        /// <summary>
        /// The angular velocity of the platform the character is standing on,
        /// zero (Vector3.zero) if not on a platform.
        /// </summary>

        public Vector3 platformAngularVelocity { get; private set; }

        /// <summary>
        /// Character's velocity vector.
        /// NOTE: When on a platform, this is different of rigidbody's velocity as this
        /// reflects only the character's velocity.
        /// </summary>

        public Vector3 velocity
        {
            get { return _rigidbody.velocity - platformVelocity; }
            set { _rigidbody.velocity = value + platformVelocity; }
        }

        /// <summary>
        /// The character signed forward speed (along its forward vector).
        /// </summary>

        public float forwardSpeed
        {
            get { return Vector3.Dot(velocity, transform.forward); }
        }

        /// <summary>
        /// The character's current rotation.
        /// Setting it comply with the Rigidbody's interpolation setting.
        /// </summary>

        public Quaternion rotation
        {
            get { return _rigidbody.rotation; }
            set { _rigidbody.MoveRotation(value); }
        }

        /// <summary>
        /// The current GroundHit info.
        /// </summary>

        public GroundHit groundHit
        {
            get { return groundDetection.groundHit; }
        }

        /// <summary>
        /// The previous frame GroundHit info.
        /// </summary>

        public GroundHit prevGroundHit
        {
            get { return groundDetection.prevGroundHit; }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Depenetrate this from static objects.
        /// </summary>

        private void OverlapRecovery(ref Vector3 probingPosition, Quaternion probingRotation)
        {
            int overlapCount;
            var overlappedColliders = groundDetection.OverlapCapsule(probingPosition, probingRotation, out overlapCount);

            var capsuleCollider = groundDetection.capsuleCollider;

            for (var i = 0; i < overlapCount; i++)
            {
                var overlappedCollider = overlappedColliders[i];

                var overlappedColliderRigidbody = overlappedCollider.attachedRigidbody;
                if (overlappedColliderRigidbody != null)
                    continue;

                var overlappedColliderTransform = overlappedCollider.transform;

                float distance;
                Vector3 direction;
                if (!Physics.ComputePenetration(capsuleCollider, probingPosition, probingRotation, overlappedCollider,
                    overlappedColliderTransform.position, overlappedColliderTransform.rotation, out direction,
                    out distance))
                    continue;

                probingPosition += direction * distance;
            }
        }

        /// <summary>
        /// Compute 'ground' hit info casting downwards the character's volume,
        /// if found any 'ground' groundHitInfo will contain additional information about it.
        /// Returns true when intersects any 'ground' collider, otherwise false.
        /// </summary>
        /// <param name="probingPosition">A probing position, this can be different from character's position.</param>
        /// <param name="probingRotation">A probing position, this can be different from character's rotation.</param>
        /// <param name="groundHitInfo">If found any 'ground', this will contain more information about it</param>
        /// <param name="scanDistance">The maximum scan distnce (cast distance)</param>

        public bool ComputeGroundHit(Vector3 probingPosition, Quaternion probingRotation, out GroundHit groundHitInfo,
            float scanDistance = Mathf.Infinity)
        {
            groundHitInfo = new GroundHit();
            return groundDetection.ComputeGroundHit(probingPosition, probingRotation, ref groundHitInfo, scanDistance);
        }

        /// <summary>
        /// Compute 'ground' hit info casting downwards the character's volume.
        /// Returns true when the intersects any 'ground' collider, otherwise false.
        /// </summary>
        /// <param name="hitInfo">If found any 'ground', this will contain more information about it</param>
        /// <param name="scanDistance">The maximum scan distnce (cast distance)</param>

        public bool ComputeGroundHit(out GroundHit hitInfo, float scanDistance = Mathf.Infinity)
        {
            var p = transform.position;
            var q = transform.rotation;

            return ComputeGroundHit(p, q, out hitInfo, scanDistance);
        }

        /// <summary>
        /// Rotates the character to face the given direction.
        /// </summary>
        /// <param name="direction">The target direction vector.</param>
        /// <param name="angularSpeed">Maximum turning speed in (deg/s).</param>
        /// <param name="onlyLateral">Should the y-axis be ignored?</param>

        public void Rotate(Vector3 direction, float angularSpeed, bool onlyLateral = true)
        {
            if (onlyLateral)
                direction.y = 0.0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            var targetRotation = Quaternion.LookRotation(direction);
            var newRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation,
                angularSpeed * Mathf.Deg2Rad * Time.deltaTime);

            _rigidbody.MoveRotation(newRotation);
        }

        /// <summary>
        /// Apply a drag to character, an opposing force that scales with current velocity.
        /// Drag reduces the effective maximum speed of the character.
        /// </summary>
        /// <param name="drag">The amount of drag to be applied.</param>
        /// <param name="onlyLateral">Should velocity along the y-axis be ignored?</param>

        public void ApplyDrag(float drag, bool onlyLateral = true)
        {
            var v = onlyLateral ? velocity.onlyXZ() : velocity;

            var d = -drag * v.magnitude * v;
            if (onlyLateral)
                d = Vector3.ProjectOnPlane(d, Vector3.up);

            _rigidbody.AddForce(d, ForceMode.Acceleration);
        }

        /// <summary>
        /// Apply a force to the character's rigidbody.
        /// </summary>
        /// <param name="force">The force to be applied.</param>
        /// <param name="forceMode">Option for how to apply the force.</param>

        public void ApplyForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            _rigidbody.AddForce(force, forceMode);
        }

        /// <summary>
        /// Apply a vertical impulse (along world's up vector).
        /// E.g. Use this to make character jump.
        /// </summary>
        /// <param name="impulse">The magnitude of the impulse to be applied.</param>

        public void ApplyVerticalImpulse(float impulse)
        {
            var verticalImpulse = Vector3.up * impulse;
            _rigidbody.velocity = _rigidbody.velocity.onlyXZ() + verticalImpulse;
        }

        /// <summary>
        /// Halts character's grounding (ground detection, ground snap, etc) to allow safely leave the 'ground'.
        /// Eg: This must be called on Jump to prevent any 'stickness'.
        /// </summary>

        public void DisableGrounding()
        {
            _forceUnground = true;
            groundDetection.castDistance = 0.0f;
        }

        /// <summary>
        /// Defaults ground info.
        /// </summary>

        private void ResetGroundInfo()
        {
            groundDetection.ResetGroundInfo();

            isSliding = false;

            isOnPlatform = false;
            platformVelocity = Vector3.zero;
            platformAngularVelocity = Vector3.zero;

            _normal = Vector3.up;
        }

        /// <summary>
        /// Perform ground detection.
        /// </summary>

        private void DetectGround()
        {
            // Reset 'grounding' info

            ResetGroundInfo();

            // If must unground (eg: on jump), skip ground detection this frame to prevent any 'stickness'

            if (_forceUnground)
                _forceUnground = false;
            else
            {
                // Perform ground detection and update cast distance based on where we are

                groundDetection.DetectGround();
                groundDetection.castDistance = isGrounded ? _referenceCastDistance : 0.0f;
            }

            // If not on 'ground', return

            if (!isOnGround)
                return;

            // Update movement normal, based on where are we standing

            var up = transform.up;

            if (isValidGround)
                _normal = isOnLedgeSolidSide ? up : groundDetection.groundNormal;
            else
            {
                // Flatten normal on invalid 'ground' to prevent climbing it
                
                _normal = Vector3.Cross(Vector3.Cross(up, groundDetection.groundNormal), up).normalized;
            }

            // Check if we are over a rigidbody...

            var otherRigidbody = groundRigidbody;
            if (otherRigidbody == null)
                return;

            if (otherRigidbody.isKinematic)
            {
                // If other rigidbody is a dynamic platform (KINEMATIC rigidbody), update platform info

                isOnPlatform = true;
                platformVelocity = otherRigidbody.GetPointVelocity(groundPoint);
                platformAngularVelocity = Vector3.Project(otherRigidbody.angularVelocity, up);
            }
            else
            {
                // If other is a non-kinematic rigidbody, prevent climbing it

                _normal = Vector3.up;
            }
        }

        /// <summary>
        /// Sweep towards rigidbody's velocity looking for 'ground',
        /// if find valid 'ground', adjust rigidbody's velocity to prevent 'ground' penetration.
        /// </summary>
        
        private void PreventGroundPenetration()
        {
            // If on ground, return

            if (isOnGround)
                return;

            // Sweep towards rigidbody's velocity looking for valid 'ground'

            var v = velocity;

            var speed = v.magnitude;

            var direction = speed > 0.0f ? v / speed : Vector3.zero;
            var distance = speed * Time.deltaTime;

            RaycastHit hitInfo;
            if (!groundDetection.FindGround(direction, out hitInfo, distance))
                return;

            // If no remaining distance, return

            var remainingDistance = distance - hitInfo.distance;
            if (remainingDistance <= 0.0f)
                return;

            // Compute new velocity vector to impact point

            var velocityToGround = direction * (hitInfo.distance / Time.deltaTime);

            // Compute remaining lateral velocity,
            // we use lateral velocity here to prevent sliding down when landing on a slope

            var remainingLateralVelocity = (v - velocityToGround).onlyXZ();

            // Project remaining lateral velocity on plane without speed loss

            remainingLateralVelocity = MathLibrary.GetTangent(remainingLateralVelocity, hitInfo.normal) *
                                       remainingLateralVelocity.magnitude;

            // Compute new final velocity,
            // this is the velocity to contact point plus any remaining lateral velocity projected onto the plane

            var newVelocity = velocityToGround + remainingLateralVelocity;

            // Update rigidbody's velocity

            _rigidbody.velocity = newVelocity;

            // If we have found valid ground reset ground detection cast distance

            groundDetection.castDistance = _referenceCastDistance;
        }

        /// <summary>
        /// Performs character's movement.
        /// Causes an instant velocity change to the rigidbody, ignoring its mass.
        /// </summary>
        /// <param name="desiredVelocity">Target velocity vector.</param>
        /// <param name="maxDesiredSpeed">Target maximum desired speed.</param>
        /// <param name="onlyLateral">Should velocity along the y-axis be ignored?</param>

        private void ApplyMovement(Vector3 desiredVelocity, float maxDesiredSpeed, bool onlyLateral)
        {
            // If onlyLateral, discards any vertical velocity

            var up = Vector3.up;

            if (onlyLateral)
                desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, up);

            // On valid 'ground'

            if (isGrounded)
            {
                if (!slideOnSteepSlope || groundAngle < slopeLimit)
                {
                    // Walkable 'ground' movement

                    desiredVelocity = MathLibrary.GetTangent(desiredVelocity, _normal) *
                                      Mathf.Min(desiredVelocity.magnitude, maxDesiredSpeed);

                    velocity += desiredVelocity - velocity;
                }
                else
                {
                    // Slide off steep slope

                    isSliding = true;

                    var deltaTime = Time.deltaTime;
                    desiredVelocity = Vector3.MoveTowards(velocity, desiredVelocity, gravity * deltaTime);

                    velocity += Vector3.ProjectOnPlane(desiredVelocity - velocity, up) -
                                up * (slideGravity * deltaTime);
                }
            }
            else
            {
                // On Air / invalid 'ground' movement

                if (isOnGround)
                {
                    var isBraking = desiredVelocity.sqrMagnitude < 0.000001f;
                    if (isBraking && onlyLateral)
                    {
                        // On invalid ground, bypass any braking to force slide down of it

                        desiredVelocity = velocity;
                    }
                    else
                    {
                        // If moving towards invalid 'ground', cancel movement towards it to prevent climb it

                        if (Vector3.Dot(desiredVelocity, _normal) <= 0.0f)
                        {
                            var speedLimit = Mathf.Min(desiredVelocity.magnitude, maxDesiredSpeed);

                            var lateralVelocity = Vector3.ProjectOnPlane(velocity, up);

                            desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, _normal) +
                                              Vector3.Project(lateralVelocity, _normal);

                            desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, speedLimit);
                        }
                    }
                }

                // Update velocity

                velocity += onlyLateral
                    ? Vector3.ProjectOnPlane(desiredVelocity - velocity, up)
                    : desiredVelocity - velocity;

                // If desired, apply gravity

                if (useGravity)
                    velocity += Vector3.down * (gravity * Time.deltaTime);
            }
            
            // If moving towards a step,
            // prevent too steep velocities, anything above 75 degrees will be dampened

            if (!isOnStep)
                return;

            var dot = Vector3.Dot(velocity, groundPoint - transform.position);
            if (dot <= 0.0f)
                return;

            var angle = Mathf.Abs(90.0f - Vector3.Angle(up, velocity));
            if (angle < 75.0f)
                return;

            var factor = Mathf.Lerp(1.0f, 0.0f, Mathf.InverseLerp(75.0f, 90.0f, angle));
            factor = factor * (2.0f - factor);

            velocity *= factor;
        }

        /// <summary>
        /// Perform an accelerated friction based movement when on ground.
        /// </summary>

        private void ApplyGroundMovement(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,
            float deceleration, float friction, float brakingFriction)
        {
            var deltaTime = Time.deltaTime;

            // On walkable 'ground'

            if (!slideOnSteepSlope || groundAngle < slopeLimit)
            {
                // Cancel any vertical velocity on landing

                var v = wasGrounded ? velocity : Vector3.ProjectOnPlane(velocity, Vector3.up);

                // Split desiredVelocity into direction and magnitude

                var desiredSpeed = desiredVelocity.magnitude;
                var speedLimit = desiredSpeed > 0.0f ? Mathf.Min(desiredSpeed, maxDesiredSpeed) : maxDesiredSpeed;

                // Only apply braking if there is no acceleration (input == zero || acceleration == 0)

                var desiredDirection = MathLibrary.GetTangent(desiredVelocity, _normal);
                var desiredAcceleration = desiredDirection * acceleration * deltaTime;

                if (desiredAcceleration.isZero() || v.isExceeding(speedLimit))
                {
                    // Reorient velocity along surface

                    v = MathLibrary.GetTangent(v, _normal) * v.magnitude;

                    // Braking friction (drag)

                    v = v * Mathf.Clamp01(1f - brakingFriction * deltaTime);

                    // Deceleration

                    v = Vector3.MoveTowards(v, desiredVelocity, deceleration * deltaTime);
                }
                else
                {
                    // Reorient velocity along surface

                    v = MathLibrary.GetTangent(v, _normal) * v.magnitude;

                    // Friction (grip / snappy)

                    v = v - (v - desiredDirection * v.magnitude) * Mathf.Min(friction * deltaTime, 1.0f);

                    // Acceleration

                    v = Vector3.ClampMagnitude(v + desiredAcceleration, speedLimit);
                }

                // Update character's velocity

                velocity += v - velocity;
            }
            else
            {
                // Slide on steep slope

                isSliding = true;

                desiredVelocity = Vector3.MoveTowards(velocity, desiredVelocity,
                    Mathf.Min(acceleration, gravity) * deltaTime);

                velocity += Vector3.ProjectOnPlane(desiredVelocity - velocity, Vector3.up) +
                            Vector3.down * (slideGravity * deltaTime);
            }
        }

        /// <summary>
        /// Perform an accelerated friction based movement when in air (or invalid ground).
        /// </summary>
        
        private void ApplyAirMovement(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,
            float deceleration, float friction, float brakingFriction, bool onlyLateral = true)
        {
            // If onlyLateral, discards any vertical velocity (leaves rigidbody's vertical velocity unaffected)
            
            var up = Vector3.up;
            var v = onlyLateral ? Vector3.ProjectOnPlane(velocity, up) : velocity;

            // If onlyLateral, discards any vertical velocity

            if (onlyLateral)
                desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, up);

            // On invalid 'ground'

            if (isOnGround)
            {
                // If moving towards invalid 'ground', cancel movement towards it to prevent climb it

                if (Vector3.Dot(desiredVelocity, _normal) <= 0.0f)
                {
                    var maxLength = Mathf.Min(desiredVelocity.magnitude, maxDesiredSpeed);

                    var lateralVelocity = Vector3.ProjectOnPlane(velocity, up);

                    desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, _normal) +
                                      Vector3.Project(lateralVelocity, _normal);

                    desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxLength);
                }

            }

            // Split desiredVelocity into direction and magnitude

            var desiredSpeed = desiredVelocity.magnitude;
            var speedLimit = desiredSpeed > 0.0f ? Mathf.Min(desiredSpeed, maxDesiredSpeed) : maxDesiredSpeed;

            // Only apply braking if there is no acceleration (input == zero || acceleration == 0)

            var deltaTime = Time.deltaTime;

            var desiredDirection = desiredSpeed > 0.0f ? desiredVelocity / desiredSpeed : Vector3.zero;
            var desiredAcceleration = desiredDirection * acceleration * deltaTime;

            if (desiredAcceleration.isZero() || v.isExceeding(speedLimit))
            {
                // If braking...

                if (isOnGround && onlyLateral)
                {
                    // On invalid 'ground' bypass any braking to force to slide down of it
                }
                else
                {
                    // Braking friction (drag)

                    v = v * Mathf.Clamp01(1f - brakingFriction * deltaTime);

                    // Deceleration

                    v = Vector3.MoveTowards(v, desiredVelocity, deceleration * deltaTime);
                }
            }
            else
            {
                // Friction (grip / snappy)

                v = v - (v - desiredDirection * v.magnitude) * Mathf.Min(friction * deltaTime, 1.0f);

                // Acceleration

                v = Vector3.ClampMagnitude(v + desiredAcceleration, speedLimit);
            }

            // Update character's velocity

            if (onlyLateral)
                velocity += Vector3.ProjectOnPlane(v - velocity, up);
            else
                velocity += v - velocity;

            // If desired, apply gravity

            if (useGravity)
                velocity -= up * (gravity * deltaTime);
        }

        /// <summary>
        /// Perform an accelerated friction based character's movement.
        /// </summary>
        /// <param name="desiredVelocity">Target velocity vector.</param>
        /// <param name="maxDesiredSpeed">Target desired speed.</param>
        /// <param name="acceleration">The rate of change of velocity.</param>
        /// <param name="deceleration">The rate at which the character's slows down.</param>
        /// <param name="friction">Friction coefficient to be applied when moving.</param>
        /// <param name="brakingFriction">Friction coefficient to be applied when braking.</param>
        /// <param name="onlyLateral">Should velocity along the y-axis be ignored?</param>

        private void ApplyMovement(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,
            float deceleration, float friction, float brakingFriction, bool onlyLateral)
        {
            if (isGrounded)
            {
                ApplyGroundMovement(desiredVelocity, maxDesiredSpeed, acceleration, deceleration, friction,
                    brakingFriction);
            }
            else
            {
                ApplyAirMovement(desiredVelocity, maxDesiredSpeed, acceleration, deceleration, friction,
                    brakingFriction, onlyLateral);
            }

            // If moving towards a step,
            // prevent too steep velocities, anything above 75 degrees will be dampened

            if (!isOnStep)
                return;

            var dot = Vector3.Dot(velocity, groundPoint - transform.position);
            if (dot <= 0.0f)
                return;

            var angle = Mathf.Abs(90.0f - Vector3.Angle(Vector3.up, velocity));
            if (angle < 75.0f)
                return;

            var factor = Mathf.Lerp(1.0f, 0.0f, Mathf.InverseLerp(75.0f, 90.0f, angle));
            factor = factor * (2.0f - factor);

            velocity *= factor;
        }

        /// <summary>
        /// Make sure we don't move any faster than our maxLateralSpeed.
        /// </summary>

        private void LimitLateralVelocity()
        {
            var lateralVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            if (lateralVelocity.sqrMagnitude > maxLateralSpeed * maxLateralSpeed)
                _rigidbody.velocity += lateralVelocity.normalized * maxLateralSpeed - lateralVelocity;
        }

        /// <summary>
        /// Limit vertical velocity along Y axis.
        /// Make sure we cant fall faster than maxFallSpeed, and cant rise faster than maxRiseSpeed.
        /// </summary>

        private void LimitVerticalVelocity()
        {
            var verticalSpeed = Vector3.Dot(velocity, Vector3.up);
            if (verticalSpeed < -maxFallSpeed)
                _rigidbody.velocity += new Vector3(0.0f, -maxFallSpeed - verticalSpeed, 0.0f);
            else if (verticalSpeed > maxRiseSpeed)
                _rigidbody.velocity += new Vector3(0.0f, maxRiseSpeed - verticalSpeed, 0.0f);
        }

        /// <summary>
        /// Performs character's movement. Causes an instant velocity change to the rigidbody, ignoring its mass.
        /// If useGravity == true will apply custom gravity.
        /// 
        /// Must be called in FixedUpdate.
        /// 
        /// </summary>
        /// <param name="desiredVelocity">Target velocity vector.</param>
        /// <param name="maxDesiredSpeed">Target desired speed.</param>
        /// <param name="onlyLateral">Should velocity along the y-axis be ignored?</param>
        
        public void Move(Vector3 desiredVelocity, float maxDesiredSpeed, bool onlyLateral = true)
        {
            // Perform ground detection

            DetectGround();

            // Perform character's movement

            ApplyMovement(desiredVelocity, maxDesiredSpeed, onlyLateral);

            // If enabled, snap to ground

            if (snapToGround && isOnGround)
                SnapToGround();

            // Speed Limit

            LimitLateralVelocity();
            LimitVerticalVelocity();

            // Prevent ground penetration,
            // this basically 'smooth' character's landing

            PreventGroundPenetration();
        }
        
        /// <summary>
        /// Perform character's movement.
        /// If useGravity == true will apply custom gravity.
        /// 
        /// Must be called in FixedUpdate.
        /// 
        /// </summary>
        /// <param name="desiredVelocity">Target velocity vector.</param>
        /// <param name="maxDesiredSpeed">Target desired speed.</param>
        /// <param name="acceleration">The rate of change of velocity.</param>
        /// <param name="deceleration">The rate at which the character's slows down.</param>
        /// <param name="friction">Friction coefficient to be applied when moving.</param>
        /// <param name="brakingFriction">Friction coefficient to be applied when braking.</param>
        /// <param name="onlyLateral">Should velocity along the y-axis be ignored?</param>
        
        public void Move(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration, float deceleration,
            float friction, float brakingFriction, bool onlyLateral = true)
        {
            // Perform ground detection

            DetectGround();

            // Perform character's movement

            ApplyMovement(desiredVelocity, maxDesiredSpeed, acceleration, deceleration, friction, brakingFriction, onlyLateral);

            // If enabled, snap to ground

            if (snapToGround && isGrounded)
                SnapToGround();

            // Speed Limit

            LimitLateralVelocity();
            LimitVerticalVelocity();

            // Prevent ground penetration,
            // this basically 'smooth' character's landing

            PreventGroundPenetration();
        }

        /// <summary>
        /// When grounded, modify characters velocity to mantain 'ground'.
        /// </summary>
        
        private void SnapToGround()
        {
            // If distance to 'ground' is ~small, return

            if (groundDistance < 0.001f)
                return;

            // On a platform, return (it is handled after physx internal update)

            var otherRigidbody = groundRigidbody;
            if (otherRigidbody != null && otherRigidbody.isKinematic)
                return;

            // Compute snap distance

            const float groundOffset = 0.01f;

            var distanceToGround = Mathf.Max(0.0f, groundDistance - groundOffset);

            // On a ledge 'solid' side, compute a 'flattened' snap distance

            if (isOnLedgeSolidSide)
                distanceToGround = Mathf.Max(0.0f, Vector3.Dot(transform.position - groundPoint, transform.up) - groundOffset);

            // Compute final snap velocity and update character's velocity

            var snapVelocity = transform.up * (-distanceToGround * snapStrength / Time.deltaTime);

            var newVelocity = velocity + snapVelocity;

            velocity = newVelocity.normalized * velocity.magnitude;
        }

        /// <summary>
        /// When on a platform, adjust character's position and velocity to mantain the platform.
        /// </summary>
        /// <param name="probingPosition">A probing position, this can be different from character's current position.</param>
        /// <param name="probingRotation">A probing rotation, this can be different from character's current position.</param>

        private void SnapToPlatform(ref Vector3 probingPosition, ref Quaternion probingRotation)
        {
            // Compute distance to platform

            GroundHit hitInfo;
            if (!ComputeGroundHit(probingPosition, probingRotation, out hitInfo, groundDetection.castDistance))
                return;

            // If not on a platform, return

            var otherRigidbody = hitInfo.groundRigidbody;
            if (otherRigidbody == null || !otherRigidbody.isKinematic)
                return;

            // On a platform...

            // Compute target grounded position

            var up = probingRotation * Vector3.up;
            var groundedPosition = probingPosition - up * hitInfo.groundDistance;

            // Update character's velocity and snap to platform

            var pointVelocity = otherRigidbody.GetPointVelocity(groundedPosition);
            _rigidbody.velocity = velocity + pointVelocity;

            var deltaVelocity = pointVelocity - platformVelocity;
            groundedPosition += deltaVelocity.onlyXZ() * Time.fixedDeltaTime;

            // On ledge 'solid' side, compute a flattened snap point

            if (isOnLedgeSolidSide)
                groundedPosition = MathLibrary.ProjectPointOnPlane(groundedPosition, hitInfo.groundPoint, up);

            // Update character's position

            probingPosition = groundedPosition;

            // On rotating platform, update character's rotation

            if (otherRigidbody.angularVelocity == Vector3.zero)
                return;

            var yaw = Vector3.Project(otherRigidbody.angularVelocity, up);
            var yawRotation = Quaternion.Euler(yaw * Mathf.Rad2Deg * Time.deltaTime);

            probingRotation *= yawRotation;
        }
        
        /// <summary>
        /// Coroutine used to simulate a LateFixedUpdate method.
        /// </summary>

        private IEnumerator LateFixedUpdate()
        {
            var waitTime = new WaitForFixedUpdate();
            
            while (true)
            {
                yield return waitTime;

                // Solve any possible overlap after internal physics update

                var p = transform.position;
                var q = transform.rotation;

                OverlapRecovery(ref p, q);

                // Attemp to snap to a moving platform (if any)
                
                if (isOnGround && isOnPlatform)
                    SnapToPlatform(ref p, ref q);

                // Update rigidbody

                _rigidbody.MovePosition(p);
                _rigidbody.MoveRotation(q);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        #endregion

        #region MONOBEHAVIOUR

        private void OnValidate()
        {
            maxLateralSpeed = _maxLateralSpeed;
            maxRiseSpeed = _maxRiseSpeed;
            maxFallSpeed = _maxFallSpeed;

            useGravity = _useGravity;
            gravity = _gravity;

            slideOnSteepSlope = _slideOnSteepSlope;
            slopeLimit = _slopeLimit;
            slideGravityMultiplier = _slideGravityMultiplier;

            snapToGround = _snapToGround;
            snapStrength = _snapStrength;
        }

        private void Awake()
        {
            // Cache an initialize components

            groundDetection = GetComponent<BaseGroundDetection>();
            if (groundDetection == null)
            {
                Debug.LogError(
                    string.Format(
                        "CharacterMovement: No 'GroundDetection' found for '{0}' game object.\n" +
                        "Please add a 'GroundDetection' component to '{0}' game object",
                        name));

                return;
            }

            _referenceCastDistance = groundDetection.castDistance;

            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError(
                    string.Format(
                        "CharacterMovement: No 'Rigidbody' found for '{0}' game object.\n" +
                        "Please add a 'Rigidbody' component to '{0}' game object",
                        name));

                return;
            }
            
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = false;
            _rigidbody.freezeRotation = true;

            // Attempt to validate frictionless material

            var aCollider = GetComponent<Collider>();
            if (aCollider == null)
                return;

            var physicMaterial = aCollider.sharedMaterial;
            if (physicMaterial != null)
                return;

            physicMaterial = new PhysicMaterial("Frictionless")
            {
                dynamicFriction = 0.0f,
                staticFriction = 0.0f,
                bounciness = 0.0f,
                frictionCombine = PhysicMaterialCombine.Multiply,
                bounceCombine = PhysicMaterialCombine.Average
            };

            aCollider.material = physicMaterial;

            Debug.LogWarning(
                string.Format(
                    "CharacterMovement: No 'PhysicMaterial' found for '{0}'s Collider, a frictionless one has been created and assigned.\n" +
                    "Please add a Frictionless 'PhysicMaterial' to '{0}' game object.",
                    name));
        }

        private void OnEnable()
        {
            // Initialize LateFixedUpdate coroutine

            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);

            _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());
        }

        private void OnDisable()
        {
            // Stop LateFixedUpdate coroutine

            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);
        }

        #endregion
    }
}
