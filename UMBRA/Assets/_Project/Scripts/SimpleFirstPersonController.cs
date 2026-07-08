using UnityEngine;

namespace Umbra.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleFirstPersonController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private PlayerInteractionRaycaster interactionRaycaster;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private bool lockCursorOnStart = true;

        private CharacterController characterController;
        private float cameraPitch;
        private float verticalVelocity;

        private void Reset()
        {
            playerCamera = GetComponentInChildren<Camera>();
            interactionRaycaster = GetComponent<PlayerInteractionRaycaster>();
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }

            if (interactionRaycaster == null)
            {
                interactionRaycaster = GetComponent<PlayerInteractionRaycaster>();
            }

            if (playerCamera == null)
            {
                Debug.LogWarning($"{nameof(SimpleFirstPersonController)} on {name} needs a player camera reference.", this);
            }
        }

        private void Start()
        {
            if (lockCursorOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            if (playerCamera == null || IsControllingSpotlight())
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);
            cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -85f, 85f);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            input = Vector3.ClampMagnitude(input, 1f);

            Vector3 move = transform.TransformDirection(input) * moveSpeed;

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (characterController.isGrounded && Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity += gravity * Time.deltaTime;
            move.y = verticalVelocity;

            characterController.Move(move * Time.deltaTime);
        }

        private bool IsControllingSpotlight()
        {
            return interactionRaycaster != null
                && interactionRaycaster.CurrentInteractable is RotatableSpotlight spotlight
                && spotlight.IsControlling;
        }
    }
}
