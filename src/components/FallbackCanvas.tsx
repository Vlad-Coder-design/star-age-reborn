import { useEffect, useRef } from "react";
import type { GameState } from "../game/types";

export function FallbackCanvas({ game }: { game: GameState }) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const context = canvas.getContext("2d");
    if (!context) return;

    const ratio = window.devicePixelRatio || 1;
    canvas.width = Math.floor(window.innerWidth * ratio);
    canvas.height = Math.floor(window.innerHeight * ratio);
    context.scale(ratio, ratio);
    context.fillStyle = "#02030a";
    context.fillRect(0, 0, window.innerWidth, window.innerHeight);
    context.fillStyle = "#d8ecff";
    context.font = "600 22px Segoe UI, sans-serif";
    context.fillText("Star Age Reborn", 28, 48);
    context.font = "16px Segoe UI, sans-serif";
    context.fillText("Your browser does not support WebGL, so this safe fallback is showing.", 28, 84);
    context.fillText(`Current system: ${game.currentSystem.name}`, 28, 122);

    for (let i = 0; i < 140; i += 1) {
      context.globalAlpha = 0.25 + Math.random() * 0.75;
      context.fillStyle = "#ffffff";
      context.fillRect(Math.random() * window.innerWidth, Math.random() * window.innerHeight, 1.5, 1.5);
    }

    context.globalAlpha = 1;
  }, [game.currentSystem.name]);

  return <canvas ref={canvasRef} className="fallback-canvas" />;
}
