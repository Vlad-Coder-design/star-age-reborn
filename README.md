# Star Age Reborn

A browser-based 2D space strategy MMO-style MVP inspired by the original Star Age.

The game runs in the browser with HTML5 Canvas, vanilla JavaScript ES modules, and localStorage persistence. There is no backend, account system, React, TypeScript, or build step.

## Features

- 2D space flight with click-to-move controls
- Procedural galaxy with multiple star systems and planets
- Galaxy map and hyperspace travel
- Asteroids, comets, NPC miners, and pirates
- Projectile combat with ship weapon stats
- Cargo, colony storage, trading post, and shipyard
- Colony building grid and offline resource production
- Ship progression with Scout, Fighter, and Destroyer classes
- Equipment progression with Basic/Improved Laser and Basic/Improved Engine
- Local save/load persistence

## Run Locally

Because the source uses ES modules, serve the folder with a static web server:

```bash
python -m http.server 8080
```

Then open:

```text
http://127.0.0.1:8080/
```

## Project Structure

```text
index.html
src/
  config.js
  main.js
  game.js
  entities/
  systems/
  ui/
  utils/
assets/
assets-provided/
documentation/
ui-references/
```

## Persistence

Progress is stored in browser localStorage under `starage_reborn_save`.
