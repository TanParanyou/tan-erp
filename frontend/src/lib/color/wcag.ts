/**
 * WCAG 2.x contrast helpers and organization brand colour derivation (design.md 7.4, ADR 0031).
 * Mirrors backend TanErp.Domain.Organization.BrandColor and mobile/lib/core/theme/brand_color.dart.
 */

import { BRAND_RULES, DARK_CANVAS, HOVER_TARGET, LIGHT_CANVAS, WHITE } from "./brand-rules.g";

export type Rgb = readonly [number, number, number];

export const MIN_TEXT_CONTRAST = BRAND_RULES.textContrastMin;
export const MIN_CANVAS_CONTRAST = BRAND_RULES.canvasContrastMin;

const HEX_PATTERN = /^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$/;

export function parseHexColor(value: string): Rgb | null {
  const match = HEX_PATTERN.exec(value.trim());
  if (!match) return null;
  return [parseInt(match[1], 16), parseInt(match[2], 16), parseInt(match[3], 16)];
}

export function relativeLuminance([r, g, b]: Rgb): number {
  const linear = (channel: number): number => {
    const s = channel / 255;
    return s <= 0.03928 ? s / 12.92 : Math.pow((s + 0.055) / 1.055, 2.4);
  };
  return 0.2126 * linear(r) + 0.7152 * linear(g) + 0.0722 * linear(b);
}

export function contrastRatio(a: Rgb, b: Rgb): number {
  const la = relativeLuminance(a);
  const lb = relativeLuminance(b);
  return (Math.max(la, lb) + 0.05) / (Math.min(la, lb) + 0.05);
}

export interface BrandContrastResult {
  textRatio: number;
  canvasRatio: number;
  textPasses: boolean;
  canvasPasses: boolean;
}

/** White text on the colour (>= 4.5:1) and the colour on the light canvas (>= 3:1). */
export function checkBrandContrast(color: Rgb): BrandContrastResult {
  const textRatio = contrastRatio(WHITE, color);
  const canvasRatio = contrastRatio(color, LIGHT_CANVAS);
  return {
    textRatio,
    canvasRatio,
    textPasses: textRatio >= MIN_TEXT_CONTRAST,
    canvasPasses: canvasRatio >= MIN_CANVAS_CONTRAST,
  };
}

function mix(from: Rgb, to: Rgb, amount: number): Rgb {
  const channel = (i: 0 | 1 | 2): number => Math.round(from[i] + (to[i] - from[i]) * amount);
  return [channel(0), channel(1), channel(2)];
}

/**
 * Dark-mode action colour: lightens toward white until the fill stands out from the dark canvas (>= 3:1)
 * while white text on it still passes (>= 4.5:1); keeps the original colour if no step satisfies both.
 */
export function darkVariant(color: Rgb): Rgb {
  for (let step = 0; step <= BRAND_RULES.darkVariantSteps; step += 1) {
    const candidate = mix(color, WHITE, step * BRAND_RULES.darkVariantStep);
    if (contrastRatio(WHITE, candidate) < MIN_TEXT_CONTRAST) break;
    if (contrastRatio(candidate, DARK_CANVAS) >= MIN_CANVAS_CONTRAST) return candidate;
  }
  return color;
}

export function toCssColor([r, g, b]: Rgb): string {
  return `rgb(${r} ${g} ${b})`;
}

/** CSS custom properties that replace the Atelier Navy action tokens for the active theme. */
export const BRAND_CSS_VARIABLES = ["--erp-navy", "--erp-navy-hover", "--erp-navy-border", "--erp-navy-light"] as const;

export function brandCssVariables(color: Rgb, theme: "light" | "dark"): Record<(typeof BRAND_CSS_VARIABLES)[number], string> {
  const action = theme === "dark" ? darkVariant(color) : color;
  return {
    "--erp-navy": toCssColor(action),
    "--erp-navy-hover": toCssColor(mix(action, HOVER_TARGET, BRAND_RULES.hoverMixRatio)),
    "--erp-navy-border": toCssColor(action),
    "--erp-navy-light": toCssColor(theme === "dark" ? mix(DARK_CANVAS, action, BRAND_RULES.lightMixRatioDarkTheme) : mix(action, WHITE, BRAND_RULES.lightMixRatioLightTheme)),
  };
}
