import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./src/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      colors: {
        erp: {
          navy: "var(--erp-navy)",
          "navy-hover": "var(--erp-navy-hover)",
          "navy-light": "var(--erp-navy-light)",
          canvas: "var(--erp-canvas)",
          surface: "var(--erp-surface)",
          "surface-muted": "var(--erp-surface-muted)",
          "surface-subtle": "var(--erp-surface-subtle)",
          border: "var(--erp-border)",
          "border-subtle": "var(--erp-border-subtle)",
          "border-strong": "var(--erp-border-strong)",
          "text-main": "var(--erp-text-main)",
          "text-body": "var(--erp-text-body)",
          "text-muted": "var(--erp-text-muted)",
          "text-subtle": "var(--erp-text-subtle)",
          warning: "var(--erp-warning)",
          "warning-bg": "var(--erp-warning-bg)",
          "warning-border": "var(--erp-warning-border)",
          "warning-text": "var(--erp-warning-text)",
          danger: "var(--erp-danger)",
          "danger-bg": "var(--erp-danger-bg)",
          "danger-border": "var(--erp-danger-border)",
          success: "var(--erp-success)",
          "success-bg": "var(--erp-success-bg)",
          "success-border": "var(--erp-success-border)",
          info: "var(--erp-info)",
          "info-bg": "var(--erp-info-bg)",
        },
      },
      borderRadius: {
        DEFAULT: "0px",
        none: "0px",
        sm: "0px",
        md: "0px",
        lg: "0px",
        xl: "0px",
        "2xl": "0px",
        "3xl": "0px",
        full: "0px",
      },
    },
  },
  plugins: [],
};

export default config;
