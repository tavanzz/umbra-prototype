# umbra-prototype

UMBRA is a Unity first-person 3D puzzle prototype about manipulating light and shadows, then turning shadows into temporary walkable platforms.

## First playable proof-of-concept setup

The Unity project lives in `UMBRA/`. The prototype scripts are in `UMBRA/Assets/_Project/Scripts/`.

### Scripts added

- `PlayerInteractionRaycaster.cs` raycasts from the player camera and calls `IInteractable.Interact` when the player presses **E**.
- `RotatableSpotlight.cs` makes a lamp/spotlight interactable. Press **E** while looking at it to enter or exit lamp control mode, then move the mouse to rotate it within inspector-configured yaw and pitch limits.
- `ShadowSolidifier.cs` spawns a temporary walkable fake shadow platform when the player presses **F**. It projects a simple ray from the light through the shadow caster onto the receiver plane and places the platform there.
- `IInteractable.cs` is the tiny interface shared by the interaction raycaster and interactable scene objects.


### Prototype controls

- **WASD** = move player.
- **Mouse** = look around.
- **Look at lamp + E** = enter or exit lamp control mode.
- While controlling the lamp:
  - **WASD** or **arrow keys** = move the lamp within its rail bounds.
  - **Mouse** = aim the spotlight.
- **F** = solidify the current fake shadow into a temporary walkable platform.

### Manual Unity test scene

1. Open the Unity project in the `UMBRA/` folder.
2. Create the project folders if they do not already exist:
   - `Assets/_Project/Scripts`
   - `Assets/_Project/Prefabs`
   - `Assets/_Project/Materials`
   - `Assets/_Project/Scenes`
   - `Assets/_Project/UI`
3. Create a simple test scene and save it under `Assets/_Project/Scenes/`.
4. Create two platform objects with a gap between them:
   - Add a cube for the starting platform.
   - Add a second cube for the exit platform.
   - Scale and position them so the player cannot cross without a temporary platform.
5. Create a floor or receiver plane below/around the gap. This transform will be assigned to `ShadowSolidifier.receiverPlane`.
6. Add a player:
   - Use a `CharacterController`-based first-person player if you already have one, or Unity's starter/sample first-person controller.
   - Add a child `Camera` at head height.
   - Add `PlayerInteractionRaycaster` to the player and assign the child camera.
   - Add `ShadowSolidifier` to the player or another always-active scene object.
7. Add a lamp/spotlight:
   - Create an empty GameObject for the lamp pivot.
   - Add a child `Spot Light` aimed toward the gap and the caster object.
   - Add `RotatableSpotlight` to the lamp pivot.
   - Assign the pivot as `Rotation Pivot` and the child spot light as `Controlled Light`.
   - Put the lamp or its collider on a layer included by the player's `PlayerInteractionRaycaster.interactableLayers`.
   - Add a collider to the lamp so the interaction ray can hit it.
8. Add a shadow caster object:
   - Place a cube, pillar, or simple mesh between the spotlight and the receiver plane.
   - Assign this transform to `ShadowSolidifier.shadowCaster`.
9. Create the solid shadow platform prefab:
   - Create a cube in the scene.
   - Give it a dark material so it reads as a solidified shadow.
   - Scale it to a useful bridge size, for example `(3, 0.25, 1.5)`.
   - Ensure it has a `BoxCollider`.
   - Drag it into `Assets/_Project/Prefabs/` to create a prefab, then delete the scene instance.
10. Assign `ShadowSolidifier` references in the Inspector:
    - `Light Source`: the spotlight transform or lamp light transform.
    - `Shadow Caster`: the caster object transform.
    - `Receiver Plane`: the floor/receiver transform.
    - `Solid Shadow Prefab`: the platform prefab.
    - Tune `Platform Duration`, `Surface Offset`, `Max Projection Distance`, and `Platform Size` as needed.
11. Enter Play Mode and test:
    - Look at the lamp and press **E** to enter lamp control mode.
    - Move the mouse to rotate the spotlight.
    - Press **E** again to release the lamp.
    - Press **F** to spawn the temporary solid shadow platform.
    - Cross the gap before the platform disappears.

### Prototype limitations

This is intentionally a fake, replaceable proof of concept. It does not generate accurate shadow meshes, use advanced shaders, implement menus, save data, multiplayer, procedural generation, mobile controls, or a full game loop.
