import type { NextConfig } from "next";
import createNextIntlPlugin from "next-intl/plugin";

const withNextIntl = createNextIntlPlugin("./src/i18n/request.ts");

const nextConfig: NextConfig = {
  reactStrictMode: true,
  allowedDevOrigins: ["*.trycloudflare.com", "192.168.1.113"],
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: "http://localhost:5005/api/:path*",
      },
    ];
  },
};

export default withNextIntl(nextConfig);
