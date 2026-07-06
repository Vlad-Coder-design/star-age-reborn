# Core Game Loop
## Space MMO Browser Game Project

---

## Executive Summary

**Core Philosophy:** The game supports **mood-based play** where players choose their activity based on available time and current interest. Four distinct modes (Building, Exploring, Trading, Combat) are equally viable and interconnect naturally. Sessions can be as short as 5 minutes or as long as 60+ minutes, with meaningful progression at all time scales.

**Key Design Principle:** *"Come for 5 minutes or stay for an hour - both feel complete."*

**Loop Architecture:**
- **Micro-Loop (5-15 min):** Quick check-in, single focused activity, satisfying completion
- **Session Loop (15-60 min):** Deep engagement in chosen mode, visible progress
- **Async Loop (4-6 hours offline):** Resource production, building upgrades continue
- **Meta Loop (Days/weeks):** Ship progression, colony network, system unlocking

**Success Metric:** Player can stop playing at any moment without feeling like they "wasted time" or "need to finish something."

---

## Table of Contents

1. [The Four Play Modes](#the-four-play-modes)
2. [Loop Breakdown by Time](#loop-breakdown-by-time)
3. [Session Flow & Navigation](#session-flow--navigation)
4. [Mode Interconnections](#mode-interconnections)
5. [Progression Systems](#progression-systems)
6. [Return Hooks & Engagement](#return-hooks--engagement)
7. [Implementation Guidelines](#implementation-guidelines)

---

## The Four Play Modes

### 1. Building Mode 🏗️

**Core Activity:** Colony optimization and resource management

**What Players Do:**
- Collect produced resources from buildings
- Start/queue building upgrades
- Plan colony layouts and specialization
- Manage resource storage and production chains
- Expand to new colonies on strategic planets

**Time Investment:**
- **5 min:** Collect resources, start upgrades, check production
- **15 min:** Plan next expansion, optimize production chains
- **30 min:** Establish new colony, manage multi-colony network
- **60 min:** Deep optimization, multiple colonies, production balancing

**Rewards:**
- Increased resource production rates
- Unlocked buildings and capabilities
- Strategic advantages (faster ships via better engines, etc.)
- Economic power (more resources = more credits)

**Interconnections:**
- Enables Trading (need resources to sell)
- Enables Combat (need resources for ship upgrades)
- Requires Exploring (need new colonies for planet-specific resources)

---

### 2. Exploring Mode 🗺️

**Core Activity:** Discovery and galaxy navigation

**What Players Do:**
- Travel to new star systems via hyperspace
- Scout planets for colony potential
- Discover resource-rich asteroid fields
- Map safe/dangerous zones
- Unlock access to new areas

**Time Investment:**
- **5 min:** Quick scout of nearby planet
- **15 min:** Explore new system, identify colonizable planets
- **30 min:** Deep exploration of 2-3 systems
- **60 min:** Long-range expedition to distant systems

**Rewards:**
- New systems unlocked for colonization
- Discovery of rare planet types (scientific, industrial)
- Knowledge of resource locations
- Fuel-efficient travel routes mapped

**Interconnections:**
- Enables Building (find planets to colonize)
- Enables Trading (discover price differences between systems)
- Leads to Combat (encounter pirates in new areas)

---

### 3. Trading Mode 💰

**Core Activity:** Economic optimization and credit accumulation

**What Players Do:**
- Sell excess resources at auction/warehouse
- Buy resources needed for upgrades
- Run cargo between colonies
- Optimize trade routes for profit
- Accumulate credits for ship purchases

**Time Investment:**
- **5 min:** Sell full storage, buy immediate needs
- **15 min:** Run cargo delivery between colonies
- **30 min:** Multiple trade runs, optimize routes
- **60 min:** Full economic cycle, major purchases

**Rewards:**
- Credits for ship/module purchases
- Resource stockpiles for upgrades
- Economic self-sufficiency
- Funding for expansion

**Interconnections:**
- Requires Building (produce resources to sell)
- Requires Exploring (access to multiple systems for trading)
- Enables Combat (credits buy better ships/modules)

---

### 4. Combat Mode ⚔️

**Core Activity:** Mission completion and active resource gathering

**What Players Do:**
- Accept and complete procedural missions
- Hunt pirates for credits and loot
- Gather resources by destroying asteroids/comets
- Test ship builds and tactics
- Progress through difficulty tiers

**Time Investment:**
- **5 min:** Quick resource gathering run (destroy asteroids)
- **15 min:** Complete 1-2 combat missions
- **30 min:** Extended mission chain, multiple encounters
- **60 min:** Boss attempt, difficult mission series

**Rewards:**
- Credits and resources from missions
- Equipment drops (modules, weapons)
- Gathered resources (stone from asteroids, ice from comets)
- Mission unlocks and reputation

**Interconnections:**
- Requires Building (need ship upgrades from colony resources)
- Requires Trading (need credits to buy better ships)
- Requires Exploring (missions in new systems)

---

## Loop Breakdown by Time

### Micro-Loop: 5-Minute Session

**"The Coffee Break Check-In"**

**Entry:** Player loads game at last location (colony or space)

**If at Colony:**
```
1. Check resource storage (2-5 seconds)
   → Storage full? Visual indicator shows "FULL"
   
2. Collect resources (10-15 seconds)
   → Click collection button
   → Resources transfer to inventory
   → Satisfying collection animation/sound
   
3. Check building upgrades (5-10 seconds)
   → Any upgrades complete? Visual indicator
   → If yes: Collect completed building
   
4. Start new upgrades (20-30 seconds)
   → Click building → Upgrade button
   → Review cost (auto-shows if resources available)
   → Confirm upgrade
   → Timer starts (visible countdown)
   
5. Exit satisfied (5 seconds)
   → Everything set in motion
   → Progress will continue offline
   → Natural stopping point achieved
```

**If in Space:**
```
1. Assess situation (2-5 seconds)
   → Nearby asteroids? Pirates? Planet?
   
2. Quick activity (2-3 minutes)
   → Destroy 3-5 asteroids for stone
   OR
   → Quick combat with single pirate
   OR
   → Fly to nearby planet and land
   
3. Collect loot/resources (10 seconds)
   → Auto-pickup or manual collection
   
4. Return to colony OR log out in space (10 seconds)
   → If cargo full: Return to colony
   → If cargo not full: Safe to log out
```

**Result:** Player accomplished something concrete in 5 minutes:
- ✅ Resources collected
- ✅ Upgrades started
- ✅ Resources gathered in space
- ✅ Progress continues offline

---

### Session Loop: 15-30 Minute Session

**"The Lunch Break Play"**

**Mode Selection:** Player chooses activity based on mood

#### Building Focus (15-30 min)

```
1. Colony Check-In (2 min)
   → Collect all resources from all colonies
   → Review storage capacity
   
2. Optimization Planning (3-5 min)
   → Which buildings to upgrade next?
   → Do I have required resources?
   → If not: Need to gather/trade
   
3. Multi-Colony Management (5-10 min)
   → Switch between colonies (house button + colony selector)
   → Check production rates
   → Identify bottlenecks
   → Plan specialization strategy
   
4. Expansion Decision (5-10 min)
   → Do I need new colony?
   → Check map for planet types
   → If yes: Travel to planet, establish colony
   → If no: Continue optimization
   
5. Resource Gathering (if needed) (5 min)
   → Quick space run to gather missing resources
   → Return to colony, start upgrades
   
6. Exit Point (1 min)
   → All upgrades queued
   → Clear next steps identified
   → Satisfying progress made
```

#### Exploring Focus (15-30 min)

```
1. Launch Preparation (2 min)
   → At colony: Check fuel supply (need 1+ fuel for hyperspace)
   → Load cargo hold with essentials
   → Launch from planet (click ship → flies to space)
   
2. System Travel (3-5 min)
   → Open galaxy map (map button)
   → Select destination system
   → Confirm fuel cost
   → Hyperspace jump (travel animation)
   
3. System Exploration (10-20 min)
   → Arrive at new system
   → Scan planets (visual inspection)
   → Identify planet types:
     • Agrarian (food)
     • Military (uranium)
     • Mining (ore)
     • Industrial (elderium)
     • Scientific (electronics)
   → Check asteroid fields
   → Note NPC activity (miners, pirates)
   
4. Colony Planning (2-5 min)
   → Found good planet for specialization?
   → Land and establish colony (if HQ level allows)
   → Or: Note location for future
   
5. Return Journey (3 min)
   → Open map → Select home system
   → Hyperspace back
   → Land at home colony
```

#### Trading Focus (15-30 min)

```
1. Resource Assessment (2 min)
   → Check all colony storage
   → Identify excess resources
   → Identify needed resources
   
2. Sales Run (5 min)
   → At colony: Open auction/warehouse
   → Sell excess resources for credits
   → Note: Ice = 2 credits, Stone = 3 credits, etc.
   
3. Cargo Haul (10-15 min)
   → Need resource from Colony B at Colony A?
   → Load cargo at Colony B
   → Fly to Colony A (map → system → planet)
   → Unload cargo
   → Efficient route planning
   
4. Purchases (5 min)
   → At auction: Buy needed resources
   → Or: Save credits for ship purchase
   
5. Economic Review (2-3 min)
   → Current credit total
   → Can afford next ship class? (Fighter = ~5,000 credits?)
   → Progress toward economic goals visible
```

#### Combat Focus (15-30 min)

```
1. Mission Selection (2 min)
   → Open mission interface
   → Review available missions:
     • Combat: "Destroy 5 pirates in System X"
     • Gathering: "Collect 20 stone"
     • Escort: "Protect NPC trader"
   → Accept mission matching current mood
   
2. Preparation (1-2 min)
   → Check ship weapons/modules
   → Ensure ammo/energy ready
   → Launch from colony
   
3. Mission Execution (10-20 min)
   → Travel to mission location
   → Engage in combat:
     • Real-time movement (8 directions)
     • Continuous weapon fire
     • Tactical positioning (weapon ranges matter)
   → Destroy pirates OR gather resources OR escort NPC
   
4. Loot Collection (1-2 min)
   → Pirates drop credits, modules, resources
   → Auto-collect or manual pickup
   → Cargo management
   
5. Mission Completion (2 min)
   → Return to mission giver (or auto-complete)
   → Receive rewards:
     • Credits
     • Experience
     • Resources
     • Equipment
   → Next mission immediately available
   
6. Ship Upgrade (optional, 3-5 min)
   → If earned credits/modules:
   → Return to colony
   → Install new modules (drag-and-drop)
   → Test improved build
```

---

### Extended Session: 60+ Minute Session

**"The Weekend Gaming Session"**

**Multimodal Approach:** Players naturally flow between modes

**Example Session Flow:**

```
Hour 1: Building & Planning (0-20 min)
├─ Log in at home colony
├─ Collect all resources from 3 colonies
├─ Review production chains
├─ Start 5-6 building upgrades across colonies
├─ Identify bottleneck: Need uranium (only on military planets)
└─ Decision: Must establish military colony

Hour 1: Exploration (20-40 min)
├─ Launch from home planet
├─ Open galaxy map
├─ Travel to System 4 (known military system)
├─ Scout 4 planets, find volcanic planet with good slots
├─ Land, establish new colony (costs credits)
├─ Build initial buildings: Mine, Power Station, Warehouse
└─ Set production in motion

Hour 1: Combat & Gathering (40-60 min)
├─ While in System 4, notice pirates
├─ Accept combat mission: "Destroy 3 pirates"
├─ Engage in combat (15 min)
├─ Collect pirate loot (modules, credits)
├─ Destroy asteroids for stone (5 min)
├─ Cargo full: Return to new military colony
└─ Unload resources

Hour 2: Trading & Economy (60-75 min)
├─ Uranium produced at military colony
├─ Load cargo with uranium
├─ Hyperspace to home system
├─ Unload uranium at home colony
├─ Sell excess stone/ice at auction
├─ Credits earned: 500 → 1,500
└─ Can afford better modules!

Hour 2: Ship Upgrade (75-85 min)
├─ At home colony: Open shipyard
├─ Install new modules with earned credits:
  ├─ Better engine (increased speed)
  ├─ Shield module (energy absorption)
  └─ Upgraded weapon (higher damage)
├─ Test new build in space
└─ Noticeably more powerful

Hour 2: Next Goals (85-90 min)
├─ Check progress toward Fighter ship (need 5,000 credits)
├─ Review mission list for high-value targets
├─ Plan next colonization (need Scientific planet for electronics)
├─ Set long-term upgrades in motion
└─ Natural stopping point: Everything queued, goals clear
```

**Result After 90 Minutes:**
- ✅ 4 colonies established and producing
- ✅ 10+ building upgrades completed or in progress
- ✅ Ship upgraded with better modules
- ✅ 1,500+ credits earned
- ✅ Clear progression toward Fighter ship
- ✅ Multiple systems explored
- ✅ Satisfying sense of empire growth

---

### Async Loop: Offline Progression

**"The Space Farm Mechanic"**

**What Happens While Player Is Offline:**

```
BUILDINGS PRODUCE RESOURCES
├─ Mine Level 5: Produces 100 metal/hour
├─ Power Station Level 5: Produces energy
├─ Warehouse Level 5: Storage capacity 2,000 units
└─ Production continues until storage full

TIMELINE:
Hour 1: Storage at 100/2,000
Hour 2: Storage at 200/2,000
Hour 3: Storage at 300/2,000
...
Hour 4-6: Storage reaches 2,000/2,000 (FULL)
After Hour 6: Production STOPS (wasting time)

BUILDING UPGRADES COMPLETE
├─ Mine Level 5→6: Started 2 hours ago, completes 18 hours from start
├─ Power Station Level 5→6: Started 30 min ago, completes 1.5 hours from start
└─ Visual indicators show "COMPLETE" when player returns

OPTIMAL CHECK-IN FREQUENCY
├─ If storage fills in 4-6 hours
├─ Player should check in 2-3 times per day
└─ Morning: Collect, start upgrades
    Lunch: Collect, adjust production
    Evening: Collect, plan next day
```

**Return Hooks (Why Player Comes Back):**

1. **Storage Full** 🟢
   - Visual: Flashing "FULL" indicator
   - Urgency: "I'm wasting production time"
   - Satisfaction: Big collection = big numbers

2. **Upgrade Complete** 🟢
   - Visual: "READY" indicator on building
   - Anticipation: "Can start next upgrade"
   - Progression: Visible power increase

3. **Mission Availability** 🟡
   - New missions refresh periodically
   - Higher-value targets appear
   - Boss spawn notifications

4. **Economic Goals** 🟡
   - "Close to affording Fighter ship"
   - "Almost enough for new colony"
   - Progress bar toward major purchase

5. **Intrinsic Interest** 🟢
   - "Wonder what my colonies produced"
   - "Ready to explore new system"
   - "Want to try new ship build"

---

## Session Flow & Navigation

### Starting a Session

**Player loads game → Loads at last location**

```
IF last_location == "colony":
    LOAD colony_view
    SHOW resource_indicators (flashing if storage full)
    SHOW building_status (timers, complete indicators)
    SHOW house_button (left side, always visible)
    
IF last_location == "space":
    LOAD space_view at last_coordinates
    SHOW nearby_objects (asteroids, planets, NPCs)
    SHOW ship_status (HP, cargo, fuel)
    SHOW minimap (local area)
```

### Navigation System

**Core Navigation Mechanic:**

```
COLONY VIEW:
├─ House Button (left side): Already at colony, shows colony interface
├─ Ship Icon (on planet surface): Click to launch to space
└─ Map Button: Opens galaxy map for hyperspace travel

SPACE VIEW:
├─ House Button (left side): Opens colony interface (remote access)
├─ Map Button: Opens galaxy map for hyperspace travel
├─ Click Planet: Approach and land on planet
└─ WASD/Arrow Keys: Direct ship movement

GALAXY MAP:
├─ Shows all star systems
├─ Indicators: Green (safe), Yellow (medium), Red (dangerous)
├─ Click system: Select destination
├─ Fuel Cost Shown: "1 Fuel Required"
└─ Confirm: Hyperspace jump (load animation)
```

### Mode Transitions

**Building → Exploring:**
```
1. At colony, collecting resources
2. Decision: "Need to find Scientific planet for electronics"
3. Click Ship icon → Launch animation → Space view
4. Click Map button → Galaxy map opens
5. Select unexplored system → Hyperspace jump
6. Arrive, scout planets, identify Scientific planet
7. Return to colony OR establish new colony
```

**Exploring → Combat:**
```
1. In space, scouting new system
2. Pirates detected on minimap
3. Approach pirates OR accept combat mission
4. Engage in real-time combat
5. Defeat pirates, collect loot
6. Continue exploration OR return home
```

**Combat → Trading:**
```
1. Combat complete, cargo full of loot
2. Return to colony (click Map → home system → planet)
3. Land on planet
4. Open Auction/Warehouse interface
5. Sell loot for credits
6. Buy needed resources
7. Economic loop complete
```

**Trading → Building:**
```
1. Sold resources, earned credits
2. Bought needed resources for upgrades
3. Already at colony from trading
4. Use purchased resources to upgrade buildings
5. Building loop resumes
```

**Flow is Seamless:** No forced boundaries, players transition naturally based on needs and goals.

---

## Mode Interconnections

### Resource Flow Map

```
BUILDING MODE
    ↓ Produces Resources
    ↓
TRADING MODE ←→ Credits ←→ EXPLORING MODE
    ↓                           ↓
    ↓                      Find Planets
    ↓                           ↓
Ship Upgrades            Establish Colonies
    ↓                           ↓
COMBAT MODE ←────────────────←──┘
    ↓
Loot & Credits
    ↓
Back to BUILDING or TRADING
```

### Dependency Chain

**To Succeed in Combat, You Need:**
- Good ship (costs credits from Trading)
- Upgraded modules (costs resources from Building)
- Access to mission zones (requires Exploring)

**To Succeed in Trading, You Need:**
- Resources to sell (from Building)
- Multiple colonies (from Exploring)
- Cargo capacity (from ship upgrades via Combat rewards)

**To Succeed in Building, You Need:**
- Diverse planets (from Exploring)
- Resources from other colonies (via Trading/hauling)
- Credits for upgrades (from Combat or Trading)

**To Succeed in Exploring, You Need:**
- Fuel (from Building: uranium + ice)
- Safe ship (from Combat experience)
- Credits for colony establishment (from Trading)

**Result:** All modes are interdependent and equally necessary for progression.

---

## Progression Systems

### Implicit Progression Model

**No Explicit Player Level** - Progression is demonstrated through:

1. **Economic Power**
   - Credits: 1,000 → 5,000 → 25,000 → 100,000+
   - Visible in UI: "Credits: 25,430"
   - Milestone markers: "Can afford Fighter (5,000)" → "Can afford Destroyer (25,000)"

2. **Ship Class Ownership**
   - Start: Scout (2 weapons, 1 module)
   - Mid: Fighter (3 weapons, 2 modules)
   - Late: Destroyer (4 weapons, 4 modules)
   - Each ship feels significantly more powerful

3. **Colony Network Size**
   - Start: 1 colony (home planet)
   - Early: 2 colonies (home + resource specialization)
   - Mid: 3 colonies (full resource coverage)
   - Each colony unlocked by Headquarters level

4. **System Access**
   - Start: System 1-2 (green, safe)
   - Early: System 3-4 (yellow, medium)
   - Late: System 5-6 (red, dangerous)
   - New systems = new challenges, new resources

5. **Building Levels**
   - Mine Level 1 → Level 10 → Level 20
   - Visual: Level number on building
   - Functional: Production rate increases dramatically
   - Satisfaction: "My Level 15 mine produces 600 metal/hour!"

### Progression Gates

**What Unlocks What:**

```
HEADQUARTERS LEVEL:
├─ Level 1: 1 colony max
├─ Level 5: 2 colonies max, basic buildings
├─ Level 10: 3 colonies max, advanced buildings
├─ Level 15: All buildings unlocked
└─ Level 20: Maximum efficiency

CREDITS EARNED:
├─ 5,000: Can afford Fighter ship
├─ 10,000: Can afford multiple colonies
├─ 25,000: Can afford Destroyer ship
└─ 50,000+: Can afford maximum upgrades

SYSTEMS EXPLORED:
├─ System 1: Tutorial zone, always accessible
├─ System 2: Requires completing System 1 missions
├─ System 3: Requires Fighter ship OR HQ Level 5
├─ System 4: Requires 10,000 credits earned (total)
├─ System 5: Requires Destroyer ship OR HQ Level 10
└─ System 6: Requires defeating System 5 boss

MISSION DIFFICULTY:
├─ Easy Missions: Available from start
├─ Medium Missions: Require Fighter ship
├─ Hard Missions: Require Destroyer ship
└─ Boss Missions: Require full module loadout
```

### Progression Pacing

**Week 1:**
- Scout ship, 1-2 colonies
- Systems 1-2 accessible
- Basic buildings (levels 1-5)
- Focus: Learning mechanics, establishing base

**Week 2:**
- Fighter ship acquired
- 2-3 colonies established
- Systems 3-4 accessible
- Buildings reaching levels 5-10
- Focus: Optimization, specialization

**Week 3-4:**
- Destroyer ship acquired
- 3 colonies fully specialized
- Systems 5-6 accessible
- Buildings at levels 10-15
- Focus: Endgame content, boss fights

**Month 2+:**
- Maximum optimization
- Buildings approaching level 20
- All systems mastered
- Focus: Perfect builds, economic dominance

---

## Return Hooks & Engagement

### Short-Term Hooks (Next Session)

**Why Log Back In Tomorrow:**

1. **Storage Will Be Full** (4-6 hours)
   - "I need to collect before production stops"
   - Satisfying: Big collection numbers
   - Habit-forming: Regular check-in schedule

2. **Upgrades Will Complete** (1-24 hours)
   - "My Mine will be Level 6 tomorrow"
   - Anticipation: Visible power increase
   - Next step clear: Start next upgrade

3. **Resources Needed** (Immediate)
   - "I'm 500 stone short of Fighter ship"
   - Goal proximity: "Almost there!"
   - Action clear: "One more gathering run"

4. **Mission Completion** (In-progress)
   - "I'm 2/5 pirates killed for mission"
   - Investment: "Already started, should finish"
   - Reward waiting: "Mission gives 1,000 credits"

### Medium-Term Hooks (This Week)

**Why Keep Playing This Week:**

1. **Ship Progression Goal**
   - "Saving for Fighter ship (need 5,000 credits)"
   - Progress visible: "Currently: 3,200 credits"
   - Estimate: "2-3 more sessions"

2. **Colony Expansion**
   - "Need Scientific planet for electronics production"
   - Strategy: "Explore System 4 this week"
   - Unlock: "Electronics enable advanced modules"

3. **Building Milestones**
   - "Mine reaching Level 10 (production doubles)"
   - Anticipation: "Will unlock next tier of upgrades"
   - Snowball effect: "Faster progression unlocked"

4. **System Unlocking**
   - "System 3 unlocks at HQ Level 5"
   - Currently: "HQ Level 4, upgrading to 5"
   - Curiosity: "What's in System 3?"

### Long-Term Hooks (This Month+)

**For MVP, Limited Long-Term Focus:**

Given your directive to "focus on technical prototype first, worry about details later," long-term engagement is **not MVP priority**. However, these hooks emerge naturally from the systems:

1. **Ship Class Mastery**
   - Destroyer is endgame goal
   - Requires significant credits and progression
   - Changes gameplay meaningfully

2. **Colony Network Perfection**
   - Optimizing 3-colony production
   - Specialized planet types
   - Economic engine

3. **System Completion**
   - "Clear all 6 systems"
   - Defeat all bosses
   - Unlock all planets

**Post-MVP additions** can enhance long-term hooks:
- Leaderboards (economic, combat)
- Rare modules/equipment
- Seasonal events
- New systems/content updates

---

## Implementation Guidelines

### For Step 6 (MVP Feature Set)

**Must-Have Features to Support Core Loop:**

✅ **Colony Management:**
- Resource collection UI
- Building upgrade interface with timers
- Remote colony access via house button
- Multi-colony switching

✅ **Space Navigation:**
- Real-time 8-directional ship movement
- Planet landing/launching animations
- Galaxy map with hyperspace travel
- Fuel consumption system

✅ **Combat System:**
- Real-time weapon fire
- 20 ships on screen simultaneously
- Asteroid/comet destruction for resources
- Pirate AI (chase, attack, retreat, reinforce)

✅ **Mission System:**
- Procedural mission generation
- 8 mission types (at least 4 for MVP)
- Reward distribution
- Mission tracking UI

✅ **Trading System:**
- Auction/warehouse interface
- Buy/sell at fixed prices
- Cargo management
- Credit economy

✅ **Progression Tracking:**
- Credits display (always visible)
- Ship class indicators
- System unlock status
- Building levels visible

### For Step 7 (Game Systems Design)

**Balance Requirements:**

**Time-to-Credits Parity:**
- 15 min of Combat = ~500 credits (mission rewards)
- 15 min of Trading = ~500 credits (selling resources)
- 15 min of Building = 4-6 hours of production = ~500 credits worth of resources
- Result: All modes feel equally valuable

**Resource Production Rates:**
- Storage fills in 4-6 hours at early levels
- Storage fills in 6-8 hours at mid levels
- Storage fills in 8-12 hours at high levels
- Result: 2-3 check-ins per day optimal

**Building Upgrade Times:**
- Level 1→2: 1 hour
- Level 5→6: 4 hours
- Level 10→11: 12 hours
- Level 15→16: 24 hours
- Level 19→20: 24 hours (max)
- Result: Always something completing each session

**Ship Costs:**
- Scout: 1,000 credits (starting ship)
- Fighter: 5,000 credits (~5-7 hours gameplay)
- Destroyer: 25,000 credits (~15-20 hours gameplay)
- Result: Clear progression milestones

### For Step 8 (Technical Architecture)

**Critical Technical Requirements:**

**Session Persistence:**
- Save player location on logout
- Save building timers and completion times
- Calculate offline production on login
- Handle storage capacity overflow

**Real-Time Systems:**
- 60 FPS combat minimum (30 FPS acceptable)
- Network latency handling (MVP is single-player, future consideration)
- Collision detection for 20+ ships
- Efficient sprite rendering

**UI Responsiveness:**
- Instant UI transitions (colony ↔ space)
- Smooth galaxy map interactions
- Real-time resource updates
- Clear visual feedback for all actions

**Data Structures:**
```javascript
PlayerState {
  location: "colony_1" | "space_system_3" | "map",
  credits: number,
  resources: { stone, ice, uranium, ... },
  ships: [ { class, modules, hp, ... } ],
  colonies: [ { planet_id, buildings, production, ... } ],
  missions: [ { type, progress, rewards, ... } ],
  unlocked_systems: [ 1, 2, 3, ... ]
}

ColonyState {
  planet_id: string,
  buildings: [
    {
      type: "mine",
      level: number,
      upgrading: boolean,
      upgrade_complete_at: timestamp,
      production_rate: number,
      storage_current: number,
      storage_max: number,
      last_collected: timestamp
    }
  ]
}

MissionInstance {
  template_id: string,
  objectives: [ { type, target, progress, max } ],
  rewards: { credits, resources, items },
  status: "active" | "complete",
  accept_time: timestamp
}
```

### For Step 9-12 (Visual Design & Assets)

**Key Visual Requirements:**

**UI Elements:**
- Always-visible house button (left side)
- Always-visible credits counter (top right)
- Resource indicators with "FULL" alerts
- Building timers (countdown display)
- Galaxy map with system icons
- Ship status bar (HP, cargo, fuel)

**Animations:**
- Ship launch from planet (2 sec)
- Ship landing on planet (2 sec)
- Hyperspace jump (3 sec)
- Resource collection (particle effects)
- Building upgrade complete (flash/glow)

**Visual Feedback:**
- Storage full: Flashing indicator
- Upgrade complete: Green checkmark
- Low fuel: Yellow warning
- Combat damage: Red flash
- Credits earned: +number popup

---

## Success Metrics (For Step 16 - User Testing)

**Core Loop Validation:**

✅ **5-Minute Sessions Are Satisfying:**
- Do testers feel "I accomplished something" after 5 min?
- Do they understand what will happen while offline?
- Do they know what to do next session?

✅ **Mode Flexibility Works:**
- Do testers switch between modes based on mood?
- Or do they only engage with one mode?
- Are all four modes used by most players?

✅ **Return Hooks Effective:**
- Do testers voluntarily return after 4-6 hours?
- What's the primary return motivation?
- Do storage/upgrades create anticipation?

✅ **Progression Feels Good:**
- Do testers feel progress in each session?
- Is ship progression satisfying?
- Do colony upgrades feel impactful?

✅ **No Frustration Points:**
- Do testers get stuck or confused?
- Is navigation intuitive?
- Are upgrade times too long/short?

**Target Metrics:**
- 80%+ testers complete 5+ sessions
- 60%+ testers use all four modes
- 50%+ testers return within 8 hours
- Average session: 20-45 minutes
- Day 7 retention: 30%+

---

## Core Game Loop Summary

**The Loop in One Sentence:**
*"Choose your activity based on mood and time, make meaningful progress in minutes, feel compelled to return when storage fills or upgrades complete, gradually expand your space empire through interconnected building/exploring/trading/combat systems."*

**Design Pillars:**

1. **Flexibility:** All four modes are equally viable
2. **Respect Time:** 5 minutes is a valid session
3. **Async Progression:** Things happen offline
4. **No Forced Path:** Players discover organically
5. **Interconnection:** Modes support each other naturally
6. **Clear Goals:** Always know what to do next
7. **Satisfying Stops:** Can quit at any moment
8. **Return Hooks:** Natural reasons to come back

**Success Criteria:**
- ✅ Player never feels "I wasted time"
- ✅ Player always feels "I made progress"
- ✅ Player knows "What I'll do next time"
- ✅ Player wants "Just one more session"

---

**Document Version:** 1.0  
**Created:** November 24, 2025  
**Status:** Complete - Ready for Step 6  
**Next Step:** MVP Feature Set (define exact features for implementation)  
**Project:** Space MMO Browser Game
