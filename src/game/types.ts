export type Vector3Tuple = [number, number, number];

export type Cargo = {
  stone: number;
  ice: number;
  uranium: number;
  fuel: number;
};

export type Planet = {
  id: string;
  name: string;
  radius: number;
  orbit: number;
  angle: number;
  colorA: string;
  colorB: string;
  resource: keyof Cargo;
  population: string;
};

export type StarSystem = {
  id: string;
  name: string;
  tint: string;
  danger: number;
  description: string;
  planets: Planet[];
};

export type FlightState = {
  position: Vector3Tuple;
  velocity: Vector3Tuple;
  speed: number;
  destination: Vector3Tuple | null;
};

export type GameState = {
  credits: number;
  hp: number;
  cargo: Cargo;
  currentSystem: StarSystem;
  selectedPlanet: Planet;
};
