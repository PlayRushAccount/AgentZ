using UnityEngine;

namespace ReadyPlayerMe.Samples.QuickStart
{
    [RequireComponent(typeof(CharacterController), typeof(GroundCheck))]
    public class ThirdPersonMovement : MonoBehaviour
    {
        private const float TURN_SMOOTH_TIME = 0.08f;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7.5f;
        [SerializeField] private float airControlMultiplier = 0.6f;

        [Header("Jump & Gravity")]
        [SerializeField] private float gravity = -25f;
        [SerializeField] private float jumpHeight = 3f;

        [Header("References")]
        [SerializeField] private CameraFollow cameraFollow;

        private CharacterController controller;
        private GroundCheck groundCheck;

        private GameObject avatar;

        private float verticalVelocity;
        private float turnSmoothVelocity;
        private bool isRunning;
        private bool jumpTrigger;

        // 🔑 STATE FLAGS
        private bool isEquipped;
        private bool isAiming;

        public float CurrentMoveSpeed { get; private set; }

        // ===================== UNITY =====================

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            groundCheck = GetComponent<GroundCheck>();

            if (cameraFollow == null)
                cameraFollow = FindObjectOfType<CameraFollow>();
        }

        public void Setup(GameObject target)
        {
            avatar = target;
        }

        // ===================== PUBLIC API =====================

        public void SetEquipped(bool equipped)
        {
            isEquipped = equipped;
            // Intentionally does NOT affect rotation
        }

        public void SetAiming(bool aiming)
        {
            isAiming = aiming;
        }

        public void SetIsRunning(bool running)
        {
            isRunning = running;
        }

        public bool TryJump()
        {
            if (IsGrounded())
            {
                jumpTrigger = true;
                return true;
            }
            return false;
        }

        // ===================== MOVEMENT =====================

        public void Move(float inputX, float inputY)
        {
            if (cameraFollow == null || avatar == null)
                return;

            Vector3 inputDirection = new Vector3(inputX, 0f, inputY);
            float moveMagnitude = Mathf.Clamp01(inputDirection.magnitude);
            float moveSpeed = isRunning ? runSpeed : walkSpeed;

            ApplyGravityAndJump();

            if (moveMagnitude < 0.01f)
            {
                controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
                CurrentMoveSpeed = 0f;
                return;
            }

            // Camera-relative movement (ALWAYS)
            Vector3 moveDirection =
                Quaternion.Euler(0f, cameraFollow.Yaw, 0f) * inputDirection;

            float controlMultiplier = IsGrounded() ? 1f : airControlMultiplier;

            controller.Move(
                moveDirection.normalized *
                moveSpeed *
                moveMagnitude *
                controlMultiplier *
                Time.deltaTime +
                Vector3.up * verticalVelocity * Time.deltaTime
            );

            CurrentMoveSpeed = moveSpeed * moveMagnitude;

            HandleRotation(moveDirection);
        }

        // ===================== ROTATION =====================

        private void HandleRotation(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude < 0.001f)
                return;

            float targetAngle;

            if (isAiming)
            {
                // 🎯 Shooting → face camera
                targetAngle = cameraFollow.Yaw;
            }
            else
            {
                // 🚶 Normal locomotion → face movement
                targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            }

            float smoothAngle = Mathf.SmoothDampAngle(
                avatar.transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                TURN_SMOOTH_TIME
            );

            avatar.transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        // ===================== GRAVITY & JUMP =====================

        private void ApplyGravityAndJump()
        {
            if (IsGrounded() && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpTrigger && IsGrounded())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTrigger = false;
            }

            verticalVelocity += gravity * Time.deltaTime;
        }

        public bool IsGrounded()
        {
            return groundCheck.IsGrounded() && verticalVelocity <= 0f;
        }
    }
}
