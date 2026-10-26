// Builds design-tokens/ (W3C DTCG) into CSS variables (web) and Dart constants (Flutter app).
// Run: npm run tokens:build
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
    return `/* ${HEADER} */\n\n:root {\n${light.join('\n')}\n}\n\n[data-theme="dark"] {\n${dark.join('\n')}\n}\n`;
  },
});

StyleDictionary.registerFormat({
  name: 'tanerp/dart',
  format: ({ dictionary }) => {
    const colors = (tokens) => tokens.filter((t) => t.$type === 'color')
      .map((t) => `  static const Color ${camel(cssName(t))} = Color(0xFF${hex(t.$value)});`).join('\n');
    const nums = dictionary.allTokens.filter((t) => t.$type === 'dimension')
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
