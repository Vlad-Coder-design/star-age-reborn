import type { Vector3Tuple } from "./types";

export function length3(vector: Vector3Tuple) {
  return Math.hypot(vector[0], vector[1], vector[2]);
}

export function integrateFlight(
  position: Vector3Tuple,
  velocity: Vector3Tuple,
  destination: Vector3Tuple | null,
  delta: number
) {
  const nextVelocity: Vector3Tuple = [...velocity];
  const nextPosition: Vector3Tuple = [...position];
  let nextDestination = destination;

  if (destination) {
    const dx = destination[0] - position[0];
    const dz = destination[2] - position[2];
    const distance = Math.hypot(dx, dz);
    const speed = Math.hypot(nextVelocity[0], nextVelocity[2]);
    const brakeAcceleration = 8.5;
    const stoppingDistance = speed * speed / (2 * brakeAcceleration);

    if (distance < 0.32 && speed < 0.36) {
      nextPosition[0] = destination[0];
      nextPosition[2] = destination[2];
      nextVelocity[0] = 0;
      nextVelocity[2] = 0;
      nextDestination = null;
    } else if (speed > 0.05 && distance <= stoppingDistance + 0.35) {
      nextVelocity[0] -= (nextVelocity[0] / speed) * brakeAcceleration * delta;
      nextVelocity[2] -= (nextVelocity[2] / speed) * brakeAcceleration * delta;
    } else if (distance > 0.25) {
      const acceleration = 7.5;
      nextVelocity[0] += (dx / distance) * acceleration * delta;
      nextVelocity[2] += (dz / distance) * acceleration * delta;
    }
  }

  const speed = Math.hypot(nextVelocity[0], nextVelocity[2]);
  const maxSpeed = 5.6;
  if (speed > maxSpeed) {
    nextVelocity[0] = (nextVelocity[0] / speed) * maxSpeed;
    nextVelocity[2] = (nextVelocity[2] / speed) * maxSpeed;
  }

  const drag = Math.pow(0.82, delta);
  nextVelocity[0] *= drag;
  nextVelocity[2] *= drag;

  if (Math.hypot(nextVelocity[0], nextVelocity[2]) < 0.035) {
    nextVelocity[0] = 0;
    nextVelocity[2] = 0;
  }

  nextPosition[0] += nextVelocity[0] * delta;
  nextPosition[2] += nextVelocity[2] * delta;

  return {
    position: nextPosition,
    velocity: nextVelocity,
    speed: Math.hypot(nextVelocity[0], nextVelocity[2]),
    destination: nextDestination
  };
}
