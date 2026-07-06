import type { StarSystem } from "./types";

export const systems: StarSystem[] = [
  {
    id: "aurora",
    name: "Aurora Gate",
    tint: "#62b6ff",
    danger: 1,
    description: "Starter frontier with science colonies and safe trade lanes.",
    planets: [
      { id: "myth-sec", name: "Myth-Sec", radius: 0.82, orbit: 4.3, angle: 0.3, colorA: "#2264aa", colorB: "#7fd7ff", resource: "ice", population: "Science colony" },
      { id: "rime", name: "Rime", radius: 0.55, orbit: 6.1, angle: 2.2, colorA: "#4fd2d9", colorB: "#d9ffff", resource: "ice", population: "Mining outpost" },
      { id: "vesta-nine", name: "Vesta Nine", radius: 0.62, orbit: 7.8, angle: 4.5, colorA: "#767b8f", colorB: "#e3ddc8", resource: "stone", population: "Supply depot" }
    ]
  },
  {
    id: "cinder",
    name: "Cinder March",
    tint: "#ff6a3d",
    danger: 2,
    description: "Hot military border where pirates raid refinery convoys.",
    planets: [
      { id: "cinder-prime", name: "Cinder Prime", radius: 0.9, orbit: 4.7, angle: 1.1, colorA: "#9f2f20", colorB: "#ffb15e", resource: "fuel", population: "Refinery world" },
      { id: "brass-moon", name: "Brass Moon", radius: 0.5, orbit: 6.6, angle: 3.1, colorA: "#8f6a22", colorB: "#f2d068", resource: "uranium", population: "Fortress moon" },
      { id: "ember", name: "Ember", radius: 0.7, orbit: 8.3, angle: 5.2, colorA: "#442020", colorB: "#ff5b42", resource: "stone", population: "Ash farms" }
    ]
  },
  {
    id: "nyx",
    name: "Nyx Frontier",
    tint: "#b071ff",
    danger: 3,
    description: "Deep-space mining region with rare uranium and hostile patrols.",
    planets: [
      { id: "nyx-hold", name: "Nyx Hold", radius: 0.78, orbit: 4.5, angle: 0.7, colorA: "#462266", colorB: "#d0a0ff", resource: "uranium", population: "Independent miners" },
      { id: "black-reef", name: "Black Reef", radius: 0.58, orbit: 6.4, angle: 2.7, colorA: "#1f2430", colorB: "#6585ff", resource: "uranium", population: "Hidden station" },
      { id: "quietus", name: "Quietus", radius: 0.68, orbit: 8.4, angle: 4.7, colorA: "#1c3246", colorB: "#89ffe0", resource: "ice", population: "Research vault" }
    ]
  }
];
