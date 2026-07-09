using UnityEngine;

namespace Umbra.Prototype
{
    public class PlayerInteractionRaycaster : MonoBehaviour
    {
        [SerializeField, Tooltip("Camera used as the origin and direction for interaction raycasts.")]
        private Camera playerCamera;

        [SerializeField, Tooltip("How far the player can interact from the camera.")]
        private float interactionDistance = 3f;

        [SerializeField, Tooltip("Layers that can contain interactable objects.")]
        private LayerMask interactableLayers = ~0;

        [SerializeField, Tooltip("Key used to interact with the currently targeted object.")]
        private KeyCode interactKey = KeyCode.E;

        [SerializeField, Tooltip("Draw the interaction ray in the Scene view while playing.")]
        private bool drawDebugRay = true;

        private IInteractable currentInteractable;

        public IInteractable CurrentInteractable => currentInteractable;

        private void Reset()
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        private void Awake()
        {
            ResolvePlayerCamera();
        }

        private void Update()
        {
            UpdateTarget();

            if (currentInteractable != null && Input.GetKeyDown(interactKey))
            {
                currentInteractable.Interact(gameObject);
            }
        }

        public void Configure(Camera cameraReference)
        {
            playerCamera = cameraReference;
        }

        private void UpdateTarget()
        {
            currentInteractable = null;

            if (playerCamera == null)
            {
                ResolvePlayerCamera();

                if (playerCamera == null)
                {
                    Debug.LogWarning($"{nameof(PlayerInteractionRaycaster)} on {name} needs a player camera reference.", this);
                    return;
                }
            }

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (drawDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.cyan);
            }

            if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayers, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            currentInteractable = hit.collider.GetComponentInParent<IInteractable>();
        }

        private void ResolvePlayerCamera()
        {
            if (playerCamera != null)
            {
                return;
            }

            playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera == null && Camera.main != null)
            {
                playerCamera = Camera.main;
            }
        }
    }
}
