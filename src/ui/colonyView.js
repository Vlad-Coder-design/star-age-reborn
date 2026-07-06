import { CONFIG } from '../config.js';

export class ColonyView {
  constructor(game) {
    this.game = game;
    this.isOpen = false;
    this.currentColony = null;
    this.selectedSlot = null;
    this.showBuildingMenu = false;
    this.gridSize = 3;
    this.slotSize = 120;
    this.gridStartX = CONFIG.CANVAS_WIDTH / 2 - (this.gridSize * this.slotSize) / 2;
    this.gridStartY = 200;
  }

  open(colony) {
    this.isOpen = true;
    this.currentColony = colony;
    this.selectedSlot = null;
    this.showBuildingMenu = false;
  }

  close() {
    this.isOpen = false;
    this.currentColony = null;
    this.selectedSlot = null;
    this.showBuildingMenu = false;
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

    // Check collect button
    const collectButton = {
      x: CONFIG.CANVAS_WIDTH / 2 - 400 + 40,
      y: 100 + 700 - 100,
      width: 200,
      height: 50
    };
    if (this._isPointInside(x, y, collectButton)) {
      if (this.currentColony) {
        this.game.colonySystem.collectAll(this.currentColony);
      }
      return true;
    }

    // Check trading post button
    const tradingPostButton = {
      x: CONFIG.CANVAS_WIDTH / 2 - 400 + 260,
      y: 100 + 700 - 100,
      width: 200,
      height: 50
    };
    if (this._isPointInside(x, y, tradingPostButton)) {
      if (this.currentColony && this.game.ui.tradingPost) {
        this.game.ui.tradingPost.open(this.currentColony);
      }
      return true;
    }

    // Check shipyard button
    const shipyardButton = {
      x: CONFIG.CANVAS_WIDTH / 2 - 400 + 480,
      y: 100 + 700 - 100,
      width: 200,
      height: 50
    };
    if (this._isPointInside(x, y, shipyardButton)) {
      if (this.currentColony && this.game.ui.shipyard) {
        this.game.ui.shipyard.open(this.currentColony);
      }
      return true;
    }

    // Check building menu clicks
    if (this.showBuildingMenu) {
      const handled = this._handleBuildingMenuClick(x, y);
      if (handled) return true;
    }

    // Check grid slot clicks
    const slot = this._getSlotAt(x, y);
    if (slot) {
      this.selectedSlot = slot;
      const hasBuilding = this._hasBuildingAt(slot.x, slot.y);
      if (!hasBuilding) {
        this.showBuildingMenu = true;
      }
      return true;
    }

    // Click outside menu closes it
    if (this.showBuildingMenu) {
      this.showBuildingMenu = false;
      return true;
    }

    return false;
  }

  _getSlotAt(x, y) {
    const gridX = Math.floor((x - this.gridStartX) / this.slotSize);
    const gridY = Math.floor((y - this.gridStartY) / this.slotSize);
    
    if (gridX >= 0 && gridX < this.gridSize && gridY >= 0 && gridY < this.gridSize) {
      return { x: gridX, y: gridY };
    }
    return null;
  }

  _hasBuildingAt(gridX, gridY) {
    if (!this.currentColony) return false;
    return this.currentColony.buildings.some(
      (b) => b.gridX === gridX && b.gridY === gridY
    );
  }

  _handleBuildingMenuClick(x, y) {
    const menuX = CONFIG.CANVAS_WIDTH / 2 - 200;
    const menuY = this.gridStartY + this.gridSize * this.slotSize + 40;
    const menuWidth = 400;
    const buildingTypes = [
      'storage',
      'stoneExtractor',
      'iceExtractor',
      'uraniumMine',
      'fuelFactory'
    ];

    buildingTypes.forEach((type, index) => {
      const buttonY = menuY + index * 60;
      if (
        x >= menuX &&
        x <= menuX + menuWidth &&
        y >= buttonY &&
        y <= buttonY + 50
      ) {
        this._buildBuilding(type);
        this.showBuildingMenu = false;
        return true;
      }
    });

    return false;
  }

  _buildBuilding(buildingType) {
    if (!this.currentColony || !this.selectedSlot) return;

    const success = this.game.colonySystem.buildBuilding(
      this.currentColony,
      buildingType,
      this.selectedSlot.x,
      this.selectedSlot.y
    );

    if (!success) {
      // Show error message (could add UI for this)
      console.warn('Cannot build building');
    }
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
      `${this.currentColony.planetName} - ${CONFIG.PLANET_TYPES[this.currentColony.planetType]?.label || 'Unknown'}`,
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

    // Render grid
    this._renderGrid(ctx);

    // Render buildings
    this._renderBuildings(ctx);

    // Render storage info
    this._renderStorage(ctx, panelX + 40, panelY + panelHeight - 180);

    // Render building menu if slot selected
    if (this.showBuildingMenu && this.selectedSlot) {
      this._renderBuildingMenu(ctx);
    }

    // Collect All button
    this._renderCollectButton(ctx, panelX + 40, panelY + panelHeight - 100);
    
    // Trading Post button
    this._renderTradingPostButton(ctx, panelX + 260, panelY + panelHeight - 100);
    
    // Shipyard button
    this._renderShipyardButton(ctx, panelX + 480, panelY + panelHeight - 100);
  }

  _renderGrid(ctx) {
    ctx.strokeStyle = 'rgba(200, 220, 255, 0.3)';
    ctx.lineWidth = 2;

    for (let x = 0; x <= this.gridSize; x += 1) {
      ctx.beginPath();
      ctx.moveTo(
        this.gridStartX + x * this.slotSize,
        this.gridStartY
      );
      ctx.lineTo(
        this.gridStartX + x * this.slotSize,
        this.gridStartY + this.gridSize * this.slotSize
      );
      ctx.stroke();
    }

    for (let y = 0; y <= this.gridSize; y += 1) {
      ctx.beginPath();
      ctx.moveTo(
        this.gridStartX,
        this.gridStartY + y * this.slotSize
      );
      ctx.lineTo(
        this.gridStartX + this.gridSize * this.slotSize,
        this.gridStartY + y * this.slotSize
      );
      ctx.stroke();
    }

    // Highlight selected slot
    if (this.selectedSlot) {
      ctx.fillStyle = 'rgba(76, 201, 240, 0.2)';
      ctx.fillRect(
        this.gridStartX + this.selectedSlot.x * this.slotSize + 2,
        this.gridStartY + this.selectedSlot.y * this.slotSize + 2,
        this.slotSize - 4,
        this.slotSize - 4
      );
    }
  }

  _renderBuildings(ctx) {
    if (!this.currentColony) return;

    this.currentColony.buildings.forEach((building) => {
      const x = this.gridStartX + building.gridX * this.slotSize + this.slotSize / 2;
      const y = this.gridStartY + building.gridY * this.slotSize + this.slotSize / 2;

      // Building background
      ctx.fillStyle = 'rgba(100, 150, 200, 0.4)';
      ctx.fillRect(
        this.gridStartX + building.gridX * this.slotSize + 10,
        this.gridStartY + building.gridY * this.slotSize + 10,
        this.slotSize - 20,
        this.slotSize - 20
      );

      // Building label
      ctx.fillStyle = '#e0efff';
      ctx.font = '14px "Segoe UI", Arial, sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(
        this._getBuildingLabel(building.type),
        x,
        y
      );

      // Storage indicator
      if (building.storage > 0) {
        ctx.fillStyle = '#ffd700';
        ctx.font = '12px Arial';
        ctx.fillText(`${building.storage}`, x, y + 20);
      }
    });
  }

  _getBuildingLabel(type) {
    const labels = {
      storage: 'Storage',
      stoneExtractor: 'Stone',
      iceExtractor: 'Ice',
      uraniumMine: 'Uranium',
      fuelFactory: 'Fuel'
    };
    return labels[type] || type;
  }

  _renderStorage(ctx, x, y) {
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

  _renderBuildingMenu(ctx) {
    const menuX = CONFIG.CANVAS_WIDTH / 2 - 200;
    const menuY = this.gridStartY + this.gridSize * this.slotSize + 40;
    const menuWidth = 400;
    const menuHeight = 320;

    ctx.fillStyle = 'rgba(20, 35, 60, 0.95)';
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.8)';
    ctx.lineWidth = 2;
    this._roundedRect(ctx, menuX, menuY, menuWidth, menuHeight, 15);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#d7f9ff';
    ctx.font = '20px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('Select Building', menuX + menuWidth / 2, menuY + 35);

    const buildingTypes = [
      { type: 'storage', label: 'Storage', cost: '3 Stone, 2 Ice' },
      { type: 'stoneExtractor', label: 'Stone Extractor', cost: '2 Stone' },
      { type: 'iceExtractor', label: 'Ice Extractor', cost: '2 Ice' },
      { type: 'uraniumMine', label: 'Uranium Mine', cost: '3 Stone, 2 Ice' },
      { type: 'fuelFactory', label: 'Fuel Factory', cost: '5 Stone, 3 Ice' }
    ];

    buildingTypes.forEach((building, index) => {
      const buttonY = menuY + 60 + index * 50;
      const canBuild = this.game.colonySystem.canBuildBuilding(
        this.currentColony,
        building.type,
        this.selectedSlot.x,
        this.selectedSlot.y
      );

      ctx.fillStyle = canBuild ? 'rgba(76, 201, 240, 0.3)' : 'rgba(100, 100, 100, 0.2)';
      ctx.fillRect(menuX + 10, buttonY, menuWidth - 20, 45);

      ctx.fillStyle = canBuild ? '#e0efff' : '#888888';
      ctx.font = '16px "Segoe UI", Arial, sans-serif';
      ctx.textAlign = 'left';
      ctx.fillText(building.label, menuX + 20, buttonY + 20);
      ctx.font = '12px Arial';
      ctx.fillText(building.cost, menuX + 20, buttonY + 35);
    });
  }

  _renderCollectButton(ctx, x, y) {
    const buttonWidth = 200;
    const buttonHeight = 50;

    ctx.fillStyle = 'rgba(76, 201, 240, 0.4)';
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.8)';
    ctx.lineWidth = 2;
    this._roundedRect(ctx, x, y, buttonWidth, buttonHeight, 10);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#e0efff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('Collect All', x + buttonWidth / 2, y + buttonHeight / 2 + 6);
  }

  _renderTradingPostButton(ctx, x, y) {
    const buttonWidth = 200;
    const buttonHeight = 50;

    ctx.fillStyle = 'rgba(255, 215, 0, 0.4)';
    ctx.strokeStyle = 'rgba(255, 215, 0, 0.8)';
    ctx.lineWidth = 2;
    this._roundedRect(ctx, x, y, buttonWidth, buttonHeight, 10);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#fff8dc';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('Trading Post', x + buttonWidth / 2, y + buttonHeight / 2 + 6);
  }

  _renderShipyardButton(ctx, x, y) {
    const buttonWidth = 200;
    const buttonHeight = 50;

    ctx.fillStyle = 'rgba(76, 201, 240, 0.4)';
    ctx.strokeStyle = 'rgba(76, 201, 240, 0.8)';
    ctx.lineWidth = 2;
    this._roundedRect(ctx, x, y, buttonWidth, buttonHeight, 10);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#d7f9ff';
    ctx.font = '18px "Segoe UI", Arial, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('Shipyard', x + buttonWidth / 2, y + buttonHeight / 2 + 6);
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

