import { CONFIG } from '../config.js';
import { Building } from '../entities/building.js';

export class ColonySystem {
  constructor(game) {
    this.game = game;
  }

  establishColony(planet, isHome = false) {
    if (planet.colonized) {
      return null;
    }

    const colony = {
      planetId: planet.id,
      planetType: planet.type,
      planetName: planet.name,
      buildings: [],
      storage: {
        stone: isHome ? 5 : 0,
        ice: isHome ? 5 : 0,
        uranium: 0,
        fuel: 0
      },
      storageCapacity: 500,
      establishedAt: Date.now()
    };

    planet.colonized = true;
    planet.colony = colony;

    return colony;
  }

  canBuildBuilding(colony, buildingType, gridX, gridY) {
    // Check if slot is occupied
    const occupied = colony.buildings.some(
      (b) => b.gridX === gridX && b.gridY === gridY
    );
    if (occupied) return false;

    // Check planet restrictions
    if (buildingType === 'uraniumMine' && colony.planetType !== 'military') {
      return false;
    }

    // Check resource costs
    const costs = this.getBuildingCosts(buildingType);
    if (!costs) return false;

    // Check if colony has resources
    for (const [resource, amount] of Object.entries(costs)) {
      if (colony.storage[resource] < amount) {
        return false;
      }
    }

    return true;
  }

  buildBuilding(colony, buildingType, gridX, gridY) {
    if (!this.canBuildBuilding(colony, buildingType, gridX, gridY)) {
      return false;
    }

    // Deduct costs
    const costs = this.getBuildingCosts(buildingType);
    for (const [resource, amount] of Object.entries(costs)) {
      colony.storage[resource] -= amount;
    }

    // Create building
    const building = new Building({
      type: buildingType,
      gridX,
      gridY,
      colony
    });

    colony.buildings.push(building);
    return building;
  }

  getBuildingCosts(buildingType) {
    const costs = {
      storage: { stone: 3, ice: 2 },
      stoneExtractor: { stone: 2 },
      iceExtractor: { ice: 2 },
      uraniumMine: { stone: 3, ice: 2 },
      fuelFactory: { stone: 5, ice: 3 }
    };
    return costs[buildingType] || null;
  }

  updateProduction(colony, currentTime) {
    colony.buildings.forEach((building) => {
      building.tick(currentTime);
    });
  }

  collectFromBuilding(colony, building) {
    const result = building.collect();
    if (result && result.amount > 0) {
      const resource = result.resource;
      const capacity = colony.storageCapacity;
      const current = colony.storage[resource];
      const toAdd = Math.min(result.amount, capacity - current);
      colony.storage[resource] += toAdd;
      return toAdd;
    }
    return 0;
  }

  collectAll(colony) {
    let totalCollected = 0;
    colony.buildings.forEach((building) => {
      const collected = this.collectFromBuilding(colony, building);
      totalCollected += collected;
    });
    return totalCollected;
  }

  getColonizationCost(colonyCount) {
    return CONFIG.COLONY_COSTS[colonyCount] || Infinity;
  }

  canColonize(player, planet) {
    if (planet.colonized) return false;
    const colonyCount = player.colonies?.length || 0;
    const cost = this.getColonizationCost(colonyCount);
    return player.credits >= cost;
  }

  colonizePlanet(player, planet) {
    if (!this.canColonize(player, planet)) {
      return { success: false, reason: 'Cannot colonize' };
    }

    const colonyCount = player.colonies?.length || 0;
    const cost = this.getColonizationCost(colonyCount);
    player.credits -= cost;

    const colony = this.establishColony(planet, false);
    if (!player.colonies) {
      player.colonies = [];
    }
    player.colonies.push(colony);

    return { success: true, colony };
  }
}

