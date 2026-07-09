using UnityEngine;

namespace Umbra.Prototype
{
    public class PrototypeSceneBootstrap : MonoBehaviour
    {
        [SerializeField, Tooltip("Build the prototype layout automatically when this scene starts.")]
        private bool buildOnStart = true;

        private void Awake()
        {
            if (buildOnStart)
            {
                BuildScene();
            }
        }

        private void BuildScene()
        {
            Material platformMaterial = CreateMaterial("Platform Material", new Color(0.45f, 0.45f, 0.45f));
            Material shadowMaterial = CreateMaterial("Solid Shadow Material", new Color(0.03f, 0.025f, 0.04f));
            Material casterMaterial = CreateMaterial("Caster Material", new Color(0.1f, 0.1f, 0.12f));
            Material exitMaterial = CreateMaterial("Exit Material", new Color(0.1f, 0.5f, 0.25f));

            Transform startPlatform = CreateCube("Start Platform", new Vector3(0f, -0.25f, 0f), new Vector3(5f, 0.5f, 5f), platformMaterial).transform;
            CreateCube("Exit Platform", new Vector3(0f, -0.25f, 9f), new Vector3(5f, 0.5f, 5f), platformMaterial);
            GameObject receiverObject = CreateCube("Receiver Plane", new Vector3(0f, -0.06f, 4.5f), new Vector3(5f, 0.02f, 14f), platformMaterial);
            if (receiverObject.TryGetComponent(out Collider receiverCollider))
            {
                receiverCollider.enabled = false;
            }

            Transform receiver = receiverObject.transform;
            CreateCube("Exit Marker", new Vector3(0f, 0.75f, 10.75f), new Vector3(1f, 1.5f, 0.2f), exitMaterial);

            GameObject caster = CreateCube("Shadow Caster", new Vector3(0f, 0.75f, 3.2f), new Vector3(0.9f, 1.5f, 0.9f), casterMaterial);
            GameObject solidShadowPrefab = CreateCube("Solid Shadow Platform Prefab Source", new Vector3(0f, -20f, 0f), new Vector3(3.2f, 0.25f, 2f), shadowMaterial);
            solidShadowPrefab.SetActive(false);

            RotatableSpotlight lamp = CreateLamp();
            CreatePlayer(new Vector3(startPlatform.position.x, 0.05f, -1.25f), lamp, caster.transform, receiver, solidShadowPrefab);
        }

        private void CreatePlayer(Vector3 position, RotatableSpotlight lamp, Transform caster, Transform receiver, GameObject solidShadowPrefab)
        {
            GameObject player = new GameObject("Player");
            player.transform.position = position;
            player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.stepOffset = 0.35f;

            GameObject cameraObject = new GameObject("Player Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            cameraObject.transform.localRotation = Quaternion.identity;

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.03f;
            cameraObject.AddComponent<AudioListener>();

            PlayerInteractionRaycaster interaction = player.AddComponent<PlayerInteractionRaycaster>();
            interaction.Configure(camera);

            ShadowSolidifier solidifier = player.AddComponent<ShadowSolidifier>();
            solidifier.Configure(lamp.transform, caster, receiver, solidShadowPrefab);

            SimpleFirstPersonController movement = player.AddComponent<SimpleFirstPersonController>();
            movement.Configure(cameraObject.transform, lamp);
        }

        private RotatableSpotlight CreateLamp()
        {
            Material lampMaterial = CreateMaterial("Lamp Body Material", new Color(0.95f, 0.72f, 0.2f));
            Material railMaterial = CreateMaterial("Lamp Rail Material", new Color(0.18f, 0.18f, 0.2f));

            GameObject rail = CreateCube("Lamp Movement Rail", new Vector3(0f, 1.95f, 1f), new Vector3(4.25f, 0.06f, 2.5f), railMaterial);
            if (rail.TryGetComponent(out Collider railCollider))
            {
                railCollider.enabled = false;
            }

            GameObject lampRoot = new GameObject("Interactable Spotlight Lamp");
            lampRoot.transform.SetPositionAndRotation(new Vector3(0f, 2.2f, 0.4f), Quaternion.LookRotation(new Vector3(0f, -0.35f, 1f), Vector3.up));

            SphereCollider collider = lampRoot.AddComponent<SphereCollider>();
            collider.radius = 0.8f;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Lamp Body";
            body.transform.SetParent(lampRoot.transform, false);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            if (body.TryGetComponent(out Renderer bodyRenderer))
            {
                bodyRenderer.sharedMaterial = lampMaterial;
            }

            if (body.TryGetComponent(out Collider bodyCollider))
            {
                bodyCollider.enabled = false;
            }

            GameObject lightObject = new GameObject("Spot Light");
            lightObject.transform.SetParent(lampRoot.transform, false);
            lightObject.transform.localPosition = Vector3.zero;
            lightObject.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);

            Light spot = lightObject.AddComponent<Light>();
            spot.type = LightType.Spot;
            spot.range = 18f;
            spot.spotAngle = 42f;
            spot.intensity = 8f;
            spot.shadows = LightShadows.Soft;

            RotatableSpotlight rotatable = lampRoot.AddComponent<RotatableSpotlight>();
            rotatable.Configure(lampRoot.transform, spot);
            rotatable.ConfigureMovementBounds(new Vector3(-2f, 2.2f, -0.25f), new Vector3(2f, 2.2f, 2.25f));
            return rotatable;
        }

        private static GameObject CreateCube(string objectName, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = objectName;
            cube.transform.position = position;
            cube.transform.localScale = scale;

            if (cube.TryGetComponent(out Renderer renderer))
            {
                renderer.sharedMaterial = material;
            }

            return cube;
        }

        private static Material CreateMaterial(string materialName, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.name = materialName;
            material.color = color;
            return material;
        }
    }
}
