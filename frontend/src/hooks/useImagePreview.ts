"use client";

import { useState, useCallback, useRef, useEffect } from "react";

export interface UseImagePreviewOptions {
  value?: string;
  onChange?: (file: File | null) => void;
  allowedTypes?: string[];
  maxSizeBytes?: number;
}

const DEFAULT_ALLOWED_TYPES = [
  "image/jpeg",
  "image/png",
  "image/webp",
];
const DEFAULT_MAX_SIZE = 5 * 1024 * 1024; // 5MB

export function useImagePreview(options: UseImagePreviewOptions = {}) {
  const {
    value: initialPreviewUrl = "",
    onChange,
    allowedTypes = DEFAULT_ALLOWED_TYPES,
    maxSizeBytes = DEFAULT_MAX_SIZE,
  } = options;

  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string>(initialPreviewUrl);
  const [error, setError] = useState<string | null>(null);
  const objectUrlRef = useRef<string | null>(null);

  const clearPreview = useCallback(() => {
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }
    setSelectedFile(null);
    setPreviewUrl("");
    setError(null);
    onChange?.(null);
  }, [onChange]);

  const selectFile = useCallback(
    (file: File) => {
      setError(null);

      if (allowedTypes.length > 0 && !allowedTypes.includes(file.type)) {
        setError("Invalid file type. Supported: JPG, PNG, WebP");
        return false;
      }

      if (file.size > maxSizeBytes) {
        const mb = Math.round(maxSizeBytes / (1024 * 1024));
        setError(`File exceeds maximum size of ${mb}MB`);
        return false;
      }

      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
      }

      const url = URL.createObjectURL(file);
      objectUrlRef.current = url;
      setSelectedFile(file);
      setPreviewUrl(url);
      onChange?.(file);
      return true;
    },
    [allowedTypes, maxSizeBytes, onChange]
  );

  useEffect(() => {
    return () => {
      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
      }
    };
  }, []);

  return {
    selectedFile,
    previewUrl,
    error,
    selectFile,
    clearPreview,
  };
}

export default useImagePreview;
