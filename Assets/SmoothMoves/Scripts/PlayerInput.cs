using UnityEngine;

namespace SmoothMovesDemo
{
    /// <summary>
    /// Centralized place to handle all player input.
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector]
        public Vector2 move;
        [HideInInspector]
        public Vector2 aim;

        // Using custom key codes is more work but ensures we avoid input manager conflicts and errors.
        public KeyCode ForwardKey = KeyCode.W;
        public KeyCode BackwardKey = KeyCode.S;
        public KeyCode StrafeLeftKey = KeyCode.A;
        public KeyCode StrafeRightKey = KeyCode.D;
        public KeyCode JumpKey = KeyCode.Space;

        [SerializeField]
        private float _aimSensitivity = 1.0f;
        private Quaternion _bodyRotation;
        private Quaternion _headRotation;

        public bool IsMoving
        {
            get => move.sqrMagnitude > float.Epsilon;
        }

        public bool IsAiming
        {
            get => aim.sqrMagnitude > float.Epsilon;
        }

        private void Update()
        {
            GetMoveInput();
        }

        public void InitAim(Transform body, Transform head)
        {
            _bodyRotation = body.localRotation;
            _headRotation = head.localRotation;
        }

        public void UpdateAim(Transform body, Transform head)
        {
            aim = new Vector2();

            if (Cursor.lockState == CursorLockMode.None)
                return;

            aim.x += Input.GetAxisRaw("Mouse X") * _aimSensitivity;
            aim.y += Input.GetAxisRaw("Mouse Y") * _aimSensitivity;
            _bodyRotation *= Quaternion.Euler(0f, aim.x, 0f);
            _headRotation *= Quaternion.Euler(-aim.y, 0f, 0f);
            _headRotation = ClampXRotation(_headRotation);
            body.localRotation = _bodyRotation;
            head.localRotation = _headRotation;
        }

        private void GetMoveInput()
        {
            move = new Vector2();
            move.y += Input.GetKey(ForwardKey) ? 1.0f : 0.0f;
            move.y -= Input.GetKey(BackwardKey) ? 1.0f : 0.0f;
            move.x += Input.GetKey(StrafeRightKey) ? 1.0f : 0.0f;
            move.x -= Input.GetKey(StrafeLeftKey) ? 1.0f : 0.0f;
        }

        private Quaternion ClampXRotation(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;
            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -90.0F, 90.0F);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}