# Asset Requirements Document
**Space MMO Browser Game - Visual & Audio Asset Specifications**

---

## Executive Summary

Complete asset requirements for Space MMO MVP including sprite dimensions, AI generation prompts, and implementation specifications. All assets optimized for browser delivery and AI generation with Nano Banana/Recraft.

**Asset Strategy:**
- Hero assets first (ships, UI) for professional demo
- Placeholders acceptable for secondary assets (effects, particles)
- Consistent art style inspired by Star Age screenshots
- PNG format with transparency for sprites

**Total Asset Count:** 25+ sprites, 10+ UI elements, 5+ sound effects (optional)

---

## Ship Sprites

### Player Ships (Priority: CRITICAL)

**Scout Ship**
- **Filename:** `scout.png`
- **Dimensions:** 64x64 pixels
- **Style:** Small, nimble fighter design
- **Colors:** Greenish-blue primary, metallic accents
- **View:** Top-down, 45° angle
- **AI Prompt:** "Top-down view of a small futuristic scout spaceship, sleek design, greenish-blue hull with metallic silver accents, two small engine thrusters visible, 64x64 pixel sprite, transparent background, inspired by Star Age game aesthetic"

**Fighter Ship**
- **Filename:** `fighter.png`
- **Dimensions:** 64x64 pixels  
- **Style:** Medium combat vessel, bulkier than scout
- **Colors:** Dark blue primary, orange engine glow
- **AI Prompt:** "Top-down view of a medium combat spaceship, more weapons visible than scout, dark blue armored hull with orange glowing engines, 64x64 pixel sprite, transparent background, space game style"

**Destroyer Ship**
- **Filename:** `destroyer.png`
- **Dimensions:** 96x96 pixels (larger ship)
- **Style:** Heavy battleship, intimidating presence
- **Colors:** Dark gray/black hull, red weapon pods
- **AI Prompt:** "Top-down view of a heavy destroyer spaceship, large imposing design, dark gray armored hull with red weapon systems, four visible engine thrusters, 96x96 pixel sprite, transparent background, military space vessel"

### NPC Ships (Priority: HIGH)

**Miner NPC**
- **Filename:** `miner-npc.png`
- **Dimensions:** 48x48 pixels (smaller utility ship)
- **Style:** Industrial mining vessel, cargo pods visible
- **Colors:** Yellow/orange warning stripes, dull metal
- **AI Prompt:** "Top-down view of a small mining utility spaceship, industrial design with visible cargo pods, yellow and orange warning stripes on dull gray metal hull, 48x48 pixel sprite, transparent background"

**Pirate NPC**
- **Filename:** `pirate-npc.png`
- **Dimensions:** 56x56 pixels
- **Style:** Aggressive fighter, makeshift armor plating
- **Colors:** Red markings, rusted metal, menacing
- **AI Prompt:** "Top-down view of a hostile pirate spaceship, aggressive angular design with makeshift armor plating, red skull markings on rusted metal hull, 56x56 pixel sprite, transparent background, menacing appearance"

---

## Building Sprites

### Colony Buildings (Priority: HIGH)

**Storage Building**
- **Filename:** `storage.png`
- **Dimensions:** 128x128 pixels
- **Style:** Large warehouse structure
- **Colors:** Gray metal, blue energy indicators
- **AI Prompt:** "Isometric view of a futuristic storage warehouse building, large cylindrical silos, gray metallic structure with blue glowing energy indicators, 128x128 pixel sprite, transparent background, sci-fi colony building"

**Stone Extractor**
- **Filename:** `stone-extractor.png`
- **Dimensions:** 128x128 pixels
- **Style:** Mining drill facility
- **Colors:** Brown/gray industrial, rotating drill visible
- **AI Prompt:** "Isometric view of a resource extraction facility with large drill, industrial brown and gray colors, mechanical moving parts visible, 128x128 pixel sprite, transparent background, mining building"

**Ice Extractor**
- **Filename:** `ice-extractor.png`
- **Dimensions:** 128x128 pixels
- **Style:** Refinery with cooling towers
- **Colors:** White/cyan ice crystals, blue coolant pipes
- **AI Prompt:** "Isometric view of an ice extraction refinery, white and cyan ice crystal accents, blue glowing coolant pipes, 128x128 pixel sprite, transparent background, cold industrial aesthetic"

**Uranium Mine**
- **Filename:** `uranium-mine.png`
- **Dimensions:** 128x128 pixels
- **Style:** Heavily shielded mining complex
- **Colors:** Dark gray shielding, yellow radiation warnings
- **AI Prompt:** "Isometric view of a uranium mining facility, heavy dark gray radiation shielding, yellow and black hazard warning symbols, ominous green radioactive glow, 128x128 pixel sprite, transparent background, dangerous industrial building"

**Fuel Factory**
- **Filename:** `fuel-factory.png`
- **Dimensions:** 128x128 pixels
- **Style:** Refinery with processing tanks
- **Colors:** Orange flames, silver tanks, black smoke
- **AI Prompt:** "Isometric view of a fuel refining factory, large silver processing tanks, orange flames and black smoke stacks, industrial complex design, 128x128 pixel sprite, transparent background, energy production facility"

---

## Planet Sprites

### Planet Types (Priority: MEDIUM)

**Agrarian Planet**
- **Filename:** `agrarian.png`
- **Dimensions:** 256x256 pixels
- **Colors:** Green continents, blue oceans, white clouds
- **AI Prompt:** "Top-down view of an Earth-like habitable planet, green and brown continents with blue oceans, swirling white clouds, 256x256 pixel sprite, transparent background, welcoming appearance"

**Military Planet (Volcanic)**
- **Filename:** `military.png`
- **Dimensions:** 256x256 pixels
- **Colors:** Red/orange lava, black volcanic rock, smoke
- **AI Prompt:** "Top-down view of a volcanic military planet, glowing red and orange lava flows on black volcanic rock surface, smoke and ash clouds, 256x256 pixel sprite, transparent background, hostile dangerous appearance"

**Mining Planet (Gas Giant)**
- **Filename:** `mining.png`
- **Dimensions:** 256x256 pixels
- **Colors:** Purple/brown gas swirls, massive storms
- **AI Prompt:** "Top-down view of a gas giant planet, swirling purple and brown atmospheric bands, massive storm systems visible, no solid surface, 256x256 pixel sprite, transparent background, Jupiter-like appearance"

**Industrial Planet**
- **Filename:** `industrial.png`
- **Dimensions:** 256x256 pixels
- **Colors:** Gray cities, smog, lights
- **AI Prompt:** "Top-down view of an industrialized planet, gray metal cities covering surface, yellow-brown smog atmosphere, city lights visible on dark side, 256x256 pixel sprite, transparent background, polluted but advanced"

**Scientific Planet (Desert)**
- **Filename:** `scientific.png`
- **Dimensions:** 256x256 pixels
- **Colors:** Tan/beige deserts, red rocky areas
- **AI Prompt:** "Top-down view of a desert planet, tan and beige sandy dunes with red rocky regions, thin atmosphere, research facilities visible as small dots, 256x256 pixel sprite, transparent background, Mars-like appearance"

---

## Space Object Sprites

### Destructible Objects (Priority: MEDIUM)

**Asteroid**
- **Filename:** `asteroid.png`
- **Dimensions:** 32x32 pixels
- **Colors:** Gray/brown rock, cratered surface
- **AI Prompt:** "Top-down view of a space asteroid, irregular rocky shape, gray and brown cratered surface, 32x32 pixel sprite, transparent background, simple space rock"

**Comet**
- **Filename:** `comet.png`
- **Dimensions:** 48x32 pixels (with tail)
- **Colors:** Ice blue core, white/cyan tail
- **AI Prompt:** "Side view of a comet with glowing tail, bright blue icy core with trailing white and cyan tail particles, 48x32 pixel sprite, transparent background, moving through space"

---

## UI Elements

### Resource Icons (Priority: CRITICAL)

**Stone Icon**
- **Filename:** `icon-stone.png`
- **Dimensions:** 32x32 pixels
- **Style:** Rocky boulder icon
- **AI Prompt:** "Icon of a gray rocky stone boulder, simple design, 32x32 pixels, transparent background, game UI icon style"

**Ice Icon**
- **Filename:** `icon-ice.png`
- **Dimensions:** 32x32 pixels
- **Style:** Blue ice crystal
- **AI Prompt:** "Icon of a blue ice crystal, geometric facets, 32x32 pixels, transparent background, game UI icon style"

**Uranium Icon**
- **Filename:** `icon-uranium.png`
- **Dimensions:** 32x32 pixels
- **Style:** Radioactive symbol on barrel
- **AI Prompt:** "Icon of a radioactive uranium container, yellow radiation symbol on dark barrel, 32x32 pixels, transparent background, game UI icon style"

**Fuel Icon**
- **Filename:** `icon-fuel.png`
- **Dimensions:** 32x32 pixels
- **Style:** Fuel canister with flame
- **AI Prompt:** "Icon of a fuel canister with orange flame symbol, metallic container, 32x32 pixels, transparent background, game UI icon style"

**Credits Icon**
- **Filename:** `icon-credits.png`
- **Dimensions:** 32x32 pixels
- **Style:** Gold coin
- **AI Prompt:** "Icon of a gold coin with star emblem, shiny metallic gold, 32x32 pixels, transparent background, game currency icon"

### UI Panels (Priority: LOW - Can Use CSS)

**Panel Background**
- **Filename:** `panel.png`
- **Dimensions:** 512x512 pixels (tileable)
- **Style:** Semi-transparent dark panel
- **AI Prompt:** "Tileable UI panel background texture, dark semi-transparent blue-gray with subtle tech pattern, 512x512 pixels, game interface design"

**Button**
- **Filename:** `button.png`
- **Dimensions:** 200x50 pixels
- **Style:** Futuristic button
- **AI Prompt:** "Futuristic UI button, metallic blue-gray with subtle glow effect, 200x50 pixels, transparent background, game interface button"

---

## Visual Effects (Priority: LOW)

### Combat Effects

**Laser Beam**
- **Filename:** `laser-beam.png`
- **Dimensions:** 4x32 pixels (stretched in code)
- **Colors:** Bright cyan/blue energy
- **AI Prompt:** "Vertical laser beam sprite, bright cyan blue energy, 4x32 pixels, transparent background, weapon effect"

**Explosion (Sprite Sheet)**
- **Filename:** `explosion.png`
- **Dimensions:** 256x64 pixels (4 frames of 64x64)
- **Colors:** Orange/red/yellow flames, expanding blast
- **AI Prompt:** "4-frame explosion animation sprite sheet, expanding orange red and yellow fire blast, 256x64 pixels total (4 frames of 64x64), transparent background, game explosion effect"

**Particle Effects**
- **Filename:** `particles.png`
- **Dimensions:** 128x128 pixels (various small particles)
- **Colors:** White, blue, orange dots
- **AI Prompt:** "Collection of small particle sprites for effects, white blue orange glowing dots and sparkles of various sizes, 128x128 pixel sheet, transparent background"

---

## Audio Assets (Optional for MVP)

### Sound Effects

**Laser Shot**
- **Filename:** `laser-shot.mp3`
- **Duration:** 0.5 seconds
- **Description:** "Sci-fi laser weapon firing sound, electronic zap"

**Explosion**
- **Filename:** `explosion.mp3`
- **Duration:** 1 second
- **Description:** "Space explosion sound, deep bass boom"

**UI Click**
- **Filename:** `ui-click.mp3`
- **Duration:** 0.2 seconds
- **Description:** "UI button click, satisfying mechanical sound"

**Resource Collection**
- **Filename:** `collect.mp3`
- **Duration:** 0.3 seconds
- **Description:** "Resource collection sound, pleasant chime"

**Hyperspace Jump**
- **Filename:** `hyperspace.mp3`
- **Duration:** 2 seconds
- **Description:** "Hyperspace travel sound, whoosh with energy buildup"

---

## Asset Generation Workflow

### Phase 1: Hero Assets (Week 1)
1. Generate 3 player ships (scout, fighter, destroyer)
2. Generate 5 resource icons (stone, ice, uranium, fuel, credits)
3. Test in-game integration
4. Iterate based on visual cohesion

### Phase 2: Buildings (Week 3)
5. Generate 5 building sprites
6. Ensure consistent isometric angle
7. Test in colony view
8. Add visual variety if needed

### Phase 3: Environment (Week 5)
9. Generate 5 planet types
10. Generate asteroid and comet sprites
11. Test in space view
12. Polish lighting/colors for consistency

### Phase 4: Effects & Polish (Week 9)
13. Generate laser and explosion sprites
14. Test combat visual feedback
15. Add UI panels if needed
16. Generate sound effects (optional)

---

## Asset Integration Guide

### Loading Assets in Code

```javascript
// src/systems/assets.js
export class AssetManager {
  constructor() {
    this.images = new Map();
    this.sounds = new Map();
  }
  
  async loadAll() {
    const imageList = [
      // Ships
      { key: 'scout', path: 'assets/sprites/ships/scout.png' },
      { key: 'fighter', path: 'assets/sprites/ships/fighter.png' },
      { key: 'destroyer', path: 'assets/sprites/ships/destroyer.png' },
      { key: 'miner-npc', path: 'assets/sprites/ships/miner-npc.png' },
      { key: 'pirate-npc', path: 'assets/sprites/ships/pirate-npc.png' },
      
      // Buildings
      { key: 'storage', path: 'assets/sprites/buildings/storage.png' },
      { key: 'stone-extractor', path: 'assets/sprites/buildings/stone-extractor.png' },
      // ... etc
    ];
    
    await Promise.all(imageList.map(asset => this.loadImage(asset)));
  }
  
  loadImage(asset) {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => {
        this.images.set(asset.key, img);
        resolve();
      };
      img.onerror = () => {
        console.warn(`Failed to load ${asset.path}, using placeholder`);
        resolve(); // Don't fail, just use placeholder
      };
      img.src = asset.path;
    });
  }
  
  get(key) {
    return this.images.get(key) || null;
  }
}
```

### Using Assets in Rendering

```javascript
// If sprite loaded, use it; otherwise use placeholder
const sprite = assetManager.get('scout');

if (sprite) {
  ctx.drawImage(sprite, x - 32, y - 32, 64, 64);
} else {
  // Placeholder: colored shape
  ctx.fillStyle = '#00ff00';
  ctx.fillRect(x - 32, y - 32, 64, 64);
}
```

---

## Asset Checklist

**Before Development Starts:**
- [ ] Decide: Generate all assets upfront OR use placeholders?
- [ ] Create `/assets` folder structure
- [ ] If generating: Prepare AI generation prompts
- [ ] If placeholders: Document placeholder color scheme

**Phase 1 (Critical Path):**
- [ ] 3 player ship sprites
- [ ] 2 NPC ship sprites
- [ ] 5 resource icons
- [ ] Test asset loading in game

**Phase 2 (Building System):**
- [ ] 5 building sprites
- [ ] Test in colony view
- [ ] Verify consistent style

**Phase 3 (Space Environment):**
- [ ] 5 planet sprites
- [ ] Asteroid sprite
- [ ] Comet sprite
- [ ] Test in space view

**Phase 4 (Polish):**
- [ ] Laser effect
- [ ] Explosion sprite sheet
- [ ] UI panels (if needed)
- [ ] Sound effects (optional)

---

**Document Version:** 1.0  
**Last Updated:** November 24, 2025  
**Next:** implementation-guide.md
