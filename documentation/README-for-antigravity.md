# README for Antigravity AI
**Master Implementation Guide - Space MMO Browser Game MVP**

---

## 🎯 Quick Start

**You are Antigravity AI, tasked with implementing a browser-based space MMO game.**

### 🆕 Continuing Development?

**If you're continuing from a previous session:**
1. **Read `NEXT-SESSION-PROMPT.md` FIRST** - Contains handoff context and current status
2. **Read `PROGRESS.md`** - See what's been completed
3. **Continue with current phase** - Follow the prompt instructions

### 🆕 Starting Fresh?

This README is your master guide. Follow these steps in order:

1. **Read this README completely** (understand the project)
2. **Read project-structure.md** (understand file organization)
3. **Read technical-architecture.md** (understand system designs)
4. **Read mvp-requirements.md** (understand specifications)
5. **Read implementation-guide.md** (understand development phases)
6. **Begin Phase 1** (start coding)

---

## 📋 Project Overview

### What You're Building

A browser-based space MMO strategy game inspired by Star Age, featuring:
- Real-time ship combat in space
- Colony building on planets
- Resource production (offline progression)
- Mission system
- Ship progression (Scout → Fighter → Destroyer)
- NPC-only gameplay (no multiplayer in MVP)

### Technology Stack

- **HTML5 Canvas** (2D rendering)
- **Vanilla JavaScript** (ES6 modules)
- **Local Storage** (persistence)
- **No frameworks** (keep it simple)
- **Fixed 1920x1080 canvas** (CSS scaled)

### Development Approach

- **Phase-by-phase:** Complete Phase 1 before Phase 2
- **Test after each milestone:** Verify it works before proceeding
- **Hybrid AI:** You generate skeleton code, human refines critical logic
- **Placeholder-first:** Use colored shapes, replace with sprites later

---

## 📚 Documentation Structure

### Core Documents (Read in Order)

**1. project-structure.md**
- Complete directory layout
- File naming conventions
- Module organization patterns
- **When to use:** Before creating any files

**2. technical-architecture.md**
- System designs with code examples
- Data structures
- Game loop architecture
- **When to use:** Before implementing each system

**3. mvp-requirements.md**
- Complete feature specifications
- Exact formulas and values
- Success criteria
- **When to use:** Reference for every implementation detail

**4. asset-requirements.md**
- All sprite specifications
- AI generation prompts
- Integration guide
- **When to use:** When ready to add visuals

**5. implementation-guide.md**
- 6-phase development roadmap
- Milestone breakdowns
- Testing checkpoints
- **When to use:** Planning each work session

### Supporting Documents

**From Previous Steps:**
- `problem-statement.md` - Why this game exists
- `target-audience.md` - Who it's for
- `core-assumptions.md` - Design constraints
- `research-findings.md` - Star Age mechanics
- `core-game-loop.md` - Gameplay loops
- `Comprehensive Game System Document` - Deep reference
- `Star Age logic + UI.docx` - Visual reference

---

## 🚀 Getting Started

### Step 1: Understand the Project

**Read these sections first:**
1. mvp-requirements.md → "Executive Summary"
2. core-game-loop.md → "The Four Play Modes"
3. technical-architecture.md → "System Architecture Overview"

**Key Concepts:**
- **Four Play Modes:** Building, Exploring, Trading, Combat
- **Async Progression:** Resources produce offline (like Clash of Clans)
- **NPC-Only:** No multiplayer in MVP
- **Real-Time Combat:** No pause, click-to-attack
- **Manual Cargo Hauling:** No automatic resource transfer

### Step 2: Setup Project

**Create directory structure:**
```bash
mkdir space-mmo-game
cd space-mmo-game
mkdir -p src/{systems,entities,ui,renderer,utils}
mkdir -p assets/{sprites,effects,sounds}
mkdir -p data
mkdir -p docs
```

**Create core files:**
1. `index.html` - Canvas setup
2. `src/config.js` - All constants
3. `src/main.js` - Game loop
4. `src/game.js` - State manager

**Reference:** project-structure.md for complete structure

### Step 3: Begin Phase 1

**Read:** implementation-guide.md → "Phase 1: Core Foundation"

**Your first task:**
- Implement galaxy generation system
- Generate 6 star systems with 18-24 planets
- Render planets as colored circles (placeholders OK)

**Prompt Template:**
```
I'm implementing Phase 1, Milestone 1.2 (Galaxy Generation).

Please create src/systems/galaxy.js following these specifications:
- Reference: technical-architecture.md section "Galaxy Generation System"
- Requirements: mvp-requirements.md section "Galaxy & Universe System"
- Generate 6 systems, each with 3-4 planets
- Return galaxy object matching data structure specification

After generating code, explain what it does and how to test it.
```

---

## 💡 How to Use These Documents

### When Implementing a Feature

**Step-by-step process:**

1. **Find the feature in implementation-guide.md**
   - Example: "Phase 2, Milestone 2.2: Combat System"

2. **Read the technical specification**
   - Go to technical-architecture.md
   - Find "Combat System" section
   - Study the code examples

3. **Check exact requirements**
   - Go to mvp-requirements.md
   - Find "Combat System" section
   - Note all specifications and formulas

4. **Understand the data structures**
   - Still in mvp-requirements.md
   - Find data structure definitions
   - Note all properties and types

5. **Generate the code**
   - Use technical-architecture.md code as template
   - Follow mvp-requirements.md specifications exactly
   - Reference project-structure.md for file location

6. **Test the feature**
   - Use testing criteria from implementation-guide.md
   - Verify all success criteria met
   - Test edge cases

### Example: Implementing Combat System

**1. Read Implementation Guide:**
```
Phase 2, Milestone 2.2: Combat System
Tasks: 
- Implement CombatSystem class
- Add click-to-attack targeting
- Implement damage calculation
- Add weapon firing
```

**2. Read Technical Architecture:**
```javascript
// Found in technical-architecture.md
export class CombatSystem {
  constructor(game) { }
  attackTarget(attacker, target) { }
  update(deltaTime) { }
  // ... code examples
}
```

**3. Read MVP Requirements:**
```
Combat System must have:
- Click-to-attack: Range check 150px
- Damage: 10 per hit (basic laser)
- Fire rate: 1.0 shots per second
- Projectile speed: 800 pixels/second
```

**4. Generate Code:**
```
Create src/systems/combat.js implementing:
- CombatSystem class from technical-architecture.md
- Range checking: 150px for basic laser
- Damage calculation: 10 damage per hit
- Fire rate: 1.0 shots/second
- Projectile movement: 800px/s

Reference technical-architecture.md section "Combat System" for structure.
Use mvp-requirements.md section "Combat System" for exact values.
```

**5. Test:**
```
Success Criteria (from implementation-guide.md):
- ✓ Can click enemy to attack
- ✓ Weapon fires at correct fire rate
- ✓ Damage applies to target HP
- ✓ HP bars display correctly
- ✓ Enemy dies when HP reaches 0
```

---

## ⚙️ Configuration & Constants

### Critical Constants (src/config.js)

**Never hardcode these values** - always use CONFIG object:

```javascript
export const CONFIG = {
  // Canvas
  CANVAS_WIDTH: 1920,
  CANVAS_HEIGHT: 1080,
  TARGET_FPS: 60,
  
  // Galaxy
  SYSTEM_COUNT: 6,
  PLANETS_PER_SYSTEM_MIN: 3,
  PLANETS_PER_SYSTEM_MAX: 4,
  
  // Ships (reference mvp-requirements.md for full specs)
  SCOUT_HP: 100,
  SCOUT_SPEED: 200,
  SCOUT_CARGO: 200,
  
  // Combat
  BASIC_LASER_DAMAGE: 10,
  BASIC_LASER_RANGE: 150,
  BASIC_LASER_FIRE_RATE: 1.0,
  
  // NPCs
  NPC_MINER_SPAWN_INTERVAL: 15000, // 15 seconds
  NPC_PIRATE_SPAWN_INTERVAL: 60000, // 60 seconds
  NPC_MAX_MINERS: 5,
  NPC_MAX_PIRATES: 3,
  
  // Resources
  STONE_PRODUCTION_RATE: 900000, // 15 minutes
  ICE_PRODUCTION_RATE: 300000, // 5 minutes
  URANIUM_PRODUCTION_RATE: 1200000, // 20 minutes
  
  // Economy
  STONE_PRICE: 3,
  ICE_PRICE: 2,
  URANIUM_PRICE: 5,
  FUEL_PRICE: 20,
  
  // Colony
  COLONY_COSTS: [0, 2000, 5000, 10000, 20000, 40000],
  
  // Debug
  DEBUG_MODE: true,
  SHOW_FPS: true
};
```

**Why this matters:**
- Easy to tune balance
- No magic numbers in code
- Centralized reference for all values

---

## 🎨 Asset Guidelines

### Placeholder vs. Sprites

**Phase 1-2: Use Placeholders**
```javascript
// Render ship as colored triangle
ctx.fillStyle = '#00ff00'; // Green for player
ctx.beginPath();
ctx.moveTo(x, y - 20);
ctx.lineTo(x - 15, y + 15);
ctx.lineTo(x + 15, y + 15);
ctx.closePath();
ctx.fill();
```

**Phase 3+: Add Sprites Gradually**
```javascript
// Check if sprite loaded, fallback to placeholder
const sprite = assets.get('scout-ship');
if (sprite) {
  ctx.drawImage(sprite, x - 32, y - 32, 64, 64);
} else {
  // Fallback to placeholder
  ctx.fillStyle = '#00ff00';
  ctx.fillRect(x - 32, y - 32, 64, 64);
}
```

**Reference:** asset-requirements.md for all sprite specifications

---

## 🧪 Testing Guidelines

### After Each Milestone

**Run these checks:**

1. **Visual Test**
   - Does it look right?
   - Any graphical glitches?

2. **Functional Test**
   - Does it work as specified?
   - Try edge cases

3. **Performance Test**
   - Check FPS (should be 55-60)
   - Test with max entities

4. **Save/Load Test**
   - Save game
   - Reload page
   - Verify state restored

### Critical Performance Checkpoints

**After Phase 1:**
```javascript
console.log('FPS:', measuredFPS); // Should be 60
console.log('Planets:', planetCount); // Should be 18-24
```

**After Phase 2:**
```javascript
console.log('FPS with 20 entities:', measuredFPS); // Must be >30
console.log('Projectiles:', projectileCount);
```

**If FPS < 30:**
- Reduce max NPCs
- Simplify particle effects
- Enable object pooling
- Consider canvas optimizations

---

## 🚨 Common Pitfalls to Avoid

### 1. Timestamps vs. Strings

**❌ Wrong:**
```javascript
building.lastCollected = "2024-11-24T10:30:00";
```

**✅ Correct:**
```javascript
building.lastCollected = Date.now(); // Unix timestamp in milliseconds
```

### 2. Offline Production Calculation

**❌ Wrong:**
```javascript
// Don't count production if storage full
if (storage < max) {
  production += timePassed * rate;
}
```

**✅ Correct:**
```javascript
// Calculate production, cap at storage max
const produced = (timePassed / productionRate) * storageMax;
storage = Math.min(storage + produced, storageMax);

// Only update timestamp if not full
if (storage < storageMax) {
  lastCollected = now;
}
```

### 3. NPC Spawning

**❌ Wrong:**
```javascript
// Spawning NPCs globally across all systems
spawnNPC();
```

**✅ Correct:**
```javascript
// Only spawn NPCs in player's current system
if (npc.system === player.currentSystem) {
  spawnNPC(player.currentSystem);
}

// Despawn when player leaves system
if (player.changedSystem) {
  despawnAllNPCs();
}
```

### 4. Building Placement

**❌ Wrong:**
```javascript
// Allowing any building on any planet
colony.buildings.push(new Building('uraniumMine'));
```

**✅ Correct:**
```javascript
// Check planet type before allowing build
const planet = getPlanet(colony.planetId);
if (planet.type === 'military') {
  colony.buildings.push(new Building('uraniumMine'));
} else {
  showError('Uranium Mine requires Military planet');
}
```

### 5. Resource Transfer

**❌ Wrong:**
```javascript
// Directly modifying resources
player.stone += colony.stone;
colony.stone = 0;
```

**✅ Correct:**
```javascript
// Check cargo capacity before transfer
const space = player.cargo.capacity - player.cargo.used;
const amount = Math.min(colony.storage.stone, space);

if (amount > 0) {
  player.cargo.stone += amount;
  colony.storage.stone -= amount;
  player.cargo.used += amount;
}
```

---

## 📖 Quick Reference

### Data Structure Lookup

**Player State:**
- Location: game.js → `player` object
- Specification: mvp-requirements.md → "Save/Load System"

**Ship Entity:**
- Location: entities/ship.js
- Specification: mvp-requirements.md → "Ship & Fleet System"

**Colony State:**
- Location: systems/colony.js
- Specification: mvp-requirements.md → "Colony & Building System"

**Building Entity:**
- Location: entities/building.js
- Specification: mvp-requirements.md → "Colony & Building System"

### Formula Lookup

**Resource Production:**
```javascript
// From mvp-requirements.md
const timeSinceLastCollect = now - building.lastCollected;
const produced = (timeSinceLastCollect / building.productionRate) * building.storageMax;
```

**Combat Damage:**
```javascript
// From mvp-requirements.md
const damage = weapon.damage; // Simple: just use weapon damage
target.hp -= damage;
```

**NPC Spawn Rate:**
```javascript
// From mvp-requirements.md
const minerInterval = 15000; // 15 seconds
const pirateInterval = 60000; // 60 seconds
```

---

## ✅ Phase Completion Checklist

### Before Moving to Next Phase

**Verify:**
- [ ] All milestone tasks complete
- [ ] All success criteria met
- [ ] Tested manually (no obvious bugs)
- [ ] Performance acceptable (FPS >55)
- [ ] Code follows project-structure.md patterns
- [ ] No hardcoded values (use CONFIG)
- [ ] Save/load tested if applicable

**Ask Human:**
- "Phase X is complete. All milestones achieved and tested. Ready to proceed to Phase X+1?"

**Wait for confirmation before starting next phase.**

---

## 🎓 Learning from Examples

### Good Code Example

```javascript
// From technical-architecture.md
export class CombatSystem {
  constructor(game) {
    this.game = game;
    this.targets = new Map();
    this.projectiles = [];
  }
  
  attackTarget(attacker, target) {
    const distance = calculateDistance(
      attacker.position,
      target.position
    );
    
    if (distance > attacker.weapon.range) {
      return false;
    }
    
    this.targets.set(attacker, target);
    return true;
  }
  
  update(deltaTime) {
    for (const [attacker, target] of this.targets) {
      if (!attacker.isDead && !target.isDead) {
        this.processAttack(attacker, target, deltaTime);
      }
    }
  }
}
```

**Why this is good:**
- Clear responsibility (combat logic only)
- No magic numbers (uses attacker.weapon.range)
- Handles edge cases (checks if entities dead)
- Frame-independent (uses deltaTime)
- Clean API (attackTarget returns boolean)

### Follow These Patterns

1. **Systems are stateless** - store references, not entities
2. **Entities are data** - minimal logic, mostly properties
3. **Use CONFIG** - never hardcode numbers
4. **Check edge cases** - null checks, boundary conditions
5. **Frame-independent** - use deltaTime for all movement
6. **Early returns** - fail fast, clear logic flow

---

## 🎯 Success Metrics

### MVP is Complete When:

**Functional:**
- [ ] All 6 phases complete
- [ ] All systems work as specified
- [ ] Can play full game loop (30+ minutes)
- [ ] No game-breaking bugs

**Technical:**
- [ ] 60 FPS in normal play
- [ ] 30+ FPS with 20 ships in combat
- [ ] Save/load works perfectly
- [ ] Offline production calculates correctly

**Polish:**
- [ ] UI is readable and clear
- [ ] Visual feedback is present
- [ ] Game feels professional
- [ ] Ready for investor demo

---

## 📞 When You Need Help

### Ask Human When:

1. **Specifications unclear** - "The MVP requirements say X, but should I also consider Y?"
2. **Performance issues** - "FPS is 20 with 15 ships, should I reduce max NPCs?"
3. **Design decisions** - "Should storage building be required before extractors?"
4. **Trade-offs** - "Simple approach A vs. complex approach B - which do you prefer?"

### Don't Ask Human:

1. **How to code basic features** - refer to technical-architecture.md
2. **What values to use** - refer to mvp-requirements.md
3. **What to build next** - refer to implementation-guide.md
4. **How to structure files** - refer to project-structure.md

---

## 🎬 Final Reminders

**Core Principles:**
1. **Follow the spec** - mvp-requirements.md is law
2. **Test after each milestone** - don't skip ahead
3. **Performance matters** - 60 FPS is non-negotiable
4. **Async production is critical** - test offline progress thoroughly
5. **Code for readability** - humans will maintain this later

**You've got this!** All the information you need is in these documents. Work methodically through the phases, test thoroughly, and you'll deliver an excellent MVP.

Good luck! 🚀

---

**Document Version:** 1.0  
**Last Updated:** November 24, 2025  
**Project:** Space MMO Browser Game MVP  
**Status:** Master Guide - Start Here
