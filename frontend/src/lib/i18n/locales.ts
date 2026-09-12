import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";

export const defaultLocale = "th";
export const supportedLocales = ["th", "en"] as const;
export type SupportedLocale = (typeof supportedLocales)[number];

export function isSupportedLocale(locale: string): locale is SupportedLocale {
  return (supportedLocales as readonly string[]).includes(locale);
}

const messagesByLocale: Record<SupportedLocale, typeof thMessages> = {
  th: thMessages,
  en: enMessages,
};

export function getMessages(locale: SupportedLocale): typeof thMessages {
  return messagesByLocale[locale] || messagesByLocale[defaultLocale];
}

export type Messages = typeof thMessages;

export type LocalizedText = {
  th?: string;
  en?: string;
  [key: string]: string | undefined;
};

export function getLocalizedText(
  obj: LocalizedText | string | null | undefined,
  locale: string = defaultLocale
): string {
  if (!obj) return "";
  if (typeof obj === "string") return obj;
  return obj[locale] || obj[defaultLocale] || obj.en || Object.values(obj)[0] || "";
}

