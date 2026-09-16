export interface OptimizeImageOptions {
  maxDimension?: number;
  quality?: number;
}

export interface OptimizationResult {
  file: File;
  originalSize: number;
  optimizedSize: number;
  savedBytes: number;
  savedPercent: number;
  isOptimized: boolean;
}

/**
 * Optimizes an image file by resizing if larger than maxDimension and converting to WebP format.
 * Runs completely in the user's browser via HTML5 Canvas API (0 server CPU load).
 */
export async function optimizeImageToWebP(
  file: File,
  options: OptimizeImageOptions = {}
): Promise<OptimizationResult> {
  const maxDimension = options.maxDimension ?? 2048;
  const quality = options.quality ?? 0.85;

  const originalSize = file.size;

  // Only optimize raster images (JPEG, PNG, WEBP, BMP)
  const isEligibleImage =
    file.type === "image/jpeg" ||
    file.type === "image/png" ||
    file.type === "image/webp" ||
    file.type === "image/bmp";

  if (!isEligibleImage || typeof window === "undefined") {
    return {
      file,
      originalSize,
      optimizedSize: originalSize,
      savedBytes: 0,
      savedPercent: 0,
      isOptimized: false,
    };
  }

  return new Promise((resolve) => {
    const img = new Image();
    const objectUrl = URL.createObjectURL(file);

    img.onload = () => {
      URL.revokeObjectURL(objectUrl);

      let width = img.width;
      let height = img.height;

      // Downscale if exceeds maxDimension while preserving aspect ratio
      if (width > maxDimension || height > maxDimension) {
        if (width > height) {
          height = Math.round((height * maxDimension) / width);
          width = maxDimension;
        } else {
          width = Math.round((width * maxDimension) / height);
          height = maxDimension;
        }
      }

      const canvas = document.createElement("canvas");
      canvas.width = width;
      canvas.height = height;

      const ctx = canvas.getContext("2d");
      if (!ctx) {
        resolve({
          file,
          originalSize,
          optimizedSize: originalSize,
          savedBytes: 0,
          savedPercent: 0,
          isOptimized: false,
        });
        return;
      }

      ctx.drawImage(img, 0, 0, width, height);

      canvas.toBlob(
        (blob) => {
          if (!blob) {
            resolve({
              file,
              originalSize,
              optimizedSize: originalSize,
              savedBytes: 0,
              savedPercent: 0,
              isOptimized: false,
            });
            return;
          }

          // Generate new filename with .webp extension
          const originalName = file.name;
          const baseName = originalName.substring(0, originalName.lastIndexOf(".")) || originalName;
          const newFilename = `${baseName}.webp`;

          const optimizedFile = new File([blob], newFilename, {
            type: "image/webp",
            lastModified: Date.now(),
          });

          const optimizedSize = optimizedFile.size;
          const savedBytes = Math.max(0, originalSize - optimizedSize);
          const savedPercent = originalSize > 0 ? Math.round((savedBytes / originalSize) * 100) : 0;

          resolve({
            file: optimizedFile,
            originalSize,
            optimizedSize,
            savedBytes,
            savedPercent,
            isOptimized: true,
          });
        },
        "image/webp",
        quality
      );
    };

    img.onerror = () => {
      URL.revokeObjectURL(objectUrl);
      resolve({
        file,
        originalSize,
        optimizedSize: originalSize,
        savedBytes: 0,
        savedPercent: 0,
        isOptimized: false,
      });
    };

    img.src = objectUrl;
  });
}

/**
 * Optimizes multiple image files concurrently in the user's browser.
 * Naturally strips EXIF metadata through HTML5 Canvas rendering.
 */
export async function optimizeMultipleImagesToWebP(
  files: File[],
  options: OptimizeImageOptions = {}
): Promise<OptimizationResult[]> {
  return Promise.all(files.map((file) => optimizeImageToWebP(file, options)));
}

