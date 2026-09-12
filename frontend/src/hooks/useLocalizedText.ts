import { useLocale } from "next-intl";
import { getLocalizedText, type LocalizedText } from "@/lib/i18n/locales";

export function useLocalizedText() {
  const locale = useLocale();
  return (obj: LocalizedText | string | null | undefined) => getLocalizedText(obj, locale);
}
