using UnityEngine;

namespace Umbra.Prototype
{
    public class ShadowSolidifier : MonoBehaviour
    {
        [SerializeField, Tooltip("Key used to create the temporary solid shadow platform.")]
        private KeyCode solidifyKey = KeyCode.F;

        [SerializeField, Tooltip("Transform of the light casting the fake shadow.")]
        private Transform lightSource;

        [SerializeField, Tooltip("Object whose shadow is being approximated.")]
        private Transform shadowCaster;

        [SerializeField, Tooltip("Floor or receiver plane where the solid shadow should appear. Its up axis is used as the plane normal.")]
        private Transform receiverPlane;

        [SerializeField, Tooltip("Prefab for the walkable solid shadow. Should include a visible mesh and collider.")]
        private GameObject solidShadowPrefab;

        [SerializeField, Tooltip("Fallback size used when no prefab is assigned, and also applied to spawned prefab scale when enabled.")]
        private Vector3 platformSize = new Vector3(3f, 0.25f, 1.5f);

        [SerializeField, Tooltip("Apply platformSize to the spawned prefab's local scale.")]
        private bool applySizeToPrefab = false;

        [SerializeField, Tooltip("How long the spawned solid shadow remains before being destroyed.")]
        private float platformDuration = 6f;

        [SerializeField, Tooltip("Small offset above the receiver plane to prevent z-fighting.")]
        private float surfaceOffset = 0.08f;

        [SerializeField, Tooltip("Maximum projection distance used by the fake shadow ray.")]
        private float maxProjectionDistance = 30f;

        [SerializeField, Tooltip("Replace the previous solid shadow when F is pressed again.")]
        private bool replaceExistingPlatform = true;

        [SerializeField, Tooltip("Draw the approximate projection ray in the Scene view.")]
        private bool drawGizmos = true;

        private GameObject currentPlatform;

        private void Update()
        {
            if (Input.GetKeyDown(solidifyKey))
            {
                SpawnSolidShadow();
            }
        }

        public void SpawnSolidShadow()
        {
            if (!HasRequiredReferences())
            {
                return;
            }

            if (!TryGetProjectedPose(out Vector3 position, out Quaternion rotation))
            {
                Debug.LogWarning($"{nameof(ShadowSolidifier)} on {name} could not project a fake shadow onto the receiver plane.", this);
                return;
            }

            if (replaceExistingPlatform && currentPlatform != null)
            {
                Destroy(currentPlatform);
            }

            currentPlatform = solidShadowPrefab != null
                ? Instantiate(solidShadowPrefab, position, rotation)
                : CreateFallbackPlatform(position, rotation);

            currentPlatform.name = "Solid Shadow Platform";
            currentPlatform.SetActive(true);

            if (applySizeToPrefab && solidShadowPrefab != null)
            {
                currentPlatform.transform.localScale = platformSize;
            }

            EnsureCollider(currentPlatform);
            Destroy(currentPlatform, platformDuration);
        }

        public void Configure(Transform lightTransform, Transform casterTransform, Transform receiverTransform, GameObject platformPrefab)
        {
            lightSource = lightTransform;
            shadowCaster = casterTransform;
            receiverPlane = receiverTransform;
            solidShadowPrefab = platformPrefab;
        }

        private bool HasRequiredReferences()
        {
            bool isValid = true;

            if (lightSource == null)
            {
                Debug.LogWarning($"{nameof(ShadowSolidifier)} on {name} needs a light source transform.", this);
                isValid = false;
            }

            if (shadowCaster == null)
            {
                Debug.LogWarning($"{nameof(ShadowSolidifier)} on {name} needs a shadow caster transform.", this);
                isValid = false;
            }

            if (receiverPlane == null)
            {
                Debug.LogWarning($"{nameof(ShadowSolidifier)} on {name} needs a receiver plane transform.", this);
                isValid = false;
            }

            return isValid;
        }

        private bool TryGetProjectedPose(out Vector3 position, out Quaternion rotation)
        {
            Plane plane = new Plane(receiverPlane.up, receiverPlane.position);
            Vector3 direction = (shadowCaster.position - lightSource.position).normalized;
            Ray ray = new Ray(lightSource.position, direction);

            if (!plane.Raycast(ray, out float distance) || distance > maxProjectionDistance)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }

            position = ray.GetPoint(distance) + receiverPlane.up * surfaceOffset;

            Vector3 forwardOnPlane = Vector3.ProjectOnPlane(direction, receiverPlane.up).normalized;
            if (forwardOnPlane.sqrMagnitude < 0.001f)
            {
                forwardOnPlane = receiverPlane.forward;
            }

            rotation = Quaternion.LookRotation(forwardOnPlane, receiverPlane.up);
            return true;
        }

        private GameObject CreateFallbackPlatform(Vector3 position, Quaternion rotation)
        {
            Debug.LogWarning($"{nameof(ShadowSolidifier)} on {name} has no solid shadow prefab assigned. Creating a temporary cube fallback.", this);
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.transform.SetPositionAndRotation(position, rotation);
            platform.transform.localScale = platformSize;
            return platform;
        }

        private static void EnsureCollider(GameObject platform)
        {
            if (platform.GetComponentInChildren<Collider>() == null)
            {
                platform.AddComponent<BoxCollider>();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos || lightSource == null || shadowCaster == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(lightSource.position, shadowCaster.position);

            Vector3 direction = (shadowCaster.position - lightSource.position).normalized;
            Gizmos.color = Color.black;
            Gizmos.DrawRay(shadowCaster.position, direction * 5f);
        }
    }
}
