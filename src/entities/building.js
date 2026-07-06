import { CONFIG } from '../config.js';

export class Building {
  constructor({ type, gridX, gridY, colony }) {
    this.type = type;
    this.gridX = gridX;
    this.gridY = gridY;
    this.colony = colony;
    this.level = 1;
    this.storage = 0;
    this.lastProduction = Date.now();
  }

  getProductionRate() {
    const rates = {
      stoneExtractor: CONFIG.STONE_PRODUCTION_RATE, // 15 min
      iceExtractor: CONFIG.ICE_PRODUCTION_RATE, // 5 min
      uraniumMine: CONFIG.URANIUM_PRODUCTION_RATE, // 20 min
      fuelFactory: 30 * 60 * 1000 // 30 min
    };
    return rates[this.type] || 0;
  }

  getStorageCapacity() {
    const capacities = {
      storage: 500,
      stoneExtractor: 5,
      iceExtractor: 5,
      uraniumMine: 5,
      fuelFactory: 4
    };
    return capacities[this.type] || 0;
  }

  getResourceType() {
    const types = {
      stoneExtractor: 'stone',
      iceExtractor: 'ice',
      uraniumMine: 'uranium',
      fuelFactory: 'fuel'
    };
    return types[this.type] || null;
  }

  tick(currentTime) {
    if (this.type === 'storage') return;

    const timeSinceLastCollect = currentTime - this.lastProduction;
    const productionRate = this.getProductionRate();
    
    if (productionRate === 0) return;

    // Calculate how many units produced
    const unitsProduced = Math.floor(timeSinceLastCollect / productionRate);
    
    if (unitsProduced > 0) {
      const capacity = this.getStorageCapacity();
      this.storage = Math.min(this.storage + unitsProduced, capacity);
      
      // Update timestamp only if storage not full
      if (this.storage < capacity) {
        this.lastProduction = currentTime;
      } else {
        // Storage full, advance time to when next unit would be ready
        this.lastProduction = currentTime - (timeSinceLastCollect % productionRate);
      }
    }
  }

  collect() {
    const resourceType = this.getResourceType();
    if (!resourceType || this.storage === 0) return 0;

    const amount = this.storage;
    this.storage = 0;
    this.lastProduction = Date.now();
    return { resource: resourceType, amount };
  }
}

