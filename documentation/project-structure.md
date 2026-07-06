# Project Structure Document
**Space MMO Browser Game - File Organization & Conventions**

---

## Executive Summary

This document defines the complete file structure, naming conventions, and organizational patterns for the Space MMO game project. The structure is optimized for:
- **AI code generation** - Clear, predictable file locations
- **Solo developer workflow** - Easy to navigate and maintain  
- **Future expansion** - Modular organization supports feature additions
- **Browser deployment** - No build tools required for MVP

**Key Principles:**
- Organize by system/feature, not by file type
- Keep modules small (200-400 lines each)
- Data-driven design with JSON configuration files
- Clear separation: game logic → entities → rendering → UI

---

## Complete Directory Structure

```
space-mmo-game/
│
├── index.html                          # Entry point, canvas setup
├── README.md                           # Project overview and setup instructions
├── package.json                        # Optional: npm scripts for dev server
│
├── src/                                # All JavaScript source code
│   ├── main.js                         # Game initialization and loop
│   ├── game.js                         # Core game state manager
│   ├── config.js                       # Configuration constants
│   │
│   ├── systems/                        # Game systems (core logic)
│   │   ├── galaxy.js                   # Galaxy generation and management
│   │   ├── combat.js                   # Combat calculations and targeting
│   │   ├── colony.js                   # Colony management and buildings
│   │   ├── resources.js                # Resource production and storage
│   │   ├── missions.js                 # Mission generation and tracking
│   │   ├── npc.js                      # NPC spawning and AI behavior
│   │   └── save.js                     # Save/load and offline progression
│   │
│   ├── entities/                       # Game entities (data structures)
│   │   ├── ship.js                     # Player and NPC ship classes
│   │   ├── planet.js                   # Planet entity class
│   │   ├── spaceObject.js              # Asteroids, comets, debris
│   │   └── building.js                 # Building entity class
│   │
│   ├── ui/                             # User interface components
│   │   ├── hud.js                      # Heads-up display (always visible)
│   │   ├── galaxyMap.js                # Galaxy map overlay
│   │   ├── colonyView.js               # Colony management interface
│   │   ├── missionPanel.js             # Mission tracking panel
│   │   ├── shipyard.js                 # Ship/equipment shop interface
│   │   └── tooltip.js                  # Tooltip system
│   │
│   ├── renderer/                       # Rendering engine
│   │   ├── canvas.js                   # Canvas setup and management
│   │   ├── draw.js                     # Drawing utilities (shapes, sprites)
│   │   ├── camera.js                   # Camera follow and viewport
│   │   └── effects.js                  # Visual effects (lasers, explosions)
│   │
│   └── utils/                          # Utility functions
│       ├── math.js                     # Vector math, distance, collision
│       ├── input.js                    # Mouse and keyboard input handling
│       ├── time.js                     # Delta time and timers
│       └── random.js                   # Random number generation utilities
│
├── assets/                             # All game assets
│   ├── sprites/                        # Sprite images (PNG)
│   │   ├── ships/                      # Ship sprites
│   │   │   ├── scout.png               # Scout class ship
│   │   │   ├── fighter.png             # Fighter class ship
│   │   │   ├── destroyer.png           # Destroyer class ship
│   │   │   ├── miner-npc.png          # NPC miner ship
│   │   │   └── pirate-npc.png         # NPC pirate ship
│   │   │
│   │   ├── buildings/                  # Building sprites
│   │   │   ├── storage.png             # Storage building
│   │   │   ├── stone-extractor.png     # Stone extractor
│   │   │   ├── ice-extractor.png       # Ice extractor
│   │   │   ├── uranium-mine.png        # Uranium mine
│   │   │   └── fuel-factory.png        # Fuel factory
│   │   │
│   │   ├── planets/                    # Planet sprites
│   │   │   ├── agrarian.png            # Agrarian planet type
│   │   │   ├── military.png            # Military (volcanic) planet
│   │   │   ├── mining.png              # Mining (gas giant) planet
│   │   │   ├── industrial.png          # Industrial planet
│   │   │   └── scientific.png          # Scientific planet
│   │   │
│   │   ├── objects/                    # Space object sprites
│   │   │   ├── asteroid.png            # Asteroid
│   │   │   ├── comet.png               # Comet
│   │   │   └── debris.png              # Space debris
│   │   │
│   │   └── ui/                         # UI element sprites
│   │       ├── button.png              # Button graphic
│   │       ├── panel.png               # Panel background
│   │       ├── icon-stone.png          # Stone resource icon
│   │       ├── icon-ice.png            # Ice resource icon
│   │       ├── icon-uranium.png        # Uranium resource icon
│   │       ├── icon-fuel.png           # Fuel resource icon
│   │       └── icon-credits.png        # Credits icon
│   │
│   ├── effects/                        # Visual effect sprites
│   │   ├── laser-beam.png              # Laser projectile
│   │   ├── explosion.png               # Explosion sprite sheet
│   │   └── particles.png               # Particle effects
│   │
│   └── sounds/                         # Audio files (optional for MVP)
│       ├── laser-shot.mp3              # Laser firing sound
│       ├── explosion.mp3               # Explosion sound
│       └── ui-click.mp3                # UI button click
│
├── data/                               # Configuration data files (JSON)
│   ├── ships.json                      # Ship class definitions
│   ├── buildings.json                  # Building type definitions
│   ├── planets.json                    # Planet type definitions
│   ├── equipment.json                  # Weapons and modules data
│   ├── npcs.json                       # NPC behavior configurations
│   └── missions.json                   # Mission templates
│
└── docs/                               # Project documentation
    ├── planning/                       # Planning documents
    │   ├── problem-statement.md
    │   ├── target-audience.md
    │   ├── core-assumptions.md
    │   ├── research-findings.md
    │   ├── core-game-loop.md
    │   └── mvp-requirements.md
    │
    ├── technical/                      # Technical documentation
    │   ├── technical-architecture.md
    │   ├── project-structure.md (this file)
    │   ├── asset-requirements.md
    │   └── implementation-guide.md
    │
    └── reference/                      # Reference materials
        ├── star-age-screenshots/       # Original game screenshots
        └── design-notes.md             # Design decision log
```

---

## Naming Conventions

### File Naming Rules

**JavaScript Files:**
- Use **kebab-case**: `galaxy-map.js`, `space-object.js`
- Class files singular: `ship.js` (contains Ship class)
- System files descriptive: `combat.js`, `missions.js`
- **NO underscores**, **NO PascalCase** in filenames

**Asset Files:**
- Use **kebab-case**: `scout-ship.png`, `stone-extractor.png`
- Descriptive names: `explosion-frame-1.png` not `exp1.png`
- Version suffixes if needed: `laser-mk1.png`, `laser-mk2.png`

**Data Files:**
- Plural nouns: `ships.json`, `buildings.json`, `missions.json`
- Lowercase, no special characters

**Documentation Files:**
- Kebab-case: `mvp-requirements.md`, `core-game-loop.md`
- Clear, descriptive names
- Use `.md` extension for all docs

### Code Naming Rules

**Classes:**
- **PascalCase**: `Ship`, `Planet`, `CombatSystem`
- Nouns, singular form
- Example: `class PlayerShip { }`

**Functions:**
- **camelCase**: `calculateDistance`, `spawnNPC`, `renderShip`
- Verb-noun pattern
- Example: `function updatePosition(entity, deltaTime) { }`

**Variables:**
- **camelCase**: `playerShip`, `currentSystem`, `missionList`
- Descriptive, not abbreviated
- Constants: **UPPER_SNAKE_CASE**: `MAX_SPEED`, `SPAWN_RATE`

**Example:**
```javascript
// Good naming
const CANVAS_WIDTH = 1920;
class PlayerShip {
  constructor(shipClass) {
    this.currentHealth = 100;
    this.maxSpeed = 200;
  }
  
  updatePosition(deltaTime) {
    // ...
  }
}

// Bad naming
const cw = 1920;
class player_ship {
  constructor(sc) {
    this.hp = 100;
    this.spd = 200;
  }
  
  upd_pos(dt) {
    // ...
  }
}
```

---

## Module Organization Principles

### System Modules (src/systems/)

**Purpose:** Core game logic and calculations

**Pattern:**
```javascript
// src/systems/combat.js
export class CombatSystem {
  constructor(game) {
    this.game = game;
    this.activeTargets = new Map();
  }
  
  // System initialization
  init() { }
  
  // Per-frame update
  update(deltaTime) { }
  
  // Public API methods
  attackTarget(attacker, target) { }
  calculateDamage(weapon, target) { }
}
```

**Rules:**
- One system class per file
- Systems are stateless (no entity storage, only references)
- Systems operate on entities passed to them
- Systems expose clean public API

### Entity Modules (src/entities/)

**Purpose:** Data structures for game objects

**Pattern:**
```javascript
// src/entities/ship.js
export class Ship {
  constructor(config) {
    // Base properties from config
    this.class = config.class;
    this.hp = config.hp;
    this.maxHp = config.hp;
    
    // Derived properties
    this.position = { x: 0, y: 0 };
    this.velocity = { x: 0, y: 0 };
    
    // State
    this.isDead = false;
    this.target = null;
  }
  
  // Entity-specific methods
  takeDamage(amount) { }
  heal(amount) { }
  setTarget(entity) { }
}
```

**Rules:**
- Entities are data containers with minimal logic
- Constructor takes config object
- Public methods for state changes only
- No rendering code in entities

### UI Modules (src/ui/)

**Purpose:** User interface rendering and interaction

**Pattern:**
```javascript
// src/ui/hud.js
export class HUD {
  constructor(game, canvas) {
    this.game = game;
    this.ctx = canvas.getContext('2d');
  }
  
  render() {
    this.renderCredits();
    this.renderResources();
    this.renderShipStatus();
  }
  
  renderCredits() { }
  renderResources() { }
  renderShipStatus() { }
  
  handleClick(x, y) { }
}
```

**Rules:**
- UI modules handle rendering and input for specific interfaces
- Each UI module is self-contained
- UI reads from game state, never modifies it directly
- Click handlers call game methods, don't change state

---

## Data File Structures

### ships.json
```json
{
  "scout": {
    "name": "Scout",
    "hp": 100,
    "speed": 200,
    "cargoCapacity": 200,
    "weaponSlots": 2,
    "moduleSlots": 1,
    "cost": 0,
    "sprite": "assets/sprites/ships/scout.png"
  },
  "fighter": {
    "name": "Fighter",
    "hp": 200,
    "speed": 150,
    "cargoCapacity": 250,
    "weaponSlots": 3,
    "moduleSlots": 2,
    "cost": 5000,
    "sprite": "assets/sprites/ships/fighter.png"
  }
}
```

### buildings.json
```json
{
  "storage": {
    "name": "Storage",
    "capacity": 500,
    "cost": {
      "stone": 3,
      "ice": 2
    },
    "buildTime": 0,
    "sprite": "assets/sprites/buildings/storage.png"
  },
  "stoneExtractor": {
    "name": "Stone Extractor",
    "produces": "stone",
    "productionRate": 900000,
    "storageCapacity": 5,
    "cost": {
      "stone": 2
    },
    "buildTime": 0,
    "sprite": "assets/sprites/buildings/stone-extractor.png"
  }
}
```

### planets.json
```json
{
  "agrarian": {
    "name": "Agrarian",
    "color": "#4CAF50",
    "allowedExtractors": ["stoneExtractor", "iceExtractor"],
    "sprite": "assets/sprites/planets/agrarian.png"
  },
  "military": {
    "name": "Military",
    "color": "#F44336",
    "allowedExtractors": ["stoneExtractor", "iceExtractor", "uraniumMine"],
    "sprite": "assets/sprites/planets/military.png"
  }
}
```

---

## Import/Export Patterns

### ES6 Module Usage

**Exporting:**
```javascript
// src/entities/ship.js
export class Ship {
  // ...
}

export const SHIP_TYPES = {
  SCOUT: 'scout',
  FIGHTER: 'fighter'
};
```

**Importing:**
```javascript
// src/main.js
import { Ship, SHIP_TYPES } from './entities/ship.js';
import { CombatSystem } from './systems/combat.js';
```

**Default Exports (avoid for MVP):**
```javascript
// Prefer named exports for clarity
// export default Ship; // DON'T USE
```

### Module Dependencies

**Dependency Hierarchy:**
```
main.js
  ↓
game.js (Game State Manager)
  ↓
systems/*.js (Game Systems)
  ↓
entities/*.js (Entity Classes)
  ↓
utils/*.js (Utilities)
```

**Rule:** Lower layers never import from higher layers
- Entities don't import systems
- Utils don't import entities
- Systems can import entities and utils
- UI can import anything but game systems shouldn't import UI

---

## Configuration File (src/config.js)

```javascript
// src/config.js
export const CONFIG = {
  // Canvas
  CANVAS_WIDTH: 1920,
  CANVAS_HEIGHT: 1080,
  TARGET_FPS: 60,
  
  // Galaxy
  SYSTEM_COUNT: 6,
  PLANETS_PER_SYSTEM_MIN: 3,
  PLANETS_PER_SYSTEM_MAX: 4,
  
  // Combat
  WEAPON_RANGE_SCOUT: 150,
  WEAPON_DAMAGE_BASIC: 10,
  WEAPON_FIRE_RATE: 1.0, // shots per second
  
  // NPCs
  NPC_MINER_SPAWN_INTERVAL: 15000, // 15 seconds
  NPC_PIRATE_SPAWN_INTERVAL: 60000, // 60 seconds
  NPC_MAX_MINERS: 5,
  NPC_MAX_PIRATES: 3,
  
  // Resources
  RESOURCE_PRICES: {
    stone: 3,
    ice: 2,
    uranium: 5,
    fuel: 20
  },
  
  // Colony
  COLONY_COSTS: [0, 2000, 5000, 10000, 20000, 40000],
  BUILDING_GRID_SIZE: 3, // 3x3 grid
  
  // Debug
  DEBUG_MODE: true,
  SHOW_HITBOXES: false,
  SHOW_FPS: true
};
```

---

## Summary Checklist

**Before starting development:**
- [ ] All directories created according to structure
- [ ] `index.html` skeleton ready
- [ ] `config.js` populated with constants
- [ ] `data/` folder with JSON templates
- [ ] `assets/` folder structure ready (even if empty)

**During development:**
- [ ] Follow naming conventions strictly
- [ ] One class per file
- [ ] Keep files under 600 lines
- [ ] Import/export using ES6 modules
- [ ] Store all magic numbers in `config.js`

**For Antigravity AI:**
- [ ] Reference this structure in all prompts
- [ ] Specify exact file paths when requesting code
- [ ] Ensure generated code follows naming conventions
- [ ] Verify imports use correct relative paths

---

**Document Version:** 1.0  
**Last Updated:** November 24, 2025  
**Project:** Space MMO Browser Game  
**Status:** Active - Reference This in All Development

---

**Next Document:** technical-architecture.md
