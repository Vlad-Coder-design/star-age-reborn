# Next Session Handoff Prompt

**Copy this entire prompt to start the next development session:**

---

## 🎯 Your Mission

You are continuing development of a browser-based space MMO game MVP. **Phases 1-3 are complete**, and **Milestones 4.1 (Trading System) and 4.2 (Shipyard) are complete**. You are now starting **Milestone 4.3: Ship Progression**.

## 📚 Context & Status

### What's Been Completed ✅

**Phase 1: Core Foundation** - Complete
- Galaxy generation (6 systems, 18-24 planets)
- Player ship with click-to-move navigation
- Galaxy map UI with hyperspace travel
- All core rendering and game loop

**Phase 2: Space Gameplay** - Complete
- Space objects (asteroids/comets) with loot drops
- Combat system (click-to-attack, projectiles, damage)
- NPC system (miners patrol asteroids, pirates attack player)
- Cargo system with capacity limits

**Phase 3: Colony System** - Complete
- Planet colonization (escalating costs: 2nd=2000, 3rd=5000, etc.)
- Building system (5 types: Storage, Stone/Ice Extractors, Uranium Mine, Fuel Factory)
- Resource production (async/offline calculation)
- Colony view UI (3x3 grid, building placement, storage display)

**Phase 4: Economy & Progression** - In Progress
- **Milestone 4.1: Trading System** - ✅ Complete
  - Trading post UI accessible from colonized planets
  - Sell resources (Stone, Ice, Uranium, Fuel) for credits
  - Prices: Stone: 3¢, Ice: 2¢, Uranium: 5¢, Fuel: 20¢
  - Credits display in HUD
  - Responsive screen scaling (1600x900 canvas)
- **Milestone 4.2: Shipyard** - ✅ Complete
  - Shipyard UI accessible from colonized planets
  - Ship purchase system (Fighter: 5000¢, Destroyer: 15000¢)
  - Equipment shop (Improved Laser: 2000¢, Improved Engine: 1500¢)
  - Ship switching when purchasing new ship
  - Equipment installation on current ship
  - Ship images displayed in shipyard UI
  - All ship sprites loading correctly

### Current Game State

- **Player starts with:** Scout ship, 1000 credits, 1 home colony (5 Stone, 5 Ice in storage)
- **Ship classes:** Scout (starting), Fighter (5000¢), Destroyer (15000¢)
- **Equipment:** Basic Laser (starting), Improved Laser (2000¢), Improved Engine (1500¢)
- **Resource prices:** Stone: 3¢, Ice: 2¢, Uranium: 5¢, Fuel: 20¢

## 🚀 Your Next Task: Milestone 4.3 - Ship Progression

**Goal:** Ensure equipment upgrades work correctly and persist properly

**Tasks:**
1. Verify equipment upgrades affect ship stats correctly
2. Ensure weapon upgrades (Improved Laser) update damage, range, and fire rate
3. Ensure engine upgrades (Improved Engine) update ship speed correctly
4. Test that upgrades persist when switching ships (they should reset to basic)
5. Verify equipment stats display correctly in shipyard
6. Test that upgrades work in combat (weapon damage/range)
7. Test that upgrades work for movement (engine speed multiplier)

**Requirements (from mvp-requirements.md):**
- Equipment upgrades must affect ship performance correctly
- Improved Laser: Damage 15, Range 180, Fire Rate 1.2/s (vs Basic: 10, 150, 1.0)
- Improved Engine: Speed multiplier 1.25x (vs Basic: 1.0x)
- Upgrades should be visible in combat (damage, range)
- Upgrades should be visible in movement (speed)
- When switching ships, new ship starts with basic equipment

**Ship Stats:**
```javascript
Scout: HP: 100, Speed: 200, Cargo: 200 (starting ship)
Fighter: HP: 200, Speed: 150, Cargo: 250 (5,000¢)
Destroyer: HP: 400, Speed: 100, Cargo: 300 (15,000¢)

Weapons:
- Basic Laser: Damage: 10, Range: 150, Fire Rate: 1.0 (starting)
- Improved Laser: Damage: 15, Range: 180, Fire Rate: 1.2 (2,000¢)

Engines:
- Basic Engine: Speed multiplier: 1.0x (starting)
- Improved Engine: Speed multiplier: 1.25x (1,500¢)
```

**Reference:**
- `documentation/mvp-requirements.md` → "Ship & Fleet System" → "Ship Acquisition" and "Equipment System"
- `documentation/implementation-guide.md` → "Phase 4: Economy & Progression" → "Milestone 4.2"

**Integration Points:**
- Equipment system already implemented in `src/ui/shipyard.js`
- Ship entity in `src/entities/ship.js` handles weapon and engine stats
- Combat system in `src/systems/combat.js` uses ship weapon stats
- Ship movement in `src/entities/ship.js` uses engine speed multiplier
- Config in `src/config.js` has all ship/weapon/engine stats defined

**File Structure:**
- `src/ui/shipyard.js` - Already created, verify equipment installation works
- `src/entities/ship.js` - Check that weapon/engine stats are applied correctly
- `src/systems/combat.js` - Verify weapon stats are used in combat
- `src/config.js` - All stats already defined

**Note:** Equipment purchase and installation is already implemented. Focus on:
- Testing that upgrades actually affect gameplay
- Verifying combat uses upgraded weapon stats
- Verifying movement uses upgraded engine speed
- Ensuring visual feedback shows upgrades are active

### After Ship Progression, Continue With:

**Milestone 4.4: Death & Respawn** - Player death mechanics

## 📖 Key Documents to Read

1. **`documentation/PROGRESS.md`** - Complete status of all phases
2. **`documentation/mvp-requirements.md`** - Full specifications (especially Phase 4 sections)
3. **`documentation/implementation-guide.md`** - Phase 4 detailed breakdown
4. **`documentation/technical-architecture.md`** - Code patterns and architecture

## 🎨 UI Reference

- Use `ui-references/` folder for visual guidance
- Follow styling patterns from existing UI (`colonyView.js`, `galaxyMap.js`)
- Match Star Age visual style (blue/cyan color scheme, rounded panels)

## ⚙️ Technical Notes

**Existing Systems Ready:**
- `this.player.credits` - Already initialized at 1000
- `this.player.ship` - Ship entity with weapon and engine properties
- `this.player.ship.weapon` - Current weapon stats (damage, range, fireRate)
- `this.player.ship.engine` - Current engine type ('basicEngine' or 'improvedEngine')
- `this.player.ship.speed` - Base speed (updated with engine multiplier)
- `this.ui.shipyard` - Shipyard interface (equipment purchase already works)
- `this.combatSystem` - Combat system (should use ship.weapon stats)
- `CONFIG.WEAPONS` - Weapon definitions with stats
- `CONFIG.ENGINES` - Engine definitions with speed multipliers

**File Structure:**
- UI components: `src/ui/`
- Systems: `src/systems/`
- Entities: `src/entities/`
- Config: `src/config.js` (add trading prices if needed)

**Testing:**
- Purchase Improved Laser and verify damage increases in combat
- Purchase Improved Engine and verify ship moves faster
- Test that weapon range increases with Improved Laser
- Test that fire rate increases with Improved Laser
- Verify engine speed multiplier is applied correctly
- Test switching ships resets equipment to basic
- Verify equipment stats persist during gameplay
- Test combat with upgraded weapon (damage, range, fire rate)
- Test movement with upgraded engine (speed multiplier)

## ✅ Success Criteria for Milestone 4.3

- [ ] Improved Laser increases damage from 10 to 15
- [ ] Improved Laser increases range from 150 to 180
- [ ] Improved Laser increases fire rate from 1.0 to 1.2/s
- [ ] Improved Engine increases speed by 1.25x multiplier
- [ ] Combat system uses upgraded weapon stats
- [ ] Movement system uses upgraded engine speed
- [ ] Equipment upgrades visible in shipyard UI
- [ ] Switching ships resets equipment to basic
- [ ] Upgrades work correctly in actual gameplay

## 🚨 Important Reminders

- **Follow the spec exactly** - mvp-requirements.md is law
- **Use CONFIG constants** - Don't hardcode values
- **Test after each feature** - Verify before proceeding
- **Match existing UI style** - Consistency is key
- **Ask if unclear** - Better to ask than guess

---

**Ready to start? Begin with Milestone 4.3: Ship Progression testing and verification!**

**Note:** 
- The game canvas has been optimized to 1600x900 for better screen fit
- Shipyard is fully implemented with ship images displaying correctly
- All ship sprites load from `/ assets-provided/ships/` directory (note the leading space)
- Equipment purchase system is functional - focus on verifying upgrades affect gameplay

Good luck! 🚀

