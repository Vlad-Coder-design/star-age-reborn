import { CONFIG } from '../config.js';

export class Planet {
  constructor({
    id,
    name,
    systemId,
    type,
    position,
    radius,
    color
  }) {
    this.id = id;
    this.name = name;
    this.systemId = systemId;
    this.type = type;
    this.position = position;
    this.radius = radius;
    this.color = color;
    this.colonized = false;
  }

  render(ctx) {
    ctx.save();
    ctx.translate(this.position.x, this.position.y);

    ctx.fillStyle = this.color;
    ctx.beginPath();
    ctx.arc(0, 0, this.radius, 0, Math.PI * 2);
    ctx.fill();

    ctx.strokeStyle = 'rgba(255, 255, 255, 0.2)';
    ctx.lineWidth = 2;
    ctx.stroke();

    ctx.fillStyle = '#ffffff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'top';
    const label = `${this.name} · ${CONFIG.PLANET_TYPES[this.type].label}`;
    ctx.fillText(label, 0, this.radius + 12);

    ctx.restore();
  }
}

