# Star Age Reborn

A browser-playable space strategy prototype inspired by the original Star Age.

Play online:

```text
https://vlad-coder-design.github.io/star-age-reborn/
```

The main browser version now uses React, TypeScript, Three.js, React Three Fiber, and WebGL. It includes a lightweight fallback screen for browsers that cannot create a WebGL context.

## Features

- WebGL space scene with React Three Fiber
- Procedural planet shader textures
- Custom atmosphere shader shells
- Ship flight with acceleration, drag, and inertia
- Responsive HUD for desktop and smaller screens
- Fallback path for browsers without WebGL
- Procedural galaxy with multiple star systems and planets
- Galaxy map and hyperspace travel
- Asteroid mining, cargo, trading, and starter economy loop

## Run Locally

Install dependencies:

```bash
pnpm install
```

Start the dev server:

```bash
pnpm dev
```

Build the static site:

```bash
pnpm build
```

## Project Structure

```text
index.html
package.json
vite.config.ts
src/
  App.tsx
  main.tsx
  components/
  game/
  styles.css
assets/
assets-provided/
documentation/
ui-references/
StarAgeUnity/      # Unity 2D prototype
```

## Unity Prototype

The `StarAgeUnity/` folder contains a Unity 2D version of the same MVP. Open that folder in Unity Hub and press Play, or build a shareable version from Unity with `Star Age > Build > Windows x64` or `Star Age > Build > WebGL`.

For Windows, zip and share the entire generated `StarAgeUnity/Builds/Windows/` folder, not just the `.exe`. For WebGL, upload the full `StarAgeUnity/Builds/WebGL/` folder to a static web host.
