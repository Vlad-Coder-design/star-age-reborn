import { CONFIG } from './config.js';
import { Game } from './game.js';

let game;
let ctx;
let lastFrameTime = performance.now();

async function bootstrap() {
  const canvas = document.getElementById('game-canvas');
  if (!canvas) {
    throw new Error('Canvas element #game-canvas not found');
  }

  canvas.width = CONFIG.CANVAS_WIDTH;
  canvas.height = CONFIG.CANVAS_HEIGHT;
  ctx = canvas.getContext('2d');

  setupCanvasScaling();
  window.addEventListener('resize', setupCanvasScaling);

  game = new Game(canvas);
  await game.init();
  window.addEventListener('beforeunload', () => {
    game.saveGame();
  });

  lastFrameTime = performance.now();
  requestAnimationFrame(gameLoop);
}

function gameLoop(timestamp) {
  const deltaTime = (timestamp - lastFrameTime) / 1000;
  lastFrameTime = timestamp;

  game.update(deltaTime);
  game.render(ctx);

  requestAnimationFrame(gameLoop);
}

function setupCanvasScaling() {
  const wrapper = document.getElementById('game-wrapper');
  const canvas = document.getElementById('game-canvas');
  if (!wrapper || !canvas) return;

  // Calculate scale to fit screen while maintaining aspect ratio
  const scaleX = window.innerWidth / CONFIG.CANVAS_WIDTH;
  const scaleY = window.innerHeight / CONFIG.CANVAS_HEIGHT;
  const scale = Math.min(scaleX, scaleY, 1.0); // Cap at 1.0 to avoid upscaling on large screens

  // Apply scaling
  canvas.style.width = `${CONFIG.CANVAS_WIDTH * scale}px`;
  canvas.style.height = `${CONFIG.CANVAS_HEIGHT * scale}px`;
  
  // Store scale for mouse coordinate transformation
  canvas.dataset.scale = scale;
}

bootstrap().catch((error) => {
  console.error('Failed to bootstrap game:', error);
});
