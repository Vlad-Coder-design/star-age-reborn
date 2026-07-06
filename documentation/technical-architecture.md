# Technical Architecture Document
**Space MMO Browser Game - System Design & Implementation Specifications**

---

## Executive Summary

Complete technical architecture for browser-based space MMO game optimized for AI code generation with Antigravity AI. Built on HTML5 Canvas with vanilla JavaScript, ECS-lite pattern, 60 FPS real-time gameplay.

**Technology Stack:**
- HTML5 Canvas (2D rendering)
- Vanilla JavaScript/ES6 Modules
- Local Storage persistence
- Fixed 1920x1080 canvas with CSS scaling

**Core Architecture:**
- Entity-Component-System (lightweight)
- State machine for game modes
- Event-driven UI system
- Async resource production with timestamp calculation

---

## System Architecture Overview

```
┌────────────────────────────────────────────┐
│          main.js (Game Loop)               │
│  - 60 FPS requestAnimationFrame            │
│  - Delta time calculation                  │
│  - Update → Render cycle                   │
└─────────────────┬──────────────────────────┘
                  ↓
┌────────────────────────────────────────────┐
│       game.js (State Manager)              │
│  - Player state                            │
│  - Galaxy state                            │
│  - Colony states                           │
│  - Game mode (space/colony/map)            │
└─────────────────┬──────────────────────────┘
                  ↓
┌────────────────────────────────────────────┐
│           Systems Layer                    │
│  ┌──────────────────────────────────────┐ │
│  │  GalaxySystem     CombatSystem       │ │
│  │  ColonySystem     ResourceSystem     │ │
│  │  MissionSystem    NPCSystem          │ │
│  │  SaveSystem                          │ │
│  └──────────────────────────────────────┘ │
└─────────────────┬──────────────────────────┘
                  ↓
┌────────────────────────────────────────────┐
│          Entities Layer                    │
│  Ship, Planet, Building, SpaceObject, NPC  │
└────────────────────────────────────────────┘
```

---

## Core Systems

### 1. Game Loop System (main.js)

```javascript
import { Game } from './game.js';
import { CONFIG } from './config.js';

let game;
let lastTime = Date.now();
let canvas, ctx;

async function init() {
  // Setup canvas
  canvas = document.getElementById('gameCanvas');
  ctx = canvas.getContext('2d');
  canvas.width = CONFIG.CANVAS_WIDTH;
  canvas.height = CONFIG.CANVAS_HEIGHT;
  
  // Scale to fit screen
  scaleCanvas();
  window.addEventListener('resize', scaleCanvas);
  
  // Initialize game
  game = new Game(canvas);
  await game.init();
  
  // Start loop
  gameLoop();
}

function gameLoop() {
  const currentTime = Date.now();
  const deltaTime = (currentTime - lastTime) / 1000; // seconds
  lastTime = currentTime;
  
  // Update game state
  game.update(deltaTime);
  
  // Clear canvas
  ctx.clearRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);
  
  // Render game
  game.render(ctx);
  
  // Continue loop
  requestAnimationFrame(gameLoop);
}

function scaleCanvas() {
  const scale = Math.min(
    window.innerWidth / CONFIG.CANVAS_WIDTH,
    window.innerHeight / CONFIG.CANVAS_HEIGHT
  );
  canvas.style.transform = `scale(${scale})`;
  canvas.style.transformOrigin = 'top left';
}

init();
```

**Key Points:**
- Fixed 60 FPS target via requestAnimationFrame
- Delta time ensures frame-independent movement
- CSS scaling handles different screen sizes
- Clear separation: update logic → rendering

---

### 2. Game State Manager (game.js)

```javascript
import { CONFIG } from './config.js';
import { GalaxySystem } from './systems/galaxy.js';
import { CombatSystem } from './systems/combat.js';
import { ColonySystem } from './systems/colony.js';
import { ResourceSystem } from './systems/resources.js';
import { MissionSystem } from './systems/missions.js';
import { NPCSystem } from './systems/npc.js';
import { SaveSystem } from './systems/save.js';
import { InputHandler } from './utils/input.js';

export class Game {
  constructor(canvas) {
    this.canvas = canvas;
    this.ctx = canvas.getContext('2d');
    
    // Game mode state
    this.mode = 'space'; // 'space', 'colony', 'galaxy_map'
    
    // Core state
    this.player = null;
    this.galaxy = null;
    this.colonies = [];
    this.currentSystem = null;
    
    // Systems
    this.galaxySystem = new GalaxySystem(this);
    this.combatSystem = new CombatSystem(this);
    this.colonySystem = new ColonySystem(this);
    this.resourceSystem = new ResourceSystem(this);
    this.missionSystem = new MissionSystem(this);
    this.npcSystem = new NPCSystem(this);
    this.saveSystem = new SaveSystem(this);
    
    // Input
    this.input = new InputHandler(canvas);
    this.input.onMouseClick = (x, y) => this.handleClick(x, y);
  }
  
  async init() {
    // Try to load save game
    const saveData = this.saveSystem.load();
    
    if (saveData) {
      // Restore from save
      await this.loadFromSave(saveData);
    } else {
      // New game
      await this.newGame();
    }
  }
  
  async newGame() {
    // Generate galaxy
    this.galaxy = this.galaxySystem.generateGalaxy();
    this.currentSystem = this.galaxy.systems[0];
    
    // Create player
    this.player = {
      credits: 1000,
      experience: 0,
      level: 1,
      ship: this.createShip('scout', 960, 540),
      inventory: {
        stone: 5,
        ice: 5,
        uranium: 0,
        fuel: 0
      }
    };
    
    // Establish home colony
    const homePlanet = this.currentSystem.planets[0];
    this.colonies.push(
      this.colonySystem.establishColony(homePlanet, true)
    );
    
    // Initial save
    this.saveSystem.save(this);
  }
  
  update(deltaTime) {
    // Update based on mode
    switch(this.mode) {
      case 'space':
        this.updateSpace(deltaTime);
        break;
      case 'colony':
        this.updateColony(deltaTime);
        break;
      case 'galaxy_map':
        // Galaxy map is mostly UI, minimal updates
        break;
    }
    
    // Always update resource production
    this.resourceSystem.update(deltaTime);
    
    // Auto-save every 30 seconds
    if (!this.lastSaveTime || Date.now() - this.lastSaveTime > 30000) {
      this.saveSystem.save(this);
      this.lastSaveTime = Date.now();
    }
  }
  
  updateSpace(deltaTime) {
    // Update player ship
    this.player.ship.update(deltaTime);
    
    // Update NPCs
    this.npcSystem.update(deltaTime);
    
    // Update combat
    this.combatSystem.update(deltaTime);
    
    // Check mission progress
    this.missionSystem.checkProgress();
  }
  
  updateColony(deltaTime) {
    // Colony view updates
    const currentColony = this.getCurrentColony();
    if (currentColony) {
      this.colonySystem.updateColony(currentColony, deltaTime);
    }
  }
  
  render(ctx) {
    switch(this.mode) {
      case 'space':
        this.renderSpace(ctx);
        break;
      case 'colony':
        this.renderColony(ctx);
        break;
      case 'galaxy_map':
        this.renderGalaxyMap(ctx);
        break;
    }
  }
  
  renderSpace(ctx) {
    // Render space background
    ctx.fillStyle = '#000000';
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);
    
    // Render planets
    this.currentSystem.planets.forEach(planet => {
      planet.render(ctx);
    });
    
    // Render space objects
    this.currentSystem.asteroids.forEach(asteroid => {
      asteroid.render(ctx);
    });
    
    // Render NPCs
    this.npcSystem.render(ctx);
    
    // Render player ship
    this.player.ship.render(ctx);
    
    // Render combat effects
    this.combatSystem.render(ctx);
    
    // Render HUD
    this.renderHUD(ctx);
  }
  
  handleClick(x, y) {
    // Handle clicks based on mode
    if (this.mode === 'space') {
      this.handleSpaceClick(x, y);
    }
  }
}
```

---

### 3. Galaxy Generation System

```javascript
export class GalaxySystem {
  constructor(game) {
    this.game = game;
  }
  
  generateGalaxy() {
    const galaxy = {
      systems: []
    };
    
    for (let i = 0; i < CONFIG.SYSTEM_COUNT; i++) {
      galaxy.systems.push(this.generateSystem(i));
    }
    
    return galaxy;
  }
  
  generateSystem(systemId) {
    const planetCount = random(
      CONFIG.PLANETS_PER_SYSTEM_MIN,
      CONFIG.PLANETS_PER_SYSTEM_MAX
    );
    
    const system = {
      id: systemId,
      name: this.generateSystemName(systemId),
      planets: [],
      asteroids: [],
      npcs: []
    };
    
    // Generate planets in orbit
    for (let i = 0; i < planetCount; i++) {
      const angle = (i / planetCount) * Math.PI * 2;
      const distance = 300 + (i * 150);
      const x = 960 + Math.cos(angle) * distance;
      const y = 540 + Math.sin(angle) * distance;
      
      system.planets.push(this.generatePlanet(i, x, y));
    }
    
    // Generate asteroids
    for (let i = 0; i < 10; i++) {
      system.asteroids.push(this.generateAsteroid());
    }
    
    return system;
  }
  
  generatePlanet(id, x, y) {
    const types = ['agrarian', 'military', 'mining', 'industrial', 'scientific'];
    const type = types[Math.floor(Math.random() * types.length)];
    
    return new Planet({
      id,
      type,
      position: {x, y},
      radius: 50 + Math.random() * 30,
      colonized: false
    });
  }
}
```

---

### 4. Combat System

```javascript
export class CombatSystem {
  constructor(game) {
    this.game = game;
    this.targets = new Map(); // attacker -> target mapping
    this.projectiles = [];
  }
  
  attackTarget(attacker, target) {
    // Check if in range
    const distance = calculateDistance(
      attacker.position,
      target.position
    );
    
    if (distance > attacker.weapon.range) {
      return false;
    }
    
    // Set target
    this.targets.set(attacker, target);
    return true;
  }
  
  update(deltaTime) {
    // Process all active attacks
    for (const [attacker, target] of this.targets) {
      if (!attacker.isDead && !target.isDead) {
        this.processAttack(attacker, target, deltaTime);
      } else {
        this.targets.delete(attacker);
      }
    }
    
    // Update projectiles
    this.updateProjectiles(deltaTime);
  }
  
  processAttack(attacker, target, deltaTime) {
    // Check fire rate cooldown
    if (!attacker.lastFireTime) {
      attacker.lastFireTime = 0;
    }
    
    const timeSinceLastFire = Date.now() - attacker.lastFireTime;
    const fireInterval = 1000 / attacker.weapon.fireRate;
    
    if (timeSinceLastFire >= fireInterval) {
      // Fire weapon
      this.fireWeapon(attacker, target);
      attacker.lastFireTime = Date.now();
    }
  }
  
  fireWeapon(attacker, target) {
    // Create projectile
    const projectile = {
      from: attacker.position,
      to: target.position,
      damage: attacker.weapon.damage,
      speed: attacker.weapon.projectileSpeed || 800,
      position: {...attacker.position},
      target: target,
      active: true
    };
    
    this.projectiles.push(projectile);
  }
  
  updateProjectiles(deltaTime) {
    for (let i = this.projectiles.length - 1; i >= 0; i--) {
      const proj = this.projectiles[i];
      
      // Move projectile
      const angle = Math.atan2(
        proj.target.position.y - proj.position.y,
        proj.target.position.x - proj.position.x
      );
      
      proj.position.x += Math.cos(angle) * proj.speed * deltaTime;
      proj.position.y += Math.sin(angle) * proj.speed * deltaTime;
      
      // Check collision
      const distance = calculateDistance(proj.position, proj.target.position);
      
      if (distance < 10) {
        // Hit target
        proj.target.takeDamage(proj.damage);
        this.projectiles.splice(i, 1);
      }
    }
  }
}
```

---

### 5. Colony & Resource System

```javascript
export class ColonySystem {
  establishColony(planet, isHome = false) {
    const colony = {
      planetId: planet.id,
      planetType: planet.type,
      buildings: [],
      storage: {
        stone: 0,
        ice: 0,
        uranium: 0,
        fuel: 0
      },
      gridSize: 3,
      establishedAt: Date.now()
    };
    
    planet.colonized = true;
    planet.colony = colony;
    
    if (isHome) {
      // Home colony starts with storage
      this.buildBuilding(colony, 'storage', 0, 0);
    }
    
    return colony;
  }
  
  buildBuilding(colony, buildingType, gridX, gridY) {
    const buildingData = BUILDINGS_DATA[buildingType];
    
    const building = {
      type: buildingType,
      gridX,
      gridY,
      level: 1,
      producing: buildingData.produces || null,
      productionRate: buildingData.productionRate || 0,
      storageCurrent: 0,
      storageMax: buildingData.storageCapacity || 0,
      lastCollected: Date.now()
    };
    
    colony.buildings.push(building);
    return building;
  }
}

export class ResourceSystem {
  update(deltaTime) {
    // Update all production buildings in all colonies
    for (const colony of this.game.colonies) {
      this.updateColonyProduction(colony);
    }
  }
  
  updateColonyProduction(colony) {
    const now = Date.now();
    
    for (const building of colony.buildings) {
      if (building.producing) {
        this.updateBuildingProduction(building, now);
      }
    }
  }
  
  updateBuildingProduction(building, now) {
    const timeSinceLastCollect = now - building.lastCollected;
    const produced = (timeSinceLastCollect / building.productionRate) * building.storageMax;
    
    // Add to building storage (cap at max)
    building.storageCurrent = Math.min(
      building.storageCurrent + produced,
      building.storageMax
    );
    
    // If storage full, stop production (don't update lastCollected)
    if (building.storageCurrent < building.storageMax) {
      building.lastCollected = now;
    }
  }
  
  collectResources(colony, building) {
    const resource = building.producing;
    const amount = building.storageCurrent;
    
    // Transfer to colony storage
    colony.storage[resource] += amount;
    
    // Reset building storage
    building.storageCurrent = 0;
    building.lastCollected = Date.now();
  }
}
```

---

## Data Structures

### Player State
```javascript
{
  credits: number,
  experience: number,
  level: number,
  ship: Ship,
  inventory: {
    stone: number,
    ice: number,
    uranium: number,
    fuel: number
  }
}
```

### Ship Entity
```javascript
{
  class: 'scout' | 'fighter' | 'destroyer',
  hp: number,
  maxHp: number,
  speed: number,
  position: {x, y},
  velocity: {x, y},
  target: null | Ship,
  weapon: {damage, range, fireRate},
  cargo: {stone, ice, uranium, fuel, capacity},
  isDead: boolean
}
```

### Colony State
```javascript
{
  planetId: number,
  planetType: string,
  buildings: Building[],
  storage: {stone, ice, uranium, fuel},
  gridSize: 3,
  establishedAt: timestamp
}
```

### Building Entity
```javascript
{
  type: string,
  gridX: number,
  gridY: number,
  level: number,
  producing: string | null,
  productionRate: number,
  storageCurrent: number,
  storageMax: number,
  lastCollected: timestamp
}
```

---

## Rendering System

```javascript
export class Renderer {
  static drawShip(ctx, ship) {
    const {x, y} = ship.position;
    
    // Draw ship sprite or placeholder
    if (ship.sprite) {
      ctx.drawImage(ship.sprite, x - 32, y - 32, 64, 64);
    } else {
      // Placeholder: colored triangle
      ctx.fillStyle = ship.isPlayer ? '#00ff00' : '#ff0000';
      ctx.beginPath();
      ctx.moveTo(x, y - 20);
      ctx.lineTo(x - 15, y + 15);
      ctx.lineTo(x + 15, y + 15);
      ctx.closePath();
      ctx.fill();
    }
    
    // Draw HP bar
    if (ship.hp < ship.maxHp) {
      const barWidth = 50;
      const barHeight = 5;
      const hpPercent = ship.hp / ship.maxHp;
      
      ctx.fillStyle = '#333';
      ctx.fillRect(x - barWidth/2, y - 30, barWidth, barHeight);
      
      ctx.fillStyle = '#00ff00';
      ctx.fillRect(x - barWidth/2, y - 30, barWidth * hpPercent, barHeight);
    }
  }
  
  static drawPlanet(ctx, planet) {
    const {x, y, radius} = planet.position;
    
    if (planet.sprite) {
      ctx.drawImage(planet.sprite, x - radius, y - radius, radius * 2, radius * 2);
    } else {
      // Placeholder: colored circle
      ctx.fillStyle = planet.color || '#888888';
      ctx.beginPath();
      ctx.arc(x, y, radius, 0, Math.PI * 2);
      ctx.fill();
    }
    
    // Draw planet name
    ctx.fillStyle = '#ffffff';
    ctx.font = '16px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(planet.name || 'Planet', x, y + radius + 20);
  }
}
```

---

## Save/Load System

```javascript
export class SaveSystem {
  save(game) {
    const saveData = {
      version: '1.0.0',
      timestamp: Date.now(),
      player: {
        credits: game.player.credits,
        experience: game.player.experience,
        level: game.player.level,
        ship: this.serializeShip(game.player.ship),
        inventory: {...game.player.inventory}
      },
      colonies: game.colonies.map(c => this.serializeColony(c)),
      currentSystemId: game.currentSystem.id
    };
    
    localStorage.setItem('starage_save', JSON.stringify(saveData));
  }
  
  load() {
    const data = localStorage.getItem('starage_save');
    if (!data) return null;
    
    try {
      const saveData = JSON.parse(data);
      
      // Calculate offline production
      this.calculateOfflineProgress(saveData);
      
      return saveData;
    } catch (e) {
      console.error('Failed to load save:', e);
      return null;
    }
  }
  
  calculateOfflineProgress(saveData) {
    const now = Date.now();
    const timeAway = now - saveData.timestamp;
    
    // Update each colony's production
    for (const colony of saveData.colonies) {
      for (const building of colony.buildings) {
        if (building.producing && building.productionRate) {
          const cycles = timeAway / building.productionRate;
          const produced = Math.floor(cycles) * building.storageMax;
          
          building.storageCurrent = Math.min(
            building.storageCurrent + produced,
            building.storageMax
          );
          
          building.lastCollected = now;
        }
      }
    }
  }
}
```

---

**Document Version:** 1.0  
**Last Updated:** November 24, 2025  
**Next:** asset-requirements.md
