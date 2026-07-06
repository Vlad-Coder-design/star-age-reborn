import { Canvas, useFrame, useThree } from "@react-three/fiber";
import type { MutableRefObject } from "react";
import { useMemo, useRef } from "react";
import * as THREE from "three";
import { integrateFlight } from "../game/physics";
import type { FlightState, Planet, StarSystem, Vector3Tuple } from "../game/types";

type SpaceSceneProps = {
  system: StarSystem;
  selectedPlanet: Planet;
  onPlanetSelect: (planet: Planet) => void;
  onMine: () => void;
  onFlightChange: (flight: FlightState) => void;
};

export function SpaceScene({
  system,
  selectedPlanet,
  onPlanetSelect,
  onMine,
  onFlightChange
}: SpaceSceneProps) {
  return (
    <section className="scene-frame" aria-label="Star Age WebGL space scene">
      <Canvas
        dpr={[1, 1.8]}
        camera={{ position: [0, 8, 12], fov: 48, near: 0.1, far: 140 }}
        gl={{ antialias: true, powerPreference: "high-performance" }}
      >
        <color attach="background" args={["#02030a"]} />
        <fog attach="fog" args={["#02030a", 18, 48]} />
        <ambientLight intensity={0.35} />
        <pointLight position={[0, 1.2, 0]} intensity={5} color={system.tint} />
        <Starfield tint={system.tint} />
        <CentralStar tint={system.tint} />
        <OrbitRings planets={system.planets} />
        {system.planets.map((planet) => (
          <PlanetMesh
            key={planet.id}
            planet={planet}
            selected={planet.id === selectedPlanet.id}
            onSelect={() => onPlanetSelect(planet)}
          />
        ))}
        <ResourceBelt onMine={onMine} />
        <PlayerShip onFlightChange={onFlightChange} />
        <FlightPlane />
      </Canvas>
    </section>
  );
}

function CentralStar({ tint }: { tint: string }) {
  return (
    <mesh position={[0, 0, 0]}>
      <sphereGeometry args={[0.75, 48, 48]} />
      <meshBasicMaterial color={tint} />
      <pointLight intensity={2.2} distance={24} color={tint} />
    </mesh>
  );
}

function OrbitRings({ planets }: { planets: Planet[] }) {
  return (
    <group>
      {planets.map((planet) => (
        <mesh key={planet.id} rotation={[-Math.PI / 2, 0, 0]}>
          <ringGeometry args={[planet.orbit - 0.01, planet.orbit + 0.01, 96]} />
          <meshBasicMaterial color="#35536f" transparent opacity={0.24} side={THREE.DoubleSide} />
        </mesh>
      ))}
    </group>
  );
}

function PlanetMesh({
  planet,
  selected,
  onSelect
}: {
  planet: Planet;
  selected: boolean;
  onSelect: () => void;
}) {
  const planetRef = useRef<THREE.Mesh>(null);
  const material = useMemo(
    () =>
      new THREE.ShaderMaterial({
        uniforms: {
          time: { value: 0 },
          colorA: { value: new THREE.Color(planet.colorA) },
          colorB: { value: new THREE.Color(planet.colorB) },
          seed: { value: planet.angle * 17.13 }
        },
        vertexShader: `
          varying vec2 vUv;
          varying vec3 vNormal;
          void main() {
            vUv = uv;
            vNormal = normalize(normalMatrix * normal);
            gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
          }
        `,
        fragmentShader: `
          uniform float time;
          uniform vec3 colorA;
          uniform vec3 colorB;
          uniform float seed;
          varying vec2 vUv;
          varying vec3 vNormal;
          float hash(vec2 p) {
            return fract(sin(dot(p, vec2(127.1, 311.7)) + seed) * 43758.5453123);
          }
          float noise(vec2 p) {
            vec2 i = floor(p);
            vec2 f = fract(p);
            float a = hash(i);
            float b = hash(i + vec2(1.0, 0.0));
            float c = hash(i + vec2(0.0, 1.0));
            float d = hash(i + vec2(1.0, 1.0));
            vec2 u = f * f * (3.0 - 2.0 * f);
            return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
          }
          void main() {
            float bands = sin((vUv.y + noise(vUv * 7.0 + time * 0.015)) * 22.0) * 0.5 + 0.5;
            float continents = smoothstep(0.36, 0.72, noise(vUv * 10.0));
            float light = dot(normalize(vec3(0.4, 0.8, 0.25)), normalize(vNormal)) * 0.5 + 0.5;
            vec3 color = mix(colorA, colorB, continents * 0.75 + bands * 0.25);
            gl_FragColor = vec4(color * (0.35 + light * 0.85), 1.0);
          }
        `
      }),
    [planet.angle, planet.colorA, planet.colorB]
  );

  const atmosphere = useMemo(
    () =>
      new THREE.ShaderMaterial({
        transparent: true,
        depthWrite: false,
        side: THREE.BackSide,
        uniforms: { color: { value: new THREE.Color(planet.colorB) } },
        vertexShader: `
          varying vec3 vNormal;
          void main() {
            vNormal = normalize(normalMatrix * normal);
            gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
          }
        `,
        fragmentShader: `
          uniform vec3 color;
          varying vec3 vNormal;
          void main() {
            float rim = pow(1.0 - abs(vNormal.z), 2.4);
            gl_FragColor = vec4(color, rim * 0.42);
          }
        `
      }),
    [planet.colorB]
  );

  useFrame((_, delta) => {
    material.uniforms.time.value += delta;
    if (planetRef.current) planetRef.current.rotation.y += delta * 0.18;
  });

  const x = Math.cos(planet.angle) * planet.orbit;
  const z = Math.sin(planet.angle) * planet.orbit;

  return (
    <group position={[x, 0, z]} onClick={(event) => { event.stopPropagation(); onSelect(); }}>
      <mesh ref={planetRef} material={material}>
        <sphereGeometry args={[planet.radius, 64, 64]} />
      </mesh>
      <mesh material={atmosphere} scale={selected ? 1.22 : 1.13}>
        <sphereGeometry args={[planet.radius, 48, 48]} />
      </mesh>
      {selected && (
        <mesh rotation={[-Math.PI / 2, 0, 0]}>
          <ringGeometry args={[planet.radius + 0.18, planet.radius + 0.23, 64]} />
          <meshBasicMaterial color="#ffffff" transparent opacity={0.68} side={THREE.DoubleSide} />
        </mesh>
      )}
    </group>
  );
}

function Starfield({ tint }: { tint: string }) {
  const points = useMemo(() => {
    const vertices: number[] = [];
    for (let i = 0; i < 900; i += 1) {
      vertices.push((Math.random() - 0.5) * 90, (Math.random() - 0.5) * 38, (Math.random() - 0.5) * 90);
    }
    return new Float32Array(vertices);
  }, [tint]);

  return (
    <points>
      <bufferGeometry>
        <bufferAttribute attach="attributes-position" args={[points, 3]} />
      </bufferGeometry>
      <pointsMaterial color="#d7ecff" size={0.045} sizeAttenuation transparent opacity={0.82} />
    </points>
  );
}

function ResourceBelt({ onMine }: { onMine: () => void }) {
  const rocks = useMemo(() => {
    return Array.from({ length: 28 }, (_, index) => {
      const angle = index * 1.71;
      const radius = 9 + (index % 6) * 0.36;
      return {
        id: index,
        position: [Math.cos(angle) * radius, -0.05, Math.sin(angle) * radius] as Vector3Tuple,
        scale: 0.08 + (index % 5) * 0.025
      };
    });
  }, []);

  return (
    <group>
      {rocks.map((rock) => (
        <mesh key={rock.id} position={rock.position} scale={rock.scale} onClick={(event) => { event.stopPropagation(); onMine(); }}>
          <dodecahedronGeometry args={[1, 0]} />
          <meshStandardMaterial color="#9b968b" roughness={0.9} metalness={0.05} />
        </mesh>
      ))}
    </group>
  );
}

function PlayerShip({ onFlightChange }: { onFlightChange: (flight: FlightState) => void }) {
  const ship = useRef<THREE.Group>(null);
  const state = useRef<{ position: Vector3Tuple; velocity: Vector3Tuple; destination: Vector3Tuple | null }>({
    position: [0, 0.1, 3.2],
    velocity: [0, 0, 0],
    destination: null
  });
  const { camera } = useThree();

  useFrame((_, delta) => {
    const result = integrateFlight(state.current.position, state.current.velocity, state.current.destination, delta);
    state.current.position = result.position;
    state.current.velocity = result.velocity;
    state.current.destination = result.destination;

    if (ship.current) {
      ship.current.position.set(...result.position);
    }

    camera.position.x += (result.position[0] - camera.position.x) * 0.035;
    camera.position.z += (result.position[2] + 10 - camera.position.z) * 0.035;
    camera.lookAt(result.position[0], 0, result.position[2]);

    onFlightChange({
      position: result.position,
      velocity: result.velocity,
      speed: result.speed,
      destination: state.current.destination
    });
  });

  return (
    <group ref={ship} position={state.current.position} scale={0.82}>
      <mesh rotation={[Math.PI / 2, 0, 0]}>
        <cylinderGeometry args={[0.18, 0.28, 1.18, 28]} />
        <meshStandardMaterial color="#c7d9e8" metalness={0.55} roughness={0.28} />
      </mesh>
      <mesh position={[0, 0, -0.74]} rotation={[-Math.PI / 2, 0, 0]}>
        <coneGeometry args={[0.2, 0.42, 28]} />
        <meshStandardMaterial color="#eef7ff" metalness={0.48} roughness={0.22} />
      </mesh>
      <mesh position={[0, 0.16, -0.22]} scale={[0.22, 0.1, 0.32]}>
        <sphereGeometry args={[1, 24, 16]} />
        <meshStandardMaterial color="#62c9ff" emissive="#0b4b72" emissiveIntensity={0.65} metalness={0.12} roughness={0.08} />
      </mesh>
      <mesh position={[-0.43, -0.03, 0.05]} rotation={[0, 0, -0.18]} scale={[0.58, 0.045, 0.2]}>
        <boxGeometry args={[1, 1, 1]} />
        <meshStandardMaterial color="#8ca6ba" metalness={0.42} roughness={0.32} />
      </mesh>
      <mesh position={[0.43, -0.03, 0.05]} rotation={[0, 0, 0.18]} scale={[0.58, 0.045, 0.2]}>
        <boxGeometry args={[1, 1, 1]} />
        <meshStandardMaterial color="#8ca6ba" metalness={0.42} roughness={0.32} />
      </mesh>
      <mesh position={[-0.18, -0.01, 0.65]} rotation={[Math.PI / 2, 0, 0]}>
        <cylinderGeometry args={[0.09, 0.11, 0.24, 18]} />
        <meshStandardMaterial color="#596b7a" metalness={0.7} roughness={0.24} />
      </mesh>
      <mesh position={[0.18, -0.01, 0.65]} rotation={[Math.PI / 2, 0, 0]}>
        <cylinderGeometry args={[0.09, 0.11, 0.24, 18]} />
        <meshStandardMaterial color="#596b7a" metalness={0.7} roughness={0.24} />
      </mesh>
      <mesh position={[-0.18, -0.01, 0.82]} scale={[0.08, 0.08, 0.18]}>
        <sphereGeometry args={[1, 18, 18]} />
        <meshBasicMaterial color="#ffb25b" transparent opacity={0.72} />
      </mesh>
      <mesh position={[0.18, -0.01, 0.82]} scale={[0.08, 0.08, 0.18]}>
        <sphereGeometry args={[1, 18, 18]} />
        <meshBasicMaterial color="#ffb25b" transparent opacity={0.75} />
      </mesh>
      <mesh position={[0, -0.06, 0.31]} scale={[0.16, 0.035, 0.22]}>
        <boxGeometry args={[1, 1, 1]} />
        <meshStandardMaterial color="#3d5367" metalness={0.38} roughness={0.4} />
      </mesh>
      <FlightTargetBridge state={state} />
    </group>
  );
}

function FlightTargetBridge({
  state
}: {
  state: MutableRefObject<{ position: Vector3Tuple; velocity: Vector3Tuple; destination: Vector3Tuple | null }>;
}) {
  const { gl, camera } = useThree();
  const raycaster = useMemo(() => new THREE.Raycaster(), []);
  const plane = useMemo(() => new THREE.Plane(new THREE.Vector3(0, 1, 0), 0), []);
  const pointer = useMemo(() => new THREE.Vector2(), []);
  const intersection = useMemo(() => new THREE.Vector3(), []);

  useFrame(() => {
    gl.domElement.onclick = (event) => {
      const bounds = gl.domElement.getBoundingClientRect();
      pointer.x = ((event.clientX - bounds.left) / bounds.width) * 2 - 1;
      pointer.y = -((event.clientY - bounds.top) / bounds.height) * 2 + 1;
      raycaster.setFromCamera(pointer, camera);
      raycaster.ray.intersectPlane(plane, intersection);
      state.current.destination = [intersection.x, 0.1, intersection.z];
    };
  });

  return null;
}

function FlightPlane() {
  return (
    <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -0.08, 0]}>
      <planeGeometry args={[160, 160]} />
      <meshBasicMaterial color="#000000" transparent opacity={0} depthWrite={false} />
    </mesh>
  );
}
