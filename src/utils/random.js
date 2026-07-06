export function randomFloat(min, max) {
  return Math.random() * (max - min) + min;
}

export function randomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

export function chooseRandom(array) {
  if (!Array.isArray(array) || array.length === 0) {
    throw new Error('Cannot choose random item from empty array');
  }
  const index = randomInt(0, array.length - 1);
  return array[index];
}

