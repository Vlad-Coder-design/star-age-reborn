import { CONFIG } from '../config.js';

const TYPE_CONFIG = {
  asteroid: {
    baseRadius: 28,
    color: '#a8795c',
    hp: CONFIG.ASTEROID_HP
  },
  comet: {
    baseRadius: 22,
    color: '#6ce5ff',
    hp: CONFIG.COMET_HP
  }
};

export class SpaceObject {
  constructor({ id, type, position }) {
    this.id = id;
    this.type = type;
    this.position = position;
    this.config = TYPE_CONFIG[this.type];
    this.maxHp = this.config.hp;
    this.hp = this.maxHp;
    this.radius = this.config.baseRadius + Math.random() * 10;
    this.active = true;
    this.respawnTimer = 0;
    this.pulse = Math.random() * Math.PI * 2;
  }

  update(deltaTime) {
    if (this.active) {
      this.pulse += deltaTime * 2;
      return;
    }

    if (this.respawnTimer > 0) {
      this.respawnTimer -= deltaTime * 1000;
      if (this.respawnTimer <= 0) {
        this._respawn();
      }
    }
  }

  takeDamage(amount) {
    if (!this.active) return false;
    this.hp = Math.max(this.hp - amount, 0);
    if (this.hp === 0) {
      this.active = false;
      this.respawnTimer = CONFIG.SPACE_OBJECT_RESPAWN_MS;
      return true;
    }
    return false;
  }

  render(ctx) {
    if (!this.active) return;
    ctx.save();
    ctx.translate(this.position.x, this.position.y);

    const glow = Math.sin(this.pulse) * 0.15 + 0.2;
    const gradient = ctx.createRadialGradient(
      0,
      0,
      this.radius * 0.2,
      0,
      0,
      this.radius
    );
    gradient.addColorStop(
      0,
      this.type === 'comet'
        ? `rgba(173, 240, 255, ${glow + 0.3})`
        : `rgba(255, 226, 182, ${glow + 0.25})`
    );
    gradient.addColorStop(1, this.config.color);

    ctx.fillStyle = gradient;
    ctx.beginPath();
    ctx.arc(0, 0, this.radius, 0, Math.PI * 2);
    ctx.fill();

    if (this.type === 'comet') {
      ctx.strokeStyle = 'rgba(180, 240, 255, 0.4)';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.moveTo(-this.radius * 1.5, 0);
      ctx.quadraticCurveTo(
        -this.radius * 3,
        -this.radius * 0.6,
        -this.radius * 4,
        0
      );
      ctx.stroke();
    } else {
      ctx.strokeStyle = 'rgba(255, 255, 255, 0.15)';
      ctx.lineWidth = 1;
      ctx.setLineDash([6, 6]);
      ctx.beginPath();
      ctx.arc(0, 0, this.radius * 1.2, 0, Math.PI * 2);
      ctx.stroke();
    }

    ctx.restore();
  }

  _respawn() {
    this.hp = this.maxHp;
    this.active = true;
    this.respawnTimer = 0;
  }
}

