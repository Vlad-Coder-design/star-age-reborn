import { CONFIG } from '../config.js';
import { SpaceObject } from '../entities/spaceObject.js';
import { chooseRandom, randomFloat } from '../utils/random.js';

export class SpaceObjectSystem {
  constructor(game) {
    this.game = game;
    this.objectsBySystem = new Map();
    this.floatingLoot = [];
  }

  initSystem(system) {
    if (this.objectsBySystem.has(system.id)) return;

    const objects = [];
    const spawnObject = (type, index) => {
      const orbit = CONFIG.PLANET_ORBIT_BASE + 120 + index * 80;
      const angle = randomFloat(0, Math.PI * 2);
      const position = {
        x: Math.cos(angle) * orbit,
        y: Math.sin(angle) * orbit
      };
      objects.push(
        new SpaceObject({
          id: `${system.id}-${type}-${index}`,
          type,
          position
        })
      );
    };

    for (let i = 0; i < CONFIG.SPACE_OBJECTS_PER_SYSTEM.asteroid; i += 1) {
      spawnObject('asteroid', i);
    }

    for (let i = 0; i < CONFIG.SPACE_OBJECTS_PER_SYSTEM.comet; i += 1) {
      spawnObject('comet', i + 30);
    }

    this.objectsBySystem.set(system.id, objects);
  }

  update(deltaTime) {
    const objects = this._getCurrentObjects();
    objects.forEach((obj) => obj.update(deltaTime));
    this._updateFloatingLoot(deltaTime);
  }

  render(ctx) {
    const objects = this._getCurrentObjects();
    objects.forEach((obj) => obj.render(ctx));
    this._renderFloatingLoot(ctx);
  }

  handleClick(worldPoint) {
    const objects = this._getCurrentObjects();
    for (const obj of objects) {
      if (!obj.active) continue;
      const dx = worldPoint.x - obj.position.x;
      const dy = worldPoint.y - obj.position.y;
      const distance = Math.hypot(dx, dy);
      if (distance <= obj.radius + 10) {
        const destroyed = obj.takeDamage(CONFIG.SPACE_OBJECT_CLICK_DAMAGE);
        if (destroyed) {
          this._grantLoot(obj);
        }
        return true;
      }
    }
    return false;
  }

  _grantLoot(obj) {
    const lootConfig = CONFIG.SPACE_OBJECT_LOOT[obj.type];
    const amount = chooseRandom(
      Array.from(
        { length: lootConfig.max - lootConfig.min + 1 },
        (_, idx) => lootConfig.min + idx
      )
    );
    const granted = this.game.collectResource(lootConfig.resource, amount);
    this.floatingLoot.push({
      x: obj.position.x,
      y: obj.position.y,
      text: `+${granted} ${lootConfig.resource}`,
      life: 0,
      duration: 1200
    });
  }

  _getCurrentObjects() {
    if (!this.game.currentSystem) return [];
    if (!this.objectsBySystem.has(this.game.currentSystem.id)) {
      this.initSystem(this.game.currentSystem);
    }
    return this.objectsBySystem.get(this.game.currentSystem.id) || [];
  }

  _updateFloatingLoot(deltaTime) {
    for (let i = this.floatingLoot.length - 1; i >= 0; i -= 1) {
      const loot = this.floatingLoot[i];
      loot.life += deltaTime * 1000;
      loot.y -= deltaTime * 20;
      if (loot.life >= loot.duration) {
        this.floatingLoot.splice(i, 1);
      }
    }
  }

  _renderFloatingLoot(ctx) {
    ctx.save();
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    this.floatingLoot.forEach((loot) => {
      const alpha = 1 - loot.life / loot.duration;
      ctx.fillStyle = `rgba(235, 255, 255, ${alpha})`;
      ctx.fillText(loot.text, loot.x, loot.y);
    });
    ctx.restore();
  }
}

