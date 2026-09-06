import "./globals.css";
import { AppProviders } from "@/providers/app-providers";

export const metadata = {
  title: "Project ERP",
  description: "Project & Manufacturing ERP",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="th">
      <body>
        <AppProviders>{children}</AppProviders>
      </body>
    </html>
  );
}
