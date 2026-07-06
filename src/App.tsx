import { useMemo, useState } from "react";
import { SpaceScene } from "./components/SpaceScene";
import { GameHud } from "./components/GameHud";
import { FallbackCanvas } from "./components/FallbackCanvas";
import { systems } from "./game/gameData";
import { hasWebGL } from "./game/webgl";
import type { FlightState, GameState, Planet } from "./game/types";

const initialFlight: FlightState = {
  position: [0, 0, 0],
  velocity: [0, 0, 0],
  speed: 0,
  destination: null
};

const initialGame: GameState = {
  credits: 1000,
  hp: 100,
  cargo: { stone: 0, ice: 0, uranium: 0, fuel: 0 },
  currentSystem: systems[0],
  selectedPlanet: systems[0].planets[0]
};

export function App() {
  const webglSupported = useMemo(() => hasWebGL(), []);
  const [game, setGame] = useState<GameState>(initialGame);
  const [flight, setFlight] = useState<FlightState>(initialFlight);

  function selectPlanet(planet: Planet) {
    setGame((current) => ({ ...current, selectedPlanet: planet }));
  }

  function travel(systemId: string) {
    const nextSystem = systems.find((system) => system.id === systemId);
    if (!nextSystem || nextSystem.id === game.currentSystem.id) return;

    setGame((current) => ({
      ...current,
      credits: Math.max(0, current.credits - 25),
      currentSystem: nextSystem,
      selectedPlanet: nextSystem.planets[0]
    }));
    setFlight(initialFlight);
  }

  function mine() {
    setGame((current) => ({
      ...current,
      cargo: {
        stone: current.cargo.stone + 3,
        ice: current.cargo.ice + 2,
        uranium: current.cargo.uranium + (current.currentSystem.danger > 1 ? 1 : 0),
        fuel: current.cargo.fuel + 1
      }
    }));
  }

  function trade() {
    setGame((current) => {
      const payout =
        current.cargo.stone * 4 +
        current.cargo.ice * 5 +
        current.cargo.uranium * 20 +
        current.cargo.fuel * 8;

      return {
        ...current,
        credits: current.credits + payout,
        cargo: { stone: 0, ice: 0, uranium: 0, fuel: 0 }
      };
    });
  }

  if (!webglSupported) {
    return <FallbackCanvas game={game} />;
  }

  return (
    <main className="app-shell">
      <SpaceScene
        system={game.currentSystem}
        selectedPlanet={game.selectedPlanet}
        onPlanetSelect={selectPlanet}
        onMine={mine}
        onFlightChange={setFlight}
      />
      <GameHud
        game={game}
        flight={flight}
        systems={systems}
        onTravel={travel}
        onMine={mine}
        onTrade={trade}
      />
    </main>
  );
}
