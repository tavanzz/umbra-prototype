using UnityEngine;

namespace Umbra.Prototype
{
    public class RotatableSpotlight : MonoBehaviour, IInteractable
    {
        [SerializeField, Tooltip("Transform to rotate. Use the spotlight transform or a lamp pivot.")]
        private Transform rotationPivot;

        [SerializeField, Tooltip("Optional light reference for validation and scene clarity.")]
        private Light controlledLight;

        [SerializeField, Tooltip("Mouse sensitivity while controlling the lamp.")]
        private Vector2 mouseSensitivity = new Vector2(120f, 90f);

        [SerializeField, Tooltip("Minimum and maximum yaw in local degrees.")]
        private Vector2 yawLimits = new Vector2(-80f, 80f);

        [SerializeField, Tooltip("Minimum and maximum pitch in local degrees. Negative values aim downward.")]
        private Vector2 pitchLimits = new Vector2(-75f, 20f);

        [SerializeField, Tooltip("Lock and hide the cursor while rotating the lamp.")]
        private bool lockCursorWhileControlling = true;

        [SerializeField, Tooltip("Message shown by simple UI/debug systems.")]
        private string interactionPrompt = "Press E to control lamp";

        private bool isControlling;
        private float yaw;
        private float pitch;

        public string InteractionPrompt => isControlling ? "Press E to release lamp" : interactionPrompt;
        public bool IsControlling => isControlling;

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

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity.x * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.deltaTime;

            yaw = Mathf.Clamp(yaw, yawLimits.x, yawLimits.y);
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

            rotationPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        public void Interact(GameObject interactor)
        {
            isControlling = !isControlling;

            if (lockCursorWhileControlling)
            {
                Cursor.lockState = isControlling ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !isControlling;
            }
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
