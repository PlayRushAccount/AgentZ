using UnityEngine;

namespace ReadyPlayerMe.Samples.QuickStart
{
    [RequireComponent(typeof(ThirdPersonMovement), typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        private const float FALL_TIMEOUT = 0.15f;

        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int JumpHash = Animator.StringToHash("JumpTrigger");
        private static readonly int FreeFallHash = Animator.StringToHash("FreeFall");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsEquippedHash = Animator.StringToHash("IsEquipped");
        private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");

        [Header("Keys")]
        [SerializeField] private KeyCode equipKey = KeyCode.E;
        [SerializeField] private KeyCode shootKey = KeyCode.Return;

        [Header("Input")]
        [SerializeField] private bool inputEnabled = true;

        public GameObject unequippedGun;
        public GameObject equippedGun;

        private Animator animator;
        private GameObject avatar;

        private ThirdPersonMovement movement;
        private PlayerInput playerInput;
        private CameraFollow cameraFollow;

        private float fallTimeoutDelta;

        private bool isEquipped;
        private bool isAiming;
        private bool isInitialized;

        // ===================== INIT =====================

        private void Init()
        {
            movement = GetComponent<ThirdPersonMovement>();
            playerInput = GetComponent<PlayerInput>();
            playerInput.OnJumpPress += OnJump;

            cameraFollow = FindObjectOfType<CameraFollow>();
            unequippedGun.SetActive(true);
            equippedGun.SetActive(false);
            isInitialized = true;
        }

        public void Setup(GameObject target, RuntimeAnimatorController controller)
        {
            if (!isInitialized)
                Init();

            avatar = target;

            movement.Setup(avatar);

            animator = avatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;

            SetEquipped(false);
            SetAiming(false);
        }

        // ===================== UPDATE =====================

        private void Update()
        {
            if (avatar == null)
                return;

            HandleEquipInput();
            HandleAimingInput();

            if (inputEnabled)
            {
                playerInput.CheckInput();

                movement.Move(
                    playerInput.AxisHorizontal,
                    playerInput.AxisVertical
                );

                movement.SetIsRunning(playerInput.IsHoldingLeftShift);
            }

            

            UpdateAnimator();
        }

        // ===================== INPUT =====================

        private void HandleEquipInput()
        {
            if (Input.GetKeyDown(equipKey))
            {
                SetEquipped(!isEquipped);
                
            }
            if(isEquipped)
            {
                unequippedGun.SetActive(false);
                equippedGun.SetActive(true);
            }else
            {
                unequippedGun.SetActive(true);
                equippedGun.SetActive(false);
            }
        }

        private void HandleAimingInput()
        {
            bool aimingNow = Input.GetKey(shootKey);

            // 🔥 Shooting ALWAYS forces equipped animations
            if (aimingNow)
            {
                if (!isEquipped)
                    SetEquipped(true);

                SetAiming(true);
            }
            else
            {
                SetAiming(false);
            }
        }

        // ===================== STATE =====================

        private void SetEquipped(bool equipped)
        {
            isEquipped = equipped;

            movement.SetEquipped(equipped);

            animator.SetBool(IsEquippedHash, equipped);

            int equippedLayer = animator.GetLayerIndex("EquippedLayer");
            if (equippedLayer != -1)
            {
                animator.SetLayerWeight(equippedLayer, equipped ? 1f : 0f);
            }
           
            // If unequipped manually, cancel aiming
            if (!equipped)
                SetAiming(false);
        }

        private void SetAiming(bool aiming)
        {
            if (isAiming == aiming)
                return;

            isAiming = aiming;

            movement.SetAiming(aiming);

            animator.SetBool(IsAimingHash, aiming);

            if (cameraFollow != null)
                cameraFollow.SetCameraLocked(aiming);
        }

        // ===================== ANIMATION =====================

        private void UpdateAnimator()
        {
            bool grounded = movement.IsGrounded();

            animator.SetFloat(MoveSpeedHash, movement.CurrentMoveSpeed);
            animator.SetBool(IsGroundedHash, grounded);

            if (grounded)
            {
                fallTimeoutDelta = FALL_TIMEOUT;
                animator.SetBool(FreeFallHash, false);
            }
            else
            {
                if (fallTimeoutDelta > 0f)
                    fallTimeoutDelta -= Time.deltaTime;
                else
                    animator.SetBool(FreeFallHash, true);
            }
        }

        private void OnJump()
        {
            if (movement.TryJump())
                animator.SetTrigger(JumpHash);
        }
    }
}
