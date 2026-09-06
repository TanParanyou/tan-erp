import "./globals.css";
import { AppProviders } from "@/providers/app-providers";
import { getLocale } from "next-intl/server";

export const metadata = {
  title: "Project ERP",
  description: "Project & Manufacturing ERP",
};

export default async function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const locale = await getLocale();
  return (
    <html lang={locale}>
      <body>
        <AppProviders>{children}</AppProviders>
      </body>
    </html>
  );
}
