import { getRequestConfig } from "next-intl/server";
import { routing } from "./routing";
import thMessages from "../messages/th.json";
import enMessages from "../messages/en.json";

const messagesMap = {
  th: thMessages,
  en: enMessages,
};

export default getRequestConfig(async ({ requestLocale }) => {
  let locale = await requestLocale;

  if (!locale || !routing.locales.includes(locale as "th" | "en")) {
    locale = routing.defaultLocale;
  }

  return {
    locale,
    messages: messagesMap[locale as "th" | "en"] || thMessages,
  };
});
