export const CONFIG = {
  // Canvas
  CANVAS_WIDTH: 1600,
  CANVAS_HEIGHT: 900,
  TARGET_FPS: 60,
  STARFIELD_DENSITY: 0.00035,

  // Galaxy
  SYSTEM_COUNT: 6,
  PLANETS_PER_SYSTEM_MIN: 3,
  PLANETS_PER_SYSTEM_MAX: 4,
  PLANET_ORBIT_BASE: 300,
  PLANET_ORBIT_STEP: 160,
  PLANET_RADIUS_MIN: 40,
  PLANET_RADIUS_MAX: 70,
  PLANET_TYPES: {
    agrarian: {
      label: 'Agrarian',
      color: '#4caf50'
    },
    military: {
      label: 'Military',
      color: '#f44336'
    },
    mining: {
      label: 'Mining',
      color: '#ff9800'
    },
    industrial: {
      label: 'Industrial',
      color: '#8bc34a'
    },
    scientific: {
      label: 'Scientific',
      color: '#ffc107'
    }
  },

  // Ships
  SCOUT_HP: 100,
  SCOUT_SPEED: 200,
  SCOUT_CARGO: 200,
  SHIP_STATS: {
    scout: {
      label: 'Scout',
      hp: 100,
      speed: 200,
      cargo: 200,
      weaponSlots: 2,
      weapon: 'basicLaser',
      engine: 'basicEngine',
      cost: 0
    },
    fighter: {
      label: 'Fighter',
      hp: 200,
      speed: 150,
      cargo: 250,
      weaponSlots: 3,
      weapon: 'basicLaser',
      engine: 'basicEngine',
      cost: 5000
    },
    destroyer: {
      label: 'Destroyer',
      hp: 400,
      speed: 100,
      cargo: 300,
      weaponSlots: 4,
      weapon: 'basicLaser',
      engine: 'basicEngine',
      cost: 15000
    }
  },

  // Combat
  BASIC_LASER_DAMAGE: 10,
  BASIC_LASER_RANGE: 150,
  BASIC_LASER_FIRE_RATE: 1.0,
  BASIC_PROJECTILE_SPEED: 800,
  WEAPONS: {
    basicLaser: {
      label: 'Basic Laser',
      damage: 10,
      range: 150,
      fireRate: 1.0,
      projectileSpeed: 800,
      beamColor: '#ff6b6b',
      cost: 0
    },
    improvedLaser: {
      label: 'Improved Laser',
      damage: 15,
      range: 180,
      fireRate: 1.2,
      projectileSpeed: 800,
      beamColor: '#ff8c8c',
      cost: 2000
    }
  },

  // Engines
  ENGINES: {
    basicEngine: {
      label: 'Basic Engine',
      speedMultiplier: 1.0,
      cost: 0
    },
    improvedEngine: {
      label: 'Improved Engine',
      speedMultiplier: 1.25,
      cost: 1500
    }
  },

  // Space objects
  SPACE_OBJECTS_PER_SYSTEM: {
    asteroid: 14,
    comet: 8
  },
  SPACE_OBJECT_RESPAWN_MS: 5 * 60 * 1000,
  SPACE_OBJECT_CLICK_DAMAGE: 50,
  ASTEROID_HP: 50,
  COMET_HP: 40,
  SPACE_OBJECT_LOOT: {
    asteroid: { resource: 'stone', min: 2, max: 4 },
    comet: { resource: 'ice', min: 2, max: 4 }
  },

  // NPCs
  NPC_MINER_SPAWN_INTERVAL: 15000,
  NPC_PIRATE_SPAWN_INTERVAL: 60000,
  NPC_MAX_MINERS: 5,
  NPC_MAX_PIRATES: 3,
  NPC_SPAWN_RADIUS: 1000,
  NPC_STATS: {
    miner: {
      hp: 50,
      speed: 80,
      color: '#ffa500',
      weaponless: true,
      drops: {
        credits: { min: 20, max: 50 },
        stone: { min: 1, max: 2 }
      }
    },
    pirate: {
      hp: 100,
      speed: 120,
      color: '#ff0000',
      weapon: 'basicLaser',
      aggroRange: 300,
      damage: 10,
      drops: {
        credits: { min: 50, max: 150 },
        stone: { min: 1, max: 3 },
        ice: { min: 1, max: 2 }
      }
    }
  },

  // Resources
  STONE_PRODUCTION_RATE: 900000,
  ICE_PRODUCTION_RATE: 300000,
  URANIUM_PRODUCTION_RATE: 1200000,
  RESOURCE_PRICES: {
    stone: 3,
    ice: 2,
    uranium: 5,
    fuel: 20
  },

  // Colony
  COLONY_COSTS: [0, 2000, 5000, 10000, 20000, 40000],
  BUILDING_GRID_SIZE: 3,

  // Navigation
  SHIP_ARRIVAL_TOLERANCE: 8,
  SHIP_CAMERA_LERP: 0.12,

  // Cargo
  CARGO_CAPACITY: {
    scout: 200,
    fighter: 250,
    destroyer: 300
  },

  // Debug
  DEBUG_MODE: true,
  SHOW_FPS: true
};
