import { notFound } from "next/navigation";
import { LoginForm } from "@/features/auth";
import { isSupportedLocale, type SupportedLocale } from "@/lib/i18n/locales";

interface LoginPageProps {
  params: Promise<{ locale: string }>;
}

export default async function LoginPage({ params }: LoginPageProps) {
  const { locale } = await params;

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <main
      style={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: "1.5rem",
        backgroundColor: "#F9FAFB",
      }}
    >
      <LoginForm locale={locale as SupportedLocale} />
    </main>
  );
}
