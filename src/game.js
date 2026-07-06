import { CONFIG } from './config.js';
import { GalaxySystem } from './systems/galaxy.js';
import { Ship } from './entities/ship.js';
import { InputHandler } from './utils/input.js';
import { lerp } from './utils/math.js';
import { loadImage } from './utils/assets.js';
import { GalaxyMapUI } from './ui/galaxyMap.js';
import { SpaceObjectSystem } from './systems/spaceObjects.js';
import { CombatSystem } from './systems/combat.js';
import { NPCSystem } from './systems/npc.js';
import { ColonySystem } from './systems/colony.js';
import { ColonyView } from './ui/colonyView.js';
import { TradingPost } from './ui/tradingPost.js';
import { Shipyard } from './ui/shipyard.js';
import { Building } from './entities/building.js';

const SAVE_KEY = 'starage_reborn_save';
const SAVE_VERSION = 2;

export class Game {
  constructor(canvas) {
    this.canvas = canvas;
    this.ctx = canvas.getContext('2d');
    this.mode = 'space'; // placeholder for upcoming modes
    this.fps = CONFIG.TARGET_FPS;
    this._fpsCounter = {
      elapsed: 0,
      frames: 0
    };

    this.galaxySystem = new GalaxySystem(this);
    this.galaxy = null;
    this.currentSystemIndex = 0;
    this.currentSystem = null;
    this.player = null;
    this.camera = { x: 0, y: 0 };
    this.worldOffset = { x: 0, y: 0 };
    this.assets = {
      ships: {}
    };
    this.mapButton = {
      x: CONFIG.CANVAS_WIDTH - 200,
      y: 20,
      width: 160,
      height: 44
    };
    this.ui = {
      galaxyMap: new GalaxyMapUI(this),
      colonyView: new ColonyView(this),
      tradingPost: new TradingPost(this),
      shipyard: new Shipyard(this)
    };
    this.transition = {
      active: false,
      elapsed: 0,
      duration: 1800,
      from: 0,
      to: 0
    };
    this.spaceObjectSystem = new SpaceObjectSystem(this);
    this.combatSystem = new CombatSystem(this);
    this.npcSystem = new NPCSystem(this);
    this.colonySystem = new ColonySystem(this);
    this.saveTimer = 0;
    this.saveInterval = 5000;

    this.input = new InputHandler(canvas);
    this.input.onClick = (x, y) => this.handleClick(x, y);
  }

  async init() {
    this.startTime = Date.now();
    await this._loadAssets();
    
    // Verify assets were loaded
    const loadedCount = Object.keys(this.assets.ships).filter(key => this.assets.ships[key] !== undefined).length;
    console.log(`Asset loading complete. Loaded ${loadedCount} ship sprites.`);
    
    this.galaxy = this.galaxySystem.generateGalaxy();
    this.currentSystem = this.galaxy.systems[this.currentSystemIndex];
    this.ui.galaxyMap.syncFromGalaxy(this.galaxy);
    this.player = this._createPlayer();
    
    // Establish starting colony
    const homePlanet = this.currentSystem.planets[0];
    const homeColony = this.colonySystem.establishColony(homePlanet, true);
    this.player.colonies = [homeColony];
    this.loadGame();
    
    this.camera.x = this.player.ship.position.x;
    this.camera.y = this.player.ship.position.y;
    this._updateWorldOffset();
  }

  update(deltaTime) {
    this._updateFpsCounter(deltaTime);

    if (this.transition.active) {
      this.transition.elapsed += deltaTime * 1000;
      if (this.transition.elapsed >= this.transition.duration) {
        this._completeSystemTravel();
      }
      return;
    }

    this.player.ship.update(deltaTime);
    this._updateCamera(deltaTime);
    this.spaceObjectSystem.update(deltaTime);
    this.npcSystem.update(deltaTime);
    this.combatSystem.update(deltaTime);
    this._updateAutosave(deltaTime);
    
    // Update colony production
    if (this.player.colonies) {
      const currentTime = Date.now();
      this.player.colonies.forEach((colony) => {
        this.colonySystem.updateProduction(colony, currentTime);
      });
    }
  }

  render(ctx) {
    this._renderSpaceBackdrop(ctx);
    this._renderCurrentSystem(ctx);
    this._renderSystemHeader(ctx);
    this._renderShipHud(ctx);
    this._renderResourceHud(ctx);
    this.ui.galaxyMap.render(ctx);
    this.ui.colonyView.render(ctx);
    this.ui.tradingPost.render(ctx);
    this.ui.shipyard.render(ctx);
    this._renderDebugOverlay(ctx);
    this._renderTransitionOverlay(ctx);
  }

  _renderSpaceBackdrop(ctx) {
    ctx.fillStyle = '#02030a';
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);

    if (!this.galaxy?.starfield) {
      return;
    }

    ctx.save();
    const parallax = 0.08;
    const wrapWidth = CONFIG.CANVAS_WIDTH * 2;
    const wrapHeight = CONFIG.CANVAS_HEIGHT * 2;
    this.galaxy.starfield.forEach((star) => {
      const drawX =
        ((star.x - this.camera.x * parallax) % wrapWidth + wrapWidth) %
        wrapWidth;
      const drawY =
        ((star.y - this.camera.y * parallax) % wrapHeight + wrapHeight) %
        wrapHeight;
      ctx.globalAlpha = star.brightness;
      ctx.fillStyle = '#ffffff';
      ctx.beginPath();
      ctx.arc(drawX, drawY, star.size, 0, Math.PI * 2);
      ctx.fill();
    });
    ctx.restore();
  }

  _renderWorld(ctx, renderFn) {
    ctx.save();
    ctx.translate(this.worldOffset.x, this.worldOffset.y);
    renderFn(ctx);
    ctx.restore();
  }

  _renderCurrentSystem(ctx) {
    if (!this.currentSystem) return;

    this._renderWorld(ctx, (worldCtx) => {
      this._renderSystemNebula(worldCtx);
      this._renderSystemOrbits(worldCtx);
      this.spaceObjectSystem.render(worldCtx);
      this.currentSystem.planets.forEach((planet) => {
        planet.render(worldCtx);
      });
      this.npcSystem.render(worldCtx);
      this._renderPlayerShip(worldCtx);
      this.combatSystem.render(worldCtx);
    });
  }

  _renderSystemNebula(ctx) {
    const gradient = ctx.createRadialGradient(0, 0, 200, 0, 0, 900);
    const hue = this.currentSystem.backgroundHue;
    gradient.addColorStop(0, `rgba(${hue}, 80, 140, 0.08)`);
    gradient.addColorStop(1, 'rgba(0, 0, 0, 0)');
    ctx.fillStyle = gradient;
    ctx.fillRect(
      -CONFIG.CANVAS_WIDTH,
      -CONFIG.CANVAS_HEIGHT,
      CONFIG.CANVAS_WIDTH * 2,
      CONFIG.CANVAS_HEIGHT * 2
    );
  }

  _renderSystemOrbits(ctx) {
    ctx.strokeStyle = 'rgba(255, 255, 255, 0.08)';
    ctx.lineWidth = 1;
    this.currentSystem.planets.forEach((planet) => {
      const radius = Math.hypot(planet.position.x, planet.position.y);

      ctx.beginPath();
      ctx.arc(0, 0, radius, 0, Math.PI * 2);
      ctx.stroke();
    });
  }

  _renderPlayerShip(ctx) {
    if (!this.player?.ship) return;
    this.player.ship.render(ctx);
  }

  _renderSystemHeader(ctx) {
    if (!this.currentSystem) return;

    ctx.fillStyle = 'rgba(2, 3, 10, 0.7)';
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, 80);

    ctx.fillStyle = '#89c2ff';
    ctx.font = '28px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(
      `Current System: ${this.currentSystem.name}`,
      40,
      50
    );

    ctx.fillStyle = '#cfe0ff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.fillText(
      `${this.currentSystem.planets.length} Planets · ${this.galaxy.totalPlanets} total in galaxy`,
      40,
      75
    );

    // Credits display
    if (this.player?.credits !== undefined) {
      ctx.fillStyle = '#ffd700';
      ctx.font = '20px "Segoe UI", Arial, sans-serif';
      ctx.textAlign = 'right';
      ctx.fillText(
        `Credits: ${this.player.credits.toLocaleString()}¢`,
        CONFIG.CANVAS_WIDTH - 220,
        50
      );
    }

    this._renderMapButton(ctx);
  }

  _renderMapButton(ctx) {
    const { x, y, width, height } = this.mapButton;
    const isActive = this.ui.galaxyMap.isOpen;
    const gradient = ctx.createLinearGradient(x, y, x + width, y + height);
    gradient.addColorStop(0, isActive ? '#4cc9f0' : '#1f2a44');
    gradient.addColorStop(1, isActive ? '#5390d9' : '#243a73');

    ctx.fillStyle = gradient;
    ctx.strokeStyle = 'rgba(255, 255, 255, 0.25)';
    ctx.lineWidth = 2;
    ctx.beginPath();
    this._roundedRectPath(ctx, x, y, width, height, 10);
    ctx.closePath();
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#e0efff';
    ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText('Galaxy Map', x + width / 2, y + height / 2);
  }

  _createPlayer() {
    const ship = new Ship({
      position: { x: 0, y: -CONFIG.PLANET_ORBIT_BASE + 40 },
      sprite: this.assets.ships.scout || null
    });
    return {
      ship,
      credits: 1000,
      cargo: {
        stone: 0,
        ice: 0,
        uranium: 0,
        fuel: 0,
        capacity: CONFIG.CARGO_CAPACITY.scout
      }
    };
  }

  getCurrentShipStats() {
    const stats = this.player?.ship?.getCurrentShipStats();
    if (!stats) return null;

    return {
      ...stats,
      cargoUsed: this._getCargoUsed(),
      cargoCapacity: this.player.cargo.capacity
    };
  }

  applyEquipmentStats() {
    if (!this.player?.ship) return null;

    this.player.ship.applyEquipmentStats();
    this.player.cargo.capacity =
      CONFIG.SHIP_STATS[this.player.ship.shipClass]?.cargo || CONFIG.CARGO_CAPACITY.scout;
    return this.getCurrentShipStats();
  }

  equipWeapon(weaponId) {
    if (!this.player?.ship || !CONFIG.WEAPONS[weaponId]) return false;

    const equipped = this.player.ship.equipWeapon(weaponId);
    if (equipped) {
      this.saveGame();
    }
    return equipped;
  }

  equipEngine(engineId) {
    if (!this.player?.ship || !CONFIG.ENGINES[engineId]) return false;

    const equipped = this.player.ship.equipEngine(engineId);
    if (equipped) {
      this.saveGame();
    }
    return equipped;
  }

  purchaseEquipment(type, equipmentId) {
    if (!this.player?.ship) {
      return { success: false, reason: 'No player ship' };
    }

    const collection = type === 'weapon' ? CONFIG.WEAPONS : CONFIG.ENGINES;
    const config = collection?.[equipmentId];
    if (!config) {
      return { success: false, reason: 'Unknown equipment' };
    }

    if (type === 'weapon' && this.player.ship.weaponId === equipmentId) {
      return { success: false, reason: 'Already equipped' };
    }
    if (type === 'engine' && this.player.ship.engineId === equipmentId) {
      return { success: false, reason: 'Already equipped' };
    }
    if (this.player.credits < config.cost) {
      return { success: false, reason: 'Not enough credits' };
    }

    this.player.credits -= config.cost;
    const equipped = type === 'weapon'
      ? this.equipWeapon(equipmentId)
      : this.equipEngine(equipmentId);

    if (!equipped) {
      this.player.credits += config.cost;
      return { success: false, reason: 'Could not equip item' };
    }

    this.saveGame();
    return { success: true, stats: this.getCurrentShipStats() };
  }

  purchaseShip(shipClass) {
    if (!this.player?.ship) {
      return { success: false, reason: 'No player ship' };
    }

    const shipStats = CONFIG.SHIP_STATS[shipClass];
    if (!shipStats) {
      return { success: false, reason: 'Unknown ship' };
    }
    if (this.player.ship.shipClass === shipClass) {
      return { success: false, reason: 'Already flying this ship' };
    }
    if (this.player.credits < shipStats.cost) {
      return { success: false, reason: 'Not enough credits' };
    }

    const currentShip = this.player.ship;
    const newShip = new Ship({
      id: 'player-ship',
      name: shipStats.label || shipClass,
      position: currentShip.position,
      shipClass,
      hp: shipStats.hp,
      weaponId: shipStats.weapon || 'basicLaser',
      engineId: shipStats.engine || 'basicEngine',
      sprite: this.assets.ships[shipClass] || this.assets.ships.scout || null
    });

    newShip.destination = { ...currentShip.destination };
    newShip.isMoving = currentShip.isMoving;
    if (newShip.isMoving) {
      newShip.refreshVelocity();
    }

    this.player.credits -= shipStats.cost;
    this.player.ship = newShip;
    this.player.cargo.capacity = shipStats.cargo;
    this.saveGame();

    return { success: true, stats: this.getCurrentShipStats() };
  }

  saveGame() {
    if (!this.player || typeof localStorage === 'undefined') return false;

    const saveData = {
      version: SAVE_VERSION,
      savedAt: Date.now(),
      currentSystemIndex: this.currentSystemIndex,
      player: {
        credits: this.player.credits,
        cargo: { ...this.player.cargo },
        ship: this._serializeShip(this.player.ship),
        colonies: (this.player.colonies || []).map((colony) => this._serializeColony(colony))
      }
    };

    try {
      localStorage.setItem(SAVE_KEY, JSON.stringify(saveData));
      return true;
    } catch (error) {
      console.warn('Failed to save game:', error);
      return false;
    }
  }

  loadGame() {
    if (typeof localStorage === 'undefined') return false;

    try {
      const raw = localStorage.getItem(SAVE_KEY) || localStorage.getItem('starage_save');
      if (!raw) {
        this.applyEquipmentStats();
        return false;
      }

      const saveData = JSON.parse(raw);
      this._applySaveData(saveData);
      return true;
    } catch (error) {
      console.warn('Failed to load save, starting fresh:', error);
      this.applyEquipmentStats();
      return false;
    }
  }

  _applySaveData(saveData) {
    const playerData = saveData.player || saveData;
    const shipData = playerData.ship || {};
    const shipClass = CONFIG.SHIP_STATS[shipData.shipClass] ? shipData.shipClass : 'scout';
    const shipStats = CONFIG.SHIP_STATS[shipClass];
    const weaponId = this._resolveWeaponId(shipData, shipStats.weapon);
    const engineId = this._resolveEngineId(shipData, shipStats.engine);

    const ship = new Ship({
      id: 'player-ship',
      name: shipStats.label,
      position: shipData.position || this.player.ship.position,
      shipClass,
      hp: Number.isFinite(shipData.hp) ? shipData.hp : shipStats.hp,
      weaponId,
      engineId,
      sprite: this.assets.ships[shipClass] || this.assets.ships.scout || null
    });
    ship.destination = shipData.destination || { ...ship.position };
    ship.isMoving = Boolean(shipData.isMoving);
    if (ship.isMoving) {
      ship.refreshVelocity();
    }

    this.player.ship = ship;
    this.player.credits = Number.isFinite(playerData.credits) ? playerData.credits : this.player.credits;
    this.player.cargo = this._normalizeCargo(playerData.cargo, shipStats.cargo);
    this.player.colonies = Array.isArray(playerData.colonies)
      ? playerData.colonies.map((colony) => this._hydrateColony(colony))
      : this.player.colonies;

    if (Number.isInteger(saveData.currentSystemIndex)) {
      this.currentSystemIndex = Math.min(
        Math.max(saveData.currentSystemIndex, 0),
        this.galaxy.systems.length - 1
      );
      this.currentSystem = this.galaxy.systems[this.currentSystemIndex];
    }

    this._syncColoniesToPlanets();
    this.applyEquipmentStats();
  }

  _serializeShip(ship) {
    return {
      shipClass: ship.shipClass,
      hp: ship.hp,
      weaponId: ship.weaponId || 'basicLaser',
      engineId: ship.engineId || ship.engine || 'basicEngine',
      position: { ...ship.position },
      destination: { ...ship.destination },
      isMoving: ship.isMoving
    };
  }

  _resolveWeaponId(shipData, fallback = 'basicLaser') {
    if (CONFIG.WEAPONS[shipData.weaponId]) {
      return shipData.weaponId;
    }

    if (shipData.weapon) {
      const match = Object.entries(CONFIG.WEAPONS).find(([, weapon]) => (
        weapon.damage === shipData.weapon.damage &&
        weapon.range === shipData.weapon.range &&
        weapon.fireRate === shipData.weapon.fireRate
      ));
      if (match) return match[0];
    }

    return CONFIG.WEAPONS[fallback] ? fallback : 'basicLaser';
  }

  _resolveEngineId(shipData, fallback = 'basicEngine') {
    const explicitId = shipData.engineId || shipData.engine;
    if (CONFIG.ENGINES[explicitId]) {
      return explicitId;
    }

    return CONFIG.ENGINES[fallback] ? fallback : 'basicEngine';
  }

  _serializeColony(colony) {
    return {
      planetId: colony.planetId,
      planetType: colony.planetType,
      planetName: colony.planetName,
      storage: { ...colony.storage },
      storageCapacity: colony.storageCapacity,
      establishedAt: colony.establishedAt || Date.now(),
      buildings: (colony.buildings || []).map((building) => ({
        type: building.type,
        gridX: building.gridX,
        gridY: building.gridY,
        level: building.level || 1,
        storage: building.storage || 0,
        lastProduction: building.lastProduction || Date.now()
      }))
    };
  }

  _hydrateColony(colonyData) {
    const colony = {
      planetId: colonyData.planetId,
      planetType: colonyData.planetType,
      planetName: colonyData.planetName,
      buildings: [],
      storage: this._normalizeResources(colonyData.storage),
      storageCapacity: colonyData.storageCapacity || 500,
      establishedAt: colonyData.establishedAt || Date.now()
    };

    colony.buildings = (colonyData.buildings || []).map((buildingData) => {
      const building = new Building({
        type: buildingData.type,
        gridX: buildingData.gridX,
        gridY: buildingData.gridY,
        colony
      });
      building.level = buildingData.level || 1;
      building.storage = buildingData.storage || 0;
      building.lastProduction = buildingData.lastProduction || Date.now();
      return building;
    });

    return colony;
  }

  _normalizeCargo(cargo = {}, capacity = CONFIG.CARGO_CAPACITY.scout) {
    return {
      ...this._normalizeResources(cargo),
      capacity: Number.isFinite(cargo.capacity) ? cargo.capacity : capacity
    };
  }

  _normalizeResources(resources = {}) {
    return {
      stone: Number.isFinite(resources.stone) ? resources.stone : 0,
      ice: Number.isFinite(resources.ice) ? resources.ice : 0,
      uranium: Number.isFinite(resources.uranium) ? resources.uranium : 0,
      fuel: Number.isFinite(resources.fuel) ? resources.fuel : 0
    };
  }

  _syncColoniesToPlanets() {
    const coloniesByPlanet = new Map(
      (this.player.colonies || []).map((colony) => [colony.planetId, colony])
    );

    this.galaxy.systems.forEach((system) => {
      system.planets.forEach((planet) => {
        const colony = coloniesByPlanet.get(planet.id);
        planet.colonized = Boolean(colony);
        planet.colony = colony || null;
      });
    });
  }

  _updateAutosave(deltaTime) {
    if (!this.player) return;

    this.saveTimer += deltaTime * 1000;
    if (this.saveTimer >= this.saveInterval) {
      this.saveTimer = 0;
      this.saveGame();
    }
  }

  _getCargoUsed() {
    const cargo = this.player?.cargo;
    if (!cargo) return 0;
    return cargo.stone + cargo.ice + cargo.uranium + cargo.fuel;
  }

  _updateCamera(deltaTime) {
    const targetX = this.player.ship.position.x;
    const targetY = this.player.ship.position.y;
    this.camera.x = lerp(this.camera.x, targetX, CONFIG.SHIP_CAMERA_LERP);
    this.camera.y = lerp(this.camera.y, targetY, CONFIG.SHIP_CAMERA_LERP);
    this._updateWorldOffset();
  }

  _updateWorldOffset() {
    this.worldOffset.x = CONFIG.CANVAS_WIDTH / 2 - this.camera.x;
    this.worldOffset.y = CONFIG.CANVAS_HEIGHT / 2 - this.camera.y;
  }

  _screenToWorld(x, y) {
    return {
      x: x - this.worldOffset.x,
      y: y - this.worldOffset.y
    };
  }

  handleClick(x, y) {
    if (this.transition.active) return;

    // Handle shipyard clicks first
    if (this.ui.shipyard.isOpen) {
      const handled = this.ui.shipyard.handleClick(x, y);
      if (handled) return;
    }

    // Handle trading post clicks
    if (this.ui.tradingPost.isOpen) {
      const handled = this.ui.tradingPost.handleClick(x, y);
      if (handled) return;
    }

    // Handle colony view clicks
    if (this.ui.colonyView.isOpen) {
      const handled = this.ui.colonyView.handleClick(x, y);
      if (handled) return;
    }

    if (this.ui.galaxyMap.isOpen) {
      this.ui.galaxyMap.handleClick(x, y);
      return;
    }

    if (this._isPointInside(x, y, this.mapButton)) {
      this.ui.galaxyMap.toggle();
      return;
    }

    if (this.mode === 'space') {
      const worldPoint = this._screenToWorld(x, y);
      
      // Check for planet clicks
      if (this.currentSystem) {
        for (const planet of this.currentSystem.planets) {
          const dist = Math.hypot(
            worldPoint.x - planet.position.x,
            worldPoint.y - planet.position.y
          );
          if (dist <= planet.radius + 20) {
            // Find colony for this planet
            const colony = this.player.colonies?.find(
              (c) => c.planetId === planet.id
            );
            if (colony) {
              // Open colony view
              this.ui.colonyView.open(colony);
            } else {
              // Try to colonize
              const result = this.colonySystem.colonizePlanet(this.player, planet);
              if (result.success) {
                this.ui.colonyView.open(result.colony);
              }
            }
            return;
          }
        }
      }
      
      // Check for NPC clicks (combat)
      const allNPCs = this.npcSystem.getAllNPCs();
      for (const npc of allNPCs) {
        if (npc.isDead) continue;
        const dist = Math.hypot(
          worldPoint.x - npc.position.x,
          worldPoint.y - npc.position.y
        );
        if (dist <= 30) {
          this.combatSystem.attackTarget(this.player.ship, npc);
          return;
        }
      }
      
      // Check for space object clicks
      const handled = this.spaceObjectSystem.handleClick(worldPoint);
      if (!handled) {
        this.player.ship.setDestination(worldPoint.x, worldPoint.y);
      }
    }
  }

  requestPlayerAttack(target) {
    this.combatSystem.attackTarget(this.player.ship, target);
  }

  requestSystemTravel(targetIndex) {
    if (
      targetIndex === this.currentSystemIndex ||
      targetIndex < 0 ||
      targetIndex >= this.galaxy.systems.length ||
      this.transition.active
    ) {
      return;
    }

    this.transition = {
      active: true,
      elapsed: 0,
      duration: 1800,
      from: this.currentSystemIndex,
      to: targetIndex
    };
    this.ui.galaxyMap.close();
  }

  _renderDebugOverlay(ctx) {
    if (!CONFIG.DEBUG_MODE || !CONFIG.SHOW_FPS) {
      return;
    }

    ctx.fillStyle = 'rgba(0, 0, 0, 0.55)';
    ctx.fillRect(20, 20, 160, 70);

    ctx.fillStyle = '#00ff9c';
    ctx.font = '16px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(`FPS: ${this.fps.toFixed(1)}`, 30, 45);
    const uptimeSeconds = ((Date.now() - this.startTime) / 1000).toFixed(1);
    ctx.fillText(`Uptime: ${uptimeSeconds}s`, 30, 70);
  }

  _renderTransitionOverlay(ctx) {
    if (!this.transition.active) return;
    const progress = this.transition.elapsed / this.transition.duration;

    ctx.save();
    ctx.fillStyle = `rgba(255, 255, 255, ${Math.sin(progress * Math.PI) * 0.45})`;
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);

    ctx.strokeStyle = 'rgba(120, 190, 255, 0.4)';
    ctx.lineWidth = 4;
    ctx.setLineDash([20, 10]);
    for (let i = 0; i < 6; i += 1) {
      ctx.beginPath();
      const offset = (progress * (CONFIG.CANVAS_WIDTH + 200)) % (CONFIG.CANVAS_WIDTH + 200);
      const y = (i / 5) * CONFIG.CANVAS_HEIGHT;
      ctx.moveTo(-100 + offset, y);
      ctx.lineTo(CONFIG.CANVAS_WIDTH + offset, y);
      ctx.stroke();
    }
    ctx.restore();
  }

  _updateFpsCounter(deltaTime) {
    this._fpsCounter.elapsed += deltaTime;
    this._fpsCounter.frames += 1;

    if (this._fpsCounter.elapsed >= 0.5) {
      this.fps = this._fpsCounter.frames / this._fpsCounter.elapsed;
      this._fpsCounter.elapsed = 0;
      this._fpsCounter.frames = 0;
    }
  }

  async _loadAssets() {
    console.log('Loading ship assets...');
    
    try {
      this.assets.ships.scout = await loadImage(
        '/ assets-provided/ships/scout.jpg'
      );
      console.log('✓ Scout sprite loaded');
    } catch (error) {
      console.error('✗ Failed to load scout sprite:', error);
    }
    
    try {
      this.assets.ships.fighter = await loadImage(
        '/ assets-provided/ships/fighter.jpg'
      );
      console.log('✓ Fighter sprite loaded');
    } catch (error) {
      console.error('✗ Failed to load fighter sprite:', error);
    }
    
    try {
      this.assets.ships.destroyer = await loadImage(
        '/ assets-provided/ships/destroyer.png'
      );
      console.log('✓ Destroyer sprite loaded');
    } catch (error) {
      console.error('✗ Failed to load destroyer sprite:', error);
    }
    
    try {
      this.assets.ships.miner = await loadImage(
        '/ assets-provided/ships/miner.jpg'
      );
      console.log('✓ Miner sprite loaded');
    } catch (error) {
      console.error('✗ Failed to load miner sprite:', error);
    }
    
    try {
      this.assets.ships.pirate = await loadImage(
        '/ assets-provided/ships/pirate.jpg'
      );
      console.log('✓ Pirate sprite loaded');
    } catch (error) {
      console.error('✗ Failed to load pirate sprite:', error);
    }
    
    // Log what was actually loaded
    const loadedShips = Object.keys(this.assets.ships).filter(key => this.assets.ships[key] !== undefined);
    console.log('Loaded ships:', loadedShips);
  }

  _isPointInside(px, py, rect) {
    return (
      px >= rect.x &&
      px <= rect.x + rect.width &&
      py >= rect.y &&
      py <= rect.y + rect.height
    );
  }

  _completeSystemTravel() {
    this.currentSystemIndex = this.transition.to;
    this.currentSystem = this.galaxy.systems[this.currentSystemIndex];
    this.ui.galaxyMap.syncFromGalaxy(this.galaxy);
    this.player.ship.position = { x: 0, y: -CONFIG.PLANET_ORBIT_BASE + 40 };
    this.player.ship.destination = { ...this.player.ship.position };
    this.player.ship.isMoving = false;
    this.camera.x = this.player.ship.position.x;
    this.camera.y = this.player.ship.position.y;
    this._updateWorldOffset();
    this.combatSystem.reset();
    this.npcSystem.reset();
    this.transition.active = false;
    this.transition.elapsed = 0;
  }

  _roundedRectPath(ctx, x, y, width, height, radius) {
    const r = Math.min(radius, width / 2, height / 2);
    ctx.moveTo(x + r, y);
    ctx.lineTo(x + width - r, y);
    ctx.arcTo(x + width, y, x + width, y + r, r);
    ctx.lineTo(x + width, y + height - r);
    ctx.arcTo(x + width, y + height, x + width - r, y + height, r);
    ctx.lineTo(x + r, y + height);
    ctx.arcTo(x, y + height, x, y + height - r, r);
    ctx.lineTo(x, y + r);
    ctx.arcTo(x, y, x + r, y, r);
  }

  _renderResourceHud(ctx) {
    if (!this.player?.cargo) return;
    const panelWidth = 330;
    const panelHeight = 110;
    const x = CONFIG.CANVAS_WIDTH - panelWidth - 30;
    const y = CONFIG.CANVAS_HEIGHT - panelHeight - 30;

    ctx.save();
    ctx.fillStyle = 'rgba(3, 10, 24, 0.8)';
    ctx.beginPath();
    this._roundedRectPath(ctx, x, y, panelWidth, panelHeight, 14);
    ctx.closePath();
    ctx.fill();

    const { cargo } = this.player;
    const cargoUsed = cargo.stone + cargo.ice + cargo.uranium + cargo.fuel;
    const percent = cargoUsed / cargo.capacity;

    ctx.fillStyle = '#d7f9ff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('Cargo Hold', x + 20, y + 30);

    ctx.fillStyle = '#8fb6ff';
    ctx.font = '14px "Segoe UI", Arial, sans-serif';
    ctx.fillText(`${cargoUsed}/${cargo.capacity} units`, x + 20, y + 50);

    ctx.fillStyle = 'rgba(255,255,255,0.2)';
    ctx.fillRect(x + 20, y + 60, panelWidth - 40, 8);
    ctx.fillStyle = percent >= 1 ? '#ff5f7e' : '#4cc9f0';
    ctx.fillRect(x + 20, y + 60, (panelWidth - 40) * Math.min(percent, 1), 8);

    ctx.fillStyle = '#cfe0ff';
    ctx.font = '16px "Segoe UI", Arial, sans-serif';
    ctx.fillText(`Stone: ${cargo.stone}`, x + 20, y + 90);
    ctx.fillText(`Ice: ${cargo.ice}`, x + 170, y + 90);
    ctx.restore();
  }

  _renderShipHud(ctx) {
    const stats = this.getCurrentShipStats();
    if (!stats) return;

    const panelWidth = 360;
    const panelHeight = 155;
    const x = 30;
    const y = CONFIG.CANVAS_HEIGHT - panelHeight - 30;

    ctx.save();
    ctx.fillStyle = 'rgba(3, 10, 24, 0.82)';
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.35)';
    ctx.lineWidth = 2;
    ctx.beginPath();
    this._roundedRectPath(ctx, x, y, panelWidth, panelHeight, 12);
    ctx.closePath();
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#d7f9ff';
    ctx.font = '20px "Segoe UI Semibold", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(`${stats.label}`, x + 18, y + 30);

    ctx.fillStyle = '#cfe0ff';
    ctx.font = '15px "Segoe UI", Arial, sans-serif';
    ctx.fillText(`HP: ${stats.hp}/${stats.maxHp}`, x + 18, y + 58);
    ctx.fillText(`Speed: ${Math.round(stats.speed)} (${stats.engineMultiplier}x)`, x + 180, y + 58);
    ctx.fillText(`Cargo: ${stats.cargoUsed}/${stats.cargoCapacity}`, x + 18, y + 83);
    ctx.fillText(`Weapon Slots: ${stats.weaponSlots}`, x + 180, y + 83);
    ctx.fillText(`Damage: ${stats.weaponDamage}`, x + 18, y + 108);
    ctx.fillText(`Range: ${stats.weaponRange}`, x + 180, y + 108);

    ctx.fillStyle = '#8fb6ff';
    ctx.font = '14px "Segoe UI", Arial, sans-serif';
    ctx.fillText(`Weapon: ${stats.weaponLabel}`, x + 18, y + 132);
    ctx.fillText(`Engine: ${stats.engineLabel}`, x + 180, y + 132);
    ctx.restore();
  }

  collectResource(resourceType, amount) {
    const cargo = this.player.cargo;
    const used = cargo.stone + cargo.ice + cargo.uranium + cargo.fuel;
    const available = Math.max(cargo.capacity - used, 0);
    const added = Math.min(amount, available);
    if (added <= 0) {
      return 0;
    }
    cargo[resourceType] += added;
    return added;
  }
}
