import { CONFIG } from '../config.js';

export class Shipyard {
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

    // Check ship purchase buttons
    const ships = ['scout', 'fighter', 'destroyer'];
    const panelY = 100; // Match render function
    const shipStartX = CONFIG.CANVAS_WIDTH / 2 - 200;
    const shipStartY = panelY + 245; // Match render function
    const shipButtonWidth = 400;
    const shipButtonHeight = 80;
    const shipButtonSpacing = 90;

    for (let index = 0; index < ships.length; index++) {
      const shipClass = ships[index];
      const buttonY = shipStartY + index * shipButtonSpacing;
      if (
        this._isPointInside(x, y, {
          x: shipStartX,
          y: buttonY,
          width: shipButtonWidth,
          height: shipButtonHeight
        })
      ) {
        this._purchaseShip(shipClass);
        return true;
      }
    }

    // Check equipment purchase buttons
    const equipmentStartY = panelY + 575; // Match render function
    const equipmentButtonHeight = 70;
    const equipmentButtonSpacing = 80;

    // Improved Laser button
    if (
      this._isPointInside(x, y, {
        x: shipStartX,
        y: equipmentStartY,
        width: shipButtonWidth,
        height: equipmentButtonHeight
      })
    ) {
      this._purchaseEquipment('weapon', 'improvedLaser');
      return true;
    }

    // Improved Engine button
    if (
      this._isPointInside(x, y, {
        x: shipStartX,
        y: equipmentStartY + equipmentButtonSpacing,
        width: shipButtonWidth,
        height: equipmentButtonHeight
      })
    ) {
      this._purchaseEquipment('engine', 'improvedEngine');
      return true;
    }

    return false;
  }

  _purchaseShip(shipClass) {
    this.game.purchaseShip(shipClass);
  }

  _purchaseEquipment(type, equipmentId) {
    this.game.purchaseEquipment(type, equipmentId);
  }

  render(ctx) {
    if (!this.isOpen || !this.currentColony) return;

    // Dark overlay
    ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
    ctx.fillRect(0, 0, CONFIG.CANVAS_WIDTH, CONFIG.CANVAS_HEIGHT);

    // Main panel
    const panelWidth = 800;
    const panelHeight = 760;
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
      'Shipyard',
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

    this._renderCurrentStats(ctx, panelX + 40, panelY + 120, panelWidth - 80, 86);

    // Ships section header
    ctx.fillStyle = '#d7f9ff';
    ctx.font = '22px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('Ships:', panelX + 40, panelY + 220);

    // Render ship buttons
    const ships = [
      { class: 'scout', label: 'Scout' },
      { class: 'fighter', label: 'Fighter' },
      { class: 'destroyer', label: 'Destroyer' }
    ];
    const shipStartX = CONFIG.CANVAS_WIDTH / 2 - 200;
    const shipStartY = panelY + 245;
    const shipButtonWidth = 400;
    const shipButtonHeight = 80;
    const shipButtonSpacing = 90;

    ships.forEach((ship, index) => {
      const buttonY = shipStartY + index * shipButtonSpacing;
      this._renderShipButton(
        ctx,
        ship.class,
        ship.label,
        shipStartX,
        buttonY,
        shipButtonWidth,
        shipButtonHeight
      );
    });

    // Equipment section header
    ctx.fillStyle = '#d7f9ff';
    ctx.font = '22px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('Equipment:', panelX + 40, panelY + 545);

    // Render equipment buttons
    const equipmentStartY = panelY + 575;
    const equipmentButtonHeight = 70;
    const equipmentButtonSpacing = 80;

    this._renderEquipmentButton(
      ctx,
      'weapon',
      'improvedLaser',
      'Improved Laser',
      shipStartX,
      equipmentStartY,
      shipButtonWidth,
      equipmentButtonHeight
    );

    this._renderEquipmentButton(
      ctx,
      'engine',
      'improvedEngine',
      'Improved Engine',
      shipStartX,
      equipmentStartY + equipmentButtonSpacing,
      shipButtonWidth,
      equipmentButtonHeight
    );
  }

  _renderCurrentStats(ctx, x, y, width, height) {
    const stats = this.game.getCurrentShipStats();
    if (!stats) return;

    ctx.fillStyle = 'rgba(3, 10, 24, 0.55)';
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.25)';
    ctx.lineWidth = 1;
    this._roundedRect(ctx, x, y, width, height, 8);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#d7f9ff';
    ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(`Current: ${stats.label}`, x + 16, y + 26);

    ctx.fillStyle = '#cfe0ff';
    ctx.font = '14px "Segoe UI", Arial, sans-serif';
    ctx.fillText(`HP ${stats.hp}/${stats.maxHp}`, x + 16, y + 52);
    ctx.fillText(`Speed ${Math.round(stats.speed)}`, x + 130, y + 52);
    ctx.fillText(`Cargo ${stats.cargoUsed}/${stats.cargoCapacity}`, x + 250, y + 52);
    ctx.fillText(`Damage ${stats.weaponDamage}`, x + 405, y + 52);
    ctx.fillText(`Range ${stats.weaponRange}`, x + 530, y + 52);

    ctx.fillStyle = '#8fb6ff';
    ctx.fillText(`${stats.weaponLabel} | ${stats.engineLabel}`, x + 16, y + 76);
  }

  _renderShipButton(ctx, shipClass, label, x, y, width, height) {
    const shipStats = CONFIG.SHIP_STATS[shipClass];
    const isCurrentShip = this.game.player.ship.shipClass === shipClass;
    const canAfford = this.game.player.credits >= shipStats.cost;
    const canPurchase = !isCurrentShip && canAfford && shipStats.cost > 0;

    // Button background
    ctx.fillStyle = isCurrentShip
      ? 'rgba(76, 201, 240, 0.4)'
      : canPurchase
      ? 'rgba(76, 201, 240, 0.2)'
      : 'rgba(100, 100, 100, 0.15)';
    ctx.strokeStyle = isCurrentShip
      ? 'rgba(76, 201, 240, 0.9)'
      : canPurchase
      ? 'rgba(76, 201, 240, 0.6)'
      : 'rgba(100, 100, 100, 0.3)';
    ctx.lineWidth = isCurrentShip ? 3 : 2;
    this._roundedRect(ctx, x, y, width, height, 10);
    ctx.fill();
    ctx.stroke();

    // Draw ship sprite/image
    const shipSprite = this.game.assets?.ships?.[shipClass];
    const spriteSize = 64;
    const spriteX = x + 20;
    const spriteY = y + height / 2;

    if (shipSprite) {
      ctx.save();
      // Draw sprite with slight transparency if can't afford
      ctx.globalAlpha = canPurchase || isCurrentShip ? 1.0 : 0.5;
      
      // Draw the image - canvas will handle it even if still loading
      try {
        ctx.drawImage(
          shipSprite,
          spriteX,
          spriteY - spriteSize / 2,
          spriteSize,
          spriteSize
        );
      } catch (error) {
        // If drawing fails, show placeholder
        console.warn(`Failed to draw ship sprite for ${shipClass}:`, error);
        ctx.fillStyle = 'rgba(100, 100, 100, 0.5)';
        ctx.fillRect(spriteX, spriteY - spriteSize / 2, spriteSize, spriteSize);
      }
      ctx.restore();
    } else {
      // No sprite available - draw placeholder
      ctx.save();
      ctx.fillStyle = 'rgba(100, 100, 100, 0.3)';
      ctx.strokeStyle = 'rgba(150, 150, 150, 0.5)';
      ctx.lineWidth = 1;
      ctx.fillRect(spriteX, spriteY - spriteSize / 2, spriteSize, spriteSize);
      ctx.strokeRect(spriteX, spriteY - spriteSize / 2, spriteSize, spriteSize);
      ctx.restore();
    }

    // Ship name (moved right to make room for sprite)
    ctx.fillStyle = isCurrentShip ? '#4cc9f0' : canPurchase ? '#e0efff' : '#888888';
    ctx.font = '22px "Segoe UI Semibold", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(label, x + 100, y + 25);

    // Ship stats
    ctx.fillStyle = isCurrentShip ? '#cfe0ff' : canPurchase ? '#b8d4ff' : '#666666';
    ctx.font = '14px "Segoe UI", Arial, sans-serif';
    ctx.fillText(
      `HP: ${shipStats.hp} | Speed: ${shipStats.speed} | Cargo: ${shipStats.cargo}`,
      x + 100,
      y + 50
    );

    // Price or status (right aligned)
    ctx.textAlign = 'right';
    if (isCurrentShip) {
      ctx.fillStyle = '#4cc9f0';
      ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
      ctx.fillText('Current Ship', x + width - 20, y + 45);
    } else if (shipStats.cost === 0) {
      ctx.fillStyle = '#888888';
      ctx.font = '16px "Segoe UI", Arial, sans-serif';
      ctx.fillText('Starting Ship', x + width - 20, y + 45);
    } else if (canAfford) {
      ctx.fillStyle = '#ffd700';
      ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
      ctx.fillText(`${shipStats.cost.toLocaleString()}¢`, x + width - 20, y + 45);
    } else {
      ctx.fillStyle = '#ff5f7e';
      ctx.font = '16px "Segoe UI", Arial, sans-serif';
      ctx.fillText(`Need ${(shipStats.cost - this.game.player.credits).toLocaleString()}¢ more`, x + width - 20, y + 45);
    }
  }

  _renderEquipmentButton(ctx, type, equipmentId, label, x, y, width, height) {
    let config = null;
    let isEquipped = false;

    if (type === 'weapon') {
      config = CONFIG.WEAPONS[equipmentId];
      if (config && this.game.player.ship.weaponId) {
        isEquipped = this.game.player.ship.weaponId === equipmentId;
      }
    } else if (type === 'engine') {
      config = CONFIG.ENGINES[equipmentId];
      if (config && this.game.player.ship.engineId) {
        isEquipped = this.game.player.ship.engineId === equipmentId;
      }
    }

    if (!config) return;

    const canAfford = this.game.player.credits >= config.cost;
    const canPurchase = !isEquipped && canAfford;

    // Button background
    ctx.fillStyle = isEquipped
      ? 'rgba(76, 201, 240, 0.4)'
      : canPurchase
      ? 'rgba(76, 201, 240, 0.2)'
      : 'rgba(100, 100, 100, 0.15)';
    ctx.strokeStyle = isEquipped
      ? 'rgba(76, 201, 240, 0.9)'
      : canPurchase
      ? 'rgba(76, 201, 240, 0.6)'
      : 'rgba(100, 100, 100, 0.3)';
    ctx.lineWidth = isEquipped ? 3 : 2;
    this._roundedRect(ctx, x, y, width, height, 10);
    ctx.fill();
    ctx.stroke();

    // Equipment name
    ctx.fillStyle = isEquipped ? '#4cc9f0' : canPurchase ? '#e0efff' : '#888888';
    ctx.font = '20px "Segoe UI Semibold", Arial, sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText(label, x + 20, y + 25);

    // Equipment stats
    ctx.fillStyle = isEquipped ? '#cfe0ff' : canPurchase ? '#b8d4ff' : '#666666';
    ctx.font = '14px "Segoe UI", Arial, sans-serif';
    if (type === 'weapon') {
      ctx.fillText(
        `Damage: ${config.damage} | Range: ${config.range} | Fire Rate: ${config.fireRate}/s`,
        x + 20,
        y + 50
      );
    } else if (type === 'engine') {
      ctx.fillText(
        `Speed Multiplier: ${config.speedMultiplier}x`,
        x + 20,
        y + 50
      );
    }

    // Price or status (right aligned)
    ctx.textAlign = 'right';
    if (isEquipped) {
      ctx.fillStyle = '#4cc9f0';
      ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
      ctx.fillText('Equipped', x + width - 20, y + 40);
    } else if (canAfford) {
      ctx.fillStyle = '#ffd700';
      ctx.font = '18px "Segoe UI Semibold", Arial, sans-serif';
      ctx.fillText(`${config.cost.toLocaleString()}¢`, x + width - 20, y + 40);
    } else {
      ctx.fillStyle = '#ff5f7e';
      ctx.font = '16px "Segoe UI", Arial, sans-serif';
      ctx.fillText(`Need ${(config.cost - this.game.player.credits).toLocaleString()}¢ more`, x + width - 20, y + 40);
    }
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
