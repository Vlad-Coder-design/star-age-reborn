import { CONFIG } from '../config.js';
import { distance, normalize } from '../utils/math.js';

export class Ship {
  constructor({
    id = 'player-ship',
    name = 'Scout',
    position = { x: 0, y: 0 },
    shipClass = 'scout',
    speed = null,
    hp = null,
    weaponId = null,
    engineId = null,
    color = '#00f5d4',
    sprite = null
  } = {}) {
    this.id = id;
    this.name = name;
    this.position = { ...position };
    this.destination = { ...position };
    this.velocity = { x: 0, y: 0 };
    this.speed = speed ?? CONFIG.SHIP_STATS[shipClass]?.speed ?? CONFIG.SCOUT_SPEED;
    this.color = color;
    this.sprite = sprite;
    this.spriteSize = 72;
    this.arrivalTolerance = CONFIG.SHIP_ARRIVAL_TOLERANCE;
    this.isMoving = false;
    this.shipClass = shipClass;
    this.weaponId = weaponId || CONFIG.SHIP_STATS[shipClass]?.weapon || 'basicLaser';
    this.engineId = engineId || CONFIG.SHIP_STATS[shipClass]?.engine || 'basicEngine';
    this.engine = this.engineId;
    this.maxHp = CONFIG.SHIP_STATS[shipClass]?.hp || CONFIG.SCOUT_HP;
    this.hp = hp ?? this.maxHp;
    this.isDead = false;
    this.weapon = { ...CONFIG.WEAPONS[this.weaponId] };
    this.fireCooldown = 0;
    this.currentTarget = null;
    this.applyEquipmentStats();
  }

  getCurrentShipStats() {
    const shipStats = CONFIG.SHIP_STATS[this.shipClass] || {
      label: this.name,
      hp: this.maxHp,
      speed: this.speed,
      cargo: 0,
      weaponSlots: 0
    };
    const weapon = CONFIG.WEAPONS[this.weaponId] || CONFIG.WEAPONS.basicLaser;
    const engine = CONFIG.ENGINES[this.engineId] || CONFIG.ENGINES.basicEngine;

    return {
      shipClass: this.shipClass,
      label: shipStats.label || this.name,
      hp: this.hp,
      maxHp: this.maxHp,
      baseSpeed: shipStats.speed,
      speed: shipStats.speed * engine.speedMultiplier,
      cargo: shipStats.cargo,
      weaponSlots: shipStats.weaponSlots || 0,
      weaponId: this.weaponId,
      weaponLabel: weapon.label || this.weaponId,
      weaponDamage: weapon.damage,
      weaponRange: weapon.range,
      engineId: this.engineId,
      engineLabel: engine.label || this.engineId,
      engineMultiplier: engine.speedMultiplier
    };
  }

  applyEquipmentStats() {
    const shipStats = CONFIG.SHIP_STATS[this.shipClass] || {
      label: this.name,
      hp: this.maxHp,
      speed: this.speed,
      weapon: 'basicLaser',
      engine: 'basicEngine'
    };
    const weaponId = CONFIG.WEAPONS[this.weaponId] ? this.weaponId : shipStats.weapon;
    const engineId = CONFIG.ENGINES[this.engineId] ? this.engineId : shipStats.engine;
    const hpPercent = this.maxHp > 0 ? this.hp / this.maxHp : 1;

    this.weaponId = weaponId || 'basicLaser';
    this.engineId = engineId || 'basicEngine';
    this.engine = this.engineId;
    this.weapon = { ...CONFIG.WEAPONS[this.weaponId] };
    this.speed = shipStats.speed;
    this.maxHp = shipStats.hp || this.maxHp;
    this.hp = Math.max(1, Math.min(this.maxHp, Math.round(this.maxHp * hpPercent)));
    this.name = shipStats.label || this.name;

    if (this.isMoving) {
      this.refreshVelocity();
    }
  }

  equipWeapon(weaponId) {
    if (!CONFIG.WEAPONS[weaponId]) return false;
    this.weaponId = weaponId;
    this.applyEquipmentStats();
    return true;
  }

  equipEngine(engineId) {
    if (!CONFIG.ENGINES[engineId]) return false;
    this.engineId = engineId;
    this.applyEquipmentStats();
    return true;
  }

  refreshVelocity() {
    const remaining = distance(this.position, this.destination);
    if (remaining <= this.arrivalTolerance) {
      this.velocity = { x: 0, y: 0 };
      this.isMoving = false;
      return;
    }

    const dir = normalize(
      this.destination.x - this.position.x,
      this.destination.y - this.position.y
    );
    const engineMultiplier = CONFIG.ENGINES[this.engineId]?.speedMultiplier || 1.0;
    const effectiveSpeed = this.speed * engineMultiplier;
    this.velocity = {
      x: dir.x * effectiveSpeed,
      y: dir.y * effectiveSpeed
    };
  }

  setDestination(targetX, targetY) {
    this.destination = { x: targetX, y: targetY };
    this.isMoving = true;
    this.refreshVelocity();
  }

  update(deltaTime) {
    if (!this.isMoving) {
      return;
    }

    const toDestination = distance(this.position, this.destination);
    if (toDestination <= this.arrivalTolerance) {
      this.position.x = this.destination.x;
      this.position.y = this.destination.y;
      this.velocity = { x: 0, y: 0 };
      this.isMoving = false;
      return;
    }

    this.position.x += this.velocity.x * deltaTime;
    this.position.y += this.velocity.y * deltaTime;
  }

  render(ctx) {
    const angle = Math.atan2(this.velocity.y, this.velocity.x) || -Math.PI / 2;

    ctx.save();
    ctx.translate(this.position.x, this.position.y);
    ctx.rotate(angle);

    if (this.sprite) {
      ctx.drawImage(
        this.sprite,
        -this.spriteSize / 2,
        -this.spriteSize / 2,
        this.spriteSize,
        this.spriteSize
      );
    } else {
      ctx.fillStyle = this.color;
      ctx.beginPath();
      ctx.moveTo(0, -18);
      ctx.lineTo(12, 14);
      ctx.lineTo(-12, 14);
      ctx.closePath();
      ctx.fill();

      ctx.fillStyle = 'rgba(0, 245, 212, 0.25)';
      ctx.beginPath();
      ctx.moveTo(0, 6);
      ctx.lineTo(8, 18);
      ctx.lineTo(-8, 18);
      ctx.closePath();
      ctx.fill();
    }

    ctx.restore();

    if (this.isMoving) {
      this._renderDestinationMarker(ctx);
    }
  }

  takeDamage(amount) {
    if (this.isDead) return;
    this.hp = Math.max(this.hp - amount, 0);
    if (this.hp === 0) {
      this.isDead = true;
    }
  }

  setTarget(target) {
    this.currentTarget = target;
  }

  _renderDestinationMarker(ctx) {
    ctx.save();
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.6)';
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(this.destination.x, this.destination.y, 18, 0, Math.PI * 2);
    ctx.stroke();

    ctx.strokeStyle = 'rgba(76, 201, 240, 0.4)';
    ctx.setLineDash([6, 4]);
    ctx.beginPath();
    ctx.moveTo(this.position.x, this.position.y);
    ctx.lineTo(this.destination.x, this.destination.y);
    ctx.stroke();
    ctx.restore();
  }
}
