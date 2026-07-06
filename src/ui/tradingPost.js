import { CONFIG } from '../config.js';

export class TradingPost {
  constructor(game) {
    this.game = game;
    this.isOpen = false;
    this.currentColony = null;
  }

  open(colony) {
    this.isOpen = true;
    this.currentColony = colony;
  }

  close() {
    this.isOpen = false;
    this.currentColony = null;
  }

  toggle(colony) {
    if (this.isOpen) {
      this.close();
    } else if (colony) {
      this.open(colony);
    }
  }

  handleClick(x, y) {
    if (!this.isOpen) return false;

    // Check close button
    if (this._isPointInside(x, y, { x: CONFIG.CANVAS_WIDTH - 60, y: 20, width: 40, height: 40 })) {
      this.close();
      return true;
    }

    // Check sell buttons for each resource
    const resources = ['stone', 'ice', 'uranium', 'fuel'];
    const buttonStartX = CONFIG.CANVAS_WIDTH / 2 - 200;
    const buttonStartY = 350;
    const buttonWidth = 400;
    const buttonHeight = 60;
    const buttonSpacing = 70;

    resources.forEach((resource, index) => {
      const buttonY = buttonStartY + index * buttonSpacing;
      if (
        this._isPointInside(x, y, {
          x: buttonStartX,
          y: buttonY,
          width: buttonWidth,
          height: buttonHeight
        })
      ) {
        this._sellResource(resource);
        return true;
      }
    });

    return false;
  }

  _sellResource(resourceType) {
    if (!this.currentColony || !this.game.player) return;

    const amount = this.currentColony.storage[resourceType] || 0;
    if (amount <= 0) {
      // Could show error message here
      return;
    }

    const price = CONFIG.RESOURCE_PRICES[resourceType] || 0;
    if (price <= 0) {
      return;
    }

    // Sell all available
    const creditsEarned = amount * price;
    this.currentColony.storage[resourceType] = 0;
    this.game.player.credits += creditsEarned;
  }

  render(ctx) {
    if (!this.isOpen || !this.currentColony) return;

    // Dark overlay
    ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);

    // Main panel
    const panelWidth = 800;
    const panelHeight = 700;
    const panelX = CONFIG.CANVAS_WIDTH / 2 - panelWidth / 2;
    const panelY = 100;

    ctx.fillStyle = 'rgba(15, 25, 45, 0.95)';
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.6)';
    ctx.lineWidth = 3;
    this._roundedRect(ctx, panelX, panelY, panelWidth, panelHeight, 20);
    ctx.fill();
    ctx.stroke();

    // Header
    ctx.fillStyle = '#d7f9ff';
    ctx.font = '32px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(
      'Trading Post',
      CONFIG.CANVAS_WIDTH / 2,
      panelY + 50
    );

    // Close button
    ctx.fillStyle = 'rgba(255, 100, 100, 0.8)';
    ctx.fillRect(CONFIG.CANVAS_WIDTH - 60, 20, 40, 40);
    ctx.fillStyle = '#ffffff';
    ctx.font = '24px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('×', CONFIG.CANVAS_WIDTH - 40, 48);

    // Player credits display
    ctx.fillStyle = '#ffd700';
    ctx.font = '24px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(
      `Credits: ${this.game.player.credits.toLocaleString()}¢`,
      CONFIG.CANVAS_WIDTH / 2,
      panelY + 100
    );

    // Instructions
    ctx.fillStyle = '#cfe0ff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(
      'Click on a resource to sell all available',
      CONFIG.CANVAS_WIDTH / 2,
      panelY + 140
    );

    // Resource list
    const resources = [
      { type: 'stone', label: 'Stone' },
      { type: 'ice', label: 'Ice' },
      { type: 'uranium', label: 'Uranium' },
      { type: 'fuel', label: 'Fuel' }
    ];

    const buttonStartX = CONFIG.CANVAS_WIDTH / 2 - 200;
    const buttonStartY = panelY + 180;
    const buttonWidth = 400;
    const buttonHeight = 60;
    const buttonSpacing = 70;

    resources.forEach((resource, index) => {
      const buttonY = buttonStartY + index * buttonSpacing;
      this._renderResourceButton(
        ctx,
        resource.type,
        resource.label,
        buttonStartX,
        buttonY,
        buttonWidth,
        buttonHeight
      );
    });

    // Colony storage summary at bottom
    this._renderStorageSummary(ctx, panelX + 40, panelY + panelHeight - 120);
  }

  _renderResourceButton(ctx, resourceType, label, x, y, width, height) {
    const amount = this.currentColony.storage[resourceType] || 0;
    const price = CONFIG.RESOURCE_PRICES[resourceType] || 0;
    const totalValue = amount * price;
    const canSell = amount > 0;

    // Button background
    ctx.fillStyle = canSell
      ? 'rgba(76, 201, 240, 0.2)'
      : 'rgba(100, 100, 100, 0.15)';
    ctx.strokeStyle = canSell
      ? 'rgba(76, 201, 240, 0.6)'
      : 'rgba(100, 100, 100, 0.3)';
    ctx.lineWidth = 2;
    this._roundedRect(ctx, x, y, width, height, 10);
    ctx.fill();
    ctx.stroke();

    // Resource name and amount
    ctx.fillStyle = canSell ? '#e0efff' : '#888888';
    ctx.font = '20px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(`${label}: ${amount}`, x + 20, y + 25);

    // Price and total value
    ctx.font = '16px "Segoe UI", Arial, sans-serif';
    ctx.fillText(
      `${price}¢ per unit`,
      x + 20,
      y + 45
    );

    // Total value (right aligned)
    ctx.textAlign = 'right';
    if (canSell) {
      ctx.fillStyle = '#ffd700';
      ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
      ctx.fillText(
        `Sell All: ${totalValue}¢`,
        x + width - 20,
        y + 35
      );
    } else {
      ctx.fillStyle = '#888888';
      ctx.font = '16px "Segoe UI", Arial, sans-serif';
      ctx.fillText('No stock', x + width - 20, y + 35);
    }
  }

  _renderStorageSummary(ctx, x, y) {
    ctx.fillStyle = '#cfe0ff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('Colony Storage:', x, y);

    ctx.font = '16px "Segoe UI", Arial, sans-serif';
    const resources = ['stone', 'ice', 'uranium', 'fuel'];
    resources.forEach((resource, index) => {
      const amount = this.currentColony.storage[resource] || 0;
      const capacity = this.currentColony.storageCapacity;
      ctx.fillText(
        `${resource.charAt(0).toUpperCase() + resource.slice(1)}: ${amount}/${capacity}`,
        x + (index % 2) * 200,
        y + 30 + Math.floor(index / 2) * 25
      );
    });
  }

  _isPointInside(px, py, rect) {
    return (
      px >= rect.x &&
      px <= rect.x + rect.width &&
      py >= rect.y &&
      py <= rect.y + rect.height
    );
  }

  _roundedRect(ctx, x, y, width, height, radius) {
    const r = Math.min(radius, width / 2, height / 2);
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.lineTo(x + width - r, y);
    ctx.quadraticCurveTo(x + width, y, x + width, y + r);
    ctx.lineTo(x + width, y + height - r);
    ctx.quadraticCurveTo(x + width, y + height, x + width - r, y + height);
    ctx.lineTo(x + r, y + height);
    ctx.quadraticCurveTo(x, y + height, x, y + height - r);
    ctx.lineTo(x, y + r);
    ctx.quadraticCurveTo(x, y, x + r, y);
    ctx.closePath();
  }
}

