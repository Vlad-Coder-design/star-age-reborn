# MVP Requirements Document
**Space MMO Browser Game - Investment-Ready Prototype**

---

## Executive Summary

This document defines the Minimum Viable Product (MVP) scope for a browser-based space MMO strategy game. The MVP validates three core hypotheses: (1) flexible play styles are engaging, (2) async progression creates return behavior, and (3) NPC-only gameplay feels alive. 

**MVP Purpose:** Technical prototype for solo developer testing - proves core mechanics work and creates foundation for future expansion.

**Key Scope Decisions:**
- 6 star systems with simplified galaxy structure
- 3 ship classes with basic equipment
- 4 building types (no upgrades)
- Pure PvE with procedural NPC generation
- Local browser storage (Google auth post-MVP)
- Real-time combat and async resource production

**Development Approach:** Solo developer using AI coding tools (Antigravity AI). All technical specifications designed for AI-assisted implementation.

**Timeline Focus:** Achievable prototype that can attract investment and validate design before scaling to full production.

---

## Table of Contents

1. [MVP Validation Goals](#mvp-validation-goals)
2. [Galaxy & Universe System](#galaxy--universe-system)
3. [Ship & Fleet System](#ship--fleet-system)
4. [Combat System](#combat-system)
5. [Colony & Building System](#colony--building-system)
6. [Resource & Economy System](#resource--economy-system)
7. [Mission System](#mission-system)
8. [UI/UX System](#uiux-system)
9. [Save/Load System](#saveload-system)
10. [Implementation Priority Order](#implementation-priority-order)
11. [Success Criteria](#success-criteria)
12. [Explicit Out-of-Scope](#explicit-out-of-scope)

---

## MVP Validation Goals

### What This MVP Proves

**Technical Validation:**
- ✅ Browser-based real-time gameplay works smoothly
- ✅ Async resource production calculates correctly offline
- ✅ Point-and-click space navigation feels responsive
- ✅ Real-time combat with multiple entities performs well
- ✅ Local storage save system handles game state

**Gameplay Validation:**
- ✅ Four play modes (Building, Exploring, Trading, Combat) are all functional
- ✅ Players can switch between modes based on mood/time
- ✅ 5-minute micro-sessions and 60-minute deep sessions both work
- ✅ Multi-colony strategy is necessary and engaging
- ✅ NPC-only universe doesn't feel empty

**Design Validation:**
- ✅ Core game loop creates return behavior (check resources every 4-6 hours)
- ✅ Ship progression feels meaningful (Scout → Fighter → Destroyer)
- ✅ Resource gathering via combat feels unified
- ✅ Manual cargo hauling creates interesting logistics gameplay

### What This MVP Does NOT Prove

- ❌ Multiplayer interactions or PvP balance
- ❌ Long-term retention (30+ days)
- ❌ Monetization effectiveness
- ❌ Server architecture or scaling
- ❌ Tutorial effectiveness (no tutorial in MVP)

---

## Galaxy & Universe System

### Must-Have Features

**Galaxy Structure:**
```
6 Star Systems Total
├─ System 1-2: Starting area (mixed planet types)
├─ System 3-4: Mid-level area (specialized biomes)
└─ System 5-6: High-level area (military focus)

Each System Contains:
├─ 3-4 planets (18-24 total planets in galaxy)
├─ Asteroid fields (space objects)
├─ Comet paths (space objects)
└─ Empty space for travel
```

**Planet Types (5 types, repeating across systems):**

| Planet Type | Visual | Unique Resource | Can Extract Universal Resources? |
|-------------|--------|-----------------|----------------------------------|
| Agrarian (Earth-like) | Blue-green | Food (post-MVP) | Yes (Stone, Ice from space) |
| Military (Volcanic) | Red-orange lava | **Uranium** | Yes (Stone, Ice from space) |
| Mining (Gas Giant) | Swirling gas | Ore (post-MVP) | Yes (Stone, Ice from space) |
| Industrial (Green) | Green industrial | Elderium (post-MVP) | Yes (Stone, Ice from space) |
| Scientific (Desert) | Sandy yellow | Electronics (post-MVP) | Yes (Stone, Ice from space) |

**MVP Simplification:**
- Only Uranium is extractable from colonies (Military planets)
- Stone and Ice gathered in space by destroying asteroids/comets
- Other planet-specific resources marked as "coming soon" in UI

**Planet Properties:**
```javascript
Planet {
  id: string
  name: string (procedurally generated, e.g., "Kepler-442b")
  systemId: string
  type: 'agrarian' | 'military' | 'mining' | 'industrial' | 'scientific'
  size: 'medium' // all medium for MVP (9 building slots)
  buildingSlots: 9
  position: {x, y} // position in system view
  colonized: boolean
  buildings: Building[]
}
```

**Space Objects:**
```javascript
SpaceObject {
  id: string
  type: 'asteroid' | 'comet'
  position: {x, y}
  hp: 50 // health points
  resource: 'stone' | 'ice'
  resourceAmount: 3 // drops when destroyed
  respawnTime: 300 // 5 minutes
}
```

### Travel System

**Movement Mechanics:**
- Click-to-move: Player clicks on screen, ship flies to that point
- Ship speed based on: Ship class + Engine type
- No fuel costs in MVP (fuel-free travel)
- Travel between systems: Click galaxy map → select system → instant arrival

**Speed Values:**
```javascript
// Base speeds (pixels per second)
ShipSpeeds {
  Scout: {
    basicEngine: 200,
    improvedEngine: 250
  },
  Fighter: {
    basicEngine: 150,
    improvedEngine: 180
  },
  Destroyer: {
    basicEngine: 100,
    improvedEngine: 120
  }
}
```

**Galaxy Map Navigation:**
```
1. Click "Map" button
2. See 6 systems displayed in galaxy view
3. Click destination system
4. Ship transitions to new system (fade out/in, 2 seconds)
5. Arrive in new system at random safe position
```

### Nice-to-Have (MVP)

- ⭐ Visual variety between systems (different skybox colors)
- ⭐ Planet info tooltip on hover (shows type, colonization status)
- ⭐ Minimap in corner showing current system layout

### Post-MVP

- 🔲 More planet types (12 total)
- 🔲 Wormholes and special locations
- 🔲 Dynamic events (meteor showers, nebula storms)
- 🔲 System ownership and territory control
- 🔲 Fuel consumption mechanics

---

## Ship & Fleet System

### Must-Have Features

**Player Ship Classes:**

| Ship Class | HP | Speed (base) | Weapon Slots | Module Slots | Cargo Capacity | Cost |
|------------|-----|--------------|--------------|--------------|----------------|------|
| Scout | 100 | 200 | 2 | 1 | 200 | Starting ship |
| Fighter | 200 | 150 | 3 | 2 | 250 | 5,000 credits |
| Destroyer | 400 | 100 | 4 | 4 | 300 | 15,000 credits |

**Ship Acquisition:**
- Purchase at any colonized planet's Shipyard
- Must have sufficient credits (no loans in MVP)
- Can only own 1 ship at a time (no fleet management)
- Buying new ship replaces old ship (no ship storage)

**Equipment System (Simplified):**

**Weapons (2 types in MVP):**
```javascript
Weapons {
  basicLaser: {
    damage: 10,
    range: 150,
    fireRate: 1.0, // shots per second
    cost: 0 // starting weapon
  },
  improvedLaser: {
    damage: 15,
    range: 180,
    fireRate: 1.2,
    cost: 2000 // available in shop
  }
}
```

**Engines (2 types in MVP):**
```javascript
Engines {
  basicEngine: {
    speedMultiplier: 1.0,
    cost: 0 // starting engine
  },
  improvedEngine: {
    speedMultiplier: 1.25,
    cost: 1500 // available in shop
  }
}
```

**Equipment Installation:**
- Visit colonized planet → Open Shipyard/Shop
- Purchase equipment with credits
- Equipment automatically installs on current ship
- Can only have 1 weapon type and 1 engine type active
- Changing ships: new ship comes with basic equipment (lose installed upgrades)

**Ship Data Structure:**
```javascript
PlayerShip {
  class: 'scout' | 'fighter' | 'destroyer'
  hp: number // current health
  maxHp: number
  position: {x, y}
  velocity: {x, y}
  cargo: {
    stone: number,
    ice: number,
    uranium: number,
    fuel: number,
    capacity: number
  }
  equipment: {
    weapon: 'basicLaser' | 'improvedLaser',
    engine: 'basicEngine' | 'improvedEngine'
  }
  target: NPCShip | null // current attack target
}
```

### NPC Ships

**NPC Types:**

**Miner Ships:**
```javascript
MinerNPC {
  hp: 50
  speed: 80
  behavior: 'mining' // flies to asteroids, mines slowly
  drops: {
    credits: 20-50,
    stone: 1-2
  }
  spawnRate: 15 // seconds
  weaponless: true
}
```

**Pirate Ships:**
```javascript
PirateNPC {
  hp: 100
  speed: 120
  damage: 10
  behavior: 'aggressive' // attacks player on sight within range
  aggro_range: 300
  drops: {
    credits: 50-150,
    stone: 1-3,
    ice: 1-2
  }
  spawnRate: 60 // seconds
  weapon: 'pirateLaser' // same stats as basicLaser
}
```

**NPC Spawning System:**
```javascript
NPCSpawner {
  // Spawns NPCs in player's current system only
  spawnRadius: 1000 // pixels from player position
  maxNPCs: {
    miners: 5, // max 5 miners at once
    pirates: 3  // max 3 pirates at once
  }
  timers: {
    minerSpawn: 15, // every 15 seconds
    pirateSpawn: 60 // every 60 seconds
  }
  // Spawns at random position within spawnRadius of player
  // Despawns if player leaves system
}
```

### Nice-to-Have (MVP)

- ⭐ Ship repair at colonies (costs credits)
- ⭐ Visual damage effects on ship
- ⭐ Equipment comparison tooltips in shop

### Post-MVP

- 🔲 Multiple ships per player (fleet management)
- 🔲 More equipment types (shields, armor, utilities)
- 🔲 Ship customization (colors, decals)
- 🔲 NPC boss enemies
- 🔲 NPC traders and merchants

---

## Combat System

### Must-Have Features

**Combat Mechanics:**

**Player Engagement:**
1. Player clicks on NPC ship (miner or pirate)
2. If NPC within weapon range → automatic attack begins
3. Ship auto-fires at rate specified by weapon
4. Player can click new target to switch
5. Player can click space to move (disengages current target)

**Attack Calculation:**
```javascript
CombatSystem {
  attackTarget(player, npc) {
    // Check range
    distance = calculateDistance(player.position, npc.position)
    if (distance > player.weapon.range) {
      return "out of range"
    }
    
    // Check fire rate timer
    if (currentTime - lastShotTime < 1/fireRate) {
      return "cooldown"
    }
    
    // Deal damage
    npc.hp -= player.weapon.damage
    lastShotTime = currentTime
    
    // Check if destroyed
    if (npc.hp <= 0) {
      spawnLoot(npc)
      removeNPC(npc)
      player.addExperience(npc.experienceValue)
    }
  }
}
```

**NPC AI (Pirates):**
```javascript
PirateAI {
  update(pirate, player) {
    distance = calculateDistance(pirate.position, player.position)
    
    // Aggro check
    if (distance < pirate.aggro_range) {
      // Move toward player
      moveToward(pirate, player.position, pirate.speed)
      
      // Attack if in range
      if (distance < pirate.weapon.range) {
        attackPlayer(pirate, player)
      }
    } else {
      // Idle patrol
      randomPatrol(pirate)
    }
  }
  
  attackPlayer(pirate, player) {
    if (currentTime - pirate.lastAttack > 1/pirate.fireRate) {
      player.hp -= pirate.damage
      pirate.lastAttack = currentTime
      
      // Check player death
      if (player.hp <= 0) {
        handlePlayerDeath(player)
      }
    }
  }
}
```

**Player Death:**
```javascript
handlePlayerDeath(player) {
  // Respawn at last colonized planet
  player.hp = player.maxHp
  player.position = player.homeColony.position
  
  // Cargo penalty: lose 50% of carried resources
  player.cargo.stone = Math.floor(player.cargo.stone * 0.5)
  player.cargo.ice = Math.floor(player.cargo.ice * 0.5)
  player.cargo.uranium = Math.floor(player.cargo.uranium * 0.5)
  
  // No credit loss in MVP
}
```

**Loot System:**
```javascript
spawnLoot(npc) {
  loot = {
    position: npc.position,
    credits: random(npc.drops.credits.min, npc.drops.credits.max),
    resources: {}
  }
  
  // Random resource drops
  if (npc.drops.stone) {
    loot.resources.stone = random(1, npc.drops.stone.max)
  }
  if (npc.drops.ice) {
    loot.resources.ice = random(1, npc.drops.ice.max)
  }
  
  // Auto-collect if player within 50 pixels
  if (distance(player, loot) < 50) {
    player.credits += loot.credits
    player.cargo += loot.resources
    showFloatingText(`+${loot.credits} credits`)
  } else {
    // Drop stays in space for 60 seconds
    spawnLootObject(loot, 60)
  }
}
```

**Visual Feedback:**
- Red targeting circle on selected NPC
- Laser beam animation from ship to target
- Damage numbers float above target
- Explosion animation on NPC death
- Floating "+Credits" text on loot collection

### Nice-to-Have (MVP)

- ⭐ Health bars above NPCs
- ⭐ Combat sounds (laser fire, explosions)
- ⭐ Screen shake on taking damage
- ⭐ Shield visual effect when hit

### Post-MVP

- 🔲 Different weapon types (rockets, plasma, railguns)
- 🔲 Active abilities (afterburner, shield boost, EMP)
- 🔲 Fleet combat (multiple ships per player)
- 🔲 Boss enemies with special mechanics
- 🔲 PvP combat

---

## Colony & Building System

### Must-Have Features

**Colony Establishment:**

**Starting Colony:**
- Player begins with 1 colonized planet (home colony)
- Home colony type: randomly chosen from available types
- Starting buildings: None (player builds from scratch)
- Starting resources in storage: 5 Stone, 5 Ice, 0 Uranium, 0 Fuel

**Additional Colonies:**
```javascript
ColonizationSystem {
  costs: {
    colony2: 2000,  // 2nd colony
    colony3: 5000,  // 3rd colony
    colony4: 10000, // 4th colony
    colony5: 20000, // 5th colony
    colony6: 40000  // 6th colony
  },
  
  colonizePlanet(player, planet) {
    // Check if player can afford next colony
    colonyCount = player.colonies.length
    cost = this.costs[`colony${colonyCount + 1}`]
    
    if (player.credits < cost) {
      return "Insufficient credits"
    }
    
    if (planet.colonized) {
      return "Planet already colonized"
    }
    
    // Deduct credits and colonize
    player.credits -= cost
    planet.colonized = true
    player.colonies.push(planet)
    
    return "Success"
  }
}
```

**Colony Interface:**
- Planet view shows 9 building slots in 3x3 grid
- Click empty slot → building selection menu opens
- Select building type → pay resource cost → building places instantly
- No construction time in MVP
- No building upgrades in MVP

**Building Types (5 types in MVP):**

| Building | Purpose | Cost | Production Rate | Storage Capacity |
|----------|---------|------|-----------------|------------------|
| **Universal Storage** | Stores all resources | 3 Stone, 2 Ice | N/A | 500 per resource type |
| **Stone Extractor** | Mines stone | 2 Stone | 1 stone/15 min | Holds 5 stone |
| **Ice Extractor** | Mines ice | 2 Ice | 1 ice/5 min | Holds 5 ice |
| **Uranium Mine** | Mines uranium (Military planets only) | 3 Stone, 2 Ice | 1 uranium/20 min | Holds 5 uranium |
| **Fuel Factory** | Converts resources to fuel | 5 Stone, 3 Ice | Converts 2 uranium + 1 ice = 1 fuel/30 min | Holds 4 fuel |

**Building Functionality:**

```javascript
Building {
  type: 'storage' | 'stoneExtractor' | 'iceExtractor' | 'uraniumMine' | 'fuelFactory'
  level: 1 // always level 1 in MVP
  lastProduction: timestamp
  storage: number // resources waiting to be collected
  
  // Production tick (called every game frame or when player opens colony)
  tick(currentTime) {
    if (this.type === 'storage') return
    
    // Calculate time since last production
    timePassed = currentTime - this.lastProduction
    
    // Calculate how many units produced
    productionTime = this.getProductionTime()
    unitsProduced = Math.floor(timePassed / productionTime)
    
    // Cap at storage capacity
    this.storage = Math.min(
      this.storage + unitsProduced,
      this.getStorageCapacity()
    )
    
    // Update last production time
    if (unitsProduced > 0) {
      this.lastProduction = currentTime
    }
  }
  
  collect(player) {
    // Transfer from building storage to colony universal storage
    resourceType = this.getResourceType()
    player.colonies[currentColony].storage[resourceType] += this.storage
    this.storage = 0
  }
}
```

**Async Production (CRITICAL):**
```javascript
// When player opens game after being offline
GameLoader {
  loadGame() {
    savedTime = localStorage.getItem('lastSaveTime')
    currentTime = Date.now()
    timePassed = currentTime - savedTime
    
    // Update all buildings across all colonies
    player.colonies.forEach(colony => {
      colony.buildings.forEach(building => {
        building.tick(currentTime)
      })
    })
  }
}
```

**Colony Storage System:**
```javascript
ColonyStorage {
  // Universal storage building holds ALL resource types
  capacity: 500, // per resource type
  resources: {
    stone: 0,
    ice: 0,
    uranium: 0,
    fuel: 0
  },
  
  // Collect from all extractors
  collectAll() {
    this.colony.buildings.forEach(building => {
      if (building.storage > 0) {
        resourceType = building.getResourceType()
        amount = Math.min(
          building.storage,
          this.capacity - this.resources[resourceType]
        )
        this.resources[resourceType] += amount
        building.storage -= amount
      }
    })
  }
}
```

**Resource Restrictions:**
- Uranium Mine can ONLY be built on Military (volcanic) planets
- Stone/Ice extractors can be built on any colonized planet
- Fuel Factory can be built on any colonized planet (but needs uranium input)

### Nice-to-Have (MVP)

- ⭐ "Collect All" button in colony view
- ⭐ Visual progress bars on buildings showing production
- ⭐ Notification dot when resources ready to collect
- ⭐ Building count limits (e.g., max 2 of each type per colony)

### Post-MVP

- 🔲 Building upgrades (Level 1-20)
- 🔲 More building types (hydroponics, shipyard, defenses)
- 🔲 Population mechanics
- 🔲 Planet specialization bonuses
- 🔲 Automated resource transfer between colonies

---

## Resource & Economy System

### Must-Have Features

**Resource Types (MVP):**

| Resource | Acquisition Method | Primary Use | Starting Amount |
|----------|-------------------|-------------|-----------------|
| **Credits** | Sell resources, kill NPCs, complete missions | Buy ships, equipment, colonize planets | 1000 |
| **Stone** | Destroy asteroids in space (3 per asteroid) OR extract at colonies | Build buildings, produce metal (post-MVP) | 5 |
| **Ice** | Destroy comets in space (3 per comet) OR extract at colonies | Produce fuel | 5 |
| **Uranium** | Extract at Military planet colonies only | Produce fuel | 0 |
| **Fuel** | Produce at Fuel Factory (2 uranium + 1 ice = 1 fuel) | Reserved for post-MVP travel costs | 0 |

**Resource Gathering in Space:**

```javascript
SpaceGathering {
  shootAsteroid(asteroid, weapon) {
    asteroid.hp -= weapon.damage
    
    if (asteroid.hp <= 0) {
      // Drop stone
      loot = {
        type: 'stone',
        amount: 3,
        position: asteroid.position
      }
      spawnLoot(loot)
      
      // Respawn asteroid after 5 minutes
      setTimeout(() => {
        respawnAsteroid(asteroid)
      }, 300000)
    }
  },
  
  shootComet(comet, weapon) {
    comet.hp -= weapon.damage
    
    if (comet.hp <= 0) {
      // Drop ice
      loot = {
        type: 'ice',
        amount: 3,
        position: comet.position
      }
      spawnLoot(loot)
      
      // Respawn comet after 5 minutes
      setTimeout(() => {
        respawnComet(comet)
      }, 300000)
    }
  }
}
```

**Cargo System:**

```javascript
ShipCargo {
  capacity: 300, // Scout: 200, Fighter: 250, Destroyer: 300
  contents: {
    stone: 0,
    ice: 0,
    uranium: 0,
    fuel: 0
  },
  
  add(resourceType, amount) {
    currentTotal = Object.values(this.contents).reduce((a, b) => a + b)
    spaceAvailable = this.capacity - currentTotal
    
    if (spaceAvailable <= 0) {
      return { success: false, reason: "Cargo full" }
    }
    
    amountToAdd = Math.min(amount, spaceAvailable)
    this.contents[resourceType] += amountToAdd
    
    return { success: true, added: amountToAdd }
  },
  
  transfer(destination, resourceType, amount) {
    if (this.contents[resourceType] < amount) {
      return { success: false, reason: "Insufficient resources" }
    }
    
    this.contents[resourceType] -= amount
    destination.resources[resourceType] += amount
    
    return { success: true, transferred: amount }
  }
}
```

**Manual Cargo Hauling:**

```javascript
// Player must manually transport resources between colonies
CargoHauling {
  // At Colony A: Load cargo
  loadCargo(colony, ship, resourceType, amount) {
    // Check colony has resources
    if (colony.storage.resources[resourceType] < amount) {
      return "Insufficient resources in colony storage"
    }
    
    // Try to add to ship cargo
    result = ship.cargo.add(resourceType, amount)
    
    if (result.success) {
      colony.storage.resources[resourceType] -= result.added
      showMessage(`Loaded ${result.added} ${resourceType}`)
    }
  },
  
  // At Colony B: Unload cargo
  unloadCargo(colony, ship, resourceType, amount) {
    result = ship.cargo.transfer(colony.storage, resourceType, amount)
    
    if (result.success) {
      showMessage(`Unloaded ${result.transferred} ${resourceType}`)
    }
  }
}
```

**Economy System:**

**Selling Resources:**
```javascript
// At any colonized planet: Open Auction/Trading Post
TradingPost {
  prices: {
    stone: 3,    // credits per unit
    ice: 2,      // credits per unit
    uranium: 5,  // credits per unit
    fuel: 20     // credits per unit
  },
  
  sellResource(player, resourceType, amount) {
    // Check player has resources in current colony
    colony = player.currentColony
    
    if (colony.storage.resources[resourceType] < amount) {
      return "Insufficient resources"
    }
    
    totalValue = this.prices[resourceType] * amount
    
    colony.storage.resources[resourceType] -= amount
    player.credits += totalValue
    
    showMessage(`Sold ${amount} ${resourceType} for ${totalValue} credits`)
  }
}
```

**Buying Resources:**
```javascript
// For MVP, players CANNOT buy resources
// Only sell resources for credits
// This encourages gathering and production gameplay
```

### Nice-to-Have (MVP)

- ⭐ Resource price variations between planets
- ⭐ Bulk sell/load/unload actions
- ⭐ Cargo management interface showing capacity bar

### Post-MVP

- 🔲 Buy resources from NPCs or trading posts
- 🔲 Dynamic pricing based on supply/demand
- 🔲 Resource production chains (Tier 2 and 3 resources)
- 🔲 Automated cargo routes
- 🔲 Player-to-player trading

---

## Mission System

### Must-Have Features

**Mission Types (2 types in MVP):**

**Combat Mission:**
```javascript
CombatMission {
  type: 'combat',
  title: 'Destroy Pirates',
  description: 'Destroy 3 pirate ships in this system',
  objectives: {
    piratesKilled: 0,
    piratesRequired: 3
  },
  rewards: {
    credits: 300,
    experience: 50
  },
  
  checkProgress(player) {
    if (this.objectives.piratesKilled >= this.objectives.piratesRequired) {
      this.complete(player)
    }
  }
}
```

**Gathering Mission:**
```javascript
GatheringMission {
  type: 'gathering',
  title: 'Mine Asteroids',
  description: 'Gather 3 stone by destroying asteroids',
  objectives: {
    stoneGathered: 0,
    stoneRequired: 3
  },
  rewards: {
    credits: 150,
    experience: 30
  },
  
  checkProgress(player) {
    // Track stone added to cargo from asteroids
    if (this.objectives.stoneGathered >= this.objectives.stoneRequired) {
      this.complete(player)
    }
  }
}
```

**Mission Generation:**

```javascript
MissionGenerator {
  // Procedurally generate random missions
  generateMission() {
    missionType = random(['combat', 'gathering'])
    
    if (missionType === 'combat') {
      targets = random(2, 5) // 2-5 pirates
      return {
        type: 'combat',
        title: `Destroy ${targets} Pirates`,
        objectives: { piratesKilled: 0, piratesRequired: targets },
        rewards: {
          credits: targets * 100,
          experience: targets * 20
        }
      }
    }
    
    if (missionType === 'gathering') {
      amount = random(2, 5) // 2-5 resources
      resourceType = random(['stone', 'ice'])
      return {
        type: 'gathering',
        title: `Mine ${amount} ${resourceType}`,
        objectives: { gathered: 0, required: amount },
        rewards: {
          credits: amount * 50,
          experience: amount * 15
        }
      }
    }
  }
}
```

**Mission System:**

```javascript
MissionManager {
  activeMissions: [], // max 3 active missions
  
  acceptMission(mission) {
    if (this.activeMissions.length >= 3) {
      return "Too many active missions"
    }
    
    this.activeMissions.push(mission)
    showMessage(`Mission accepted: ${mission.title}`)
  },
  
  completeMission(mission, player) {
    // Give rewards
    player.credits += mission.rewards.credits
    player.experience += mission.rewards.experience
    
    // Remove from active missions
    this.activeMissions = this.activeMissions.filter(m => m !== mission)
    
    showMessage(`Mission complete! +${mission.rewards.credits} credits`)
    
    // Generate new mission immediately
    newMission = this.generator.generateMission()
    this.availableMissions.push(newMission)
  }
}
```

**Mission Interface:**
- "Missions" button in UI
- Opens panel showing:
  - Available missions (3-5 always available)
  - Active missions (max 3)
  - Progress bars for active missions
- Click mission → "Accept" button
- Auto-complete when objectives met

### Nice-to-Have (MVP)

- ⭐ Mission difficulty tiers (easy/medium/hard)
- ⭐ Time-limited missions (complete within X minutes for bonus)
- ⭐ Mission chains (completing one unlocks another)

### Post-MVP

- 🔲 Escort missions
- 🔲 Delivery missions (cargo transport)
- 🔲 Exploration missions (discover new systems)
- 🔲 Boss missions (special enemies)
- 🔲 Story missions with narrative
- 🔲 Player-generated missions (post multiplayer)

---

## UI/UX System

### Must-Have Features

**Core UI Elements:**

**HUD (Always Visible):**
```
┌─────────────────────────────────────────────────┐
│ Credits: 1,250    HP: 85/100    [Map] [Missions]│
│ Stone: 12  Ice: 8  Uranium: 3  Fuel: 1          │
│ Cargo: 23/300     Speed: 150                     │
└─────────────────────────────────────────────────┘
```

**Navigation Buttons:**
- **Map Button**: Opens galaxy map for system travel
- **Missions Button**: Opens mission panel
- **Colony Button**: Opens colony management (when at planet)
- **Shipyard Button**: Opens ship/equipment shop (when at colonized planet)

**Galaxy Map Interface:**
```javascript
GalaxyMap {
  // Full-screen overlay showing 6 systems
  systems: [
    { id: 1, name: "Sol", position: {x, y}, visited: true },
    { id: 2, name: "Alpha Centauri", position: {x, y}, visited: false },
    // ... 4 more systems
  ],
  
  render() {
    // Show system icons
    // Highlight current system
    // Click system → travel confirmation → arrive
  }
}
```

**Colony Interface:**
```
┌─────────────────────────────────────────────────┐
│  [Colony Name] - Military Planet                │
│                                                  │
│  Building Slots (3x3 grid):                     │
│  [ Stone ] [ Ice  ] [Uranium]                   │
│  [  +   ] [  +   ] [  +   ]                     │
│  [  +   ] [  +   ] [  +   ]                     │
│                                                  │
│  Colony Storage: Stone: 45  Ice: 32  Uranium: 8 │
│  [Collect All] [Build] [Launch Ship]            │
└─────────────────────────────────────────────────┘
```

**Building Construction Menu:**
```
┌─────────────────────────────────────────────────┐
│  Select Building:                                │
│                                                  │
│  [Storage] - Cost: 3 Stone, 2 Ice              │
│  Stores 500 of each resource                    │
│                                                  │
│  [Stone Extractor] - Cost: 2 Stone              │
│  Produces 1 stone per 15 minutes                │
│                                                  │
│  [Ice Extractor] - Cost: 2 Ice                  │
│  Produces 1 ice per 5 minutes                   │
│                                                  │
│  [Uranium Mine] - Cost: 3 Stone, 2 Ice          │
│  Produces 1 uranium per 20 minutes              │
│  (Military planets only)                        │
│                                                  │
│  [Fuel Factory] - Cost: 5 Stone, 3 Ice          │
│  Converts 2 uranium + 1 ice = 1 fuel (30 min)  │
└─────────────────────────────────────────────────┘
```

**Shipyard Interface:**
```
┌─────────────────────────────────────────────────┐
│  Ships:                                          │
│  [Scout] - HP: 100, Speed: 200 (Current)        │
│  [Fighter] - HP: 200, Speed: 150 - 5,000¢      │
│  [Destroyer] - HP: 400, Speed: 100 - 15,000¢   │
│                                                  │
│  Equipment:                                      │
│  [Basic Laser] - Damage: 10 (Current)           │
│  [Improved Laser] - Damage: 15 - 2,000¢        │
│  [Basic Engine] - Speed: 1.0x (Current)         │
│  [Improved Engine] - Speed: 1.25x - 1,500¢     │
└─────────────────────────────────────────────────┘
```

**Mission Panel:**
```
┌─────────────────────────────────────────────────┐
│  Active Missions:                                │
│  • Destroy 3 Pirates (2/3) - Reward: 300¢      │
│  • Mine 5 Stone (5/5) ✓ [COLLECT REWARD]       │
│                                                  │
│  Available Missions:                             │
│  • Destroy 4 Pirates - Reward: 400¢ [ACCEPT]   │
│  • Gather 3 Ice - Reward: 150¢ [ACCEPT]        │
│  • Destroy 2 Miners - Reward: 200¢ [ACCEPT]    │
└─────────────────────────────────────────────────┘
```

**Visual Feedback:**
- Floating damage numbers (red for damage taken, white for damage dealt)
- Floating "+Credits" text on loot collection
- Health bars above NPCs
- Targeting circle (red) on selected enemy
- Laser beam visual from ship to target
- Explosion animation on NPC death
- Screen flash on taking damage

**Controls:**
- **Mouse Click (Space)**: Move to location
- **Mouse Click (NPC)**: Attack target
- **Mouse Click (Planet)**: Land on planet
- **UI Buttons**: Click to open menus

### Nice-to-Have (MVP)

- ⭐ Minimap showing current system
- ⭐ Tooltip on hover over UI elements
- ⭐ Settings menu (sound, graphics quality)
- ⭐ Keybindings (1-9 for quick actions)

### Post-MVP

- 🔲 Chat system (for multiplayer)
- 🔲 Social features (alliances, friends list)
- 🔲 Advanced filters and sorting
- 🔲 Mobile-responsive UI
- 🔲 Customizable HUD layout

---

## Save/Load System

### Must-Have Features

**Local Storage (MVP):**

```javascript
SaveSystem {
  // Auto-save on every significant action
  saveGame() {
    gameState = {
      version: '1.0.0',
      lastSaveTime: Date.now(),
      
      player: {
        credits: this.player.credits,
        experience: this.player.experience,
        level: this.player.level,
        
        ship: {
          class: this.player.ship.class,
          hp: this.player.ship.hp,
          position: this.player.ship.position,
          currentSystemId: this.player.currentSystemId,
          cargo: this.player.ship.cargo,
          equipment: this.player.ship.equipment
        }
      },
      
      colonies: this.player.colonies.map(colony => ({
        planetId: colony.planetId,
        buildings: colony.buildings.map(building => ({
          type: building.type,
          slotIndex: building.slotIndex,
          lastProduction: building.lastProduction,
          storage: building.storage
        })),
        storage: colony.storage.resources
      })),
      
      missions: {
        active: this.missionManager.activeMissions,
        available: this.missionManager.availableMissions
      },
      
      npcs: {
        miners: this.npcManager.miners,
        pirates: this.npcManager.pirates
      }
    }
    
    localStorage.setItem('starage_save', JSON.stringify(gameState))
  },
  
  loadGame() {
    savedData = localStorage.getItem('starage_save')
    
    if (!savedData) {
      // New game
      return this.newGame()
    }
    
    gameState = JSON.parse(savedData)
    
    // Restore player state
    this.player = this.reconstructPlayer(gameState.player)
    
    // Restore colonies and tick production
    this.player.colonies = gameState.colonies.map(colonyData => {
      colony = this.reconstructColony(colonyData)
      
      // Update production for offline time
      timePassed = Date.now() - gameState.lastSaveTime
      colony.buildings.forEach(building => {
        building.tick(Date.now())
      })
      
      return colony
    })
    
    // Restore missions
    this.missionManager.restore(gameState.missions)
    
    // Restore NPCs (or respawn fresh)
    this.npcManager.restore(gameState.npcs)
  },
  
  newGame() {
    // Create starting state
    player = new Player({
      credits: 1000,
      experience: 0,
      level: 1,
      ship: new Ship('scout')
    })
    
    // Assign random starting planet
    startingPlanet = this.galaxyGenerator.getRandomStartingPlanet()
    startingPlanet.colonized = true
    player.colonies = [startingPlanet]
    
    // Add starting resources to colony storage
    startingPlanet.storage.resources = {
      stone: 5,
      ice: 5,
      uranium: 0,
      fuel: 0
    }
    
    return player
  }
}
```

**Auto-Save Triggers:**
- Every 30 seconds (background save)
- When colonizing new planet
- When building structures
- When completing missions
- When buying ships/equipment
- Before closing browser (window.onbeforeunload)

**Save State Size:**
- Target: < 50KB per save
- Local storage limit: 5-10MB (plenty of room)

**Cache Clearing:**
- If cache cleared: game progress lost
- Post-MVP: Add Google auth + cloud saves
- MVP: Warn user about local storage dependency

### Nice-to-Have (MVP)

- ⭐ Export save to file (download JSON)
- ⭐ Import save from file
- ⭐ Multiple save slots

### Post-MVP

- 🔲 Google authentication
- 🔲 Cloud save sync
- 🔲 Cross-device save transfer
- 🔲 Save backup system
- 🔲 Version migration handling

---

## Implementation Priority Order

### Phase 1: Core Foundation (Week 1-2)

**Goal:** Basic game world and movement

1. **Project Setup**
   - HTML5 Canvas setup
   - Game loop architecture
   - Input handling (mouse clicks)
   - Basic render pipeline

2. **Galaxy & Universe**
   - Generate 6 star systems
   - Generate 18-24 planets with types
   - Render space view (stars, planets)
   - Galaxy map interface

3. **Player Ship**
   - Ship class definitions
   - Ship rendering (sprite or simple shape)
   - Click-to-move navigation
   - Ship position tracking

4. **Camera System**
   - Follow player ship
   - Pan/zoom controls
   - System transitions

**Deliverable:** Player can navigate space, see galaxy map, travel between systems

---

### Phase 2: Space Gameplay (Week 3-4)

**Goal:** Combat and resource gathering

5. **Space Objects**
   - Spawn asteroids and comets
   - Object collision detection
   - Object destruction mechanics
   - Loot drop system

6. **Combat System**
   - Click-to-attack targeting
   - Damage calculation
   - Health system
   - Laser visual effects

7. **NPC System**
   - NPC ship classes (Miner, Pirate)
   - NPC spawning algorithm
   - Basic pirate AI (chase and attack)
   - Miner AI (patrol and mine)

8. **Loot & Cargo**
   - Loot collection
   - Cargo capacity tracking
   - Cargo UI display

**Deliverable:** Player can fight NPCs, destroy asteroids/comets, collect resources

---

### Phase 3: Colony System (Week 5-6)

**Goal:** Planet colonization and resource production

9. **Colony Foundation**
   - Planet landing mechanics
   - Colonization system (pay credits)
   - Colony UI (3x3 grid view)
   - Building placement

10. **Buildings**
    - 5 building types implemented
    - Building construction (instant)
    - Resource costs
    - Building rendering in colony view

11. **Resource Production**
    - Production tick system
    - Async calculation (offline progress)
    - Building storage capacity
    - Collection mechanics

12. **Colony Storage**
    - Universal storage building
    - Resource transfer (ship ↔ colony)
    - Cargo loading/unloading

**Deliverable:** Player can colonize planets, build extractors, produce resources offline

---

### Phase 4: Economy & Progression (Week 7-8)

**Goal:** Credits, trading, ship upgrades

13. **Trading System**
    - Sell resources for credits
    - Trading post UI
    - Price system

14. **Shipyard**
    - Ship purchase interface
    - Equipment shop
    - Ship switching logic

15. **Ship Progression**
    - Fighter and Destroyer classes
    - Equipment installation
    - Weapon and engine upgrades

16. **Player Stats**
    - Experience tracking
    - Level indicator (cosmetic only)
    - Death and respawn system

**Deliverable:** Player can earn credits, buy better ships, upgrade equipment

---

### Phase 5: Missions & Polish (Week 9-10)

**Goal:** Mission system and gameplay loop closure

17. **Mission System**
    - Mission generator
    - 2 mission types (combat, gathering)
    - Mission tracking
    - Mission UI panel

18. **Mission Completion**
    - Objective tracking
    - Reward distribution
    - Mission refresh system

19. **UI Polish**
    - HUD refinement
    - Tooltips
    - Visual feedback improvements
    - Sound effects (if time permits)

20. **Save/Load**
    - Local storage implementation
    - Auto-save triggers
    - Load game with offline progress

**Deliverable:** Complete gameplay loop with missions, progression, and persistence

---

### Phase 6: Testing & Optimization (Week 11-12)

**Goal:** Bug fixes, balance, performance

21. **Balance Tuning**
    - Resource production rates
    - Combat difficulty
    - Credit economy
    - Ship progression curve

22. **Performance Optimization**
    - Canvas rendering optimization
    - NPC spawning limits
    - Memory leak fixes
    - Save system optimization

23. **Bug Fixing**
    - Collision detection issues
    - UI bugs
    - Save/load edge cases
    - Browser compatibility

24. **Final Polish**
    - Visual consistency
    - UX improvements
    - Error handling
    - Prepare for demo/pitch

**Deliverable:** Stable, playable MVP ready for investor demos

---

## Success Criteria

### Technical Success

**Gameplay Performance:**
- ✅ Runs at 60 FPS with 20+ entities on screen
- ✅ No memory leaks after 60+ minutes of play
- ✅ Save/load completes in < 2 seconds
- ✅ Async production calculates correctly after 12+ hours offline

**Core Functionality:**
- ✅ All 4 play modes work (Building, Exploring, Trading, Combat)
- ✅ Player can progress from Scout → Fighter → Destroyer
- ✅ Multi-colony strategy is functional and necessary
- ✅ NPC spawning creates living universe feel
- ✅ Missions generate and complete correctly

**Technical Stability:**
- ✅ No game-breaking bugs
- ✅ Works in Chrome, Firefox, Safari
- ✅ Mobile-friendly (touch controls work, even if not optimized)
- ✅ Responsive UI at different screen sizes

### Design Success

**Engagement Validation:**
- ✅ 5-minute sessions feel productive (can collect resources, complete mission)
- ✅ 60-minute sessions feel deep and strategic (explore, colonize, upgrade)
- ✅ Mood-based activity selection works (can choose what to do)
- ✅ Return motivation exists (resources producing, missions available)

**Progression Feel:**
- ✅ Clear short-term goals (save for Fighter ship)
- ✅ Clear medium-term goals (establish 3-4 specialized colonies)
- ✅ Long-term vision visible (Destroyer ship, full galaxy colonization)
- ✅ No "optimal path" problem (multiple valid strategies)

**Core Loop Validation:**
- ✅ Building mode feels relaxing and strategic
- ✅ Exploring mode feels adventurous
- ✅ Trading mode feels logistical and satisfying
- ✅ Combat mode feels exciting but not stressful
- ✅ All modes connect meaningfully (no isolated systems)

### Investment Readiness

**Demo Quality:**
- ✅ Can demonstrate full game loop in 10-minute pitch
- ✅ Visual style is cohesive and appealing
- ✅ Unique selling points are clear (flexible play, async progression, NPC universe)
- ✅ Technical foundation is solid for expansion

**Scalability Evidence:**
- ✅ Architecture supports multiplayer addition
- ✅ Galaxy structure can expand (6 → 50+ systems)
- ✅ Building/ship/mission systems designed for content expansion
- ✅ Code is organized and documented for team collaboration

---

## Explicit Out-of-Scope

### NOT in MVP

**Multiplayer Features:**
- ❌ PvP combat
- ❌ Player trading
- ❌ Alliances or guilds
- ❌ Chat system
- ❌ Leaderboards
- ❌ Cooperative missions
- ❌ Territory control

**Advanced Systems:**
- ❌ Building upgrades (Level 2-20)
- ❌ Tech tree or research
- ❌ Complex equipment system (shields, armor, utilities)
- ❌ Fleet management (multiple ships)
- ❌ Story missions or narrative
- ❌ Boss enemies
- ❌ Dynamic events

**Polish Features:**
- ❌ Tutorial system
- ❌ Achievement system
- ❌ Sound design and music
- ❌ Particle effects
- ❌ Cutscenes or animations
- ❌ Voice acting
- ❌ Localization (English only)

**Economy Features:**
- ❌ Buying resources from NPCs
- ❌ Dynamic pricing
- ❌ Market system
- ❌ Loans or credit system
- ❌ Insurance or ship loss penalties
- ❌ Tier 2-3 resource production chains

**Advanced Gameplay:**
- ❌ Population mechanics
- ❌ Planet specialization bonuses
- ❌ Automated resource transfer
- ❌ Escort missions
- ❌ Delivery missions
- ❌ Exploration missions (beyond basic travel)
- ❌ Fuel consumption mechanics

**Technical Features:**
- ❌ Cloud saves
- ❌ Google authentication
- ❌ Mobile app (mobile browser only)
- ❌ Mod support
- ❌ Level editor
- ❌ Analytics dashboard

---

## Document Control

**Version:** 1.0  
**Status:** Final - Ready for Implementation  
**Date:** November 24, 2024  
**Next Step:** Begin Phase 1 Implementation  
**Author:** Igor (Product Owner) with AI assistance

**Revision History:**
- v1.0 (Nov 24, 2024): Initial MVP requirements defined

**Related Documents:**
- core-game-loop.md (completed Step 5)
- core-assumptions-UPDATE-nov24.md
- research-findings.md
- problem-statement.md
- target-audience.md

---

**End of MVP Requirements Document**
