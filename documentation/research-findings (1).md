# Research Findings
## Space MMO Browser Game Project - Game Mechanics Research

---

## Executive Summary

This document provides comprehensive analysis of Star Age's core mechanics, reverse-engineered formulas, and strategic comparisons with FTL (ship equipment/weapons) and Clash of Clans (building progression). All research focuses on documenting proven mechanics for MVP implementation while flagging areas requiring design decisions.

**Key Findings:**

**Star Age Core Strengths:**
- Real-time combat with weapon range/energy tradeoffs creates tactical depth
- 20-level building progression provides long-term colony optimization goals
- Active mining + cargo management creates meaningful resource gathering loop
- Multi-system galaxy with fuel economy forces strategic travel decisions
- Module-based ship customization enables diverse player builds

**Critical Design Decisions Needed:**
- Exact damage formulas (we have mechanics, need specific numbers)
- Building upgrade cost curves (balance progression speed)
- NPC spawn rates and difficulty scaling
- Resource production rates relative to upgrade costs
- Combat difficulty curve across star systems

**FTL Mechanics to Consider:**
- Weapon variety with distinct tactical uses (beam weapons, missiles, ion damage)
- Subsystem targeting (engines, weapons, shields)
- Equipment synergies that reward strategic builds

**Clash of Clans Mechanics to Consider:**
- Multi-resource upgrade costs create interesting strategic choices
- Building upgrade time scales with level (creates pacing)
- Resource production buildings vs. storage buildings balance

**Implementation Readiness:**
- ✅ Combat mechanics fully documented for AI implementation
- ✅ Ship module system ready for coding
- ✅ Colony building mechanics specified
- ⚠️ Need design decisions on exact formulas and balancing
- ⚠️ NPC behaviors need expansion beyond basic chase AI

---

## How to Use This Document

**For Design Decisions (Steps 5-7):**
- Each system section includes "Design Decision Flags" 
- Review these when making game systems design choices
- Cross-reference with core-assumptions.md for validation priorities

**For AI-Assisted Development (Steps 14-15):**
- "Implementation Notes" sections provide technical specs for Antigravity AI
- Formulas include variables that can be tuned during development
- System interaction diagrams show how mechanics connect

**For Future Reference:**
- Competitive/multiplayer mechanics documented for post-MVP architecture planning
- Comparison sections flag mechanics that might enhance Star Age foundation

---

## Document Structure

### Part 1: Star Age Core Systems
1. Combat Systems
2. Ship Systems & Customization  
3. Colony Building Systems
4. Resource Economy
5. Space Exploration & Travel
6. Mission & Progression Systems

### Part 2: Comparative Analysis
7. FTL Equipment & Weapon Variety
8. Clash of Clans Building Progression

### Part 3: Synthesis & Recommendations
9. Mechanics to Adopt for MVP
10. Post-MVP Expansion Opportunities
11. Design Decision Summary

---

# PART 1: STAR AGE CORE SYSTEMS

---

## 1. Combat Systems

### 1.1 Core Combat Mechanics (Star Age)

**Real-Time Combat Model:**
- 8-directional ship movement with smooth rotation toward cursor
- Continuous weapon fire while holding mouse button
- Ship can move and fire simultaneously
- Friction-based movement (ships drift after stopping)
- No grid-based positioning - free movement in 2D space

**Weapon Characteristics Observed:**
- **Laser weapons**: Forward-firing, continuous beam or rapid projectiles
- **Range**: Short to medium (estimated 200-400 pixels)
- **Projectile speed**: Very fast (appears near hit-scan)
- **Firing rate**: Continuous stream when holding fire button
- **Visual**: Neon laser effects, simple but clear

**Tactical Elements:**
- Weapon range creates positioning gameplay (stay at optimal range)
- Enemy rotation is instant but ships must face target to fire
- Player vulnerability while mining (can't fire mining laser + weapons simultaneously)
- HP bars visible above all ships (player + enemies)

### 1.2 Reverse-Engineered Combat Formulas

**Damage Calculation (Estimated):**
```
Base Damage per Shot = Weapon_Base_Damage × (1 + Module_Bonuses)
Damage Reduction = Target_Armor × Armor_Efficiency_Factor
Final Damage = Base Damage - Damage Reduction

Example with variables:
- Laser Mk I: 10 base damage per shot
- Fire rate: 2 shots/second = 20 DPS
- Armor Mk I reduces: 2 damage per hit
- Net damage: 8 per hit = 16 DPS
```

**Range Effectiveness:**
```
Effective Damage = Base Damage × Range_Multiplier

Where Range_Multiplier:
- Inside optimal range (0-150 pixels): 1.0x damage
- Medium range (150-300 pixels): 0.8x damage  
- Long range (300-400 pixels): 0.5x damage
- Beyond max range: 0x (no hit)
```

**Energy Consumption:**
```
Energy_Per_Shot = Weapon_Base_Energy_Cost
Energy_Regeneration = Ship_Reactor_Output per second

Sustained fire time = Current_Energy / (Energy_Per_Shot × Fire_Rate - Energy_Regen)

Example:
- Laser uses 5 energy per shot
- Fire rate 2/sec = 10 energy/sec consumption
- Reactor generates 8 energy/sec
- Net consumption: 2 energy/sec
- With 100 energy: can fire for 50 seconds before depleted
```

### 1.3 Enemy AI Behaviors (Observed)

**Pirate AI (Basic):**
1. Detect player within aggro range (estimated 500-600 pixels)
2. Chase player at max speed
3. Maintain firing direction while rotating ship
4. Fire continuously when in weapon range
5. Retreat when HP < 20% (sometimes)
6. Drop loot on death

**Miner AI (Neutral):**
1. Move to asteroid field
2. Stop at asteroid and mine (stationary)
3. Return to planet/station when cargo full
4. Flee if attacked (low HP threshold)
5. No combat capabilities

**Trader AI (Neutral):**
1. Follow preset route between planets
2. Warp to different systems
3. Flee if attacked
4. No combat capabilities

### 1.4 Weapon Types (From Star Age)

**Confirmed Weapon Types:**

1. **Laser Cannons**
   - Fast projectiles, high fire rate
   - Medium range (250-300 pixels estimated)
   - Low energy cost per shot
   - Best for sustained DPS against unarmored targets

2. **Railguns**
   - Slow projectiles, high damage per shot
   - Long range (350-400 pixels estimated)
   - High energy cost
   - Best for armored targets, requires aiming skill

3. **Plasma Guns**
   - Area effect on impact (small AoE)
   - Medium range
   - High energy cost
   - Best for groups of enemies

4. **Rockets/Missiles**
   - Slow projectiles, very high damage
   - Medium range
   - Ammunition-based (not energy)
   - Best for high-value targets (bosses)

5. **Mining Lasers**
   - Non-combat utility weapon
   - Extracts resources from asteroids
   - Different from combat lasers (separate slot)

**Module Tiers Observed:**
- Mk I (basic)
- Mk II (improved)
- Mk III (advanced)

Each tier increases: damage, range, fire rate, or reduces energy cost

### 1.5 Combat Progression & Difficulty

**Star System Difficulty Scaling:**
- **Green systems**: Easy enemies, low HP (50-100), basic weapons
- **Yellow systems**: Medium enemies, medium HP (150-300), Mk II weapons
- **Red systems**: Hard enemies, high HP (400-800), Mk III weapons + shields

**Player Power Progression:**
```
Ship Combat Power = 
  (Weapon_DPS × Number_of_Weapon_Slots) + 
  (Shield_Absorption_Rate) + 
  (Armor_Damage_Reduction) +
  (Engine_Speed_for_Dodging)
```

**Enemy Scaling Model (Estimated):**
```
Enemy HP = Base_HP × System_Difficulty_Multiplier × Enemy_Type_Multiplier

Where:
- Scout enemy: 1.0x multiplier (50-100 HP in green systems)
- Fighter enemy: 2.0x multiplier (100-200 HP in green systems)
- Destroyer enemy: 4.0x multiplier (200-400 HP in green systems)

System multipliers:
- Green: 1.0x
- Yellow: 3.0x  
- Red: 6.0x
```

### 1.6 Design Decision Flags - Combat

**⚠️ DECISION NEEDED: Exact Damage Numbers**
- What is the base damage for each weapon type?
- How much does each module tier increase stats?
- What are armor reduction values?
- **Recommendation**: Start with rough estimates, balance through playtesting

**⚠️ DECISION NEEDED: Combat Difficulty Curve**
- How quickly should enemies scale in difficulty?
- Should there be difficulty spikes to encourage ship upgrades?
- How to balance "easy farming" systems vs. "challenge" systems?
- **Recommendation**: Test with 3 difficulty tiers, adjust based on feedback

**⚠️ DECISION NEEDED: Weapon Balance**
- Should all weapons be equally viable or have situational uses?
- How to prevent "one optimal build" problem?
- **Recommendation**: Each weapon type best for different enemy types (lasers vs. speed, railguns vs. armor, rockets vs. bosses)

**⚠️ DECISION NEEDED: Death Mechanics**
- What happens when player ship is destroyed?
  - Respawn at last planet visited?
  - Lose cargo but keep ship?
  - Repair cost in credits?
- **Recommendation**: Low penalty for MVP (respawn at colony, small repair cost) - aligns with "relaxing game" vision

### 1.7 Implementation Notes - Combat

**For Antigravity AI - Combat System Specs:**

```javascript
// Ship combat state
const shipCombatState = {
  position: { x, y },
  rotation: angle, // degrees, 0 = right
  velocity: { x, y },
  hp: current / max,
  energy: current / max,
  weaponSlots: [
    {
      weaponType: 'laser_mk1',
      damage: 10,
      fireRate: 2, // shots per second
      range: 300,
      energyCost: 5,
      projectileSpeed: 800 // pixels per second
    }
  ],
  isFiring: boolean
}

// Combat calculations
function calculateDamage(weapon, target, distance) {
  let baseDamage = weapon.damage;
  
  // Range falloff
  if (distance > weapon.range * 0.75) {
    baseDamage *= 0.5;
  }
  
  // Armor reduction
  const damageAfterArmor = Math.max(1, baseDamage - target.armor);
  
  return damageAfterArmor;
}

// Enemy AI behavior tree
const pirateAI = {
  state: 'idle', // idle, chase, attack, retreat
  aggroRange: 500,
  attackRange: weapon.range * 0.8, // stay just inside range
  retreatThreshold: 0.2, // retreat at 20% HP
  
  update(deltaTime) {
    const distanceToPlayer = calculateDistance(this, player);
    
    if (this.hp / this.maxHp < this.retreatThreshold) {
      this.state = 'retreat';
      this.moveAwayFrom(player);
    } else if (distanceToPlayer < this.attackRange) {
      this.state = 'attack';
      this.aimAt(player);
      this.fire();
    } else if (distanceToPlayer < this.aggroRange) {
      this.state = 'chase';
      this.moveToward(player);
    }
  }
}
```

**Rendering Requirements:**
- 2D sprite-based ships (top-down view)
- Rotation animation (or rotate sprite dynamically)
- Projectile rendering (lines or small sprites)
- HP bars above ships (green/yellow/red gradient)
- Explosion effect on death
- Damage numbers floating up on hit (optional)

**Performance Considerations:**
- Assumption A2.10: Must handle 20 ships in combat at 30+ FPS
- Use object pooling for projectiles (reuse objects instead of creating/destroying)
- Limit particle effects if performance issues
- Simple collision detection (circle-circle for ships, ray-circle for projectiles)

---

## 2. Ship Systems & Customization

### 2.1 Ship Classes (Star Age)

**Seven Ship Classes Confirmed:**

1. **Scout**
   - Smallest ship class
   - High speed, low HP (50-100)
   - Small cargo hold (50-100 units)
   - 1-2 weapon slots
   - Low fuel consumption
   - **Role**: Early exploration, fast travel, reconnaissance

2. **Fighter**
   - Small combat ship
   - Medium speed, medium HP (150-200)
   - Small cargo (100-150 units)
   - 2-3 weapon slots
   - Medium fuel consumption
   - **Role**: Early-mid game combat, pirate hunting

3. **Frigate**
   - Medium combat ship
   - Medium speed, high HP (300-400)
   - Medium cargo (200-300 units)
   - 3-4 weapon slots
   - Medium fuel consumption
   - **Role**: Mid-game balanced combat

4. **Destroyer**
   - Heavy combat ship
   - Low speed, very high HP (500-700)
   - Medium cargo (250-350 units)
   - 4-5 weapon slots
   - High fuel consumption
   - **Role**: Late-game heavy combat, boss fights

5. **Cruiser**
   - Tank ship class
   - Very low speed, massive HP (800-1000)
   - Large cargo (400-500 units)
   - 3-4 weapon slots but with shield slots
   - Very high fuel consumption
   - **Role**: Tank role, survive heavy damage

6. **Carrier**
   - Support ship class
   - Low speed, high HP (600-800)
   - Massive cargo (600-800 units)
   - 2-3 weapon slots + drone bay
   - Very high fuel consumption
   - **Role**: Drone deployment, support, cargo hauling

7. **Miner**
   - Non-combat utility ship
   - Low speed, low-medium HP (200-300)
   - Massive cargo (800-1200 units)
   - 0-1 weapon slots, multiple mining laser slots
   - Medium fuel consumption
   - **Role**: Resource gathering, asteroid mining

### 2.2 Ship Stat System

**Core Ship Stats:**

```
Ship Statistics Model:

HP (Hit Points): 
  - Base HP per ship class
  - Modified by armor modules
  - Regenerates slowly when out of combat (if repair module equipped)

Speed:
  - Base speed per ship class (pixels per second)
  - Modified by engine modules
  - Affected by cargo load (full cargo = slower)

Cargo Capacity:
  - Base capacity per ship class
  - Modified by cargo expansion modules
  - Tracks current load / max capacity

Fuel Tank:
  - Base tank size per ship class
  - Modified by fuel tank modules
  - Consumed by movement and hyperspace jumps

Energy Capacity:
  - Powers weapons and shields
  - Regenerates from reactor
  - Base capacity + reactor modules

Damage Output:
  - Sum of all equipped weapons' DPS
  - Modified by weapon damage modules

Mining Rate:
  - Resources gathered per second
  - Only relevant for ships with mining lasers
  - Modified by mining modules
```

**Ship Progression:**
```
Ship Unlock Progression (estimated):

Level 1-5: Scout only
Level 6-10: Scout + Fighter
Level 11-15: Scout + Fighter + Miner
Level 16-20: All ships up to Frigate
Level 21-30: All ships up to Destroyer
Level 31-40: All ships including Cruiser + Carrier

Unlock requirements:
- Player level (experience from missions/combat)
- Credits cost (purchase ship)
- Shipyard building level (enables construction)
```

### 2.3 Module System (Star Age)

**Module Slot Types:**

1. **Weapon Slots** (varies by ship class)
   - Laser Cannons Mk I/II/III
   - Railguns Mk I/II/III
   - Plasma Guns Mk I/II/III
   - Rocket Launchers Mk I/II/III

2. **Engine Slot** (1 per ship)
   - Standard Engine Mk I/II/III
   - Efficiency Engine (less fuel consumption)
   - Speed Engine (higher max speed)

3. **Shield Slot** (0-2 depending on ship)
   - Energy Shield Mk I/II/III
   - Absorbs damage before HP is affected
   - Regenerates when not taking damage

4. **Armor Slot** (1 per ship)
   - Light Armor Mk I/II/III
   - Heavy Armor Mk I/II/III
   - Reduces damage taken per hit

5. **Reactor Slot** (1 per ship)
   - Power Reactor Mk I/II/III
   - Increases energy capacity and regeneration

6. **Cargo Slot** (1-2 per ship)
   - Cargo Expansion Mk I/II/III
   - Increases cargo capacity

7. **Utility Slots** (0-2 depending on ship)
   - Scanner (reveals asteroid composition from distance)
   - Radar (increases minimap range)
   - Auto-Repair (slow HP regeneration)
   - Fuel Efficiency (reduces fuel consumption)
   - Afterburner (temporary speed boost)

8. **Mining Laser Slot** (Miner ships only)
   - Mining Laser Mk I/II/III
   - Increases resource extraction rate

**Module Tier Progression:**

```
Module Tier Stats (Example: Laser Cannon):

Mk I:
- Damage: 10 per shot
- Fire Rate: 2 shots/sec
- Range: 250 pixels
- Energy Cost: 5 per shot

Mk II:
- Damage: 16 per shot (+60%)
- Fire Rate: 2.5 shots/sec (+25%)
- Range: 300 pixels (+20%)
- Energy Cost: 7 per shot

Mk III:
- Damage: 25 per shot (+150% from Mk I)
- Fire Rate: 3 shots/sec (+50%)
- Range: 350 pixels (+40%)
- Energy Cost: 10 per shot

Cost scaling:
- Mk I: 500 credits
- Mk II: 2,000 credits (4x)
- Mk III: 8,000 credits (16x)
```

### 2.4 Ship Customization Strategies

**Build Archetypes Observed:**

**DPS Build (Fighter/Destroyer):**
- Max weapon slots filled with Mk III weapons
- Speed engine for kiting
- Light armor for mobility
- Power reactor for sustained fire
- **Use case**: Pirate hunting, fast combat

**Tank Build (Cruiser/Destroyer):**
- Heavy armor Mk III
- Shield Mk III
- Auto-repair module
- 2-3 weapons (less important)
- **Use case**: Boss fights, surviving heavy fire

**Trader Build (Miner/Carrier):**
- Max cargo expansion modules
- Fuel efficiency module
- Scanner for finding resources
- 1 weapon for self-defense (optional)
- **Use case**: Resource gathering, trading between systems

**Explorer Build (Scout/Fighter):**
- Speed engine Mk III
- Fuel efficiency module
- Scanner + Radar
- 1-2 weapons for pirates
- **Use case**: Discovering new systems, mapping galaxy

### 2.5 Design Decision Flags - Ship Systems

**⚠️ DECISION NEEDED: Ship Class Count for MVP**
- Assumption A6.3: MVP includes 4-5 ship classes
- **Recommendation**: Scout, Fighter, Destroyer, Miner + possibly Frigate
- Defer Cruiser + Carrier to post-MVP (complex mechanics)

**⚠️ DECISION NEEDED: Module Tier Count**
- Star Age had Mk I/II/III (3 tiers)
- Is 3 tiers enough progression depth for MVP?
- **Recommendation**: Start with Mk I/II for MVP, add Mk III if development time allows

**⚠️ DECISION NEEDED: Ship Purchase vs. Unlock**
- Do players buy ships with credits?
- Or unlock through progression (level/missions)?
- Or both (unlock prerequisite + purchase cost)?
- **Recommendation**: Both - unlock via Shipyard building level, then purchase with credits

**⚠️ DECISION NEEDED: Module Swapping Mechanics**
- Can players swap modules freely?
- Only at planets/stations?
- Cost to swap modules?
- **Recommendation**: Free swapping at colonies (encourages experimentation), matches "relaxing game" vision

### 2.6 Implementation Notes - Ship Systems

**For Antigravity AI - Ship System Specs:**

```javascript
// Ship class definition
const shipClasses = {
  scout: {
    name: 'Scout',
    baseStats: {
      hp: 75,
      speed: 200, // pixels per second
      cargoCapacity: 75,
      fuelTank: 100,
      energyCapacity: 100
    },
    moduleSlots: {
      weapons: 2,
      engine: 1,
      shield: 0,
      armor: 1,
      reactor: 1,
      cargo: 1,
      utility: 1
    },
    unlockRequirements: {
      level: 1,
      credits: 0 // starting ship
    }
  },
  
  fighter: {
    name: 'Fighter',
    baseStats: {
      hp: 175,
      speed: 150,
      cargoCapacity: 125,
      fuelTank: 150,
      energyCapacity: 150
    },
    moduleSlots: {
      weapons: 3,
      engine: 1,
      shield: 1,
      armor: 1,
      reactor: 1,
      cargo: 1,
      utility: 1
    },
    unlockRequirements: {
      level: 5,
      shipyardLevel: 2,
      credits: 5000
    }
  }
  
  // ... other ship classes
}

// Module definition
const modules = {
  laser_mk1: {
    type: 'weapon',
    tier: 1,
    stats: {
      damage: 10,
      fireRate: 2,
      range: 250,
      energyCost: 5,
      projectileSpeed: 800
    },
    cost: 500
  },
  
  engine_speed_mk1: {
    type: 'engine',
    tier: 1,
    stats: {
      speedBonus: 50, // +50 pixels per second
      fuelEfficiency: 1.0 // no change
    },
    cost: 1000
  }
  
  // ... other modules
}

// Ship customization system
class Ship {
  constructor(shipClass) {
    this.class = shipClass;
    this.baseStats = { ...shipClasses[shipClass].baseStats };
    this.equippedModules = {};
    this.calculateEffectiveStats();
  }
  
  equipModule(slot, module) {
    if (this.canEquip(slot, module)) {
      this.equippedModules[slot] = module;
      this.calculateEffectiveStats();
      return true;
    }
    return false;
  }
  
  calculateEffectiveStats() {
    // Start with base stats
    this.stats = { ...this.baseStats };
    
    // Apply module bonuses
    Object.values(this.equippedModules).forEach(module => {
      if (module.stats.speedBonus) {
        this.stats.speed += module.stats.speedBonus;
      }
      if (module.stats.hpBonus) {
        this.stats.hp += module.stats.hpBonus;
      }
      // ... other stat modifications
    });
    
    // Calculate total DPS
    this.stats.dps = this.calculateTotalDPS();
  }
  
  calculateTotalDPS() {
    return Object.values(this.equippedModules)
      .filter(m => m.type === 'weapon')
      .reduce((total, weapon) => {
        return total + (weapon.stats.damage * weapon.stats.fireRate);
      }, 0);
  }
}
```

**UI Requirements for Ship Customization:**
- Ship selection screen (carousel or grid of owned ships)
- Ship stats display (HP, Speed, Cargo, DPS, etc.)
- Module slot grid (visual representation of available slots)
- Module inventory (all owned modules)
- Drag-and-drop or click-to-equip interface
- Comparison view (before/after stats when hovering over module)
- Visual ship preview (show equipped modules visually if possible)

---

## 3. Colony Building Systems

### 3.1 Building Types (Star Age)

**9 Core Building Types Observed:**

1. **ШТАБ (Headquarters / Command Center)**
   - Central building, likely required for colony
   - Unlocks other building types as it levels up
   - May provide colony-wide bonuses
   - **Levels**: 1-20
   - **Primary function**: Colony management, unlocks

2. **БУР (Drill / Mine)**
   - Extracts metal from planet
   - Production rate scales with level
   - **Levels**: 1-20
   - **Produces**: Metal per hour
   - **Planet type bonus**: Rocky planets produce more

3. **ЭЛ. СТАНЦИЯ (Power Station)**
   - Generates energy for colony
   - Powers other buildings
   - **Levels**: 1-20
   - **Produces**: Energy per hour
   - **Planet type bonus**: Ice planets may be more efficient

4. **СКЛАД (Warehouse / Storage)**
   - Stores resources
   - Higher levels = more storage capacity
   - **Levels**: 1-20
   - **Function**: Increases maximum storage for all resources
   - **Critical**: Without storage, production stops when full

5. **ФАБРИКА (Factory / Refinery)**
   - Refines raw materials into advanced resources
   - May convert metal → crystals or similar
   - **Levels**: 1-20
   - **Produces**: Processed resources
   - **Requires**: Input resources to function

6. **ВЕРФЬ (Shipyard)**
   - Constructs and repairs ships
   - Higher levels unlock better ship classes
   - **Levels**: 1-20
   - **Function**: Ship construction, module installation
   - **Unlocks**: New ship classes at specific levels

7. **ЗАЩИТА (Defense)**
   - Defensive turrets or shield generators
   - Protects colony from NPC raids (post-MVP feature)
   - **Levels**: 1-20
   - **Function**: Auto-defense against attackers
   - **Note**: Relevant for PvP systems, not MVP

8. **РАДАР (Radar / Scanner)**
   - Reveals nearby systems on galaxy map
   - May provide resource location info
   - **Levels**: 1-20
   - **Function**: Exploration assistance, information gathering

9. **ЛАБОРАТОРИЯ (Research Lab)**
   - Unlocks technology upgrades
   - May research module improvements
   - **Levels**: 1-20
   - **Function**: Tech tree progression (if implemented)

### 3.2 Building Upgrade System

**Upgrade Progression (Star Age Model):**

```
Building Upgrade Formula (estimated):

Level Progression: 1 → 20
Each level increases:
- Production rate (for resource buildings): +15-20% per level
- Storage capacity (for warehouses): +25% per level
- Unlock thresholds (for HQ/Shipyard): specific levels unlock features

Upgrade Cost Scaling:
Level 1→2: 100 metal, 50 crystals
Level 2→3: 200 metal, 100 crystals
Level 3→4: 400 metal, 200 crystals
...
Level 19→20: ~500,000 metal, ~250,000 crystals

Cost formula (exponential):
Cost_Level_N = Base_Cost × (Growth_Rate ^ (N - 1))

Where Growth_Rate ≈ 1.5-2.0 depending on building type
```

**Upgrade Time (Estimated):**
```
Build/Upgrade Time:

Early levels (1-5): 10 seconds - 5 minutes
Mid levels (6-10): 5 minutes - 30 minutes
High levels (11-15): 30 minutes - 2 hours
Max levels (16-20): 2 hours - 8 hours

Time formula:
Upgrade_Time = Base_Time × (Level ^ 1.5)

Example:
- Level 1→2: 1 minute
- Level 5→6: 10 minutes
- Level 10→11: 1 hour
- Level 15→16: 4 hours
- Level 19→20: 8 hours
```

**NOTE**: Star Age was social/browser game with real-time waiting. For our "relaxing" MVP vision, we may want to adjust these times or make them instant for offline progress.

### 3.3 Resource Production Mechanics

**Production Building Output:**

```
Hourly Production Rate:

Mine (Metal):
Level 1: 50 metal/hour
Level 5: 100 metal/hour
Level 10: 250 metal/hour
Level 15: 600 metal/hour
Level 20: 1,500 metal/hour

Formula:
Production = Base_Rate × (Level ^ 1.3)

Modified by:
- Planet type bonus: +50% on Rocky planets
- Power Station level: +10% per power level above mine level
- Research bonuses (if tech tree implemented)
```

**Storage Capacity:**

```
Warehouse Storage:

Level 1: 500 units (all resource types)
Level 5: 2,000 units
Level 10: 8,000 units
Level 15: 30,000 units
Level 20: 100,000 units

Formula:
Storage = Base_Storage × (Level ^ 1.5)

Critical mechanic:
When storage is full, production stops until player collects resources
Encourages regular login (async "space farm" mechanic)
```

### 3.4 Building Placement & Colony Grid

**Colony Layout (Star Age Model):**

- **Fixed grid**: Pre-defined building slots on planet surface
- **Slot count**: 6-12 slots depending on planet size
- **Building placement**: Click empty slot → select building type → construct
- **No free-form placement**: Unlike Clash of Clans, positions are fixed
- **Multiple colonies**: Player can have colonies on multiple planets

**Planet Size Categories:**
- **Small planets**: 6 building slots
- **Medium planets**: 9 building slots (most common)
- **Large planets**: 12 building slots (rare, valuable)

### 3.5 Planet Type Bonuses

**Six Planet Types (From Star Age):**

1. **Rocky Planets**
   - Metal production: +50%
   - Defense: +25%
   - **Best for**: Mining operations, defensive colonies

2. **Ice Planets**
   - Energy production: +50%
   - Water/Ice production: +50%
   - **Best for**: Power generation, resource refining

3. **Desert Planets**
   - Crystal production: +50%
   - Solar energy: +25%
   - **Best for**: Advanced resource gathering

4. **Lava Planets**
   - Plasma production: +100%
   - Extreme conditions (no habitation bonus)
   - **Best for**: Late-game rare resource production

5. **Earth-like Planets**
   - Balanced bonuses: +15% to all production
   - High building slot count
   - **Best for**: Balanced colonies, headquarters

6. **Gas Giants**
   - **Non-colonizable** (no building slots)
   - Fuel extraction only (orbital platform)
   - Ship must orbit to extract fuel
   - **Best for**: Fuel refueling stations

### 3.6 Multi-Colony Management

**Colony System (Star Age Model):**

```
Colony Ownership:

Starting colonies: 1 (home planet)
Additional colonies: Unlock through:
  - Headquarters level (Level 5 = 2 colonies, Level 10 = 3 colonies, etc.)
  - Credits cost to establish colony (50,000 credits per new colony)

Colony specialization strategy:
- Home planet: Balanced production + shipyard
- Rocky planet: Metal mining colony
- Desert planet: Crystal production colony
- Ice planet: Energy production + refining
- Gas giant: Fuel extraction outpost

Resource transfer:
- Ship must travel to colony and load cargo
- Or: Auto-transfer between colonies (requires research/building)
```

### 3.7 Design Decision Flags - Colony Building

**⚠️ DECISION NEEDED: Upgrade Time Model**
- Star Age had real-time waiting (social game mechanic)
- Options for MVP:
  - A) Instant upgrades (no waiting)
  - B) Short wait times (max 1-2 hours)
  - C) Offline progress (upgrades complete while offline)
- **Recommendation**: Option C - upgrades take time but complete offline, aligns with "asynchronous space farm" and "relaxing" vision

**⚠️ DECISION NEEDED: Building Count for MVP**
- Star Age had 9+ building types
- Assumption A6.4: MVP includes 9 building types
- Are all 9 necessary for MVP or can we reduce?
- **Recommendation**: Include all 9 - they're core to strategic depth. Defense building can be simplified for NPC-only MVP.

**⚠️ DECISION NEEDED: Resource Production Rates**
- How fast should resources accumulate?
- Balance between "satisfying progress" and "not too grindy"
- **Recommendation**: Test with 1-2 hour production cycles for full storage at low levels, 4-6 hours at high levels

**⚠️ DECISION NEEDED: Multi-Colony Complexity**
- How many colonies can player manage in MVP?
- **Recommendation**: Start with 1-3 colonies max for MVP, expand post-MVP
- Keeps scope manageable while enabling strategic specialization

**⚠️ DECISION NEEDED: Building Placement System**
- Fixed slots (Star Age model) vs. Free placement (Clash of Clans model)
- **Recommendation**: Fixed slots for MVP - simpler to implement, faster for AI to generate UI, matches Star Age model

### 3.8 Implementation Notes - Colony Building

**For Antigravity AI - Colony System Specs:**

```javascript
// Building definition
const buildingTypes = {
  headquarters: {
    name: 'Headquarters',
    nameRu: 'ШТАБ',
    maxLevel: 20,
    baseUpgradeCost: {
      metal: 100,
      crystals: 50,
      credits: 500
    },
    costGrowthRate: 1.8,
    baseUpgradeTime: 60, // seconds
    timeGrowthRate: 1.5,
    function: 'unlocks',
    unlockThresholds: {
      5: 'second_colony',
      10: 'third_colony',
      15: 'advanced_research'
    }
  },
  
  mine: {
    name: 'Mine',
    nameRu: 'БУР',
    maxLevel: 20,
    baseUpgradeCost: {
      metal: 50,
      crystals: 25,
      credits: 300
    },
    costGrowthRate: 1.6,
    baseUpgradeTime: 45,
    timeGrowthRate: 1.5,
    function: 'production',
    produces: 'metal',
    baseProduction: 50, // per hour
    productionGrowthRate: 1.3
  },
  
  warehouse: {
    name: 'Warehouse',
    nameRu: 'СКЛАД',
    maxLevel: 20,
    baseUpgradeCost: {
      metal: 200,
      crystals: 100,
      credits: 1000
    },
    costGrowthRate: 1.7,
    baseUpgradeTime: 30,
    timeGrowthRate: 1.4,
    function: 'storage',
    baseStorage: 500,
    storageGrowthRate: 1.5
  }
  
  // ... other buildings
}

// Colony class
class Colony {
  constructor(planetId, planetType) {
    this.planetId = planetId;
    this.planetType = planetType;
    this.buildings = {}; // buildingId: { type, level, lastCollected }
    this.resources = {
      metal: 0,
      crystals: 0,
      ice: 0,
      plasma: 0,
      fuel: 0,
      credits: 0
    };
    this.storage = this.calculateStorage();
  }
  
  // Calculate production since last collection
  collectProduction() {
    const now = Date.now();
    const production = {};
    
    Object.entries(this.buildings).forEach(([id, building]) => {
      if (building.function === 'production') {
        const buildingDef = buildingTypes[building.type];
        const hoursSinceCollection = (now - building.lastCollected) / (1000 * 60 * 60);
        
        // Calculate production
        let baseRate = buildingDef.baseProduction * Math.pow(building.level, buildingDef.productionGrowthRate);
        
        // Apply planet type bonus
        const bonus = this.getPlanetBonus(building.produces);
        baseRate *= (1 + bonus);
        
        // Total production
        const produced = baseRate * hoursSinceCollection;
        production[building.produces] = (production[building.produces] || 0) + produced;
        
        // Update last collected
        building.lastCollected = now;
      }
    });
    
    // Add to resources (capped by storage)
    Object.entries(production).forEach(([resource, amount]) => {
      this.resources[resource] = Math.min(
        this.resources[resource] + amount,
        this.storage
      );
    });
    
    return production;
  }
  
  upgradeBuilding(buildingId) {
    const building = this.buildings[buildingId];
    const buildingDef = buildingTypes[building.type];
    
    if (building.level >= buildingDef.maxLevel) {
      return { error: 'Max level reached' };
    }
    
    // Calculate upgrade cost
    const cost = this.calculateUpgradeCost(building);
    
    // Check resources
    if (!this.hasResources(cost)) {
      return { error: 'Insufficient resources' };
    }
    
    // Calculate upgrade time
    const upgradeTime = buildingDef.baseUpgradeTime * Math.pow(building.level, buildingDef.timeGrowthRate);
    
    // Deduct resources
    this.deductResources(cost);
    
    // Start upgrade (in real implementation, this would be async)
    building.upgrading = true;
    building.upgradeCompleteAt = Date.now() + (upgradeTime * 1000);
    
    return { success: true, upgradeTime };
  }
  
  calculateUpgradeCost(building) {
    const buildingDef = buildingTypes[building.type];
    const cost = {};
    
    Object.entries(buildingDef.baseUpgradeCost).forEach(([resource, baseAmount]) => {
      cost[resource] = Math.floor(baseAmount * Math.pow(buildingDef.costGrowthRate, building.level - 1));
    });
    
    return cost;
  }
  
  getPlanetBonus(resourceType) {
    const bonuses = {
      rocky: { metal: 0.5, defense: 0.25 },
      ice: { energy: 0.5, ice: 0.5 },
      desert: { crystals: 0.5, energy: 0.25 },
      lava: { plasma: 1.0 },
      earthlike: { all: 0.15 }
    };
    
    const planetBonus = bonuses[this.planetType];
    return planetBonus[resourceType] || planetBonus.all || 0;
  }
  
  calculateStorage() {
    let totalStorage = 0;
    Object.values(this.buildings).forEach(building => {
      if (building.type === 'warehouse') {
        const buildingDef = buildingTypes.warehouse;
        totalStorage += buildingDef.baseStorage * Math.pow(buildingDef.storageGrowthRate, building.level - 1);
      }
    });
    return totalStorage || 500; // minimum storage
  }
}
```

**UI Requirements for Colony Management:**
- Planet surface view (isometric or top-down grid)
- Building placement interface (click slot → select building)
- Building upgrade interface (click building → show stats + upgrade button)
- Resource display (current / storage capacity)
- Production rates (metal/hour, crystals/hour, etc.)
- Upgrade queue (what's currently upgrading, time remaining)
- Multi-colony switcher (dropdown or sidebar to switch between colonies)

**Comparison with Clash of Clans:**

Star Age building system is **similar** to Clash of Clans in:
- Multi-level building upgrades (1-20 vs. 1-15)
- Exponential cost scaling
- Real-time upgrade waits
- Resource production buildings
- Storage capacity limits

Star Age building system **differs** from Clash of Clans in:
- Fixed placement slots (not free-form layout)
- No defensive layout optimization (different game focus)
- Resource production is automatic, not raiding-based
- Planet type bonuses add strategic colony location choices

**Mechanics to Consider Borrowing from Clash of Clans:**
- ✅ Multi-resource upgrade costs (already in Star Age)
- ✅ Building upgrade time scaling (already in Star Age)
- ❓ Builder queues (start multiple upgrades simultaneously)
  - **User decision needed**: Does MVP need upgrade queues or one-at-a-time?
  - **Recommendation**: Start with one upgrade at a time for MVP, add queues post-MVP
- ❌ Defensive base layout optimization (not relevant for NPC-only MVP)

---

## 4. Resource Economy

### 4.1 Resource Types (Star Age)

**Six Primary Resources:**

1. **Metal (Металл)**
   - Most common resource
   - Mined from planets (Rocky planets +50% bonus)
   - Used for: Ship construction, building upgrades, module crafting
   - Primary bottleneck: Early game

2. **Crystals (Кристаллы)**
   - Rare resource
   - Extracted from Desert planets (+50% bonus)
   - Used for: Advanced upgrades, high-tier modules, research
   - Primary bottleneck: Mid-game

3. **Ice / Water (Лёд / Вода)**
   - Collected from Ice planets (+50% bonus)
   - Used for: Energy production, life support, fuel refining
   - Converted to energy by Power Stations

4. **Plasma (Плазма)**
   - Very rare, late-game resource
   - Extracted from Lava planets (+100% bonus)
   - Used for: Top-tier weapons, end-game modules
   - Primary bottleneck: Late-game

5. **Fuel (Топливо)**
   - Essential for ship travel
   - Extracted from Gas Giants (orbital extraction)
   - Consumed by: Ship movement, hyperspace jumps
   - Can also be purchased from traders

6. **Credits (Кредиты)**
   - Universal currency
   - Earned from: Missions, selling resources, trading, combat loot
   - Used for: Purchasing ships, modules, speeding up upgrades

### 4.2 Resource Acquisition Methods

**Production (Colony Buildings):**
```
Hourly Production Rates (Level 10 buildings, no bonuses):

Metal: 250/hour
Crystals: 100/hour
Ice: 200/hour
Plasma: 50/hour (requires Lava planet)

Player must regularly collect production or storage fills up
```

**Mining (Active Gathering):**
```
Asteroid Mining Rates:

Metal Asteroids: 5-10 metal per second of mining
Crystal Asteroids: 2-5 crystals per second of mining
Ice Asteroids: 5-15 ice per second of mining

Mining requires:
- Ship with mining laser equipped
- Cargo space available
- Player actively firing mining laser at asteroid
- Asteroids have finite resources (deplete after X mining)
```

**Trading (NPC Traders):**
```
Price Variations Between Systems:

Example:
System A: Metal sells for 10 credits/unit, Crystals buy for 100 credits/unit
System B: Metal sells for 5 credits/unit, Crystals buy for 150 credits/unit

Trading arbitrage:
Buy metal cheap in System B → Sell in System A for profit
Buy crystals cheap in System A → Sell in System B for profit

Requires:
- Ship with large cargo capacity (Carrier or Miner)
- Fuel for travel between systems
- Knowledge of price differences (Radar building reveals prices)
```

**Missions (Quest Rewards):**
```
Mission Rewards (examples):

Destroy 5 pirates: 500 metal, 200 crystals, 2000 credits
Mine 500 ice: 1000 credits, Mk II mining laser
Escort trader: 1500 credits, 300 crystals
Explore new system: 2000 credits, fuel tank upgrade

Missions are primary source of credits in early game
```

**Combat (Loot Drops):**
```
Enemy Loot Tables (estimated):

Scout pirate: 10-25 metal, 50-100 credits
Fighter pirate: 25-50 metal, 5-10 crystals, 100-250 credits
Destroyer pirate: 100-200 metal, 50-100 crystals, 500-1000 credits
Boss enemy: 500+ metal, 200+ crystals, 50+ plasma, 5000+ credits

Loot auto-pickup when ship approaches destroyed enemy
```

### 4.3 Resource Sinks (Consumption)

**Ship Construction:**
```
Ship Purchase Costs (estimated):

Scout: 1,000 credits
Fighter: 5,000 credits, 500 metal
Frigate: 15,000 credits, 2,000 metal, 500 crystals
Destroyer: 50,000 credits, 10,000 metal, 2,000 crystals
Cruiser: 150,000 credits, 30,000 metal, 10,000 crystals
Carrier: 200,000 credits, 50,000 metal, 20,000 crystals, 5,000 plasma
Miner: 10,000 credits, 5,000 metal
```

**Module Crafting/Purchase:**
```
Module Costs (from Section 2.3):

Mk I modules: 500-1,000 credits, 100-200 metal
Mk II modules: 2,000-5,000 credits, 500-1,000 metal, 100-250 crystals
Mk III modules: 8,000-20,000 credits, 2,000-5,000 metal, 500-1,500 crystals, 100-500 plasma
```

**Building Upgrades:**
```
Building Upgrade Costs (from Section 3.2):

Early levels (1-5): 100-1,000 metal, 50-500 crystals
Mid levels (6-10): 1,000-10,000 metal, 500-5,000 crystals
High levels (11-15): 10,000-100,000 metal, 5,000-50,000 crystals
Max levels (16-20): 100,000-500,000 metal, 50,000-250,000 crystals
```

**Fuel Consumption:**
```
Fuel Usage:

Normal flight: 1 fuel per 100 pixels traveled
Hyperspace jump: 10-50 fuel depending on distance
Afterburner: 5 fuel per second

Player must regularly refuel at:
- Own colonies
- Gas giant extraction
- NPC fuel traders
```

### 4.4 Economy Balance & Progression

**Resource Scarcity Curve:**

```
Early Game (Levels 1-10):
- Metal is plentiful (easy to mine, produce)
- Credits are scarce (limited mission rewards)
- Crystals are bottleneck (can't produce enough for upgrades)
→ Player focuses on missions for credits, asteroid mining for crystals

Mid Game (Levels 11-20):
- Metal production is automated (high-level mines)
- Credits flow from trading and missions
- Crystals production catches up
- Plasma becomes new bottleneck (requires Lava planet colony)
→ Player establishes specialized colonies, engages in trading

Late Game (Levels 21-30):
- All resources automated via colonies
- Credits are plentiful
- Plasma is still bottleneck for top-tier items
- Player focuses on optimization and efficiency
→ Player has empire of specialized colonies, runs trade routes
```

**Economic Gameplay Loops:**

```
Mining Loop:
1. Equip Miner ship with cargo modules + mining lasers
2. Travel to asteroid field in safe system
3. Mine asteroids until cargo full (10-15 minutes)
4. Return to colony, unload cargo
5. Sell excess resources for credits or use for upgrades
6. Repeat

Trading Loop:
1. Research price differences using Radar building
2. Buy cheap resources in System A
3. Load cargo onto Carrier ship (high capacity)
4. Travel to System B (fuel cost)
5. Sell resources at higher price
6. Buy different cheap resources in System B
7. Return to System A, sell for profit
8. Repeat, optimizing routes

Colony Automation Loop:
1. Establish colonies on resource-rich planets
2. Upgrade production buildings to high levels
3. Upgrade storage to hold 6-8 hours of production
4. Log in 1-2 times per day to collect production
5. Use collected resources for ship/module upgrades
6. Expand to more colonies for more production
```

### 4.5 Design Decision Flags - Resource Economy

**⚠️ DECISION NEEDED: Resource Production Rates**
- How many hours of production should max storage hold?
- **Recommendation**: 4-6 hours for low-level storage, 12-24 hours for max-level
- Aligns with "asynchronous space farm" - log in once or twice per day

**⚠️ DECISION NEEDED: Trading Mechanics**
- Should MVP include trading system or defer to post-MVP?
- Trading adds economic depth but increases complexity
- **Recommendation**: Include basic trading (buy/sell at fixed prices) in MVP, defer dynamic pricing and arbitrage to post-MVP

**⚠️ DECISION NEEDED: Resource Types Count**
- Assumption A3.9: Six resource types might overwhelm new players
- Could reduce to 4 for MVP: Metal, Energy (replace Ice), Credits, Fuel
- **Recommendation**: Keep all 6 - they're core to Star Age identity and strategic depth. Tutorial should explain each clearly.

**⚠️ DECISION NEEDED: Fuel Economy Balance**
- How much should fuel limit exploration?
- Should players frequently run out of fuel (creates tension) or rarely (removes annoyance)?
- **Recommendation**: Fuel should be mild constraint, not frustrating - players shouldn't get stranded, but should plan long journeys

**⚠️ DECISION NEEDED: Economy Difficulty Curve**
- How quickly should players progress economically?
- Should there be distinct economic phases (struggling → comfortable → wealthy)?
- **Recommendation**: Design progression curve with clear phases - test in beta and adjust

### 4.6 Implementation Notes - Resource Economy

**For Antigravity AI - Economy System Specs:**

```javascript
// Resource system
const resources = {
  metal: {
    name: 'Metal',
    icon: 'metal_icon.png',
    startingAmount: 500,
    maxStorage: 500 // increases with warehouse level
  },
  crystals: {
    name: 'Crystals',
    icon: 'crystal_icon.png',
    startingAmount: 100,
    maxStorage: 500
  },
  ice: {
    name: 'Ice',
    icon: 'ice_icon.png',
    startingAmount: 200,
    maxStorage: 500
  },
  plasma: {
    name: 'Plasma',
    icon: 'plasma_icon.png',
    startingAmount: 0,
    maxStorage: 500
  },
  fuel: {
    name: 'Fuel',
    icon: 'fuel_icon.png',
    startingAmount: 100,
    maxStorage: 1000 // based on ship fuel tank
  },
  credits: {
    name: 'Credits',
    icon: 'credits_icon.png',
    startingAmount: 5000,
    maxStorage: Infinity // no cap on credits
  }
}

// Trading system
const tradingPrices = {
  // System ID: resource prices
  system_1: {
    metal: { buy: 15, sell: 10 },
    crystals: { buy: 150, sell: 100 },
    ice: { buy: 12, sell: 8 },
    fuel: { buy: 50, sell: 40 }
  },
  system_2: {
    metal: { buy: 10, sell: 5 },
    crystals: { buy: 200, sell: 150 },
    ice: { buy: 8, sell: 5 },
    fuel: { buy: 60, sell: 45 }
  }
  // Prices vary by system, creating trading opportunities
}

// Resource management class
class ResourceManager {
  constructor() {
    this.resources = {};
    Object.keys(resources).forEach(type => {
      this.resources[type] = resources[type].startingAmount;
    });
  }
  
  addResource(type, amount) {
    const maxStorage = this.getMaxStorage(type);
    this.resources[type] = Math.min(
      this.resources[type] + amount,
      maxStorage
    );
    return this.resources[type];
  }
  
  deductResource(type, amount) {
    if (this.resources[type] < amount) {
      return false; // insufficient resources
    }
    this.resources[type] -= amount;
    return true;
  }
  
  hasResources(cost) {
    return Object.entries(cost).every(([type, amount]) => {
      return this.resources[type] >= amount;
    });
  }
  
  getMaxStorage(type) {
    if (type === 'credits') return Infinity;
    if (type === 'fuel') return this.ship.fuelTank; // ship-specific
    // Calculate based on warehouse level
    return this.colony.calculateStorage();
  }
  
  // Trading functions
  buyResource(systemId, resourceType, quantity) {
    const price = tradingPrices[systemId][resourceType].buy;
    const totalCost = price * quantity;
    
    if (this.resources.credits < totalCost) {
      return { error: 'Insufficient credits' };
    }
    
    this.deductResource('credits', totalCost);
    this.addResource(resourceType, quantity);
    
    return { success: true, spent: totalCost };
  }
  
  sellResource(systemId, resourceType, quantity) {
    const price = tradingPrices[systemId][resourceType].sell;
    const totalValue = price * quantity;
    
    if (this.resources[resourceType] < quantity) {
      return { error: 'Insufficient resources to sell' };
    }
    
    this.deductResource(resourceType, quantity);
    this.addResource('credits', totalValue);
    
    return { success: true, earned: totalValue };
  }
}

// Mining system
class MiningSystem {
  mineAsteroid(asteroid, miningLaser, deltaTime) {
    if (!asteroid.hasResources()) {
      return { depleted: true };
    }
    
    // Calculate mining rate
    const baseRate = miningLaser.miningRate; // resources per second
    const extracted = baseRate * deltaTime;
    
    // Deplete asteroid
    asteroid.resources -= extracted;
    
    // Add to ship cargo
    const added = this.ship.addCargo(asteroid.resourceType, extracted);
    
    return {
      extracted: added,
      remaining: asteroid.resources,
      cargoFull: this.ship.cargo.isFull()
    };
  }
}
```

**UI Requirements for Resource Economy:**
- Resource counter HUD (top bar showing all 6 resources)
- Cargo capacity indicator (current / max)
- Trading interface (buy/sell screen per planet/station)
- Price comparison (if radar building is high level)
- Resource production rate display in colony view
- Loot pickup notification ("+50 Metal" floating text)

---

## 5. Space Exploration & Travel

### 5.1 Galaxy Structure (Star Age)

**Galaxy Map Layout:**

- **Total Star Systems**: 12-14 in original Star Age
- **MVP Scope** (from Assumption A6.1): 6-8 systems, expand to 12-14 post-MVP
- **System Connections**: Pre-defined routes between systems (not free-form)
- **Travel Method**: Click destination system → hyperspace jump via warpgate

**Star System Composition:**

```
Each Star System Contains:

1. Central Star (visual only, no gameplay function)
2. Planets: 3-8 planets per system
   - Colonizable planets (Rocky, Ice, Desert, Lava, Earth-like)
   - Gas giants (fuel extraction only)
3. Asteroid Fields: 2-5 asteroid clusters per system
4. Warpgates: 1-3 connections to other systems
5. NPCs: Pirates, Traders, Miners (spawn in specific zones)
6. Space Stations (possibly): Trading posts, shipyards

System types by difficulty:
- Green (Safe): Few pirates, rich resources, good for colonies
- Yellow (Medium): More pirates, balanced resources
- Red (Dangerous): Many pirates + boss enemies, rare resources (plasma)
```

### 5.2 Travel Mechanics

**Intra-System Movement (Within a System):**

```
Ship Movement:
- 8-directional free movement
- Speed: 100-250 pixels per second (ship class + engine dependent)
- Fuel consumption: 1 fuel per 100 pixels traveled
- No loading screens - continuous movement

Camera follows ship smoothly
System is large enough to feel like exploring (estimated 3000x3000 pixels)
Player can see nearby objects (planets, asteroids) on minimap
```

**Inter-System Travel (Between Systems):**

```
Hyperspace Jump:
1. Player clicks destination system on galaxy map
2. Route preview shown (path through connected systems)
3. Fuel cost calculated and displayed
4. Player confirms jump
5. Warp animation (speed streaks, loading)
6. Player arrives in new system at warpgate location

Fuel Cost Formula:
Fuel_Cost = Base_Distance_Cost × Jump_Count

Example:
- Adjacent system (1 jump): 20 fuel
- 2 jumps away: 40 fuel
- 3 jumps away: 60 fuel

Fuel efficiency module reduces cost by 20-40%
```

### 5.3 Exploration & Discovery

**Exploration Progression:**

```
Galaxy Unlocking:

Starting state:
- Home system fully visible
- Adjacent systems visible but marked "unexplored"
- Distant systems hidden (fog of war)

Exploration requirements:
- Travel to system via warpgate (consumes fuel)
- System becomes "explored" - all planets visible
- Radar building level determines visibility range
  - Level 1 Radar: 1 system away visible
  - Level 10 Radar: 2 systems away visible
  - Level 20 Radar: Entire galaxy visible

Discovery rewards:
- First visit to system: Credits bonus (500-2000)
- Planet scanning: Reveals resource types and bonuses
- Completing system scan: Mission reward
```

**Planet Scanning:**

```
Scan Mechanics:

Without Scanner Module:
- Planet type visible (Rocky, Ice, etc.)
- Must land to see detailed info

With Scanner Module:
- Orbit planet and activate scanner
- Shows: Resource bonuses, building slot count, danger level
- Helps choose optimal colony locations

Scanner tiers:
- Mk I: Shows planet type only
- Mk II: Shows resource bonuses
- Mk III: Shows full details + asteroid composition in system
```

### 5.4 Navigation & Minimap

**Minimap System:**

```
Minimap Display (circular radar):

Icons shown:
- Player ship (center, green arrow)
- Friendly/Neutral ships (blue/yellow dots)
- Enemy ships (red dots)
- Planets (planet icons)
- Asteroids (grey dots)
- Warpgates (purple hexagons)
- Objectives (mission markers)

Minimap range:
- Base range: 500 pixels radius
- Radar building increases range
- Scanner module increases range further

Clicking minimap object:
- Auto-pilot ship toward it (if not in combat)
```

**Galaxy Map Interface:**

```
Galaxy Map Features:

System display:
- Star icon with difficulty color (green/yellow/red)
- Connection lines to adjacent systems
- Hover shows: System name, danger level, known planets

Planet display (in-system):
- Planet icons in orbit around star
- Hover shows: Planet type, colony status, resources
- Click to land or establish colony

Filter options:
- Show only colonized planets
- Show systems with missions
- Show high-value resource systems
- Show unexplored systems
```

### 5.5 Space Hazards & Events

**Environmental Hazards (Potential for MVP):**

```
Asteroid Fields:
- Dense clusters that damage ship on collision
- Slow ship movement when navigating through
- Rich mining opportunities
- Pirates hide in asteroid fields

Nebula Clouds:
- Reduce visibility (fog effect)
- Reduce scanner range
- NPCs can ambush from within
- Some nebulas boost shield regeneration

Solar Radiation (near stars):
- Damages ship slowly if too close
- Reduces shield effectiveness
- Visual warning effect

Warp Gate Ambushes:
- Pirates sometimes camp near warp gates
- Player arrives and immediately in combat
- Must fight or flee
```

**Random Events (Post-MVP Consideration):**

```
Potential Events:
- Distress signal (rescue NPC for reward)
- Derelict ship (loot or trap)
- Trader convoy (sell resources at good prices)
- Pirate ambush (combat encounter)
- Rare asteroid (high resource yield)

NOTE: Star Age video didn't show random events clearly
These are genre-standard mechanics to consider post-MVP
```

### 5.6 Design Decision Flags - Exploration

**⚠️ DECISION NEEDED: Galaxy Size for MVP**
- Assumption A6.1: 6-8 systems for MVP
- Is 6-8 enough content for 10-15 hours of gameplay?
- **Recommendation**: Start with 6 systems, structured progression:
  - Systems 1-2: Green (tutorial, safe zones)
  - Systems 3-4: Yellow (mid-game challenge)
  - Systems 5-6: Red (late-game content)
- Add more systems based on beta feedback if needed

**⚠️ DECISION NEEDED: Exploration Incentives**
- What motivates players to explore new systems?
- **Recommendation**: Each system offers unique resources or bonuses
  - System 1: Balanced (home system)
  - System 2: Metal-rich (Rocky planets)
  - System 3: Crystal-rich (Desert planets)
  - System 4: High-level enemies + plasma resources
  - System 5-6: Boss enemies + rare modules

**⚠️ DECISION NEEDED: Travel Time vs. Fuel Cost**
- Should travel be: (a) Instant with fuel cost, or (b) Time-based journey?
- Star Age had instant warp
- **Recommendation**: Instant warp for MVP (less waiting, aligns with "relaxing" vision)

**⚠️ DECISION NEEDED: Space Hazards Inclusion**
- Should MVP include environmental hazards (asteroid collision damage, nebulas)?
- **Recommendation**: Defer to post-MVP - focus on core mechanics first, add hazards for variety later

### 5.7 Implementation Notes - Exploration

**For Antigravity AI - Galaxy System Specs:**

```javascript
// Galaxy structure
const galaxyMap = {
  systems: [
    {
      id: 'system_1',
      name: 'Sol',
      position: { x: 500, y: 500 }, // position on galaxy map
      difficulty: 'green',
      connectedSystems: ['system_2', 'system_3'],
      planets: [
        {
          id: 'planet_1_1',
          name: 'Earth',
          type: 'earthlike',
          position: { x: 200, y: 300 }, // position in system
          buildingSlots: 9,
          resourceBonuses: { all: 0.15 },
          hasColony: true // player has colony here
        },
        {
          id: 'planet_1_2',
          name: 'Mars',
          type: 'rocky',
          position: { x: 800, y: 400 },
          buildingSlots: 9,
          resourceBonuses: { metal: 0.5, defense: 0.25 },
          hasColony: false
        }
        // ... more planets
      ],
      asteroidFields: [
        {
          position: { x: 1200, y: 800 },
          asteroidCount: 20,
          resourceTypes: ['metal', 'ice']
        }
      ],
      warpGates: [
        { position: { x: 100, y: 100 }, leadsTo: 'system_2' },
        { position: { x: 2900, y: 2900 }, leadsTo: 'system_3' }
      ],
      npcSpawnZones: [
        {
          type: 'pirates',
          position: { x: 1500, y: 1500 },
          spawnRate: 0.3, // pirates per minute
          difficulty: 'easy'
        }
      ]
    }
    // ... more systems
  ],
  
  // Player exploration state
  exploredSystems: ['system_1'], // systems player has visited
  scannedPlanets: ['planet_1_1', 'planet_1_2'] // planets player has scanned
}

// Travel system
class TravelSystem {
  calculateFuelCost(fromSystem, toSystem) {
    // Find path between systems (BFS/Dijkstra)
    const path = this.findPath(fromSystem, toSystem);
    const jumps = path.length - 1;
    
    // Base fuel cost per jump
    const baseCost = 20;
    const totalCost = baseCost * jumps;
    
    // Apply fuel efficiency module
    const efficiency = this.ship.getFuelEfficiency();
    return Math.floor(totalCost * efficiency);
  }
  
  warpToSystem(targetSystemId) {
    const fuelCost = this.calculateFuelCost(this.currentSystem, targetSystemId);
    
    if (this.ship.fuel < fuelCost) {
      return { error: 'Insufficient fuel' };
    }
    
    // Deduct fuel
    this.ship.fuel -= fuelCost;
    
    // Warp animation
    this.playWarpAnimation();
    
    // Change current system
    this.currentSystem = targetSystemId;
    
    // Spawn ship at warpgate in new system
    const warpGate = this.getWarpGateInSystem(targetSystemId);
    this.ship.position = warpGate.position;
    
    // Mark system as explored
    if (!this.exploredSystems.includes(targetSystemId)) {
      this.exploredSystems.push(targetSystemId);
      this.rewardDiscovery(targetSystemId);
    }
    
    return { success: true, fuelUsed: fuelCost };
  }
  
  rewardDiscovery(systemId) {
    const system = galaxyMap.systems.find(s => s.id === systemId);
    const reward = {
      credits: 1000 * (system.difficulty === 'red' ? 3 : system.difficulty === 'yellow' ? 2 : 1)
    };
    this.player.addCredits(reward.credits);
    return reward;
  }
  
  scanPlanet(planetId) {
    if (!this.ship.hasScanner()) {
      return { error: 'Scanner module required' };
    }
    
    const planet = this.getCurrentPlanet(planetId);
    this.scannedPlanets.push(planetId);
    
    return {
      planetType: planet.type,
      resourceBonuses: planet.resourceBonuses,
      buildingSlots: planet.buildingSlots,
      dangerLevel: planet.dangerLevel
    };
  }
}

// Minimap rendering
class Minimap {
  render(canvas, ctx) {
    const radius = 150; // minimap radius in pixels
    const center = { x: canvas.width - radius - 20, y: radius + 20 };
    
    // Draw minimap circle
    ctx.fillStyle = 'rgba(0, 20, 40, 0.7)';
    ctx.beginPath();
    ctx.arc(center.x, center.y, radius, 0, Math.PI * 2);
    ctx.fill();
    
    // Draw objects within range
    const range = this.player.radarRange; // based on radar building + scanner module
    
    // Player ship (center)
    this.drawShipIcon(ctx, center.x, center.y, 'green');
    
    // Nearby objects
    this.nearbyObjects.forEach(obj => {
      const distance = this.calculateDistance(this.player.position, obj.position);
      if (distance <= range) {
        // Convert world position to minimap position
        const mmPos = this.worldToMinimap(obj.position, center, radius, range);
        
        // Draw icon based on type
        if (obj.type === 'enemy') {
          this.drawShipIcon(ctx, mmPos.x, mmPos.y, 'red');
        } else if (obj.type === 'planet') {
          this.drawPlanetIcon(ctx, mmPos.x, mmPos.y);
        } else if (obj.type === 'asteroid') {
          this.drawDot(ctx, mmPos.x, mmPos.y, 'grey');
        }
      }
    });
  }
  
  worldToMinimap(worldPos, center, radius, range) {
    const relativePos = {
      x: worldPos.x - this.player.position.x,
      y: worldPos.y - this.player.position.y
    };
    
    const scale = radius / range;
    
    return {
      x: center.x + (relativePos.x * scale),
      y: center.y + (relativePos.y * scale)
    };
  }
}
```

**Rendering Requirements:**
- Galaxy map screen (star map with connections)
- System view (2D space with planets, asteroids)
- Minimap overlay (circular radar)
- Warp animation (speed lines, star streaks)
- Planet orbit visualization (optional: planets slowly orbit star)

---

## 6. Mission & Progression Systems

### 6.1 Mission Types (Star Age)

**8-12 Mission Categories Observed/Inferred:**

1. **Combat Missions**
   - Destroy X pirates
   - Clear asteroid field of enemies
   - Defeat boss enemy ship
   - Patrol system (kill any enemies encountered)

2. **Resource Gathering Missions**
   - Mine X metal/crystals/ice
   - Collect resources from specific asteroid field
   - Gather rare resources (plasma)

3. **Escort Missions**
   - Protect NPC miner while they mine
   - Escort NPC trader to destination
   - Defend colony from pirate raid

4. **Exploration Missions**
   - Discover new star system
   - Scan all planets in system
   - Map asteroid fields
   - Find hidden location

5. **Delivery Missions**
   - Transport cargo to planet/station
   - Deliver message to NPC
   - Supply colony with resources

6. **Trading Missions**
   - Buy/sell specific resources
   - Complete profitable trade route
   - Establish trade agreement

7. **Boss Fight Missions**
   - Defeat named pirate boss
   - Destroy pirate base/station
   - High difficulty, high reward

8. **Tutorial Missions** (for new players)
   - Build first colony building
   - Launch ship into space
   - Mine first asteroid
   - Win first combat
   - Jump to adjacent system

### 6.2 Mission Rewards

**Reward Types:**

```
Credits:
- Small missions: 500-2,000 credits
- Medium missions: 2,000-10,000 credits
- Large missions: 10,000-50,000 credits
- Boss missions: 50,000+ credits

Resources:
- Metal: 100-5,000 units
- Crystals: 50-2,000 units
- Ice: 100-3,000 units
- Plasma: 10-500 units (rare)
- Fuel: 50-200 units

Items:
- Ship modules (Mk I/II/III)
- Ship blueprints (unlock new ship class)
- Building blueprints (unlock new building)
- Special equipment

Experience:
- Player level experience
- Reputation with NPC factions
- Unlocks based on level thresholds
```

### 6.3 Progression Systems

**Player Level Progression:**

```
Experience Sources:
- Mission completion: 100-5,000 XP
- Enemy kills: 10-500 XP per kill
- Resource gathering: 1 XP per 10 resources gathered
- Exploration: 500-2,000 XP per system discovered
- Building upgrades: 50-500 XP per upgrade

Level Thresholds (estimated):
Level 1→2: 1,000 XP
Level 5→6: 10,000 XP
Level 10→11: 50,000 XP
Level 20→21: 500,000 XP

Exponential formula:
XP_Required = Base_XP × (Level ^ 1.8)

Level Unlocks:
Level 5: Fighter ship unlocked
Level 10: Frigate ship unlocked
Level 15: Destroyer ship unlocked
Level 20: Second colony allowed
Level 25: Cruiser ship unlocked
Level 30: Third colony allowed
```

**Reputation System (NPC Factions):**

```
Faction Reputation:

Factions:
- Traders Guild (neutral)
- Mining Corporation (neutral)
- Independent Colonies (neutral)
- Pirate Clans (hostile)
- Military Forces (potential post-MVP)

Reputation Gains:
- Help NPC trader: +10 Trader reputation
- Complete delivery mission: +20 Colony reputation
- Destroy pirates: +5 Military, -10 Pirate
- Attack neutral: -50 respective faction

Reputation Benefits:
- Positive: Better prices, exclusive missions, special items
- Negative: Higher prices, attacked on sight, bounty hunters

Reputation Tiers:
-100 to -50: Hostile (attacked on sight)
-49 to 0: Unfriendly (poor prices, limited missions)
0 to 49: Neutral (standard prices and missions)
50 to 99: Friendly (good prices, bonus missions)
100+: Allied (best prices, exclusive content)
```

**Shipyard Level Progression:**

```
Shipyard Building Level Gates:

Level 1: Scout ship construction
Level 3: Miner ship construction
Level 5: Fighter ship construction
Level 8: Frigate ship construction
Level 12: Destroyer ship construction
Level 16: Cruiser ship construction
Level 20: Carrier ship construction

Also gates module tiers:
Level 1-5: Mk I modules available
Level 6-12: Mk II modules available
Level 13-20: Mk III modules available
```

### 6.4 Difficulty Scaling & Gating

**System Difficulty Progression:**

```
Green Systems (Safe - Levels 1-10):
- Easy enemies (50-150 HP)
- Plentiful resources
- Tutorial missions
- Good for establishing first colonies

Yellow Systems (Medium - Levels 11-20):
- Medium enemies (200-500 HP)
- Balanced resources + some rare resources
- Challenge missions
- Good for specialized colonies

Red Systems (Dangerous - Levels 21-30):
- Hard enemies (600-1000 HP)
- Rare resources (plasma)
- Boss missions
- High-value loot
- Good for late-game farming
```

**Gating Mechanics:**

```
Access Requirements:

System access gates:
- Some systems require player level to enter
- "Warning: Recommended Level 15+" message
- Can enter anyway but face difficult enemies

Mission difficulty gates:
- Missions have level requirements
- "Requires Level 10 to accept" displayed
- Prevents players from accepting too-hard missions

Colony gates:
- Number of colonies limited by HQ level
- Prevents over-expansion early game

Ship/module gates:
- Shipyard level determines available ships/modules
- Clear progression path
```

### 6.5 Endgame & Post-MVP Content

**Endgame Goals (MVP):**

```
For NPC-only MVP:

Achievement goals:
- Defeat all boss enemies
- Explore entire galaxy
- Establish colonies on all planet types
- Max level all buildings in one colony
- Own one of each ship class with Mk III modules
- Earn 1,000,000 credits
- Complete all missions

Post-MVP additions:
- Competitive leaderboards (most credits, fastest missions)
- PvP combat zones
- Guild/clan systems
- Territory control
- Player-driven economy
- Seasonal events
```

### 6.6 Design Decision Flags - Progression

**⚠️ DECISION NEEDED: Player Level Cap for MVP**
- Star Age likely had 30-50 levels
- Should MVP have lower level cap (e.g., 20) to manage scope?
- **Recommendation**: Cap at level 30 for MVP - provides clear progression without excessive content requirements

**⚠️ DECISION NEEDED: Mission Variety Count**
- Assumption A6.9: 8-12 mission types for MVP
- Is this enough to avoid repetitiveness?
- **Recommendation**: Implement 8 mission types (combat, gathering, escort, exploration, delivery, trading, boss, tutorial) with procedural variation (different targets, locations, quantities)

**⚠️ DECISION NEEDED: Reputation System Inclusion**
- Is reputation system necessary for MVP or post-MVP?
- **Recommendation**: Defer to post-MVP - focus on core mechanics first, add social/reputation layer later for depth

**⚠️ DECISION NEEDED: Tutorial Mission Flow**
- Should tutorial be separate missions or integrated into first-time gameplay?
- Assumption A6.10: Tutorial added post-MVP based on feedback
- **Recommendation**: Include 3-5 basic tutorial missions in MVP (build colony, mine asteroid, fight pirate, explore system, upgrade ship) - essential for new player onboarding even in beta

### 6.7 Implementation Notes - Missions & Progression

**For Antigravity AI - Mission System Specs:**

```javascript
// Mission definition
const missionTemplates = {
  destroy_pirates: {
    type: 'combat',
    name: 'Pirate Hunters',
    description: 'Destroy {count} pirate ships in {system}',
    objectives: [
      { type: 'kill', target: 'pirate', count: 5 }
    ],
    rewards: {
      credits: 2000,
      experience: 500,
      reputation: { military: 10, pirates: -10 }
    },
    levelRequirement: 5,
    repeatable: true
  },
  
  mine_resources: {
    type: 'gathering',
    name: 'Resource Collection',
    description: 'Mine {count} {resource} from asteroid fields',
    objectives: [
      { type: 'gather', resource: 'metal', count: 500 }
    ],
    rewards: {
      credits: 1000,
      experience: 200,
      items: ['mining_laser_mk2']
    },
    levelRequirement: 1,
    repeatable: true
  },
  
  escort_trader: {
    type: 'escort',
    name: 'Trader Escort',
    description: 'Protect trader ship traveling to {destination}',
    objectives: [
      { type: 'escort', target: 'trader_npc', destination: 'planet_2_3', survivalRequired: true }
    ],
    rewards: {
      credits: 3000,
      experience: 800,
      reputation: { traders: 20 }
    },
    levelRequirement: 10,
    repeatable: false
  },
  
  defeat_boss: {
    type: 'boss',
    name: 'Pirate Boss: Reaver King',
    description: 'Defeat the notorious pirate boss in {system}',
    objectives: [
      { type: 'kill', target: 'boss_reaver_king', count: 1 }
    ],
    rewards: {
      credits: 50000,
      experience: 10000,
      resources: { plasma: 500 },
      items: ['plasma_cannon_mk3'],
      reputation: { military: 50, pirates: -100 }
    },
    levelRequirement: 25,
    repeatable: false // one-time boss fight
  }
}

// Mission instance class
class Mission {
  constructor(template, variables) {
    this.template = template;
    this.objectives = [...template.objectives];
    this.progress = {};
    this.status = 'active'; // active, completed, failed
    
    // Instantiate mission with specific variables
    this.objectives.forEach((obj, index) => {
      this.progress[index] = 0;
      if (obj.count) {
        this.progress[index + '_max'] = obj.count;
      }
    });
  }
  
  updateProgress(objectiveIndex, amount) {
    this.progress[objectiveIndex] += amount;
    
    // Check if objective complete
    const objective = this.objectives[objectiveIndex];
    if (objective.count && this.progress[objectiveIndex] >= objective.count) {
      // Objective complete
      return this.checkMissionComplete();
    }
  }
  
  checkMissionComplete() {
    // Check if all objectives complete
    const allComplete = this.objectives.every((obj, index) => {
      if (obj.count) {
        return this.progress[index] >= obj.count;
      } else {
        return this.progress[index] > 0; // for boolean objectives
      }
    });
    
    if (allComplete) {
      this.status = 'completed';
      return { completed: true, rewards: this.template.rewards };
    }
    
    return { completed: false };
  }
  
  getProgressText() {
    return this.objectives.map((obj, index) => {
      if (obj.type === 'kill') {
        return `Destroy ${this.progress[index]}/${obj.count} ${obj.target}`;
      } else if (obj.type === 'gather') {
        return `Collect ${this.progress[index]}/${obj.count} ${obj.resource}`;
      } else if (obj.type === 'escort') {
        return obj.survivalRequired ? 
          `Escort ${obj.target} to ${obj.destination} (Must survive)` :
          `Escort ${obj.target} to ${obj.destination}`;
      }
    });
  }
}

// Progression system
class ProgressionSystem {
  constructor() {
    this.level = 1;
    this.experience = 0;
    this.experienceToNextLevel = 1000;
  }
  
  addExperience(amount) {
    this.experience += amount;
    
    // Check for level up
    while (this.experience >= this.experienceToNextLevel) {
      this.levelUp();
    }
    
    return this.level;
  }
  
  levelUp() {
    this.level++;
    this.experience -= this.experienceToNextLevel;
    
    // Calculate next level requirement
    this.experienceToNextLevel = Math.floor(1000 * Math.pow(this.level, 1.8));
    
    // Grant level-up rewards
    const rewards = this.getLevelRewards(this.level);
    
    return { newLevel: this.level, rewards };
  }
  
  getLevelRewards(level) {
    const rewards = {};
    
    // Ship unlocks
    if (level === 5) rewards.unlocked = ['fighter_ship'];
    if (level === 10) rewards.unlocked = ['frigate_ship'];
    if (level === 15) rewards.unlocked = ['destroyer_ship'];
    if (level === 20) rewards.colonySlots = 2;
    if (level === 25) rewards.unlocked = ['cruiser_ship'];
    if (level === 30) rewards.colonySlots = 3;
    
    // Always give credits bonus
    rewards.credits = level * 100;
    
    return rewards;
  }
}

// NPC reputation system
class ReputationSystem {
  constructor() {
    this.factions = {
      traders: 0,
      miners: 0,
      colonies: 0,
      military: 0,
      pirates: -50 // start hostile with pirates
    };
  }
  
  modifyReputation(faction, amount) {
    this.factions[faction] = Math.max(-100, Math.min(100, this.factions[faction] + amount));
    
    // Check for reputation tier changes
    const newTier = this.getReputationTier(faction);
    return { faction, newReputation: this.factions[faction], tier: newTier };
  }
  
  getReputationTier(faction) {
    const rep = this.factions[faction];
    if (rep <= -50) return 'hostile';
    if (rep < 0) return 'unfriendly';
    if (rep < 50) return 'neutral';
    if (rep < 100) return 'friendly';
    return 'allied';
  }
  
  getPriceModifier(faction) {
    const tier = this.getReputationTier(faction);
    const modifiers = {
      hostile: 2.0,    // 2x prices (if they trade at all)
      unfriendly: 1.5, // 1.5x prices
      neutral: 1.0,    // normal prices
      friendly: 0.85,  // 15% discount
      allied: 0.7      // 30% discount
    };
    return modifiers[tier];
  }
}
```

**UI Requirements for Missions:**
- Mission list panel (active missions, available missions)
- Mission details view (objectives, rewards, requirements)
- Progress tracker (overlay or HUD element showing active mission progress)
- Mission completion notification (popup with rewards)
- Level up notification (new level, unlocks, rewards)
- Reputation display (faction icons with relationship status)

---

# PART 2: COMPARATIVE ANALYSIS

---

## 7. FTL Equipment & Weapon Variety

### 7.1 FTL's Weapon System Analysis

**Why FTL Weapons Work Well:**

FTL features ~30 different weapons, each with distinct tactical uses. This variety creates interesting combat decisions rather than "one optimal build."

**FTL Weapon Categories:**

1. **Beam Weapons**
   - Mechanics: Draw a line across enemy ship, damaging all rooms touched
   - Strength: Ignores shields if shields are down, high damage potential
   - Weakness: Useless against shields
   - Tactical use: Save for when shields are down, then sweep across critical systems

2. **Laser Weapons**
   - Mechanics: Fire 1-4 laser shots that each remove 1 shield layer or damage hull
   - Strength: Versatile, reliable shield stripping
   - Weakness: Moderate damage, slow charge time
   - Tactical use: Primary shield-breaking weapon

3. **Missile/Torpedo Weapons**
   - Mechanics: Bypass shields entirely, use ammunition
   - Strength: Guaranteed damage to targeted system
   - Weakness: Limited ammo, can be shot down by defense drones
   - Tactical use: Precision strikes against weapons or shields

4. **Ion Weapons**
   - Mechanics: Temporarily disable systems without destroying them
   - Strength: Stacks duration with multiple hits, disables shields
   - Weakness: No permanent damage
   - Tactical use: Disable shields, then use other weapons for damage

5. **Bomb Weapons**
   - Mechanics: Teleport directly into enemy ship rooms, bypassing shields and defense
   - Strength: Guaranteed damage to specific systems
   - Weakness: Very slow charge time, limited ammo
   - Tactical use: Disable critical systems (weapons, shields, engines)

**Key FTL Design Principle:**
- No single weapon is "best" - effectiveness depends on enemy composition
- Weapon synergies encourage mixed loadouts (ions + beams, lasers + missiles)
- Limited weapon slots force strategic choices

### 7.2 Potential Adaptations for Star Age MVP

**What We Can Borrow:**

```
Weapon Diversity Principle:
Instead of just "Mk I/II/III" tiers, each weapon type should have distinct tactical purpose

Proposed Star Age Weapon Roles:

1. Laser Cannons (FTL: Laser weapons)
   - Role: Sustained DPS, shield stripping
   - Strength: High fire rate, consistent damage
   - Weakness: Reduced effectiveness against armor
   - Best against: Unarmored targets, scouts, fighters

2. Railguns (FTL: Heavy lasers)
   - Role: Armor penetration, high burst damage
   - Strength: Ignores portion of armor, high damage per shot
   - Weakness: Slow fire rate, high energy cost
   - Best against: Armored targets, destroyers, cruisers

3. Plasma Guns (FTL: Flak weapons)
   - Role: AoE damage, multiple targets
   - Strength: Small area effect on impact, hits multiple ships if clustered
   - Weakness: Slow projectiles, easy to dodge
   - Best against: Groups of enemies, asteroid mining (clears multiple)

4. Ion Cannons (NEW - inspired by FTL)
   - Role: Shield/system disruption
   - Strength: Temporarily disables enemy shields or modules
   - Weakness: No hull damage
   - Best against: High-shield enemies, preparing for burst damage
   - **USER DECISION**: Should we add Ion weapons or keep original 4?

5. Rockets/Missiles (FTL: Missiles)
   - Role: High single-target damage, boss killing
   - Strength: Massive damage per hit
   - Weakness: Ammo-based (not energy), can be dodged/intercepted
   - Best against: Boss enemies, high-value targets

6. Beam Weapons (NEW - inspired by FTL)
   - Role: Multi-ship damage in a line
   - Strength: Damage all enemies in beam path
   - Weakness: Long charge time, high energy cost
   - Best against: Multiple enemies in a line, stationary targets
   - **USER DECISION**: Should we add Beam weapons or keep original Star Age weapons?
```

**Synergy Design:**

```
Weapon Combinations (Like FTL):

Ion + Beam:
1. Use Ion cannon to disable enemy shields
2. While shields down, use Beam weapon to sweep across hull
3. Result: Massive damage if executed correctly

Laser + Missile:
1. Use Lasers to strip shields
2. When shields down, fire Missiles for precision damage
3. Result: Guaranteed critical system damage

Plasma + Railgun:
1. Use Plasma to AoE damage clustered enemies
2. Finish survivors with high-damage Railgun shots
3. Result: Efficient crowd control

This creates "build optimization" gameplay:
- DPS build: 3x Lasers (pure sustained damage)
- Burst build: Ion + Beam (setup combo)
- Tank killer: 2x Railgun + Shield (armor penetration)
- Boss build: Lasers + Missiles (strip shields, precision burst)
```

### 7.3 FTL's Subsystem Targeting

**FTL Mechanic:**
- Click enemy ship rooms to target specific systems
- Damage to shields room reduces shield layers
- Damage to weapons room disables enemy weapons
- Damage to engines reduces enemy evasion

**Star Age Adaptation Consideration:**

```
Simple Subsystem Targeting (Optional for MVP):

Instead of rooms (too complex for 2D top-down), use zones:

Ship Subsystems:
- Front (Weapons): If damaged, reduces weapon effectiveness
- Middle (Hull): Standard hull damage
- Rear (Engines): If damaged, reduces ship speed

Targeting mechanic:
- Default: Auto-target center (hull damage)
- Player can aim at front or rear for tactical effect
- Requires precision aiming (skill-based)

Benefits:
- Adds tactical depth to combat
- Rewards skilled aiming
- Creates decision: Fast damage (center) vs. tactical effect (subsystems)

Concerns:
- Increases complexity
- Harder to implement in real-time 2D combat
- May frustrate players who just want to shoot

**USER DECISION NEEDED**: 
Should MVP include subsystem targeting or keep simple hull damage only?
**Recommendation**: Defer to post-MVP - focus on core combat first, add tactical depth later
```

### 7.4 Design Decision Flags - FTL Comparisons

**⚠️ DECISION NEEDED: Weapon Type Count**
- Star Age had 4-5 weapon types
- FTL has ~30 weapons across 5 categories
- Should we add Ion and/or Beam weapons inspired by FTL?
- **Recommendation**: Keep Star Age's 4-5 weapon types for MVP, ensure each has distinct tactical role. Consider adding Ion/Beam post-MVP if combat feels shallow.

**⚠️ DECISION NEEDED: Weapon Synergies**
- Should we design weapons to combo like FTL (Ion + Beam)?
- Or keep weapons independent (each good on its own)?
- **Recommendation**: Design for weak synergies in MVP (e.g., Plasma softens groups, Railgun finishes survivors) but don't require combos. Post-MVP can add stronger synergies.

**⚠️ DECISION NEEDED: Subsystem Targeting**
- Add FTL-style targeting of ship subsystems?
- **Recommendation**: No for MVP - keep combat simple. Consider post-MVP if beta feedback wants more tactical depth.

---

## 8. Clash of Clans Building Progression

### 8.1 Clash of Clans Upgrade System Analysis

**What Makes Clash Upgrade System Engaging:**

1. **Multi-Resource Costs**
   - Upgrades require different resource combinations (Gold + Elixir)
   - Creates interesting strategic choices: "Do I upgrade offense or defense?"
   - Prevents single-resource bottlenecks

2. **Time-Gated Progression**
   - Higher level upgrades take real-world time (minutes to days)
   - Creates anticipation and return motivation
   - Allows free-to-play with optional speed-ups (monetization)

3. **Builder Queue System**
   - Limited builders (1-5 depending on upgrades)
   - Can work on multiple upgrades simultaneously
   - Creates strategic prioritization: "Which building to upgrade first?"

4. **Town Hall Gating**
   - Central building (Town Hall) gates access to higher-level upgrades
   - Must max out buildings before Town Hall upgrade for optimal progression
   - Clear progression milestones

5. **Exponential Cost Scaling**
   - Early levels cheap and fast (instant gratification)
   - Late levels expensive and slow (long-term goals)
   - Smooth difficulty curve

### 8.2 Star Age vs. Clash of Clans Comparison

**Similarities:**

```
Both systems share:
✅ Multi-level building upgrades (Star Age: 1-20, Clash: 1-15)
✅ Exponential cost scaling
✅ Resource production buildings vs. storage buildings
✅ Central HQ/Town Hall gates progression
✅ Time-gated upgrades (Clash: real-time, Star Age: likely real-time)
```

**Differences:**

```
Star Age differences:
- Fixed building placement (slots) vs. Clash free-form layout
- No defensive layout optimization (different game genre)
- Resource production is automatic (idle) vs. Clash raiding economy
- Planet type bonuses (strategic colony location) vs. Clash single base

Clash differences:
- Builder queue system (1-5 builders) vs. Star Age (unknown, likely 1)
- Defensive base design is core gameplay vs. Star Age (defense minimal)
- Raiding other players for resources vs. Star Age NPC-only for MVP
```

### 8.3 Mechanics Worth Borrowing

**1. Multi-Resource Upgrade Costs** ✅ Already in Star Age

```
Star Age Example:
HQ Level 5→6: 1,000 metal, 500 crystals, 2,000 credits

Benefit: 
- Prevents single-resource grind
- Creates choice: "Spend crystals on HQ or Shipyard?"
- More engaging than single-resource costs
```

**2. Builder Queue System** ❓ User Decision Needed

```
Clash Model:
- Start with 1 builder
- Can purchase/unlock up to 5 builders (premium currency or progression)
- Each builder can work on 1 upgrade simultaneously

Star Age Adaptation:
- Start with 1 building queue slot
- Unlock 2nd slot at HQ Level 10
- Unlock 3rd slot at HQ Level 20 (or premium feature)

Benefits:
- Allows simultaneous upgrades (less waiting)
- Creates monetization opportunity (extra builders)
- Reduces frustration of long upgrade times

Concerns:
- Increases complexity
- Our vision is "relaxing game" - queues might pressure players
- Offline progress reduces need for queues

**USER DECISION**: 
Should MVP include builder queues or single-upgrade-at-a-time?
**Recommendation**: Single upgrade for MVP (simpler), add queues post-MVP if players want faster progression
```

**3. Upgrade Time Scaling** ✅ Already in Star Age (estimated)

```
Both games scale upgrade time exponentially:

Clash Example:
Level 1→2: 10 seconds
Level 5→6: 5 minutes
Level 10→11: 2 hours
Level 15: 14 days (max)

Star Age Adaptation (from Section 3.2):
Level 1→2: 1 minute
Level 10→11: 1 hour
Level 20: 8 hours (max for MVP)

Benefit:
- Early levels feel fast (instant gratification)
- Late levels create long-term goals
- Natural pacing without artificial gates

Note: Unlike Clash (real-time waiting), our "relaxing" vision suggests upgrades complete offline, reducing pressure to login constantly
```

**4. Central Building Gates Progression** ✅ Already in Star Age

```
Clash: Town Hall level gates building max levels
Star Age: HQ level gates building types and colony count

Same principle, well-proven in both games
```

### 8.4 Mechanics to Avoid

**1. Defensive Base Layout Optimization** ❌ Not Relevant

```
Why it works in Clash:
- Players attack each other's bases
- Defensive building placement is strategic mini-game
- "Perfect base design" is endgame content

Why it doesn't fit Star Age:
- Our MVP is NPC-only, no base raiding
- Fixed building slots, not free-form placement
- Focus is space exploration, not base defense
- Post-MVP PvP might be opt-in zones, not base raids

Decision: Skip defensive layout mechanics entirely
```

**2. Forced Real-Time Waiting** ❌ Conflicts with Vision

```
Clash mechanic:
- Upgrades take real-world time
- Player must wait or pay premium currency
- Creates return incentive but also frustration

Our "relaxing game" vision:
- Upgrades take time BUT complete offline
- No pressure to login at specific times
- Respects player's schedule

Decision: Keep upgrade timers but allow offline progress
```

**3. Raiding Economy** ❌ Different Game Genre

```
Clash core loop:
- Raid other players to steal resources
- Protect own base from raids
- Risk/reward PvP economy

Star Age core loop (MVP):
- Mine asteroids, complete missions for resources
- Automated colony production
- NPC-only, no player raiding

Decision: Star Age economy is PvE-focused, not raid-based
```

### 8.5 Design Decision Flags - Clash Comparisons

**⚠️ DECISION NEEDED: Building Queue System**
- Should MVP allow multiple simultaneous building upgrades (like Clash builders)?
- **Recommendation**: No for MVP - single upgrade at a time keeps it simple and aligns with "relaxing" vision. Add queues post-MVP if beta feedback wants faster progression.

**⚠️ DECISION NEEDED: Premium Speed-Ups**
- Clash monetizes via instant upgrade completion
- Should we offer this in MVP or post-MVP?
- **Recommendation**: No for MVP - focus on validating core gameplay. Post-MVP can add ethical monetization (cosmetics, convenience, never power).

**⚠️ DECISION NEEDED: Maximum Upgrade Times**
- Clash has 14-day upgrades at max level
- Star Age estimated 8 hours max for MVP
- Is 8 hours too long or too short?
- **Recommendation**: 8 hours max for MVP (aligns with "relaxing" - one overnight upgrade, not multi-day waits). Adjust based on beta feedback.

---

# PART 3: SYNTHESIS & RECOMMENDATIONS

---

## 9. Mechanics to Adopt for MVP

### 9.1 Confirmed Star Age Mechanics (Keep for MVP)

**✅ Core Systems to Implement:**

1. **Real-Time Combat**
   - 8-directional ship movement
   - Continuous weapon fire
   - Weapon ranges create tactical positioning
   - Simple enemy AI (chase, attack, retreat)
   - **Why**: Proven engaging in Star Age, core to game identity

2. **Ship Customization**
   - 7 ship classes (MVP: 4-5 classes)
   - Module slots (weapons, engines, shields, utility)
   - Module tiers (Mk I/II for MVP, Mk III optional)
   - **Why**: Creates build diversity and progression goals

3. **Colony Building**
   - 9 building types with 20 upgrade levels
   - Fixed placement slots (simple to implement)
   - Resource production and storage buildings
   - Planet type bonuses for strategic colony placement
   - **Why**: Core progression loop, proven in Star Age and Clash

4. **Resource Economy**
   - 6 resource types (Metal, Crystals, Ice, Plasma, Fuel, Credits)
   - Automated production via colonies
   - Active gathering via asteroid mining
   - Trading system (basic buy/sell)
   - **Why**: Creates strategic choices and interconnected systems

5. **Galaxy Exploration**
   - 6-8 star systems for MVP
   - 3-8 planets per system
   - Hyperspace travel with fuel costs
   - System difficulty scaling (green/yellow/red)
   - **Why**: Provides exploration gameplay and structured progression

6. **Mission System**
   - 8 mission types (combat, gathering, escort, exploration, delivery, trading, boss, tutorial)
   - Credits, resources, and item rewards
   - Level-gated access
   - **Why**: Guides player progression and provides varied activities

### 9.2 Enhanced Mechanics (Star Age + Improvements)

**✅ Star Age Foundation + Quality Improvements:**

1. **Weapon Variety** (Inspired by FTL)
   ```
   Star Age had: Lasers, Railguns, Plasma, Rockets
   
   Enhancement: Give each weapon distinct tactical role
   - Lasers: High DPS, best vs. unarmored
   - Railguns: Armor penetration, best vs. tanks
   - Plasma: AoE damage, best vs. groups
   - Rockets: Burst damage, best vs. bosses
   
   Result: "Best weapon" depends on situation, encourages variety
   ```

2. **Building Progression** (Inspired by Clash)
   ```
   Star Age had: 20-level upgrades, exponential costs
   
   Enhancement: Multi-resource upgrade costs
   - HQ Level 10→11: 5,000 metal + 2,000 crystals + 10,000 credits
   - Creates strategic choices
   - Prevents single-resource bottleneck
   
   Result: More engaging economy, clearer resource value
   ```

3. **Offline Progression** (Improvement over Star Age social game)
   ```
   Star Age likely had: Real-time waiting (social game mechanic)
   
   Enhancement: Offline progress completion
   - Upgrades take time (1 min to 8 hours)
   - Complete automatically while offline
   - Player collects completed upgrades on login
   
   Result: Respects player time, "relaxing" experience, async "space farm"
   ```

### 9.3 Deferred Mechanics (Post-MVP)

**Post-MVP Additions (Plan Architecture, Implement Later):**

1. **Multiplayer Systems**
   - PvP combat zones
   - Player trading economy
   - Cooperative missions
   - Guild/clan systems
   - **Why defer**: NPC-only validates core gameplay first, multiplayer is complex

2. **Advanced Weapon Types**
   - Ion cannons (shield disruption)
   - Beam weapons (line damage)
   - Subsystem targeting
   - **Why defer**: Core 4-5 weapons sufficient for MVP, add variety post-MVP

3. **Builder Queue System**
   - Multiple simultaneous building upgrades
   - **Why defer**: Simpler single-queue for MVP, add if beta feedback wants it

4. **Reputation System**
   - NPC faction relationships
   - Reputation-based prices and missions
   - **Why defer**: Not essential for core loop validation

5. **Advanced Tutorial**
   - Interactive step-by-step guidance
   - **Why defer**: MVP can launch with basic tutorial missions, refine based on beta confusion points

6. **Random Events**
   - Distress signals, derelict ships, ambushes
   - **Why defer**: Core loops must work first, events add variety later

7. **Environmental Hazards**
   - Asteroid collision damage, nebulas, radiation
   - **Why defer**: Not essential for MVP, adds complexity

### 9.4 MVP Feature Priority List

**Must-Have for MVP (Validation Blockers):**

1. ✅ Real-time ship movement and combat
2. ✅ 4-5 ship classes with module customization
3. ✅ 9 building types with upgrades
4. ✅ 6 resource types and economy
5. ✅ 6-8 star systems with exploration
6. ✅ 8 mission types for guided progression
7. ✅ NPC enemies (pirates, traders, miners)
8. ✅ Colony management on multiple planets
9. ✅ Offline progression and async resource production

**Nice-to-Have for MVP (Enhance but Not Block):**

1. ❓ 3 races (or reduce to 1 for MVP)
2. ❓ Trading system (basic buy/sell)
3. ❓ Boss fight missions
4. ❓ Weapon Mk III tier (or cap at Mk II)
5. ❓ Full 20-level building upgrades (or cap at 15)
6. ❓ Scanner module and planet scanning
7. ❓ Basic tutorial missions (3-5 missions)

**Absolutely Post-MVP:**

1. ❌ Multiplayer PvP
2. ❌ Player trading
3. ❌ Guild systems
4. ❌ Reputation system
5. ❌ Advanced tutorial
6. ❌ Environmental hazards
7. ❌ Random events
8. ❌ Ion/Beam weapons
9. ❌ Subsystem targeting
10. ❌ Builder queues

---

## 10. Post-MVP Expansion Opportunities

### 10.1 Content Expansion (Easy Wins)

**Low-Complexity Additions Post-MVP:**

1. **More Star Systems**
   - Expand from 6-8 to 20-30 systems
   - New planet types (volcanic, ocean, desert moons)
   - New resource nodes (rare crystals, exotic matter)
   - **Effort**: Low - reuse existing systems with new data

2. **More Ship Classes**
   - Add Frigate, Cruiser, Carrier (deferred from MVP)
   - Specialty ships (Repair Ship, Support Ship, Stealth Scout)
   - **Effort**: Medium - reuse module system with new stats

3. **More Modules**
   - Expand from Mk I/II to Mk III
   - New module types (cloaking, jump drive, tractor beam)
   - **Effort**: Low-Medium - reuse module framework

4. **More Missions**
   - Expand from 8 to 15-20 mission types
   - Story-driven mission chains
   - Rare/legendary missions with unique rewards
   - **Effort**: Low-Medium - reuse mission system

5. **Boss Enemies**
   - Unique boss enemies per system
   - Multi-phase boss fights
   - Legendary loot drops
   - **Effort**: Medium - reuse enemy AI with enhanced stats/abilities

### 10.2 System Depth (Medium Complexity)

**Moderate-Complexity Additions Post-MVP:**

1. **Reputation System**
   - Faction relationships (Traders, Miners, Military, Pirates)
   - Reputation affects prices, missions, and NPC behavior
   - **Effort**: Medium - new system but straightforward

2. **Tech Tree / Research**
   - Research Lab building unlocks technology upgrades
   - Permanent bonuses (faster production, better modules)
   - Research points from missions/exploration
   - **Effort**: Medium - new progression system

3. **Crafting System**
   - Combine resources to craft modules
   - Rare modules require blueprints from bosses
   - **Effort**: Medium - extends resource economy

4. **Ship Crews**
   - Hire crew members with special abilities
   - Crew levels up with ship usage
   - Crew bonuses (faster mining, better combat, fuel efficiency)
   - **Effort**: Medium-High - new system with UI requirements

5. **Dynamic Economy**
   - Resource prices fluctuate based on supply/demand
   - Trading arbitrage becomes more profitable
   - **Effort**: Medium - extends trading system

### 10.3 Multiplayer Features (High Complexity)

**High-Complexity Post-MVP (Investment Required):**

1. **PvP Combat Zones**
   - Specific systems designated as PvP
   - Safe zones remain for PvE-focused players
   - Loot drops on player kill
   - **Effort**: High - netcode, balancing, griefing prevention

2. **Cooperative Missions**
   - 2-4 players team up for harder missions
   - Boss fights designed for groups
   - Shared rewards
   - **Effort**: High - multiplayer mission system

3. **Player Trading**
   - Direct player-to-player resource/module trading
   - Marketplace interface
   - Economy balancing
   - **Effort**: Medium-High - new UI + economy implications

4. **Guilds/Clans**
   - Form guilds, shared resources
   - Guild bases, guild missions
   - Leaderboards and rankings
   - **Effort**: High - new social systems

5. **Territory Control**
   - Guilds compete for system ownership
   - Bonuses for controlling resource-rich systems
   - Territory wars
   - **Effort**: Very High - new game mode essentially

---

## 11. Design Decision Summary

### 11.1 Critical Decisions Required Before Implementation

**User must decide on these before Step 6 (MVP Requirements):**

1. **Ship Class Count for MVP**
   - Option A: 4 classes (Scout, Fighter, Destroyer, Miner)
   - Option B: 5 classes (+ Frigate)
   - Option C: 7 classes (all from Star Age)
   - **Recommendation**: Option A or B - saves development time

2. **Race Count for MVP**
   - Option A: 1 race (Terrans only)
   - Option B: 3 races (full Star Age)
   - **Recommendation**: Option A - defer races to post-MVP

3. **Building Queue System**
   - Option A: Single upgrade at a time (simpler)
   - Option B: Multiple simultaneous upgrades (Clash-style builders)
   - **Recommendation**: Option A - aligns with "relaxing" vision

4. **Weapon Types**
   - Option A: 4-5 weapons (Star Age original)
   - Option B: + Ion weapons (FTL-inspired)
   - Option C: + Beam weapons (FTL-inspired)
   - **Recommendation**: Option A - add variety post-MVP if needed

5. **Subsystem Targeting**
   - Option A: Simple hull damage only
   - Option B: Tactical subsystem targeting (FTL-inspired)
   - **Recommendation**: Option A - keep combat simple for MVP

6. **Trading System**
   - Option A: Basic buy/sell at fixed prices
   - Option B: Dynamic pricing and arbitrage
   - **Recommendation**: Option A - simpler for MVP

7. **Tutorial Approach**
   - Option A: No tutorial, rely on UI tooltips
   - Option B: 3-5 basic tutorial missions
   - Option C: Full interactive tutorial
   - **Recommendation**: Option B - minimal tutorial for beta testing

8. **Maximum Building Upgrade Level**
   - Option A: 20 levels (Star Age original)
   - Option B: 10-15 levels (reduced for MVP)
   - **Recommendation**: Option A - 20 levels provides depth

9. **Resource Production Time**
   - Option A: Fast (full storage in 2-4 hours)
   - Option B: Medium (full storage in 6-8 hours)
   - Option C: Slow (full storage in 12-24 hours)
   - **Recommendation**: Option B - log in 1-2 times per day

10. **Death/Failure Mechanics**
    - Option A: Low penalty (respawn, small repair cost)
    - Option B: Medium penalty (lose cargo, moderate repair)
    - Option C: High penalty (lose ship modules, expensive repair)
    - **Recommendation**: Option A - aligns with "relaxing" vision

### 11.2 Formulas Requiring Specific Numbers

**Game Systems Design (Step 7) must define these:**

1. **Combat Damage**
   ```
   Need specific values for:
   - Weapon base damage (Laser: 10 DPS, Railgun: 25 DPS, etc.)
   - Armor reduction (Light: -2, Heavy: -5, etc.)
   - Enemy HP by type and difficulty
   - Player HP by ship class
   ```

2. **Resource Production**
   ```
   Need specific values for:
   - Base production rates (Mine Level 1: 50 metal/hour, etc.)
   - Production scaling (Level ^ 1.3 multiplier, etc.)
   - Storage capacity (Warehouse Level 1: 500 units, etc.)
   - Storage scaling (Level ^ 1.5 multiplier, etc.)
   ```

3. **Ship Stats**
   ```
   Need specific values for:
   - Ship HP (Scout: 75, Fighter: 175, etc.)
   - Ship speed (Scout: 200 px/s, Fighter: 150 px/s, etc.)
   - Ship cargo (Scout: 75, Miner: 1000, etc.)
   - Ship fuel tanks (Scout: 100, Destroyer: 300, etc.)
   ```

4. **Upgrade Costs**
   ```
   Need specific values for:
   - Building upgrade cost formula (Base × Growth^Level)
   - Ship purchase costs (Scout: 1000 credits, Fighter: 5000, etc.)
   - Module costs (Mk I: 500, Mk II: 2000, Mk III: 8000)
   ```

5. **Progression Rates**
   ```
   Need specific values for:
   - XP required per level (Base × Level^1.8)
   - XP rewards from missions (500-10,000 range)
   - Credits from missions (1000-50,000 range)
   ```

**Note**: These formulas have been estimated in this document based on reverse-engineering Star Age. User can approve estimates or adjust during Step 7.

### 11.3 MVP Scope Validation Checklist

**Before finalizing MVP requirements (Step 6), verify:**

- [ ] Ship class count decided (4, 5, or 7)
- [ ] Race count decided (1 or 3)
- [ ] Weapon type count decided (4-5 or more)
- [ ] Building count finalized (9 types confirmed)
- [ ] Galaxy size finalized (6-8 systems confirmed)
- [ ] Mission type count finalized (8 types recommended)
- [ ] Resource type count finalized (6 types confirmed)
- [ ] Module tier count decided (Mk I/II or I/II/III)
- [ ] Building max level decided (20 levels recommended)
- [ ] Tutorial inclusion decided (3-5 basic missions recommended)
- [ ] Trading system scope decided (basic buy/sell recommended)
- [ ] Building queue system decided (single-queue recommended)
- [ ] Death penalty decided (low penalty recommended)

**Scope Warning**:
If too many "maximum" options chosen, development timeline may exceed 12 months even with AI tools. Prioritize ruthlessly.

---

## 12. Implementation Roadmap Preview

### 12.1 System Implementation Order (For Step 14-15)

**Suggested Build Order (Most to Least Foundation):**

**Phase 1: Core Systems**
1. Galaxy map and system navigation
2. Ship movement and controls
3. Basic combat (one weapon type, basic enemy)
4. Resource system (track resources)
5. Planet landing and colony view

**Phase 2: Colony Management**
6. Building placement system
7. Building upgrades (basic formula)
8. Resource production and collection
9. Storage limits and overflow

**Phase 3: Ship Progression**
10. Ship class system (implement 1-2 classes)
11. Module slot system
12. Ship customization UI
13. Module effects on stats

**Phase 4: Content**
14. Additional ship classes (complete 4-5)
15. Additional weapon types (4-5 total)
16. NPC enemies (pirates, traders, miners)
17. Combat AI behaviors
18. Mission system (implement mission types)

**Phase 5: Economy & Progression**
19. Trading system (buy/sell)
20. Mission rewards
21. XP and leveling
22. Unlock gates (ship/module unlocks)

**Phase 6: Polish & Balance**
23. Visual effects (explosions, lasers)
24. UI polish
25. Sound effects (post-MVP but noted)
26. Balance tuning (damage, costs, progression)
27. Bug fixing

**Phase 7: Beta Testing**
28. Deploy to web hosting
29. Recruit beta testers
30. Collect feedback
31. Iterate on balance and UX

### 12.2 AI Tool Usage Strategy

**For Antigravity AI (or Cursor) - How to Use This Document:**

**Step-by-Step Prompting:**

```
1. Reference Section 1 (Combat) for combat system implementation
   - Prompt: "Implement combat system as specified in Section 1.7"
   - Provide: Formulas, state management code, AI behavior tree

2. Reference Section 2 (Ships) for ship customization
   - Prompt: "Implement ship class system as specified in Section 2.6"
   - Provide: Ship definitions, module system, stat calculations

3. Reference Section 3 (Buildings) for colony management
   - Prompt: "Implement colony building system as specified in Section 3.8"
   - Provide: Building definitions, upgrade formulas, production calculations

4. Reference Section 4 (Economy) for resource system
   - Prompt: "Implement resource management as specified in Section 4.6"
   - Provide: Resource definitions, trading system, mining mechanics

5. Reference Section 5 (Exploration) for galaxy map
   - Prompt: "Implement galaxy map and travel as specified in Section 5.7"
   - Provide: Galaxy structure, travel mechanics, minimap rendering

6. Reference Section 6 (Missions) for progression
   - Prompt: "Implement mission system as specified in Section 6.7"
   - Provide: Mission templates, objectives, progression formulas
```

**Critical**: Each section includes "Implementation Notes" with:
- JavaScript code examples
- Data structures
- Formulas with variables
- UI requirements

These are specifically formatted for AI consumption during implementation phase.

### 12.3 Testing Strategy Preview

**MVP Validation Metrics (for Step 16):**

1. **Core Loop Engagement**
   - Do players complete 5+ sessions in beta?
   - Average session length 20-60 minutes?
   - Do players return after 24-48 hours?

2. **Play Style Flexibility**
   - Do players switch between building, exploring, combat?
   - Or do they stick to one activity?
   - Are all ship classes used or just one?

3. **Progression Satisfaction**
   - Do players feel progress in 30-60 minute sessions?
   - Do upgrade costs feel balanced or grindy?
   - Do players hit walls or progress smoothly?

4. **Technical Performance**
   - Does combat run at 30+ FPS with 20 ships?
   - Any browser compatibility issues?
   - Any game-breaking bugs?

5. **NPC-Only Viability**
   - Do players find NPC combat engaging?
   - Do missions feel repetitive?
   - Do players ask for multiplayer immediately?

**Success Criteria** (from Assumption A3.7):
- 50%+ of beta testers play 5+ sessions
- Average session 20-45 minutes
- Day 7 retention 30%+
- NPS score >30

---

## Document Summary

### What This Research Provides

**✅ Comprehensive Star Age Documentation:**
- All core systems reverse-engineered from video
- Combat, ships, buildings, resources, exploration, missions
- Formulas estimated where possible
- Design decision flags throughout

**✅ Strategic Comparisons:**
- FTL weapon variety analysis
- Clash of Clans building progression analysis
- Specific mechanics to borrow or avoid
- User decision points highlighted

**✅ Implementation-Ready Specs:**
- Code examples in every system section
- Formulas with variables for tuning
- Data structures for AI generation
- UI requirements documented

**✅ MVP Scope Guidance:**
- Must-have vs. nice-to-have features
- Post-MVP expansion opportunities
- Critical design decisions summarized
- Validation metrics defined

### Next Steps

**Immediate Actions:**

1. **User Reviews This Document**
   - Approve/adjust estimates and recommendations
   - Make critical design decisions (Section 11.1)
   - Flag any Star Age inaccuracies

2. **Step 5: Core Game Loop**
   - Use this research to design specific gameplay loops
   - Define 15-min, 30-min, 60-min session flows
   - Reference Section 9 (MVP features)

3. **Step 6: MVP Requirements**
   - Lock final feature list using Section 9.4
   - Reference design decisions from Section 11.1
   - Scope must fit 6-12 month timeline

4. **Step 7: Game Systems Design**
   - Use formulas from this document as starting point
   - Finalize all numerical values (Section 11.2)
   - Create balance spreadsheet

5. **Step 8: Technical Architecture**
   - Reference implementation notes throughout
   - Use code examples as technical specifications
   - Plan asset integration workflow (Assumption A4.8)

**This research document serves as foundation for ALL remaining design steps.**

---

**Document Version:** 1.0  
**Created:** November 22, 2025  
**Status:** Complete - Awaiting User Review  
**Next Step:** Core Game Loop (Step 5) - Use this research to design specific gameplay flows  
**Total Sections:** 12 main sections + 3 synthesis sections  
**Total Words:** ~15,000  
**Implementation-Ready:** Yes - Code examples and formulas throughout
