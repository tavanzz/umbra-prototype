using UnityEngine;

namespace Umbra.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleFirstPersonController : MonoBehaviour
    {
<<<<<<< HEAD
        [Header("References")]
        [SerializeField] private Camera playerCamera;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private CharacterController characterController;
        private float verticalVelocity;
        private float cameraPitch;
=======
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
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }

<<<<<<< HEAD
            if (playerCamera == null)
            {
                Debug.LogWarning($"{nameof(SimpleFirstPersonController)} on {name} needs a camera reference.");
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
=======
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
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9
        }

        private void Update()
        {
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
<<<<<<< HEAD
            if (playerCamera == null)
                return;
=======
            if (playerCamera == null || IsControllingSpotlight())
            {
                return;
            }
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);
<<<<<<< HEAD

            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

=======
            cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -85f, 85f);
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9
            playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        private void HandleMovement()
        {
<<<<<<< HEAD
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputZ = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * inputX + transform.forward * inputZ;
            move = Vector3.ClampMagnitude(move, 1f);
=======
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            input = Vector3.ClampMagnitude(input, 1f);

            Vector3 move = transform.TransformDirection(input) * moveSpeed;
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

<<<<<<< HEAD
            if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
=======
            if (characterController.isGrounded && Input.GetButtonDown("Jump"))
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity += gravity * Time.deltaTime;
<<<<<<< HEAD

            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;

            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
=======
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
>>>>>>> 4e729ae6a131de22bba67d651855b08474040ae9
