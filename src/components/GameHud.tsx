import type { FlightState, GameState, StarSystem } from "../game/types";

type GameHudProps = {
  game: GameState;
  flight: FlightState;
  systems: StarSystem[];
  onTravel: (systemId: string) => void;
  onMine: () => void;
  onTrade: () => void;
};

export function GameHud({ game, flight, systems, onTravel, onMine, onTrade }: GameHudProps) {
  const cargoTotal = game.cargo.stone + game.cargo.ice + game.cargo.uranium + game.cargo.fuel;

  return (
    <div className="hud-layer">
      <section className="hud-panel status-panel">
        <div className="hud-kicker">Star Age Reborn</div>
        <h1>{game.currentSystem.name}</h1>
        <p>{game.currentSystem.description}</p>
        <div className="stat-grid">
          <span>Credits</span><strong>{game.credits}</strong>
          <span>Hull</span><strong>{game.hp}%</strong>
          <span>Speed</span><strong>{flight.speed.toFixed(1)}</strong>
          <span>Cargo</span><strong>{cargoTotal}/80</strong>
        </div>
      </section>

      <section className="hud-panel planet-panel">
        <div className="hud-kicker">Selected planet</div>
        <h2>{game.selectedPlanet.name}</h2>
        <p>{game.selectedPlanet.population}</p>
        <p>Primary resource: {game.selectedPlanet.resource}</p>
        <div className="button-row">
          <button onClick={onMine}>Mine</button>
          <button onClick={onTrade} disabled={cargoTotal === 0}>Trade Cargo</button>
        </div>
      </section>

      <section className="hud-panel map-panel">
        <div className="hud-kicker">Galaxy map</div>
        <div className="system-list">
          {systems.map((system) => (
            <button
              key={system.id}
              className={system.id === game.currentSystem.id ? "active" : ""}
              onClick={() => onTravel(system.id)}
            >
              <span>{system.name}</span>
              <small>Danger {system.danger}</small>
            </button>
          ))}
        </div>
      </section>

      <section className="hud-panel cargo-panel">
        <div className="hud-kicker">Cargo hold</div>
        <div className="cargo-row"><span>Stone</span><strong>{game.cargo.stone}</strong></div>
        <div className="cargo-row"><span>Ice</span><strong>{game.cargo.ice}</strong></div>
        <div className="cargo-row"><span>Uranium</span><strong>{game.cargo.uranium}</strong></div>
        <div className="cargo-row"><span>Fuel</span><strong>{game.cargo.fuel}</strong></div>
      </section>
    </div>
  );
}
