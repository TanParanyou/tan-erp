import Link from "next/link";
import { notFound } from "next/navigation";
import { getTranslations } from "next-intl/server";
import { LoginForm } from "@/features/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { ThemeToggle } from "@/components/common/ThemeToggle";

interface LoginPageProps {
  params: Promise<{ locale: string }>;
}

export default async function LoginPage({ params }: LoginPageProps) {
  const { locale } = await params;

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const tShell = await getTranslations({ locale, namespace: "shell" });
  const targetLocale = locale === "th" ? "en" : "th";

  return (
    <main className="erp-login-page">
      <div className="erp-login-topbar">
        <ThemeToggle
          style={{
            borderColor: "var(--erp-border)",
            backgroundColor: "var(--erp-surface)",
            color: "var(--erp-text-main)",
          }}
        />
        <Link
          href={`/${targetLocale}/login`}
          className="erp-btn erp-btn-outline erp-btn-sm"
          style={{ minHeight: "44px", minWidth: "44px", backgroundColor: "var(--erp-surface)" }}
          aria-label={tShell("switchLanguage")}
        >
          {tShell("switchLanguage")}
        </Link>
      </div>
      <LoginForm />
    </main>
  );
}
