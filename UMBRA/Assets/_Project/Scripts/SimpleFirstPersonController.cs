using UnityEngine;

namespace Umbra.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleFirstPersonController : MonoBehaviour
    {
        [SerializeField, Tooltip("Camera pivot rotated by vertical mouse look.")]
        private Transform cameraPivot;

        [SerializeField, Tooltip("Movement speed in meters per second.")]
        private float moveSpeed = 4.5f;

        [SerializeField, Tooltip("Mouse sensitivity for yaw and pitch.")]
        private Vector2 mouseSensitivity = new Vector2(140f, 110f);

        [SerializeField, Tooltip("Minimum and maximum vertical look angle.")]
        private Vector2 pitchLimits = new Vector2(-80f, 80f);

        [SerializeField, Tooltip("Downward acceleration used to keep the controller grounded.")]
        private float gravity = -18f;

        [SerializeField, Tooltip("Optional lamp control that pauses player movement while the lamp is being aimed.")]
        private RotatableSpotlight lampControl;

        private CharacterController characterController;
        private float pitch;
        private float verticalVelocity;

        private void Reset()
        {
            cameraPivot = GetComponentInChildren<Camera>()?.transform;
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (cameraPivot == null)
            {
                cameraPivot = GetComponentInChildren<Camera>()?.transform;
            }

            if (cameraPivot == null)
            {
                Debug.LogWarning($"{nameof(SimpleFirstPersonController)} on {name} needs a camera pivot or child camera.", this);
            }
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (lampControl != null && lampControl.IsControlling)
            {
                return;
            }

            Look();
            Move();
        }

        public void Configure(Transform pivot, RotatableSpotlight lamp = null)
        {
            cameraPivot = pivot;
            lampControl = lamp;
        }

        private void Look()
        {
            float yaw = Input.GetAxis("Mouse X") * mouseSensitivity.x * Time.deltaTime;
            transform.Rotate(Vector3.up * yaw);

            if (cameraPivot == null)
            {
                return;
            }

            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void Move()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = Vector3.ClampMagnitude(new Vector3(horizontal, 0f, vertical), 1f);
            Vector3 motion = transform.TransformDirection(input) * moveSpeed;

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            motion.y = verticalVelocity;

            characterController.Move(motion * Time.deltaTime);
        }
    }
}
