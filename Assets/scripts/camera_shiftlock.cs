using StarterAssets;
using UnityEngine;

namespace MyGame.CameraShiftLock
{
    public class CameraShiftLock : MonoBehaviour
    {
        public bool isShiftLocked = false;

        private StarterAssetsInputs _input;
        private bool _lastLockState;

        void Start()
        {
            _lastLockState = isShiftLocked;

            // Try to find the StarterAssetsInputs on the same object first, then parent, then anywhere in scene.
            _input = GetComponent<StarterAssetsInputs>();
            if (_input == null)
                _input = GetComponentInParent<StarterAssetsInputs>();
            if (_input == null)
            {
#if UNITY_2023_1_OR_NEWER
                _input = FindAnyObjectByType<StarterAssetsInputs>();
#else
                _input = FindObjectOfType<StarterAssetsInputs>();
#endif
            }
        }

        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (
                UnityEngine.InputSystem.Keyboard.current != null
                && UnityEngine.InputSystem.Keyboard.current.leftAltKey.wasPressedThisFrame
            )
            {
                isShiftLocked = !isShiftLocked;
            }
#else
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isShiftLocked = !isShiftLocked;
            }
#endif

            if (isShiftLocked != _lastLockState)
            {
                _lastLockState = isShiftLocked;

                if (_input != null)
                {
                    _input.SetCursorState(isShiftLocked);
                    // Clear look once when toggling to avoid residual rotation.
                    _input.LookInput(Vector2.zero);
                }
            }

            // Enforce cursor lock/visibility each frame without touching look input.
            if (isShiftLocked)
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                if (Cursor.visible)
                    Cursor.visible = false;

                if (Camera.main != null)
                {
                    float cameraYaw = Camera.main.transform.eulerAngles.y;
                    transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
                }
            }
            else
            {
                if (Cursor.lockState != CursorLockMode.None)
                    Cursor.lockState = CursorLockMode.None;
                if (!Cursor.visible)
                    Cursor.visible = true;
            }
        }
    }
}
