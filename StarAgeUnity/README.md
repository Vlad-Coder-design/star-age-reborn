# Star Age - Unity 3D Prototype

This is a Unity 6 3D prototype of Star Age: a space strategy/survival MVP with colony building, resource production, crafting, trading, ship upgrades, quests, space flight, mining, and pirate combat.

## Current MVP Loop

- Start on a rocky lava planet with 9 visible building slots
- Build Stone Quarry, Uranium Mine, Ice Mine, factories, Warehouse, and Market
- Collect produced resources from buildings
- Craft metal, fuel, repair kits, and boosters
- Sell goods in the Market / Auction
- Accept and complete quests from the Quest Board
- Buy Scout, Fighter, Destroyer, weapons, engine, cargo, and armor upgrades
- Fly into space with a 3D ship
- Mine asteroids/comets by shooting them
- Fight pirate ships in real time
- JSON save/load in `Application.persistentDataPath`

Prototype timers are intentionally compressed to seconds so the loop can be tested quickly in the editor.

## How To Open

1. Open Unity Hub.
2. Add this folder as a project:

   `StarAgeUnity`

3. Let Unity import packages and assets.
4. Open any empty scene or `Assets/StarAgePlayScene.unity`.
5. Press Play.

The game bootstraps itself automatically through `StarAge3D.GameManager`.

## Controls

### Planet View

- Click a building slot to open building details
- Build or collect from the selected slot
- Use the bottom buttons for Crafting, Market, Quest Board, Shipyard, Save, and Fly To Space

### Space View

- `WASD` moves the ship
- Mouse rotates ship direction
- Left mouse button shoots laser projectiles
- `Space` uses a speed booster if available
- `R` uses a repair kit if available
- Return Planet switches back to colony view

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
- The Unity version uses generated low-poly placeholder models, so it does not require imported art assets to run.
