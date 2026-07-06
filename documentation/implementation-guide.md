# Implementation Guide
**Space MMO Browser Game - Phase-by-Phase Development Roadmap**

---

## Executive Summary

Systematic 12-week implementation roadmap for building the Space MMO MVP with Antigravity AI. Each phase builds on previous work with clear success criteria and testing checkpoints.

**Development Approach:**
- Phase-by-phase implementation (6 phases)
- Test after each milestone before proceeding
- Hybrid AI generation (AI skeleton + manual refinement)
- Performance validation at critical checkpoints

**Timeline:** 12 weeks (2 weeks per phase)
**Goal:** Investment-ready MVP with all core systems functional

---

## Development Phases Overview

```
Phase 1: Core Foundation (Week 1-2)
  → Galaxy generation, ship movement, basic rendering

Phase 2: Space Gameplay (Week 3-4)
  → Combat system, NPCs, resource gathering

Phase 3: Colony System (Week 5-6)
  → Colonization, buildings, resource production

Phase 4: Economy & Progression (Week 7-8)
  → Trading, ship upgrades, credit system

Phase 5: Missions & Polish (Week 9-10)
  → Mission system, UI refinement, visual feedback

Phase 6: Testing & Optimization (Week 11-12)
  → Balance tuning, bug fixes, performance optimization
```

---

## Phase 1: Core Foundation
**Duration:** Week 1-2  
**Goal:** Navigable game world with player ship

### Milestone 1.1: Project Setup (Days 1-2)

**Tasks:**
1. Create directory structure (see project-structure.md)
2. Setup index.html with canvas
3. Create config.js with all constants
4. Setup main.js game loop
5. Test: Black canvas renders at 60 FPS

**Deliverables:**
```
space-mmo-game/
├── index.html
├── src/
│   ├── main.js
│   ├── game.js
│   └── config.js
```

**Success Criteria:**
- [ ] Canvas displays at correct size
- [ ] Game loop runs smoothly
- [ ] FPS counter shows ~60 FPS
- [ ] No console errors

**Testing:**
```javascript
// Add FPS counter to verify performance
let fps = 0;
let lastFpsUpdate = Date.now();

function gameLoop() {
  fps++;
  if (Date.now() - lastFpsUpdate > 1000) {
    console.log(`FPS: ${fps}`);
    fps = 0;
    lastFpsUpdate = Date.now();
  }
  requestAnimationFrame(gameLoop);
}
```

---

### Milestone 1.2: Galaxy Generation (Days 3-5)

**Tasks:**
1. Implement GalaxySystem class
2. Generate 6 star systems
3. Generate 3-4 planets per system
4. Render planets as colored circles (placeholders)
5. Test: Galaxy renders with all systems visible

**Antigravity Prompt:**
```
Create src/systems/galaxy.js following technical-architecture.md specifications.

Requirements:
- Generate 6 star systems
- Each system has 3-4 planets in orbital positions
- Planets have types: agrarian, military, mining, industrial, scientific
- Return galaxy object with systems array

Reference mvp-requirements.md section "Galaxy & Universe System" for exact specifications.
```

**Success Criteria:**
- [ ] 6 systems generated
- [ ] 18-24 total planets
- [ ] Planets positioned in orbit patterns
- [ ] Each planet has correct type assignment
- [ ] Galaxy data structure matches specification

---

### Milestone 1.3: Player Ship & Movement (Days 6-8)

**Tasks:**
1. Create Ship entity class
2. Spawn player ship at starting position
3. Implement click-to-move navigation
4. Add camera follow (ship stays centered)
5. Test: Ship moves to clicked location smoothly

**Antigravity Prompt:**
```
Create src/entities/ship.js and implement click-to-move in game.js.

Ship should:
- Have position, velocity, speed properties
- Move toward clicked position
- Stop when reached (within 10px tolerance)
- Update position based on deltaTime

Reference technical-architecture.md section "Ship Entity" for data structure.
```

**Success Criteria:**
- [ ] Ship renders on screen (placeholder triangle acceptable)
- [ ] Clicking moves ship to that location
- [ ] Movement is smooth and frame-independent
- [ ] Ship stops at destination
- [ ] Camera follows ship

---

### Milestone 1.4: System Travel (Days 9-10)

**Tasks:**
1. Create galaxy map overlay UI
2. Implement system selection
3. Add hyperspace transition
4. Test: Can travel between all 6 systems

**Success Criteria:**
- [ ] Galaxy map shows all systems
- [ ] Can click system to select
- [ ] Hyperspace animation plays (fade or simple transition)
- [ ] Ship spawns in new system
- [ ] Previous system properly unloaded

---

### Phase 1 Complete Checklist

- [ ] Player can navigate space with click-to-move
- [ ] Galaxy contains 6 systems with planets
- [ ] Can travel between systems via galaxy map
- [ ] Game runs at 60 FPS
- [ ] No major bugs or crashes

**Performance Test:**
```javascript
// Ensure smooth performance before Phase 2
console.log('Phase 1 Performance Test:');
console.log(`FPS: ${measuredFPS}`); // Should be 55-60
console.log(`Frame time: ${frameTime}ms`); // Should be <16ms
console.log(`Systems rendered: 6`);
console.log(`Planets rendered: ${totalPlanets}`); // Should be 18-24
```

---

## Phase 2: Space Gameplay
**Duration:** Week 3-4  
**Goal:** Combat and resource gathering functional

### Milestone 2.1: Space Objects (Days 11-13)

**Tasks:**
1. Create SpaceObject entity (asteroids, comets)
2. Spawn 10-15 objects per system
3. Implement click-to-destroy
4. Add loot drops (stone from asteroids, ice from comets)
5. Test: Can destroy objects and collect resources

**Success Criteria:**
- [ ] Asteroids and comets spawn in system
- [ ] Clicking object targets it
- [ ] Destroying object drops resources
- [ ] Resources auto-collect or click to collect
- [ ] Ship cargo increases

---

### Milestone 2.2: Combat System (Days 14-16)

**Tasks:**
1. Implement CombatSystem class
2. Add click-to-attack targeting
3. Implement damage calculation
4. Add weapon firing (projectiles or instant hit)
5. Add HP bars and death logic
6. Test: Player can attack and destroy targets

**Antigravity Prompt:**
```
Create src/systems/combat.js following technical-architecture.md.

Combat system should:
- Track attacker→target mappings
- Check weapon range before attacking
- Calculate damage based on weapon stats
- Update projectiles if using projectile weapons
- Handle entity death

Reference mvp-requirements.md "Combat System" section.
```

**Success Criteria:**
- [ ] Can click enemy to attack
- [ ] Weapon fires at correct fire rate
- [ ] Damage applies to target HP
- [ ] HP bars display correctly
- [ ] Enemy dies when HP reaches 0
- [ ] Combat feels responsive

---

### Milestone 2.3: NPC System (Days 17-19)

**Tasks:**
1. Create NPCSystem class
2. Spawn miner NPCs (5 max)
3. Spawn pirate NPCs (3 max)
4. Implement basic AI (miners flee, pirates attack)
5. Test: NPCs behave correctly

**Success Criteria:**
- [ ] Miners spawn and move to asteroids
- [ ] Pirates spawn and attack player on sight
- [ ] Max spawn limits enforced
- [ ] NPCs despawn when changing systems
- [ ] NPC AI feels alive

---

### Milestone 2.4: Loot & Cargo (Days 20)

**Tasks:**
1. Implement cargo system
2. Add cargo capacity limit
3. Display cargo in HUD
4. Test: Cargo fills up, can't collect when full

**Success Criteria:**
- [ ] Cargo UI shows current/max
- [ ] Can't collect resources when cargo full
- [ ] Visual feedback when cargo full

---

### Phase 2 Complete Checklist

- [ ] Can destroy asteroids and comets for resources
- [ ] Combat system works against NPCs
- [ ] Miners and pirates spawn and behave correctly
- [ ] Cargo system functional with capacity limits
- [ ] FPS remains 55-60 with 20 entities on screen

**Critical Performance Test:**
```javascript
// CRITICAL: Validate combat performance
console.log('Phase 2 Performance Test:');
console.log(`FPS with 20 ships: ${measuredFPS}`); // Must be >30
console.log(`Entities active: ${entityCount}`); // Up to 20
console.log(`Projectiles active: ${projectileCount}`);

// If FPS < 30 with 20 ships:
// → Reduce max NPCs to 15
// → Simplify particle effects
// → Consider object pooling
```

---

## Phase 3: Colony System
**Duration:** Week 5-6  
**Goal:** Planet colonization and resource production

### Milestone 3.1: Colonization (Days 21-23)

**Tasks:**
1. Implement planet landing mechanics
2. Add colonization system (pay credits)
3. Create ColonySystem class
4. Test: Can establish colonies on planets

**Success Criteria:**
- [ ] Can click planet to land
- [ ] Colonization costs correct credits
- [ ] Colony UI displays (3x3 grid)
- [ ] Planet marked as colonized

---

### Milestone 3.2: Building System (Days 24-26)

**Tasks:**
1. Create Building entity class
2. Implement 5 building types
3. Add building placement (click empty slot)
4. Add resource cost checking
5. Test: Can build buildings in colony

**Antigravity Prompt:**
```
Create src/entities/building.js and src/systems/colony.js.

Building system should:
- Place buildings in 3x3 grid
- Deduct resource costs
- Store building data (type, position, level)
- Render buildings in colony view

Reference mvp-requirements.md "Colony & Building System".
```

**Success Criteria:**
- [ ] Can click empty slot to build
- [ ] Building menu shows available types
- [ ] Resource costs enforced
- [ ] Buildings render in grid
- [ ] Can't build without resources

---

### Milestone 3.3: Resource Production (Days 27-29)

**Tasks:**
1. Implement ResourceSystem class
2. Add production tick calculation
3. Implement async production (offline progress)
4. Add collection mechanics
5. Test: Resources produce while offline

**Success Criteria:**
- [ ] Buildings produce resources over time
- [ ] Production stops when storage full
- [ ] Can collect produced resources
- [ ] Offline production calculates correctly
- [ ] Resources transfer to ship cargo

**Critical Test:**
```javascript
// Test offline production
1. Build stone extractor
2. Close game
3. Wait 15 minutes
4. Reopen game
5. Verify: ~1 stone produced (rate: 15min per unit)
6. Storage should show "FULL" indicator when maxed
```

---

### Milestone 3.4: Colony Storage (Days 30)

**Tasks:**
1. Implement storage building
2. Add cargo transfer (ship ↔ colony)
3. Test: Can load/unload cargo

**Success Criteria:**
- [ ] Storage building increases capacity
- [ ] Can transfer resources to/from ship
- [ ] UI shows both ship cargo and colony storage

---

### Phase 3 Complete Checklist

- [ ] Can colonize planets (costs credits)
- [ ] Can build 5 building types
- [ ] Resource production works (including offline)
- [ ] Can collect and transfer resources
- [ ] Storage mechanics functional

---

## Phase 4: Economy & Progression
**Duration:** Week 7-8  
**Goal:** Trading and ship upgrades

### Milestone 4.1: Trading System (Days 31-33)

**Tasks:**
1. Implement trading interface
2. Add sell resources functionality
3. Add buy resources functionality
4. Test: Can earn credits by selling

**Success Criteria:**
- [ ] Can sell resources for credits
- [ ] Prices match specifications
- [ ] Credits update correctly
- [ ] Can buy resources if have credits

---

### Milestone 4.2: Shipyard (Days 34-36)

**Tasks:**
1. Create shipyard interface
2. Add ship purchase system
3. Add equipment shop
4. Test: Can buy new ships

**Success Criteria:**
- [ ] Shipyard shows available ships
- [ ] Can purchase Fighter (5,000 credits)
- [ ] Can purchase Destroyer (15,000 credits)
- [ ] Ship switching works
- [ ] Equipment shop functional

---

### Milestone 4.3: Ship Progression (Days 37-39)

**Tasks:**
1. Implement equipment installation
2. Add weapon upgrades
3. Add engine upgrades
4. Test: Upgrades affect stats

**Success Criteria:**
- [ ] Can install improved laser (2,000 credits)
- [ ] Can install improved engine (1,500 credits)
- [ ] Stats increase correctly
- [ ] Upgrades persist after save/load

---

### Milestone 4.4: Death & Respawn (Days 40)

**Tasks:**
1. Implement player death mechanics
2. Add respawn system
3. Test: Death and respawn works

**Success Criteria:**
- [ ] Player ship dies when HP = 0
- [ ] Respawn at home colony
- [ ] Respawn with Scout ship (lose upgrades)
- [ ] Credits and resources preserved

---

### Phase 4 Complete Checklist

- [ ] Can earn credits by selling resources
- [ ] Can buy resources and equipment
- [ ] Can upgrade to Fighter and Destroyer
- [ ] Equipment upgrades affect gameplay
- [ ] Death and respawn functional

---

## Phase 5: Missions & Polish
**Duration:** Week 9-10  
**Goal:** Mission system and UI refinement

### Milestone 5.1: Mission System (Days 41-45)

**Tasks:**
1. Create MissionSystem class
2. Implement mission generator
3. Add 2 mission types (combat, gathering)
4. Add mission tracking
5. Add mission UI panel
6. Test: Can complete missions

**Antigravity Prompt:**
```
Create src/systems/missions.js and src/ui/missionPanel.js.

Mission system should:
- Generate random combat missions ("Destroy 5 pirates")
- Generate random gathering missions ("Collect 20 stone")
- Track objective progress
- Award credits on completion
- Refresh available missions

Reference mvp-requirements.md "Mission System" section.
```

**Success Criteria:**
- [ ] 3-5 missions always available
- [ ] Can accept missions
- [ ] Progress tracked automatically
- [ ] Missions auto-complete when done
- [ ] Rewards given correctly
- [ ] New missions generated

---

### Milestone 5.2: UI Polish (Days 46-49)

**Tasks:**
1. Refine HUD layout
2. Add tooltips
3. Improve visual feedback
4. Add sound effects (optional)
5. Test: UI feels polished

**Success Criteria:**
- [ ] HUD is clear and readable
- [ ] Tooltips show on hover
- [ ] Damage numbers display
- [ ] Resource collection has feedback
- [ ] Combat feels impactful

---

### Milestone 5.3: Save/Load (Days 50)

**Tasks:**
1. Implement SaveSystem class
2. Add auto-save (every 30s)
3. Add load game on startup
4. Calculate offline production
5. Test: Save and load works

**Success Criteria:**
- [ ] Game auto-saves regularly
- [ ] Can close and reopen game
- [ ] All progress restored
- [ ] Offline production calculates
- [ ] No data loss

---

### Phase 5 Complete Checklist

- [ ] Mission system fully functional
- [ ] UI is polished and professional
- [ ] Save/load system works perfectly
- [ ] Offline production validated

---

## Phase 6: Testing & Optimization
**Duration:** Week 11-12  
**Goal:** Balance, bug fixes, performance

### Milestone 6.1: Balance Tuning (Days 51-55)

**Tasks:**
1. Adjust resource production rates
2. Tune combat difficulty
3. Balance credit economy
4. Test progression curve
5. Adjust as needed

**Success Criteria:**
- [ ] Can afford Fighter in 2-3 hours
- [ ] Can afford Destroyer in 5-8 hours
- [ ] Resources produce at good rate
- [ ] Combat feels challenging but fair

---

### Milestone 6.2: Bug Fixes (Days 56-59)

**Tasks:**
1. Fix all critical bugs
2. Fix high-priority bugs
3. Test all features thoroughly
4. Fix edge cases
5. Final QA pass

**Success Criteria:**
- [ ] No game-breaking bugs
- [ ] No progression blockers
- [ ] All core features work
- [ ] Edge cases handled

---

### Milestone 6.3: Performance Optimization (Days 60)

**Tasks:**
1. Profile performance
2. Optimize rendering if needed
3. Reduce memory usage if needed
4. Final performance validation

**Success Criteria:**
- [ ] 60 FPS in normal play
- [ ] 30+ FPS with 20 ships in combat
- [ ] No memory leaks
- [ ] Game runs smoothly

---

### Phase 6 Complete Checklist

- [ ] All balance values tuned
- [ ] No critical or high-priority bugs
- [ ] Performance meets targets
- [ ] MVP is demo-ready

---

## Final MVP Checklist

**Core Gameplay:**
- [ ] Can navigate galaxy (6 systems)
- [ ] Can fly ship with click-to-move
- [ ] Can destroy asteroids/comets
- [ ] Can fight NPCs (miners, pirates)
- [ ] Can collect resources

**Colony System:**
- [ ] Can establish colonies (costs credits)
- [ ] Can build 5 building types
- [ ] Resources produce offline
- [ ] Can collect produced resources
- [ ] Storage system works

**Economy:**
- [ ] Can sell resources for credits
- [ ] Can buy resources
- [ ] Can buy ships (Fighter, Destroyer)
- [ ] Can buy equipment upgrades

**Missions:**
- [ ] Combat missions work
- [ ] Gathering missions work
- [ ] Rewards given correctly
- [ ] Missions refresh

**Technical:**
- [ ] Save/load works perfectly
- [ ] Offline production calculates
- [ ] Performance meets targets (60 FPS)
- [ ] No game-breaking bugs

**Polish:**
- [ ] UI is clean and readable
- [ ] Visual feedback is clear
- [ ] Game feels polished
- [ ] Ready for investor demo

---

**Document Version:** 1.0  
**Last Updated:** November 24, 2025  
**Next:** README-for-antigravity.md
