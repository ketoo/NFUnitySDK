using ECM.Common;
using UnityEngine;
using UnityEngine.AI;

namespace ECM.Controllers
{
    /// <summary>
    /// Base Agent (NavMesh) Controller.
    /// 
    /// Base class for a 'NavMeshAgent' controlled characters.
    /// It inherits from 'BaseCharacterController' and extends it to control a 'NavMeshAgent'
    /// and intelligently move in response to mouse click (click to move).
    /// 
    /// As the base character controller, this default behaviour can easily be modified completely replaced in a derived class.
    /// </summary>

    public class BaseAgentController : BaseCharacterController
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Navigation")]
        [Tooltip("Should the agent brake automatically to avoid overshooting the destination point? \n" +
                 "If this property is set to true, the agent will brake automatically as it nears the destination.")]
        [SerializeField]
        private bool _autoBraking = true;

        [Tooltip("Distance from target position to start braking.")]
        [SerializeField]
        private float _brakingDistance = 2.0f;

        [Tooltip("Stop within this distance from the target position.")]
        [SerializeField]
        private float _stoppingDistance = 1.0f;

        [Tooltip("Layers to be considered as ground (walkables). Used by ground click detection.")]
        [SerializeField]
        public LayerMask groundMask = 1;            // Default layer

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached NavMeshAgent component.
        /// </summary>

        public NavMeshAgent agent { get; private set; }

        /// <summary>
        /// Should the agent brake automatically to avoid overshooting the destination point?
        /// If this property is set to true, the agent will brake automatically as it nears the destination.
        /// </summary>

        public bool autoBraking
        {
            get { return _autoBraking; }
            set
            {
                _autoBraking = value;

                if (agent != null)
                    agent.autoBraking = _autoBraking;
            }
        }

        /// <summary>
        /// Distance from target position to start braking.
        /// </summary>

        public float brakingDistance
        {
            get { return _brakingDistance; }
            set { _brakingDistance = Mathf.Max(0.0001f, value); }
        }

        /// <summary>
        /// The ratio (0 - 1 range) of the agent's remaining distance and the braking distance.
        /// 1 If no auto braking or if agent's remaining distance is greater than brakingDistance.
        /// less than 1, if agent's remaining distance is less than brakingDistance.
        /// </summary>

        public float brakingRatio
        {
            get
            {
                if (!autoBraking || agent == null)
                    return 1f;

                return agent.hasPath ? Mathf.Clamp(agent.remainingDistance / brakingDistance, 0.1f, 1f) : 1f;
            }
        }

        /// <summary>
        /// Stop within this distance from the target position.
        /// </summary>

        public float stoppingDistance
        {
            get { return _stoppingDistance; }
            set
            {
                _stoppingDistance = Mathf.Max(0.0f, value);

                if (agent != null)
                    agent.stoppingDistance = _stoppingDistance;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Synchronize the NavMesh Agent simulation position with the character movement position,
        /// we control the agent.
        /// 
        /// NOTE: Must be called in LateUpdate method.
        /// </summary>

        protected void SyncAgent()
        {
            agent.speed = speed;
            agent.angularSpeed = angularSpeed;

            agent.acceleration = acceleration;
            agent.velocity = movement.velocity;

            agent.nextPosition = transform.position;
        }

        /// <summary>
        /// Assign the character's desired move direction (input) based on agent's info.
        /// </summary>

        protected virtual void SetMoveDirection()
        {
            // If agent is not moving, return

            moveDirection = Vector3.zero;

            if (!agent.hasPath)
                return;

            // If destination not reached,
            // feed agent's desired velocity (lateral only) as the character move direction

            if (agent.remainingDistance > stoppingDistance)
                moveDirection = agent.desiredVelocity.onlyXZ();
            else
            {
                // If destination is reached,
                // reset stop agent and clear its path
                
                agent.ResetPath();
            }
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' CalcDesiredVelocity method,
        /// adding auto braking support.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            SetMoveDirection();

            var desiredVelocity = base.CalcDesiredVelocity();
            return autoBraking ? desiredVelocity * brakingRatio : desiredVelocity;
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput method,
        /// to perform custom input code, in this case, click-to-move.
        /// </summary>

        protected override void HandleInput()
        {
            // Handle mouse input

            if (!Input.GetButton("Fire2"))
                return;

            // If mouse right click,
            // found click position in the world

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask.value))
                return;

            // Set agent destination to ground hit point

            agent.SetDestination(hitInfo.point);
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Validate this editor exposed fields.
        /// </summary>

        public override void OnValidate()
        {
            // Calls the parent class' version of method.

            base.OnValidate();

            // This class validation

            autoBraking = _autoBraking;

            brakingDistance = _brakingDistance;
            stoppingDistance = _stoppingDistance;
        }

        /// <summary>
        /// Initialize this.
        /// </summary>

        public override void Awake()
        {
            // Calls the parent class' version of method.

            base.Awake();

            // Cache and initialize components

            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.autoBraking = autoBraking;
                agent.stoppingDistance = stoppingDistance;

                // Turn-off NavMeshAgent control,
                // we control it, not the other way

                agent.updatePosition = false;
                agent.updateRotation = false;

                agent.updateUpAxis = false;
            }
            else
            {
                Debug.LogError(
                    string.Format(
                        "NavMeshAgentCharacterController: There is no 'NavMeshAgent' attached to the '{0}' game object.\n" +
                        "Please add a 'NavMeshAgent' to the '{0}' game object.",
                        name));
            }
        }

        public virtual void LateUpdate()
        {
            // Synchronize agent with character movement

            SyncAgent();
        }

        #endregion
    }
}
