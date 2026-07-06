import { CONFIG } from '../config.js';
import { distance, normalize } from '../utils/math.js';

export class CombatSystem {
  constructor(game) {
    this.game = game;
    this.activeAttacks = new Map();
    this.projectiles = [];
  }

  attackTarget(attacker, target) {
    if (!attacker || !target || attacker.isDead || target.isDead) {
      return false;
    }

    const dist = distance(attacker.position, target.position);
    if (dist > attacker.weapon.range) {
      return false;
    }

    attacker.setTarget(target);
    this.activeAttacks.set(attacker, {
      target,
      cooldown: 0
    });
    return true;
  }

  stopAttack(attacker) {
    if (this.activeAttacks.has(attacker)) {
      attacker.setTarget(null);
      this.activeAttacks.delete(attacker);
    }
  }

  update(deltaTime) {
    this._updateAttacks(deltaTime);
    this._updateProjectiles(deltaTime);
  }

  render(ctx) {
    this._renderTargetIndicators(ctx);
    this._renderProjectiles(ctx);
  }

  reset() {
    this.activeAttacks.clear();
    this.projectiles = [];
  }

  _updateAttacks(deltaTime) {
    for (const [attacker, state] of this.activeAttacks.entries()) {
      if (!state.target || state.target.isDead) {
        this.stopAttack(attacker);
        continue;
      }

      const dist = distance(attacker.position, state.target.position);
      if (dist > attacker.weapon.range * 1.1) {
        this.stopAttack(attacker);
        continue;
      }

      state.cooldown -= deltaTime;
      if (state.cooldown <= 0) {
        this._fireProjectile(attacker, state.target);
        state.cooldown = 1 / attacker.weapon.fireRate;
      }
    }
  }

  _fireProjectile(attacker, target) {
    const dir = normalize(
      target.position.x - attacker.position.x,
      target.position.y - attacker.position.y
    );
    const projectile = {
      position: { ...attacker.position },
      velocity: {
        x: dir.x * (attacker.weapon.projectileSpeed || CONFIG.BASIC_PROJECTILE_SPEED),
        y: dir.y * (attacker.weapon.projectileSpeed || CONFIG.BASIC_PROJECTILE_SPEED)
      },
      target,
      damage: attacker.weapon.damage,
      color: attacker.weapon.beamColor || '#ff9e9e',
      ttl: 1.5
    };
    this.projectiles.push(projectile);
  }

  _updateProjectiles(deltaTime) {
    for (let i = this.projectiles.length - 1; i >= 0; i -= 1) {
      const projectile = this.projectiles[i];
      projectile.ttl -= deltaTime;
      projectile.position.x += projectile.velocity.x * deltaTime;
      projectile.position.y += projectile.velocity.y * deltaTime;

      if (projectile.ttl <= 0) {
        this.projectiles.splice(i, 1);
        continue;
      }

      if (
        projectile.target &&
        !projectile.target.isDead &&
        distance(projectile.position, projectile.target.position) < 20
      ) {
        const wasAlive = !projectile.target.isDead;
        projectile.target.takeDamage(projectile.damage);
        
        // Handle NPC death
        if (projectile.target.isNPC && projectile.target.isDead && wasAlive) {
          this.game.npcSystem.handleNPCDamage(projectile.target, projectile.damage);
        }
        
        this.projectiles.splice(i, 1);
      }
    }
  }

  _renderProjectiles(ctx) {
    this.projectiles.forEach((projectile) => {
      ctx.save();
      ctx.strokeStyle = projectile.color;
      ctx.globalAlpha = 0.8;
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.moveTo(projectile.position.x, projectile.position.y);
      ctx.lineTo(
        projectile.position.x - projectile.velocity.x * 0.05,
        projectile.position.y - projectile.velocity.y * 0.05
      );
      ctx.stroke();
      ctx.restore();
    });
  }

  _renderTargetIndicators(ctx) {
    this.activeAttacks.forEach((state) => {
      if (!state.target || state.target.isDead) return;
      ctx.save();
      ctx.strokeStyle = 'rgba(255, 105, 180, 0.8)';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(state.target.position.x, state.target.position.y, 40, 0, Math.PI * 2);
      ctx.stroke();
      ctx.restore();
    });
  }
}

