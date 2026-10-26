import babelParser from "@babel/eslint-parser";

// Existing source files carry disable comments for these plugins' rules; the plugins
// are not installed, so declare no-op rules so the directives resolve (no rules enabled).
const noop = { meta: { schema: [] }, create: () => ({}) };
const legacyDirectivePlugins = {
  "@next/next": { rules: { "no-img-element": noop } },
  "react-hooks": { rules: { "exhaustive-deps": noop } }
};

export default [
  {
    ignores: [
      ".next/**",
      "node_modules/**",
      "src/generated/**",
      "coverage/**",
      "dist/**"
    ]
  },
  {
    // Parse-only setup (no typescript-eslint: installed TypeScript 7 has no JS API).
    files: ["src/**/*.ts", "src/**/*.tsx"],
    ignores: ["src/**/*.test.ts", "src/**/*.test.tsx", "src/**/__tests__/**"],
    linterOptions: { reportUnusedDisableDirectives: "off" },
    plugins: legacyDirectivePlugins,
    languageOptions: {
      parser: babelParser,
      parserOptions: {
        requireConfigFile: false,
        babelOptions: { parserOpts: { plugins: ["typescript", "jsx"] } }
      }
    },
    rules: {
      "no-restricted-syntax": [
        "error",
        {
          selector: "Literal[value=/^#[0-9a-fA-F]{3,8}$/]",
          message:
            "Hard-coded hex colour. Use a semantic token / Tailwind class (design.md section 7), or add `// eslint-disable-next-line no-restricted-syntax -- token-exempt: <reason>`."
        }
      ]
    }
  }
];
