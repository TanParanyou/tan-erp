// Builds design-tokens/ (W3C DTCG) into CSS variables (web) and Dart constants (Flutter app).
// Run: npm run tokens:build
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import StyleDictionary from 'style-dictionary';

const here = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(here, '..');
const HEADER = 'generated — do not edit, run npm run tokens:build';

const base = [`${here}/primitive/**/*.json`, `${here}/semantic/**/*.json`];
const cssName = (t) => t.$extensions.tanerp.css;
const hex = (v) => v.replace('#', '').toUpperCase();
const camel = (css) => css.replace(/^erp-/, '').replace(/-(\w)/g, (_, c) => c.toUpperCase());

// Light build = primitive + semantic; dark build = same + themes/dark.json overriding paths.
const mk = (source, platforms) => new StyleDictionary({ source, log: { verbosity: 'silent' }, platforms });

let darkTokens = [];
let compactTokens = [];

StyleDictionary.registerFormat({
  name: 'tanerp/css',
  format: ({ dictionary }) => {
    const byPath = new Map(dictionary.allTokens.map((t) => [t.path.join('.'), cssName(t)]));
    // Keep references as var(--x) so light output matches the hand-written erp-theme.css.
    const light = dictionary.allTokens.map((t) => {
      const v = typeof t.original.$value === 'string'
        ? t.original.$value.replace(/\{([^}]+)\}/g, (_, p) => `var(--${byPath.get(p)})`)
        : t.$value;
      return `  --${cssName(t)}: ${v};`;
    });
    const dark = darkTokens.filter((t) => t.filePath.endsWith('dark.json')).map((t) => `  --${cssName(t)}: ${t.$value};`);
    const compact = compactTokens.filter((t) => t.filePath.endsWith('compact.json')).map((t) => `  --${cssName(t)}: ${t.$value};`);
    return `/* ${HEADER} */\n\n:root {\n${light.join('\n')}\n}\n\n[data-theme="dark"] {\n${dark.join('\n')}\n}\n\n[data-density="compact"] {\n${compact.join('\n')}\n}\n`;
  },
});

StyleDictionary.registerFormat({
  name: 'tanerp/dart',
  format: ({ dictionary }) => {
    const colors = (tokens) => tokens.filter((t) => t.$type === 'color')
      .map((t) => `  static const Color ${camel(cssName(t))} = Color(0xFF${hex(t.$value)});`).join('\n');
    const nums = dictionary.allTokens.filter((t) => t.$type === 'dimension' && !t.$extensions.tanerp.webOnly)
      .map((t) => `  static const double ${camel(cssName(t))} = ${parseFloat(t.$value)};`).join('\n');
    return `// ${HEADER}\n// ignore_for_file: public_member_api_docs\n\nimport 'dart:ui' show Color;\n\n`
      + `abstract final class ErpTokensLight {\n${colors(dictionary.allTokens)}\n}\n\n`
      + `abstract final class ErpTokensDark {\n${colors(darkTokens)}\n}\n\n`
      + `abstract final class ErpTokensScale {\n${nums}\n}\n`;
  },
});

const dark = mk([...base, `${here}/themes/dark.json`], { x: {} });
await dark.hasInitialized;
darkTokens = (await dark.getPlatformTokens('x')).allTokens;

const compact = mk([...base, `${here}/themes/compact.json`], { x: {} });
await compact.hasInitialized;
compactTokens = (await compact.getPlatformTokens('x')).allTokens;

const light = mk(base, {
  css: {
    buildPath: `${root}/frontend/src/styles/`,
    files: [{ destination: 'tokens.css', format: 'tanerp/css' }],
  },
  dart: {
    buildPath: `${root}/mobile/lib/core/theme/`,
    files: [{ destination: 'tokens.g.dart', format: 'tanerp/dart' }],
  },
});
await light.buildAllPlatforms();

// Brand colour rules (brand-rules.json) + canvas/white tokens -> TS, C# and Dart constants.
const rules = JSON.parse(fs.readFileSync(`${here}/brand-rules.json`, 'utf8')).brand;
const lightByCss = new Map((await light.getPlatformTokens('css')).allTokens.map((t) => [cssName(t), t.$value]));
const darkByCss = new Map(darkTokens.map((t) => [cssName(t), t.$value]));
const rgb = (h) => [0, 2, 4].map((i) => parseInt(h.replace('#', '').slice(i, i + 2), 16));
const lightCanvas = rgb(lightByCss.get('erp-canvas'));
const darkCanvas = rgb(darkByCss.get('erp-canvas'));
const white = rgb(lightByCss.get('erp-text-on-navy'));
const hoverTarget = rgb(rules.hoverTarget.$value);
const v = (k) => rules[k].$value;
const write = (file, text) => fs.writeFileSync(`${root}/${file}`, text);

write('frontend/src/lib/color/brand-rules.g.ts', `// ${HEADER}
import type { Rgb } from "./wcag";

export const BRAND_RULES = {
  textContrastMin: ${v('textContrastMin')},
  canvasContrastMin: ${v('canvasContrastMin')},
  hoverMixRatio: ${v('hoverMixRatio')},
  lightMixRatioLightTheme: ${v('lightMixRatioLightTheme')},
  lightMixRatioDarkTheme: ${v('lightMixRatioDarkTheme')},
  darkVariantStep: ${v('darkVariantStep')},
  darkVariantSteps: ${v('darkVariantSteps')},
} as const;

export const LIGHT_CANVAS: Rgb = [${lightCanvas}];
export const DARK_CANVAS: Rgb = [${darkCanvas}];
export const WHITE: Rgb = [${white}];
export const HOVER_TARGET: Rgb = [${hoverTarget}];
`);

const cs = (a) => a.map((n) => '0x' + n.toString(16).toUpperCase().padStart(2, '0')).join(', ');
write('backend/src/TanErp.Domain/Organization/BrandRules.g.cs', `// ${HEADER}
namespace TanErp.Domain.Organization;

/// <summary>Brand colour rule constants generated from design-tokens/brand-rules.json and the canvas/white tokens.</summary>
internal static class BrandRules
{
    public const double MinTextContrast = ${v('textContrastMin').toFixed(1)};
    public const double MinCanvasContrast = ${v('canvasContrastMin').toFixed(1)};

    public static readonly byte[] LightCanvas = { ${cs(lightCanvas)} };
    public static readonly byte[] White = { ${cs(white)} };
}
`);

const dartColor = (a) => `Color(0xFF${a.map((n) => n.toString(16).toUpperCase().padStart(2, '0')).join('')})`;
write('mobile/lib/core/theme/brand_rules.g.dart', `// ${HEADER}
// ignore_for_file: public_member_api_docs

import 'dart:ui' show Color;

abstract final class BrandRules {
  static const double textContrastMin = ${v('textContrastMin')};
  static const double canvasContrastMin = ${v('canvasContrastMin')};
  static const double hoverMixRatio = ${v('hoverMixRatio')};
  static const Color hoverTarget = ${dartColor(hoverTarget)};
  static const double lightMixRatioLightTheme = ${v('lightMixRatioLightTheme')};
  static const double lightMixRatioDarkTheme = ${v('lightMixRatioDarkTheme')};
  static const double darkVariantStep = ${v('darkVariantStep')};
  static const int darkVariantSteps = ${v('darkVariantSteps')};
}
`);
