type ClassValue = string | number | bigint | boolean | undefined | null | { [key: string]: unknown } | ClassValue[];

/**
 * Combines multiple CSS class values into a single space-separated string.
 * Lightweight, zero-dependency alternative to clsx/tailwind-merge.
 */
export function cn(...inputs: ClassValue[]): string {
  const classes: string[] = [];

  for (const input of inputs) {
    if (!input) continue;

    if (typeof input === "string" || typeof input === "number") {
      classes.push(String(input));
    } else if (Array.isArray(input)) {
      const nested = cn(...input);
      if (nested) classes.push(nested);
    } else if (typeof input === "object") {
      for (const [key, value] of Object.entries(input)) {
        if (value) classes.push(key);
      }
    }
  }

  return classes.join(" ");
}
