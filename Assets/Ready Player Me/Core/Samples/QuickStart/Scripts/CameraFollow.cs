using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.Samples.QuickStart
{
    public class CameraFollow : MonoBehaviour
    {
        private const string TARGET_NOT_SET = "Target not set, disabling component";
        private readonly string TAG = typeof(CameraFollow).ToString();

        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform target;

        [Header("Camera Position")]
        [SerializeField] private float cameraDistance = -3.5f;
        [SerializeField] private float shoulderOffset = 0.6f;
        [SerializeField] private float heightOffset = 1.6f;

        [Header("Rotation")]
        [SerializeField] private float mouseSensitivity = 180f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 60f;

        [Header("Camera Collision")]
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private float collisionOffset = 0.15f;
        [SerializeField] private LayerMask cameraCollisionMask;

        public float Yaw => yaw;

        private float yaw;
        private float pitch;
        private bool isFollowing;
        private bool isCameraLocked;

        // ===================== UNITY =====================

        private void Start()
        {
            if (target == null)
            {
                SDKLogger.LogWarning(TAG, TARGET_NOT_SET);
                enabled = false;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector3 startRotation = transform.eulerAngles;
            yaw = startRotation.y;
            pitch = startRotation.x;

            StartFollow();
        }

        private void LateUpdate()
        {
            if (!isFollowing) return;

            if (!isCameraLocked)
            {
                HandleRotation();
            }

            UpdateCameraPosition();
        }

        // ===================== ROTATION =====================

        private void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // ===================== POSITION =====================

        private void UpdateCameraPosition()
        {
            // Pivot follows player
            transform.position = target.position + Vector3.up * heightOffset;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            Vector3 desiredLocalOffset =
                new Vector3(shoulderOffset, 0f, cameraDistance);

            Vector3 desiredWorldPosition =
                transform.TransformPoint(desiredLocalOffset);

            ApplyCameraCollision(desiredWorldPosition);
        }

        private void ApplyCameraCollision(Vector3 desiredWorldPosition)
        {
            Vector3 pivotPosition = transform.position;
            Vector3 direction = desiredWorldPosition - pivotPosition;
            float distance = direction.magnitude;

            if (Physics.SphereCast(
                pivotPosition,
                collisionRadius,
                direction.normalized,
                out RaycastHit hit,
                distance,
                cameraCollisionMask))
            {
                float adjustedDistance = hit.distance - collisionOffset;
                adjustedDistance = Mathf.Max(adjustedDistance, 0.5f);

                playerCamera.transform.position =
                    pivotPosition + direction.normalized * adjustedDistance;
            }
            else
            {
                playerCamera.transform.position = desiredWorldPosition;
            }

            playerCamera.transform.rotation =
                Quaternion.Euler(pitch, yaw, 0f);
        }

        // ===================== PUBLIC API =====================

        public void SetCameraLocked(bool locked)
        {
            isCameraLocked = locked;
        }

        public void StartFollow() => isFollowing = true;
        public void StopFollow() => isFollowing = false;
    }
}
