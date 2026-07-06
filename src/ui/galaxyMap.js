import { CONFIG } from '../config.js';

const OVERLAY_PADDING = 80;
const NODE_RADIUS = 28;
const NODE_SPACING_X = 260;
const NODE_SPACING_Y = 180;

export class GalaxyMapUI {
  constructor(game) {
    this.game = game;
    this.isOpen = false;
    this.nodes = [];
    this._computeNodeLayout();
  }

  toggle() {
    this.isOpen = !this.isOpen;
  }

  close() {
    this.isOpen = false;
  }

  handleClick(x, y) {
    if (!this.isOpen) return false;

    for (const node of this.nodes) {
      const dx = x - node.screen.x;
      const dy = y - node.screen.y;
      if (Math.hypot(dx, dy) <= NODE_RADIUS + 10) {
        this.game.requestSystemTravel(node.id);
        return true;
      }
    }

    // click outside nodes closes overlay
    const overlayRect = this._getOverlayRect();
    if (
      x < overlayRect.x ||
      x > overlayRect.x + overlayRect.width ||
      y < overlayRect.y ||
      y > overlayRect.y + overlayRect.height
    ) {
      this.close();
    }

    return true;
  }

  render(ctx) {
    if (!this.isOpen) return;

    ctx.save();
    ctx.fillStyle = 'rgba(3, 8, 18, 0.78)';
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);

    const overlay = this._getOverlayRect();
    const gradient = ctx.createLinearGradient(
      overlay.x,
      overlay.y,
      overlay.x + overlay.width,
      overlay.y + overlay.height
    );
    gradient.addColorStop(0, 'rgba(6, 18, 56, 0.92)');
    gradient.addColorStop(1, 'rgba(16, 30, 78, 0.92)');
    ctx.fillStyle = gradient;
    ctx.fillRect(overlay.x, overlay.y, overlay.width, overlay.height);

    this._renderHeader(ctx, overlay);
    this._renderConnections(ctx);
    this._renderNodes(ctx);

    ctx.restore();
  }

  _renderHeader(ctx, overlay) {
    ctx.fillStyle = '#d7f9ff';
    ctx.font = '32px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('Galaxy Navigation Network', overlay.x + 40, overlay.y + 60);

    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.fillStyle = '#8fb6ff';
    ctx.fillText(
      'Select a star system to initiate hyperspace travel. Travel is instantaneous but limited to explored systems.',
      overlay.x + 40,
      overlay.y + 95
    );
  }

  _renderConnections(ctx) {
    ctx.strokeStyle = 'rgba(92, 146, 255, 0.35)';
    ctx.lineWidth = 2;
    ctx.setLineDash([12, 10]);

    this.nodes.forEach((node, index) => {
      if (index === 0) return;
      const prev = this.nodes[index - 1];
      ctx.beginPath();
      ctx.moveTo(prev.screen.x, prev.screen.y);
      ctx.lineTo(node.screen.x, node.screen.y);
      ctx.stroke();
    });

    ctx.setLineDash([]);
  }

  _renderNodes(ctx) {
    this.nodes.forEach((node) => {
      const isCurrent = node.id === this.game.currentSystemIndex;
      ctx.save();
      ctx.translate(node.screen.x, node.screen.y);

      const haloGradient = ctx.createRadialGradient(0, 0, 0, 0, 0, NODE_RADIUS + 16);
      haloGradient.addColorStop(
        0,
        isCurrent ? 'rgba(255, 255, 255, 0.4)' : 'rgba(100, 170, 255, 0.25)'
      );
      haloGradient.addColorStop(1, 'rgba(0, 0, 0, 0)');
      ctx.fillStyle = haloGradient;
      ctx.beginPath();
      ctx.arc(0, 0, NODE_RADIUS + 16, 0, Math.PI * 2);
      ctx.fill();

      ctx.fillStyle = isCurrent ? '#ffffff' : '#6ec5ff';
      ctx.strokeStyle = isCurrent ? '#ffffff' : 'rgba(255, 255, 255, 0.4)';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(0, 0, NODE_RADIUS, 0, Math.PI * 2);
      ctx.fill();
      ctx.stroke();

      ctx.fillStyle = isCurrent ? '#0a1028' : '#091428';
      ctx.font = '20px "Segoe UI", Arial, sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(`S${node.id + 1}`, 0, 7);

      ctx.fillStyle = '#e8f2ff';
      ctx.font = '18px "Segoe UI", Arial, sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(node.name, 0, NODE_RADIUS + 28);

      ctx.fillStyle = '#8fb6ff';
      ctx.font = '14px "Segoe UI", Arial, sans-serif';
      ctx.fillText(
        `${node.planetCount} planets`,
        0,
        NODE_RADIUS + 48
      );

      ctx.restore();
    });
  }

  _computeNodeLayout() {
    this.nodes = [];
    const rows = 2;
    const cols = Math.ceil(CONFIG.SYSTEM_COUNT / rows);
    const startX =
      CONFIG.CANVAS_WIDTH / 2 - ((cols - 1) * NODE_SPACING_X) / 2;
    const startY =
      CONFIG.CANVAS_HEIGHT / 2 - ((rows - 1) * NODE_SPACING_Y) / 2;

    for (let i = 0; i < CONFIG.SYSTEM_COUNT; i += 1) {
      const row = Math.floor(i / cols);
      const col = i % cols;
      this.nodes.push({
        id: i,
        name: '',
        planetCount: 0,
        screen: {
          x: startX + col * NODE_SPACING_X,
          y: startY + row * NODE_SPACING_Y
        }
      });
    }
  }

  syncFromGalaxy(galaxy) {
    if (!galaxy) return;
    galaxy.systems.forEach((system, index) => {
      if (!this.nodes[index]) return;
      this.nodes[index].name = system.name;
      this.nodes[index].planetCount = system.planets.length;
    });
  }

  _getOverlayRect() {
    return {
      x: OVERLAY_PADDING,
      y: OVERLAY_PADDING,
      width: CONFIG.CANVAS_WIDTH - OVERLAY_PADDING * 2,
      height: CONFIG.CANVAS_HEIGHT - OVERLAY_PADDING * 2
    };
  }
}

