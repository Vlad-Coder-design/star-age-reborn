import { CONFIG } from '../config.js';
import { Planet } from '../entities/planet.js';
import { chooseRandom, randomFloat, randomInt } from '../utils/random.js';

const SYSTEM_NAME_PREFIXES = [
  'Aurora',
  'Celestis',
  'Eclipse',
  'Helios',
  'Kepler',
  'Lyra',
  'Nova',
  'Orion',
  'Solace',
  'Vega'
];

const SYSTEM_NAME_SUFFIXES = [
  'Prime',
  'Reach',
  'Hold',
  'Station',
  'Spur',
  'Ridge',
  'Arc',
  'Depths',
  'Frontier',
  'Range'
];

const PLANET_NAME_PREFIXES = [
  'Astra',
  'Cinder',
  'Dawn',
  'Echo',
  'Frost',
  'Gale',
  'Halo',
  'Iris',
  'Jade',
  'Lumen',
  'Myth',
  'Nyx',
  'Onyx',
  'Pulse',
  'Quill',
  'Rift',
  'Spire',
  'Terra',
  'Umber',
  'Vesper'
];

const PLANET_NAME_SUFFIXES = [
  '-12',
  '-4b',
  '-6c',
  '-7d',
  '-9a',
  '-Prime',
  '-Sec',
  '-III',
  '-IV',
  '-VX'
];

export class GalaxySystem {
  constructor(game) {
    this.game = game;
    this.planetTypes = Object.keys(CONFIG.PLANET_TYPES);
  }

  generateGalaxy() {
    const systems = [];
    let totalPlanets = 0;

    for (let i = 0; i < CONFIG.SYSTEM_COUNT; i += 1) {
      const system = this.generateSystem(i);
      totalPlanets += system.planets.length;
      systems.push(system);
    }

    const starfield = this.generateStarfield();

    return {
      systems,
      totalPlanets,
      starfield
    };
  }

  generateSystem(index) {
    const planetCount = randomInt(
      CONFIG.PLANETS_PER_SYSTEM_MIN,
      CONFIG.PLANETS_PER_SYSTEM_MAX
    );

    const systemName = this.generateSystemName(index);

    const system = {
      id: index,
      name: systemName,
      description: `${systemName} · ${planetCount} planets`,
      planets: [],
      backgroundHue: randomInt(220, 260)
    };

    for (let i = 0; i < planetCount; i += 1) {
      const orbitRadius =
        CONFIG.PLANET_ORBIT_BASE + i * CONFIG.PLANET_ORBIT_STEP;
      const angle = (i / planetCount) * Math.PI * 2 + randomFloat(-0.2, 0.2);
      const position = {
        x: Math.cos(angle) * orbitRadius,
        y: Math.sin(angle) * orbitRadius
      };

      const type = chooseRandom(this.planetTypes);
      const color = CONFIG.PLANET_TYPES[type].color;
      const planet = new Planet({
        id: `${system.id}-planet-${i}`,
        name: this.generatePlanetName(),
        systemId: system.id,
        type,
        position,
        radius: randomInt(
          CONFIG.PLANET_RADIUS_MIN,
          CONFIG.PLANET_RADIUS_MAX
        ),
        color
      });

      system.planets.push(planet);
    }

    return system;
  }

  generateSystemName(index) {
    const baseName = `${chooseRandom(SYSTEM_NAME_PREFIXES)} ${chooseRandom(
      SYSTEM_NAME_SUFFIXES
    )}`;
    return `${baseName} · S${index + 1}`;
  }

  generatePlanetName() {
    return `${chooseRandom(PLANET_NAME_PREFIXES)}${chooseRandom(
      PLANET_NAME_SUFFIXES
    )}`;
  }

  generateStarfield() {
    const canvasArea = CONFIG.CANVAS_WIDTH * CONFIG.CANVAS_HEIGHT;
    const starCount = Math.floor(canvasArea * CONFIG.STARFIELD_DENSITY);

    const stars = [];
    for (let i = 0; i < starCount; i += 1) {
      stars.push({
        x: (Math.random() - 0.5) * CONFIG.CANVAS_WIDTH * 2,
        y: (Math.random() - 0.5) * CONFIG.CANVAS_HEIGHT * 2,
        brightness: randomFloat(0.2, 0.9),
        size: randomFloat(0.5, 2)
      });
    }

    return stars;
  }
}

