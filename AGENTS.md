# UMBRA Prototype - Agent Instructions

## Project goal
UMBRA is a first-person 3D puzzle game prototype made in Unity.
The core mechanic is manipulating light and shadows, then solidifying shadows into temporary physical platforms.

## Technical direction
- Engine: Unity
- Render pipeline: URP
- Target platform: PC first
- Language: C#
- Keep code simple, readable, and modular.
- Prefer small scripts with clear responsibilities.
- Avoid adding external paid assets or unnecessary dependencies.
- Do not implement online multiplayer.
- Do not build a full game loop yet.
- Focus on a playable prototype scene.

## Folder conventions
Use:
- Assets/_Project/Scripts
- Assets/_Project/Prefabs
- Assets/_Project/Materials
- Assets/_Project/Scenes
- Assets/_Project/UI

## Gameplay prototype
The first playable goal is:
1. Player starts on one platform.
2. There is a gap.
3. A light casts a shadow from an object.
4. Player can rotate the light.
5. Player can press F to solidify the shadow into a temporary walkable platform.
6. Player crosses the gap and reaches the exit.

## Code style
- Use SerializeField for inspector variables.
- Validate missing references with clear Debug.LogWarning messages.
- Avoid hardcoded scene object names.
- Add comments only where they clarify non-obvious behavior.
- Keep prototype systems easy to replace later.

## Current priority
Build the smallest playable proof of concept for shadow solidification.
