using UnityEngine;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ProjectSoldier
{
    /// <summary>
    /// Minimal desktop test controller for Milestone 0 scene validation.
    /// Auto-attaches to the main camera at scene load.
    /// </summary>
    public class DesktopTestCameraController : MonoBehaviour
    {
        public static Func<Vector2> MoveInputOverride;
        public static Func<Vector2> LookInputOverride;
        public static Func<bool> SprintHeldOverride;
        public static Func<bool> KeyQOverride;
        public static Func<bool> KeyEOverride;
        public static Func<bool> EscapeDownOverride;
        public static Func<bool> RightMouseDownOverride;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float sprintMultiplier = 1.8f;
        [SerializeField] private float verticalSpeed = 4f;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 2.2f;
        [SerializeField] private float pitchMin = -80f;
        [SerializeField] private float pitchMax = 80f;

        private float _yaw;
        private float _pitch;
        private bool _cursorLocked = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureControllerOnMainCamera()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            if (UnityEngine.Object.FindFirstObjectByType<PlayerM1Controller>() != null)
            {
                return;
            }

            if (UnityEngine.Object.FindFirstObjectByType<DesktopTestCameraController>() != null)
            {
                return;
            }

            Camera cam = Camera.main ?? UnityEngine.Object.FindFirstObjectByType<Camera>();
            if (cam == null)
            {
                return;
            }

            cam.gameObject.AddComponent<DesktopTestCameraController>();
        }

        private void Awake()
        {
            Transform spawn = GameObject.Find("PlayerSpawn")?.transform;
            if (spawn != null)
            {
                transform.position = spawn.position;
            }

            Vector3 lookTarget = new Vector3(0f, 1f, 5f);
            transform.rotation = Quaternion.LookRotation((lookTarget - transform.position).normalized, Vector3.up);

            Vector3 euler = transform.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = NormalizePitch(euler.x);
            SetCursorLock(true);
        }

        private void Update()
        {
            if (GetKeyDownEscape())
            {
                SetCursorLock(false);
            }
            else if (!_cursorLocked && GetRightMouseButtonDown())
            {
                SetCursorLock(true);
            }

            if (_cursorLocked)
            {
                UpdateLook();
            }

            UpdateMove();
        }

        private void UpdateLook()
        {
            Vector2 mouseDelta = GetMouseDelta();
            float mouseX = mouseDelta.x;
            float mouseY = mouseDelta.y;

            _yaw += mouseX * mouseSensitivity;
            _pitch -= mouseY * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        private void UpdateMove()
        {
            Vector2 move = GetMoveInput();
            float horizontal = move.x;
            float vertical = move.y;
            float upDown = 0f;

            if (GetKeyE())
            {
                upDown += 1f;
            }

            if (GetKeyQ())
            {
                upDown -= 1f;
            }

            Vector3 planar = (transform.forward * vertical) + (transform.right * horizontal);
            Vector3 verticalMove = Vector3.up * upDown;
            Vector3 direction = (planar + verticalMove).normalized;

            float speed = GetSprintHeld() ? moveSpeed * sprintMultiplier : moveSpeed;
            float appliedSpeed = Mathf.Abs(upDown) > 0f && Mathf.Abs(horizontal) < 0.01f && Mathf.Abs(vertical) < 0.01f
                ? verticalSpeed
                : speed;

            transform.position += direction * appliedSpeed * Time.deltaTime;
        }

        private void SetCursorLock(bool locked)
        {
            _cursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private static float NormalizePitch(float pitch)
        {
            if (pitch > 180f)
            {
                pitch -= 360f;
            }

            return pitch;
        }

        private static bool GetKeyDownEscape()
        {
            if (EscapeDownOverride != null)
            {
                return EscapeDownOverride();
            }

            bool legacy = TryGetLegacyKeyDown(KeyCode.Escape);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.escapeKey.wasPressedThisFrame || legacy;
            }
            return legacy;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(KeyCode.Escape);
#else
            return false;
#endif
        }

        private static bool GetRightMouseButtonDown()
        {
            if (RightMouseDownOverride != null)
            {
                return RightMouseDownOverride();
            }

            bool legacy = TryGetLegacyMouseButtonDown(1);
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                return Mouse.current.rightButton.wasPressedThisFrame || legacy;
            }
            return legacy;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetMouseButtonDown(1);
#else
            return false;
#endif
        }

        private static bool GetKeyE()
        {
            if (KeyEOverride != null)
            {
                return KeyEOverride();
            }

            bool legacy = TryGetLegacyKey(KeyCode.E);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.eKey.isPressed || legacy;
            }
            return legacy;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(KeyCode.E);
#else
            return false;
#endif
        }

        private static bool GetKeyQ()
        {
            if (KeyQOverride != null)
            {
                return KeyQOverride();
            }

            bool legacy = TryGetLegacyKey(KeyCode.Q);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.qKey.isPressed || legacy;
            }
            return legacy;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(KeyCode.Q);
#else
            return false;
#endif
        }

        private static bool GetSprintHeld()
        {
            if (SprintHeldOverride != null)
            {
                return SprintHeldOverride();
            }

            bool legacy = TryGetLegacyKey(KeyCode.LeftShift);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.leftShiftKey.isPressed || legacy;
            }
            return legacy;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(KeyCode.LeftShift);
#else
            return false;
#endif
        }

        private static Vector2 GetMoveInput()
        {
            if (MoveInputOverride != null)
            {
                return MoveInputOverride();
            }

            Vector2 legacyMove = TryGetLegacyMoveVector();
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
            {
                return legacyMove;
            }

            float x = 0f;
            float y = 0f;

            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;
            if (Keyboard.current.wKey.isPressed) y += 1f;

            Vector2 inputSystemMove = new Vector2(x, y);
            return inputSystemMove.sqrMagnitude > legacyMove.sqrMagnitude ? inputSystemMove : legacyMove;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#else
            return Vector2.zero;
#endif
        }

        private static Vector2 GetMouseDelta()
        {
            if (LookInputOverride != null)
            {
                return LookInputOverride();
            }

            Vector2 legacyMouse = TryGetLegacyMouseDelta();
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null)
            {
                return legacyMouse;
            }

            Vector2 inputSystemMouse = Mouse.current.delta.ReadValue() * 0.05f;
            return inputSystemMouse.sqrMagnitude > legacyMouse.sqrMagnitude ? inputSystemMouse : legacyMouse;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
#else
            return Vector2.zero;
#endif
        }

        private static bool TryGetLegacyKeyDown(KeyCode keyCode)
        {
            try
            {
                return Input.GetKeyDown(keyCode);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static bool TryGetLegacyMouseButtonDown(int button)
        {
            try
            {
                return Input.GetMouseButtonDown(button);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static bool TryGetLegacyKey(KeyCode keyCode)
        {
            try
            {
                return Input.GetKey(keyCode);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static Vector2 TryGetLegacyMoveVector()
        {
            try
            {
                return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            catch (InvalidOperationException)
            {
                return Vector2.zero;
            }
        }

        private static Vector2 TryGetLegacyMouseDelta()
        {
            try
            {
                return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            }
            catch (InvalidOperationException)
            {
                return Vector2.zero;
            }
        }
    }
}
