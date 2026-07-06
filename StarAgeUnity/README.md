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

## How To Share A Playable Build

The project now includes `Assets/StarAgePlayScene.unity` in Build Settings, so it can be built like a normal Unity game.

### Windows

1. Open `StarAgeUnity` in Unity 6.
2. Use `Star Age > Build > Windows x64`.
3. Wait for Unity to create:

   `StarAgeUnity/Builds/Windows/`

4. Zip the whole `Windows` folder.
5. Send the zip to another Windows user.
6. They unzip it and run:

   `StarAgeReborn.exe`

Do not send only the `.exe`; Unity builds need the generated data folders beside it.

### WebGL

1. Install Unity's WebGL Build Support module if Unity asks for it.
2. Use `Star Age > Build > WebGL`.
3. Upload everything in:

   `StarAgeUnity/Builds/WebGL/`

   to a static host such as GitHub Pages, itch.io, Netlify, or any web server.

WebGL builds normally must be served from a web server. Opening `index.html` directly from disk may fail in browsers.

### Manual Build Menu

You can also use Unity's regular menu:

1. Open `File > Build Profiles`.
2. Select Windows or WebGL.
3. Make sure `Assets/StarAgePlayScene.unity` is in the scene list.
4. Click `Build`.

## Notes

- Unity batchmode validation in Codex may require an activated Unity license, but the project is set up for normal Unity Editor builds.
- The browser version remains in the repository root.
- The Unity version uses procedural fallback sprites if imported image assets are unavailable.
