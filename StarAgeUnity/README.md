# Star Age Reborn - Unity Prototype

This is a Unity 2D port/prototype of the browser Star Age Reborn MVP.

It keeps the same core loop:

- Click-to-move space flight
- Generated star system with planets, asteroids, and comets
- Colony panel with storage and production
- Trading and cargo
- Pirates and projectile combat
- Shipyard progression for Scout, Fighter, Destroyer
- Improved Laser and Improved Engine upgrades
- JSON save/load in `Application.persistentDataPath`

## How To Open

1. Open Unity Hub.
2. Add this folder as a project:

   `StarAgeUnity`

3. Let Unity import packages and assets.
4. Open any empty scene or create a new 2D scene.
5. Press Play.

The game bootstraps itself automatically through `StarAgeUnityGame`.

## Notes

- Unity was not installed in this Codex environment, so this project was scaffolded by files and could not be played inside the Unity Editor here.
- The browser version remains in the repository root.
- The Unity version uses procedural fallback sprites if imported image assets are unavailable.
