# Core Assumptions - UPDATE from Q&A Session
## November 24, 2025 - All Critical Questions Answered

---

## Status Changes from Q&A

### VALIDATED Assumptions (Changed from Unvalidated → Validated)

**A3.2: Real-Time Combat**
- **Status:** ✅ VALIDATED
- **Confirmed:** Pure real-time combat, no tactical pause
- **Impact:** MVP combat design proceeds with real-time mechanics
- **Risk:** Remains Medium - balancing difficulty without pause is challenge

**A3.3: Colony Building Depth**
- **Status:** ✅ VALIDATED  
- **Confirmed:** 9 building types sufficient, no building level caps
- **Clarification:** Population is a resource that enables production
- **Impact:** Simpler progression system, clearer for MVP

**A3.6: Economy System Engagement**
- **Status:** ✅ VALIDATED
- **Confirmed:** Credits-based economy with auction/warehouse
- **Clarification:** All trading mediated through credits, no direct player barter in MVP
- **Impact:** Simpler economic system for MVP

**A3.7: NPC-Only MVP Sufficient**
- **Status:** ✅ VALIDATED
- **Confirmed:** Pure PvE for MVP, no PvP
- **Impact:** Reduced scope, focus on core loop validation

**A3.14: Asynchronous Mechanics**
- **Status:** ✅ VALIDATED
- **Confirmed:** Buildings produce resources over 4-6 hours, stop at 100% capacity
- **Clarification:** Offline progression model, players check in 1-2x per day
- **Impact:** "Space farm" mechanic confirmed as core design

---

## NEW Clarifications from Q&A

### Colony Building Mechanics

**Building Prerequisites:**
- ❌ NO building level caps (original assumption was wrong)
- Buildings can be upgraded independently
- Only limit is Headquarters unlocks which buildings can be built
- **Impact:** Simpler progression, more player freedom

**Population System:**
- Population is a RESOURCE, not just a stat
- Required for production (e.g., "1 population needed to mine ice")
- Produced by specific buildings:
  - Hospitals (universal)
  - Barracks (military planets)
  - Restaurants (agrarian planets)
- **Impact:** New resource type to track, enables strategic building choices

**Resource Transportation:**
- Manual cargo hauling confirmed (load → fly → unload)
- Remote access to resources for building upgrades
- If resource missing at Colony A but available at Colony B, must transport manually
- Post-MVP: Can create delivery missions for other players
- **Impact:** Creates active gameplay loop, strategic cargo management

### Planet Type System

**Hard Resource Restrictions by Planet Type:**

| Planet Type | Unique Resource | Building Appearance |
|-------------|----------------|---------------------|
| Agrarian (Earth-like) | Food | Restaurants |
| Military (Volcanic) | Uranium | Barracks |
| Mining (Gas Giant) | Ore | Industrial |
| Industrial (Green) | Elderium | Factories |
| Scientific (Desert) | Electronics | Labs |

**Universal Resources:**
- Ice: Available everywhere (destroy comets in space)
- Stone: Available everywhere (destroy asteroids in space)

**Resource Production Chain:**
```
TIER 1 (Base):
- Ice (space) + Stone (space) → everywhere
- Food, Uranium, Ore, Elderium, Electronics → planet-specific

TIER 2 (Refined):
- Metal = Stone (Factory)
- Fuel = Uranium + Ice
- Rubidium = Ore + Uranium
- Medications = Electronics + Elderium
- Animals = Food processed

TIER 3 (Advanced):
- (To be determined in Step 7)
```

**Impact:** Multi-colony strategy is MANDATORY, not optional. Cannot produce all resources from single colony.

### Resource Gathering Mechanics

**Combat-Based Resource Gathering:**
- Players CANNOT use Miner-class ships (NPCs only)
- Resource gathering = destroy objects with weapons:
  - Asteroids → drop Stone
  - Comets → drop Ice
- Cargo capacity limits collection
- Must return to colony to unload
- **Impact:** Resource gathering uses same mechanics as combat, keeps gameplay unified

### Ship System Specifications

**Player Ship Classes (MVP):**

| Ship Class | Weapon Slots | Module Slots | Role |
|------------|--------------|--------------|------|
| Scout | 2 | 1 | Exploration, speed |
| Fighter | 3 | 2 | Early combat |
| Destroyer | 4 | 4 | Heavy combat |

**Module Types:**
- Weapons (lasers, railguns, plasma, rockets)
- Engines (speed, fuel efficiency)
- Shields (energy absorption)
- Armor (damage reduction)
- Cargo (capacity expansion)
- Utility (scanner, radar, repair, afterburner)

**Starting Conditions:**
- Ship: Scout (2 weapons, 1 module)
- Credits: 1,000
- Resources: 5 Stone, 5 Ice
- Colony: 1 starting colony

### Galaxy Structure (MVP)

**6 Star Systems Total:**
- 3-5 planets per system
- Some systems are specific biomes (e.g., military system with mostly volcanic planets)
- Progression: Green (safe) → Yellow (medium) → Red (dangerous)

**Estimated Structure:**
```
System 1-2 (Tutorial/Safe):
- Mixed planet types
- Low-level NPCs
- Starting area

System 3-4 (Mid-Level):
- Specialized biomes
- Medium NPCs
- Strategic resource locations

System 5-6 (High-Level):
- Rare planet types
- Boss NPCs
- Endgame resources
```

### Mission System

**Mission Generation:**
- Procedurally generated (infinite variety)
- All missions are repeatable
- 8 mission types: Combat, Gathering, Escort, Exploration, Delivery, Trading, Boss, Tutorial
- Rewards: Credits, resources, items, reputation

**Player Progression:**
- Combination of factors determines level:
  - Experience from missions/combat
  - Headquarters level
  - Ship class owned
  - Total resources gathered

**Leveling Unlocks:**
- New star systems
- New ship classes
- New buildings
- Higher-tier equipment

### NPC Behavior Complexity

**Pirates:**
- Medium complexity
- Chase → Attack → Retreat when low HP → Call reinforcements
- **Impact:** Requires tactical positioning, escape mechanics

**Miners:**
- Simple behavior
- Stationary at asteroids
- Flee if attacked
- **Impact:** Easy targets for resource theft, creates moral choices

**Bosses:**
- Simple but powerful
- High stats, basic attack patterns
- **Impact:** Gear check more than skill check

### Economy & Progression

**Credit Economy:**
- All trading mediated through credits
- No direct player-to-player resource barter in MVP
- Auction/Warehouse for selling resources

**Storage & Production:**
- Production fills storage in 4-6 hours
- Storage stops at 100% (wastes time if not collected)
- Encourages 2-3 check-ins per day
- Early upgrades: 1-2 hours
- Late upgrades: Max 24 hours

### Visual Style

**UI/UX Direction:**
- Replicate Star Age layout and logic
- English text, Latin characters
- Modern polish acceptable if it doesn't change structure
- Buildings differ by level NUMBER only (no visual changes in MVP)

**Perspective:**
- Space: Top-down orthographic (2D shooter view)
- Colony: Isometric buildings on 2D surface

---

## Updated MVP Scope (LOCKED)

### IN SCOPE for MVP:
- ✅ 6 star systems (3-5 planets each = ~24 colonizable planets)
- ✅ 3 player ship classes (Scout, Fighter, Destroyer)
- ✅ 1 race (Terrans only)
- ✅ Pure PvE (no PvP mechanics)
- ✅ 9 building types with 20 levels each
- ✅ 6 base resources + 6 refined resources
- ✅ Procedurally generated missions (infinite)
- ✅ Manual cargo hauling gameplay
- ✅ Combat-based resource gathering (shoot asteroids/comets)
- ✅ Credits-based economy with auction
- ✅ Population as enabling resource
- ✅ Planet-type restrictions create multi-colony strategy
- ✅ Real-time combat (no pause)
- ✅ NPC enemies (pirates, miners, bosses)

### OUT OF SCOPE for MVP (Post-MVP):
- ❌ Multiple races (Zenitar, Khrax)
- ❌ PvP combat
- ❌ Player-to-player trading
- ❌ Clan systems
- ❌ Additional ship classes (Carrier, Cruiser, Miner)
- ❌ More than 6 star systems
- ❌ Dynamic economy (fixed prices for MVP)
- ❌ Player-created delivery missions
- ❌ Advanced NPC behaviors
- ❌ Visual building progression (same art per building)

---

## Critical Design Decisions Still Needed (For Step 7)

These don't block Step 5 (Core Game Loop) but will need resolution in Step 7:

**Combat Balance:**
- Exact damage formulas
- Weapon range values
- Ship HP scaling
- NPC difficulty curves

**Economic Balance:**
- Resource production rates (units per hour)
- Auction prices (credits per resource)
- Ship costs (credits per ship class)
- Module costs

**Progression Balance:**
- Experience gain rates
- Level requirements for unlocks
- Building upgrade costs
- Building upgrade times

---

## Assumptions Now READY for Step 5

With all clarifications complete, we can confidently design:

**Micro-Loop (5-15 minutes):**
- Collect colony resources
- Quick combat/resource gathering
- Start building upgrades
- Complete easy missions

**Session Loop (15-60 minutes):**
- Combat missions
- Resource gathering runs
- Colony management
- Ship/module upgrades
- Exploration

**Async Loop (Offline):**
- Buildings produce resources
- Storage fills over 4-6 hours
- Player returns to collect

**Meta Loop (Days/Weeks):**
- Colony network expansion
- Ship class progression
- System-by-system advancement
- Long-term upgrades

---

**Status:** ALL CRITICAL ASSUMPTIONS VALIDATED  
**Next Step:** Step 5 - Core Game Loop Design  
**Date:** November 24, 2025  
**Confidence Level:** 9.5/10 - Ready to proceed
