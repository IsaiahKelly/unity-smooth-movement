using UnityEngine;

namespace SmoothMovesDemo
{
    /// <summary>
    /// Physics based first-person player control.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PhysicsMotor : MonoBehaviour
    {
        [Tooltip("Transform that will be aimed up and down, independently of body.")]
        public Transform Head;
        [Min(1.0f)]
        public float MaxSpeed = 10.0f;
        [Min(10.0f)]
        public float JumpForce = 50.0f;
        [Min(0.0f)]
        public float GroundCheckDistance = 0.01f;
        [Min(0.0f)]
        public float GroundstickDistance = 0.5f;
        [Min(1.0f)]
        [Tooltip("How quickly we come to a stop when there is no input.")]
        public float DecelerationRate = 20.0f;
        [Tooltip("Optional: Amount to reduce SphereCast radius to avoid getting stuck.")]
        [Min(0.0f)]
        public float ShellOffset;

        private PlayerInput _input;
        private Rigidbody _rigidBody;
        private CapsuleCollider _capsule;
        private bool _jumped;
        private bool _wasGrounded;
        private bool _isJumping;
        private bool _isGrounded;
        // The transform that will be rotated horizontally when aiming.
        private Transform _rootObject;

        public Vector3 Velocity
        {
            get { return _rigidBody.velocity; }
        }

        public bool IsGrounded
        {
            get { return _isGrounded; }
        }

        public bool IsJumping
        {
            get { return _isJumping; }
        }

        private void Start()
        {
            // Get references.
            _input = FindObjectOfType<PlayerInput>();
            _rootObject = GetComponent<Transform>();
            _rigidBody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _input.InitAim(_rootObject, Head);
        }

        private void Update()
        {
            RotateView();

            // Check for input here since doing so in FixedUpdate might miss it.
            if (Input.GetButtonDown("Jump") && !_jumped)
            {
                _jumped = true;
            }
        }

        private void FixedUpdate()
        {
            if (!_input)
                return;

            GroundCheck();

            if (_input.IsMoving)
            {
                // Apply movement relative to facing direction.
                Vector3 relativeMovement = Head.forward * _input.move.y + Head.right * _input.move.x;
                relativeMovement *= MaxSpeed;

                if (_rigidBody.velocity.sqrMagnitude < (MaxSpeed * MaxSpeed))
                {
                    _rigidBody.AddForce(relativeMovement, ForceMode.Impulse);
                }
            }

            if (_isGrounded)
            {
                _rigidBody.drag = 5.0f;

                if (_jumped)
                {
                    _rigidBody.drag = 0.0f;
                    _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0.0f, _rigidBody.velocity.z);
                    _rigidBody.AddForce(new Vector3(0.0f, JumpForce, 0.0f), ForceMode.Impulse);
                    _isJumping = true;
                }
            }
            else
            {
                _rigidBody.drag = 0.0f;

                if (_wasGrounded && !_isJumping)
                {
                    StickToGround();
                }
            }

            _jumped = false;
        }

        private void StickToGround()
        {
            if (Physics.SphereCast(_rootObject.position, _capsule.radius * (1.0f - ShellOffset), Vector3.down, out RaycastHit hitInfo,
                                   ((_capsule.height / 2f) - _capsule.radius) +
                                   GroundstickDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85.0f)
                {
                    _rigidBody.velocity = Vector3.ProjectOnPlane(_rigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private void RotateView()
        {
            // get the rotation before it's changed
            float oldYRotation = _rootObject.eulerAngles.y;
            _input.UpdateAim(_rootObject, Head);

            // Change rigidbody velocity to match new facing direction.
            Quaternion velRotation = Quaternion.AngleAxis(_rootObject.eulerAngles.y - oldYRotation, Vector3.up);
            _rigidBody.velocity = velRotation * _rigidBody.velocity;
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            _wasGrounded = _isGrounded;

            if (Physics.SphereCast(_rootObject.position, _capsule.radius * (1.0f - ShellOffset), Vector3.down, out RaycastHit hitInfo,
                                   ((_capsule.height / 2.0f) - _capsule.radius) + GroundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }

            if (!_wasGrounded && _isGrounded && _isJumping)
            {
                _isJumping = false;
            }
        }
    }
}
