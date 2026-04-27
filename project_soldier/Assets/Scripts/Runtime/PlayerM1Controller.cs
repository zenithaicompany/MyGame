using UnityEngine;

namespace ProjectSoldier
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerM1Controller : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private SimpleVirtualJoystick moveJoystick;
        [SerializeField] private LookDragInputArea lookArea;
        [SerializeField] private HoldFireButton fireButton;

        [Header("Move")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -20f;

        [Header("Look")]
        [SerializeField] private float lookSensitivityX = 0.12f;
        [SerializeField] private float lookSensitivityY = 0.1f;
        [SerializeField] private float minPitch = -70f;
        [SerializeField] private float maxPitch = 70f;

        [Header("Weapon")]
        [SerializeField] private float fireRate = 10f;
        [SerializeField] private float fireDistance = 100f;
        [SerializeField] private LayerMask hitMask = ~0;

        private float _yaw;
        private float _pitch;
        private float _verticalVelocity;
        private float _nextFireTime;

        public bool IsFireHeld => fireButton != null && fireButton.IsHeld;
        public float CurrentPlanarSpeed { get; private set; }

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
            cameraPivot = transform;
            playerCamera = Camera.main;
        }

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            if (cameraPivot == null)
            {
                cameraPivot = transform;
            }

            Vector3 euler = cameraPivot.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = NormalizePitch(euler.x);
        }

        private void Update()
        {
            UpdateLook();
            UpdateMove();
            UpdateFire();
        }

        private void UpdateLook()
        {
            Vector2 lookInput = Vector2.zero;
            if (lookArea != null)
            {
                lookInput += lookArea.ConsumeDelta();
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            lookInput += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 5f;
#endif

            _yaw += lookInput.x * lookSensitivityX;
            _pitch -= lookInput.y * lookSensitivityY;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void UpdateMove()
        {
            Vector2 moveInput = Vector2.zero;
            if (moveJoystick != null)
            {
                moveInput += moveJoystick.Value;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            moveInput += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);

            Vector3 planar = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
            planar *= moveSpeed;
            CurrentPlanarSpeed = new Vector3(planar.x, 0f, planar.z).magnitude;

            if (characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = planar + Vector3.up * _verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }

        private void UpdateFire()
        {
            bool desktopFireHeld = false;
#if UNITY_EDITOR || UNITY_STANDALONE
            desktopFireHeld = Input.GetMouseButton(0);
#endif
            bool shouldFire = IsFireHeld || desktopFireHeld;

            if (!shouldFire || playerCamera == null)
            {
                return;
            }

            if (Time.time < _nextFireTime)
            {
                return;
            }

            _nextFireTime = Time.time + (1f / Mathf.Max(1f, fireRate));
            FireRay();
        }

        private void FireRay()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, fireDistance, hitMask, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            if (!hit.collider.CompareTag("Target"))
            {
                return;
            }

            TargetHitReaction reaction = hit.collider.GetComponent<TargetHitReaction>();
            if (reaction != null)
            {
                reaction.PlayReaction();
            }
        }

        private static float NormalizePitch(float pitch)
        {
            return pitch > 180f ? pitch - 360f : pitch;
        }
    }
}
