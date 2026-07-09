using UnityEngine;

namespace Umbra.Prototype
{
    public class RotatableSpotlight : MonoBehaviour, IInteractable
    {
        [SerializeField, Tooltip("Transform to rotate. Use the spotlight transform or a lamp pivot.")]
        private Transform rotationPivot;

        [SerializeField, Tooltip("Optional light reference for validation and scene clarity.")]
        private Light controlledLight;

        [SerializeField, Tooltip("Lamp movement speed while controlling it with WASD or arrow keys.")]
        private float movementSpeed = 2.5f;

        [SerializeField, Tooltip("Minimum world-space movement bounds for the lamp base.")]
        private Vector3 movementBoundsMin = new Vector3(-2f, 2.2f, -0.25f);

        [SerializeField, Tooltip("Maximum world-space movement bounds for the lamp base.")]
        private Vector3 movementBoundsMax = new Vector3(2f, 2.2f, 2.25f);

        [SerializeField, Tooltip("Mouse sensitivity while aiming the lamp.")]
        private Vector2 rotationSensitivity = new Vector2(120f, 90f);

        [SerializeField, Tooltip("Minimum pitch in local degrees. Negative values aim downward.")]
        private float minPitch = -75f;

        [SerializeField, Tooltip("Maximum pitch in local degrees.")]
        private float maxPitch = 20f;

        [SerializeField, Tooltip("Minimum yaw in local degrees.")]
        private float minYaw = -80f;

        [SerializeField, Tooltip("Maximum yaw in local degrees.")]
        private float maxYaw = 80f;

        [SerializeField, Tooltip("Expose whether player movement should pause while the lamp is being controlled.")]
        private bool lockPlayerMovementWhileControlling = true;

        [SerializeField, Tooltip("Lock and hide the cursor while rotating the lamp.")]
        private bool lockCursorWhileControlling = true;

        [SerializeField, Tooltip("Message shown by simple UI/debug systems.")]
        private string interactionPrompt = "Press E to control lamp";

        private bool isControlling;
        private float yaw;
        private float pitch;

        public string InteractionPrompt => isControlling ? "Press E to release lamp" : interactionPrompt;
        public bool IsControlling => isControlling && lockPlayerMovementWhileControlling;
        public bool ShouldLockPlayerMovement => IsControlling;

        private void Reset()
        {
            rotationPivot = transform;
            controlledLight = GetComponentInChildren<Light>();
        }

        private void Awake()
        {
            if (rotationPivot == null)
            {
                rotationPivot = transform;
            }

            if (controlledLight == null)
            {
                controlledLight = GetComponentInChildren<Light>();
            }

            if (controlledLight == null)
            {
                Debug.LogWarning($"{nameof(RotatableSpotlight)} on {name} has no Light assigned. Rotation still works, but assign a spotlight for the prototype.", this);
            }

            Vector3 localEuler = rotationPivot.localEulerAngles;
            yaw = NormalizeAngle(localEuler.y);
            pitch = NormalizeAngle(localEuler.x);
        }

        private void Update()
        {
            if (!isControlling)
            {
                return;
            }

            MoveLamp();
            AimLamp();
        }

        private void MoveLamp()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 movement = Vector3.ClampMagnitude(new Vector3(horizontal, 0f, vertical), 1f);

            if (movement.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Vector3 nextPosition = transform.position + movement * (movementSpeed * Time.deltaTime);
            nextPosition.x = Mathf.Clamp(nextPosition.x, movementBoundsMin.x, movementBoundsMax.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, movementBoundsMin.y, movementBoundsMax.y);
            nextPosition.z = Mathf.Clamp(nextPosition.z, movementBoundsMin.z, movementBoundsMax.z);
            transform.position = nextPosition;
        }

        private void AimLamp()
        {
            yaw += Input.GetAxis("Mouse X") * rotationSensitivity.x * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * rotationSensitivity.y * Time.deltaTime;

            yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            rotationPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        public void Interact(GameObject interactor)
        {
            isControlling = !isControlling;

            if (lockCursorWhileControlling)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void Configure(Transform pivot, Light lightReference)
        {
            rotationPivot = pivot;
            controlledLight = lightReference;
        }

        public void ConfigureMovementBounds(Vector3 minBounds, Vector3 maxBounds)
        {
            movementBoundsMin = minBounds;
            movementBoundsMax = maxBounds;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
