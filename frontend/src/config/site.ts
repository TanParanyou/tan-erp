export const siteConfig = {
  name: "Tan ERP",
  shortName: "tan-erp",
  url: process.env.NEXT_PUBLIC_APP_URL || "http://localhost:3000",
  // eslint-disable-next-line no-restricted-syntax -- token-exempt: meta theme-color metadata, not CSS
  themeColor: "#0B3056",
  icons: {
    icon: "/icon",
    apple: "/apple-icon",
  },
} as const;

export type SiteConfig = typeof siteConfig;
