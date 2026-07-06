import { CONFIG } from '../config.js';
import { Ship } from '../entities/ship.js';
import { distance, normalize } from '../utils/math.js';
import { randomFloat, chooseRandom } from '../utils/random.js';

export class NPCSystem {
  constructor(game) {
    this.game = game;
    this.miners = [];
    this.pirates = [];
    this.minerSpawnTimer = 0;
    this.pirateSpawnTimer = 0;
    this.lootDrops = [];
  }

  update(deltaTime) {
    // Update spawn timers
    this.minerSpawnTimer += deltaTime * 1000;
    this.pirateSpawnTimer += deltaTime * 1000;

    // Spawn miners
    if (
      this.miners.length < CONFIG.NPC_MAX_MINERS &&
      this.minerSpawnTimer >= CONFIG.NPC_MINER_SPAWN_INTERVAL
    ) {
      this._spawnMiner();
      this.minerSpawnTimer = 0;
    }

    // Spawn pirates
    if (
      this.pirates.length < CONFIG.NPC_MAX_PIRATES &&
      this.pirateSpawnTimer >= CONFIG.NPC_PIRATE_SPAWN_INTERVAL
    ) {
      this._spawnPirate();
      this.pirateSpawnTimer = 0;
    }

    // Update miner AI
    this.miners.forEach((miner) => {
      if (miner.isDead) return;
      this._updateMinerAI(miner, deltaTime);
      miner.update(deltaTime);
    });

    // Update pirate AI
    this.pirates.forEach((pirate) => {
      if (pirate.isDead) return;
      this._updatePirateAI(pirate, deltaTime);
      pirate.update(deltaTime);
    });

    // Update loot drops
    this._updateLootDrops(deltaTime);

    // Clean up dead NPCs
    this.miners = this.miners.filter((m) => !m.isDead);
    this.pirates = this.pirates.filter((p) => !p.isDead);
  }

  render(ctx) {
    // Render miners
    this.miners.forEach((miner) => {
      if (!miner.isDead) {
        miner.render(ctx);
        this._renderHPBar(ctx, miner);
      }
    });

    // Render pirates
    this.pirates.forEach((pirate) => {
      if (!pirate.isDead) {
        pirate.render(ctx);
        this._renderHPBar(ctx, pirate);
      }
    });

    // Render loot drops
    this._renderLootDrops(ctx);
  }

  _spawnMiner() {
    const playerPos = this.game.player.ship.position;
    const angle = randomFloat(0, Math.PI * 2);
    const distance = randomFloat(400, CONFIG.NPC_SPAWN_RADIUS);
    const position = {
      x: playerPos.x + Math.cos(angle) * distance,
      y: playerPos.y + Math.sin(angle) * distance
    };

    const miner = new Ship({
      id: `miner-${Date.now()}-${Math.random()}`,
      name: 'Miner',
      position,
      shipClass: 'miner',
      speed: CONFIG.NPC_STATS.miner.speed,
      color: CONFIG.NPC_STATS.miner.color,
      sprite: this.game.assets?.ships?.miner || null
    });
    miner.maxHp = CONFIG.NPC_STATS.miner.hp;
    miner.hp = miner.maxHp;
    miner.isNPC = true;
    miner.npcType = 'miner';
    miner.targetAsteroid = null;
    miner.miningTimer = 0;

    this.miners.push(miner);
  }

  _spawnPirate() {
    const playerPos = this.game.player.ship.position;
    const angle = randomFloat(0, Math.PI * 2);
    const distance = randomFloat(400, CONFIG.NPC_SPAWN_RADIUS);
    const position = {
      x: playerPos.x + Math.cos(angle) * distance,
      y: playerPos.y + Math.sin(angle) * distance
    };

    const pirate = new Ship({
      id: `pirate-${Date.now()}-${Math.random()}`,
      name: 'Pirate',
      position,
      shipClass: 'pirate',
      speed: CONFIG.NPC_STATS.pirate.speed,
      color: CONFIG.NPC_STATS.pirate.color,
      sprite: this.game.assets?.ships?.pirate || null
    });
    pirate.maxHp = CONFIG.NPC_STATS.pirate.hp;
    pirate.hp = pirate.maxHp;
    pirate.isNPC = true;
    pirate.npcType = 'pirate';
    pirate.weapon = { ...CONFIG.WEAPONS.basicLaser };
    pirate.lastAttackTime = 0;
    pirate.aggroRange = CONFIG.NPC_STATS.pirate.aggroRange;

    this.pirates.push(pirate);
  }

  _updateMinerAI(miner, deltaTime) {
    // Find nearest asteroid
    const asteroids = this.game.spaceObjectSystem?._getCurrentObjects() || [];
    const activeAsteroids = asteroids.filter(
      (a) => a.type === 'asteroid' && a.active && !a.isDead
    );

    if (activeAsteroids.length === 0) {
      // Patrol randomly
      if (!miner.isMoving) {
        const angle = randomFloat(0, Math.PI * 2);
        const dist = randomFloat(200, 500);
        miner.setDestination(
          miner.position.x + Math.cos(angle) * dist,
          miner.position.y + Math.sin(angle) * dist
        );
      }
      return;
    }

    // Find nearest asteroid
    let nearestAsteroid = null;
    let nearestDist = Infinity;
    activeAsteroids.forEach((asteroid) => {
      const dist = distance(miner.position, asteroid.position);
      if (dist < nearestDist) {
        nearestDist = dist;
        nearestAsteroid = asteroid;
      }
    });

    if (nearestAsteroid) {
      miner.targetAsteroid = nearestAsteroid;
      const dist = distance(miner.position, nearestAsteroid.position);
      if (dist > 60) {
        // Move toward asteroid
        const destDist = distance(miner.position, miner.destination);
        if (!miner.isMoving || destDist < 10) {
          miner.setDestination(
            nearestAsteroid.position.x,
            nearestAsteroid.position.y
          );
        }
      } else {
        // Mining at asteroid (visual only, no actual mining in MVP)
        miner.isMoving = false;
        miner.velocity = { x: 0, y: 0 };
      }
    }
  }

  _updatePirateAI(pirate, deltaTime) {
    const playerShip = this.game.player.ship;
    const distToPlayer = distance(pirate.position, playerShip.position);

    // Check aggro range
    if (distToPlayer <= pirate.aggroRange) {
      // Attack player
      pirate.currentTarget = playerShip;
      this.game.combatSystem.attackTarget(pirate, playerShip);

      // Move toward player if out of range
      if (distToPlayer > pirate.weapon.range) {
        const destDist = distance(pirate.position, pirate.destination);
        if (!pirate.isMoving || destDist < 10) {
          pirate.setDestination(playerShip.position.x, playerShip.position.y);
        }
      } else {
        // Stop and attack
        pirate.isMoving = false;
        pirate.velocity = { x: 0, y: 0 };
      }
    } else {
      // Patrol randomly
      pirate.currentTarget = null;
      if (!pirate.isMoving) {
        const angle = randomFloat(0, Math.PI * 2);
        const dist = randomFloat(200, 400);
        pirate.setDestination(
          pirate.position.x + Math.cos(angle) * dist,
          pirate.position.y + Math.sin(angle) * dist
        );
      }
    }
  }

  handleNPCDamage(npc, damage) {
    // Damage already applied by combat system, just spawn loot if dead
    if (npc.isDead) {
      this._spawnLoot(npc);
    }
  }

  _spawnLoot(npc) {
    const stats = CONFIG.NPC_STATS[npc.npcType];
    if (!stats.drops) return;

    const loot = {
      position: { ...npc.position },
      credits: chooseRandom(
        Array.from(
          { length: stats.drops.credits.max - stats.drops.credits.min + 1 },
          (_, i) => stats.drops.credits.min + i
        )
      ),
      resources: {}
    };

    if (stats.drops.stone) {
      loot.resources.stone = chooseRandom(
        Array.from(
          { length: stats.drops.stone.max - stats.drops.stone.min + 1 },
          (_, i) => stats.drops.stone.min + i
        )
      );
    }
    if (stats.drops.ice) {
      loot.resources.ice = chooseRandom(
        Array.from(
          { length: stats.drops.ice.max - stats.drops.ice.min + 1 },
          (_, i) => stats.drops.ice.min + i
        )
      );
    }

    this.lootDrops.push({
      ...loot,
      life: 0,
      duration: 60000, // 60 seconds
      collected: false
    });
  }

  _updateLootDrops(deltaTime) {
    const playerPos = this.game.player.ship.position;
    for (let i = this.lootDrops.length - 1; i >= 0; i -= 1) {
      const loot = this.lootDrops[i];
      loot.life += deltaTime * 1000;

      // Auto-collect if player nearby
      if (!loot.collected) {
        const dist = distance(playerPos, loot.position);
        if (dist < 50) {
          // Collect loot
          this.game.player.credits =
            (this.game.player.credits || 0) + loot.credits;
          Object.keys(loot.resources).forEach((resource) => {
            this.game.collectResource(resource, loot.resources[resource]);
          });
          loot.collected = true;
        }
      }

      // Remove expired loot
      if (loot.life >= loot.duration || loot.collected) {
        this.lootDrops.splice(i, 1);
      }
    }
  }

  _renderLootDrops(ctx) {
    ctx.save();
    this.lootDrops.forEach((loot) => {
      if (loot.collected) return;
      const alpha = 1 - loot.life / loot.duration;
      ctx.globalAlpha = alpha;

      // Draw loot marker
      ctx.fillStyle = '#ffd700';
      ctx.beginPath();
      ctx.arc(
        loot.position.x + this.game.worldOffset.x,
        loot.position.y + this.game.worldOffset.y,
        12,
        0,
        Math.PI * 2
      );
      ctx.fill();

      // Draw credits text
      ctx.fillStyle = '#ffffff';
      ctx.font = '14px "Segoe UI", Arial, sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(
        `${loot.credits}¢`,
        loot.position.x + this.game.worldOffset.x,
        loot.position.y + this.game.worldOffset.y - 25
      );
    });
    ctx.restore();
  }

  _renderHPBar(ctx, npc) {
    if (npc.hp >= npc.maxHp) return;

    const barWidth = 40;
    const barHeight = 4;
    const hpPercent = npc.hp / npc.maxHp;

    ctx.save();
    ctx.translate(
      npc.position.x + this.game.worldOffset.x,
      npc.position.y + this.game.worldOffset.y - 40
    );

    // Background
    ctx.fillStyle = 'rgba(0, 0, 0, 0.6)';
    ctx.fillRect(-barWidth / 2, -barHeight / 2, barWidth, barHeight);

    // HP bar
    ctx.fillStyle = npc.hp <= npc.maxHp * 0.3 ? '#ff0000' : '#00ff00';
    ctx.fillRect(-barWidth / 2, -barHeight / 2, barWidth * hpPercent, barHeight);

    ctx.restore();
  }

  reset() {
    // Clear all NPCs when changing systems
    this.miners = [];
    this.pirates = [];
    this.lootDrops = [];
    this.minerSpawnTimer = 0;
    this.pirateSpawnTimer = 0;
  }

  getAllNPCs() {
    return [...this.miners, ...this.pirates];
  }
}

