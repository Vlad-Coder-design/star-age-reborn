export class InputHandler {
  constructor(canvas) {
    this.canvas = canvas;
    this.onClick = null;
    this._handlePointerDown = this._handlePointerDown.bind(this);
    this._registerEvents();
  }

  _registerEvents() {
    this.canvas.addEventListener('pointerdown', this._handlePointerDown);
  }

  dispose() {
    this.canvas.removeEventListener('pointerdown', this._handlePointerDown);
  }

  _handlePointerDown(event) {
    if (typeof this.onClick !== 'function') {
      return;
    }

    const rect = this.canvas.getBoundingClientRect();
    const x = ((event.clientX - rect.left) / rect.width) * this.canvas.width;
    const y = ((event.clientY - rect.top) / rect.height) * this.canvas.height;
    this.onClick(x, y, event);
  }
}

