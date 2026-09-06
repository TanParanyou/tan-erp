import "@testing-library/jest-dom/vitest";
import { vi } from "vitest";
import { createTranslator } from "next-intl";
import thMessages from "@/messages/th.json";

// Provide fallback for next-intl hooks in isolated unit tests that do not wrap with NextIntlClientProvider
vi.mock("next-intl", async (importOriginal) => {
  const actual = await importOriginal<typeof import("next-intl")>();
  return {
    ...actual,
    useLocale: vi.fn(() => "th"),
    useTranslations: (namespace?: string) => {
      return createTranslator({
        locale: "th",
        messages: thMessages,
        namespace: namespace as never,
      });
    },
  };
});
