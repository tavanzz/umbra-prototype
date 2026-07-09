# UMBRA Prototype Level 01

## Open the prototype scene

Open this scene in Unity:

`Assets/_Project/Scenes/Prototype_Level_01.unity`

Press Play after the scene loads.

## Controls

- `WASD` - move
- Mouse - look around
- `Space` - jump
- Look at the lamp and press `E` - control or release the spotlight
- While controlling the lamp, move the mouse - rotate the spotlight
- `F` - solidify the current projected shadow into a temporary walkable platform

## Expected prototype flow

1. Start on the gray platform.
2. Look toward the gold lamp near the gap.
3. Press `E` while looking at the lamp to control it.
4. Move the mouse to aim the spotlight through the gold shadow caster cube toward the dark receiver plane in the gap.
5. Press `E` again to release the lamp.
6. Press `F` to spawn a temporary dark solid shadow platform.
7. Walk across the gap to the green end platform before the platform disappears.

This is intentionally a small, replaceable proof of concept. It uses approximate projection from the existing `ShadowSolidifier` script rather than generating a real shadow mesh.
