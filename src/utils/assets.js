export function loadImage(src) {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.crossOrigin = 'anonymous'; // Allow CORS if needed
    
    img.onload = () => {
      console.log(`Image loaded successfully: ${src}`);
      resolve(img);
    };
    
    img.onerror = (error) => {
      console.error(`Failed to load image: ${src}`, error);
      reject(new Error(`Failed to load image: ${src}`));
    };
    
    img.src = src;
  });
}

